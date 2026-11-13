<#
.SYNOPSIS
Builds and benchmarks the apps in benchmarks/apps using bombardier.

.DESCRIPTION
For each app: build/publish, start, measure startup time, warm up, run the load
test N times, record peak and idle memory, then stop. Writes a markdown summary
and raw JSON to benchmarks/results/<timestamp>/.
Use -Mini for a short run that displays the summary without saving result files.
Use -BuildOnly to build/publish the selected apps without starting or benchmarking them.

.NOTES
Requires bombardier on PATH. Go apps need `go`, .NET apps need `dotnet`.
The script sets ASPNETCORE_URLS (.NET apps) and PORT (Go apps) from -Port.
Run the load generator on a separate machine for the most credible numbers.

.EXAMPLE
./RunBenchmarks.ps1 -Apps AspNetMvcBench,AspNetRazorPages,FalcoBench,FrankBench,GiraffeBench,GoBench,OxpeckerBench,RazorSlicesBench -Connections 64,256 -Runs 5

.EXAMPLE
./RunBenchmarks.ps1 -Mini

.EXAMPLE
./RunBenchmarks.ps1 -BuildOnly
#>
[CmdletBinding()]
param(
    [string[]] $Apps,
    [int[]] $Connections = @(100, 250),
    [int] $DurationSeconds = 10,
    [int] $WarmupSeconds = 5,
    [int] $Runs = 3,
    [int] $IdleSeconds = 10,
    [int] $Port = 5000,
    [string] $Configuration = "Release",
    [switch] $Mini,
    [switch] $BuildOnly
)

# $ErrorActionPreference = "Stop"

if ($Mini) {
    $Connections = @(100)
    $DurationSeconds = 10
    $WarmupSeconds = 3
    $Runs = 1
    $IdleSeconds = 3
}

$root = Split-Path -Parent $PSScriptRoot
$appsDir = Join-Path $root "apps"
$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$resultsDir = Join-Path $root "results\$stamp"
$buildDir = Join-Path $root "build"
$url = "http://127.0.0.1:$Port/"

if (-not $BuildOnly -and -not (Get-Command bombardier -ErrorAction SilentlyContinue)) {
    throw "bombardier not found on PATH. Install from https://github.com/codesenberg/bombardier"
}

if (-not $Mini -and -not $BuildOnly) {
    New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
}

function Get-AppKind($dir) {
    if (Get-ChildItem $dir -Filter *.fsproj -ErrorAction SilentlyContinue) { return "dotnet" }
    if (Get-ChildItem $dir -Filter *.csproj -ErrorAction SilentlyContinue) { return "dotnet" }
    if (Test-Path (Join-Path $dir "go.mod")) { return "go" }
    return $null
}

function Build-App($name, $dir, $kind) {
    $out = Join-Path $buildDir $name
    if (Test-Path $out) { Remove-Item $out -Recurse -Force }

    if ($kind -eq "dotnet") {
        & dotnet publish $dir -c $Configuration -o $out --nologo -v quiet | Out-Host
        if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed for $name" }
        $exe = Get-ChildItem $out -Filter "$name.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $exe) { $exe = Get-ChildItem $out -File | Where-Object { $_.Extension -eq "" -and $_.Name -eq $name } | Select-Object -First 1 }
        return @{ Out = $out; Exe = $exe.FullName; Dll = (Join-Path $out "$name.dll") }
    }

    $exeName = if ($IsLinux -or $IsMacOS) { $name } else { "$name.exe" }
    $exePath = Join-Path $out $exeName
    New-Item -ItemType Directory -Force -Path $out | Out-Null
    Push-Location $dir
    try {
        & go build -ldflags "-s -w" -o $exePath . | Out-Host
        if ($LASTEXITCODE -ne 0) { throw "go build failed for $name" }
    }
    finally { Pop-Location }
    return @{ Out = $out; Exe = $exePath; Dll = $null }
}

function Get-DirSizeMb($path) {
    [math]::Round(((Get-ChildItem $path -Recurse -File | Measure-Object Length -Sum).Sum / 1MB), 1)
}

function Start-App($artifact, $kind) {
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $env:PORT = "$Port"
    $psi = [System.Diagnostics.ProcessStartInfo]::new()
    if ($artifact.Exe -and (Test-Path $artifact.Exe)) {
        $psi.FileName = $artifact.Exe
    }
    else {
        $psi.FileName = "dotnet"
        $psi.ArgumentList.Add($artifact.Dll)
    }
    $psi.WorkingDirectory = $artifact.Out
    $psi.UseShellExecute = $false
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true

    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $proc = [System.Diagnostics.Process]::Start($psi)
    # Drain output so the child never blocks on a full pipe.
    $null = $proc.StandardOutput.ReadToEndAsync()
    $stderrTask = $proc.StandardError.ReadToEndAsync()

    $ready = $false
    while ($sw.Elapsed.TotalSeconds -lt 30 -and -not $proc.HasExited) {
        try {
            $r = Invoke-WebRequest $url -UseBasicParsing -TimeoutSec 1
            if ($r.StatusCode -eq 200) { $ready = $true; break }
        }
        catch { Start-Sleep -Milliseconds 5 }
    }
    $sw.Stop()
    if (-not $ready) {
        if ($proc.HasExited) {
            $proc.WaitForExit()
            throw "App exited with code $($proc.ExitCode) before responding on $url. stderr: $($stderrTask.Result)"
        }
        $proc.Kill()
        throw "App did not respond on $url within 30s"
    }
    return @{ Process = $proc; StartupMs = [math]::Round($sw.Elapsed.TotalMilliseconds) }
}

function Get-MemMb($proc) {
    $proc.Refresh()
    [math]::Round($proc.WorkingSet64 / 1MB, 1)
}

function Get-PeakMemMb($proc) {
    $proc.Refresh()
    [math]::Round($proc.PeakWorkingSet64 / 1MB, 1)
}

function Invoke-Bombardier($scenario, $connections, $seconds, $outFile) {
    $args = @("-c", $connections, "-d", "${seconds}s", "-l", "-p", "r", "-o", "json", $scenario.Url)

    $json = & bombardier @args
    if ($LASTEXITCODE -ne 0) { throw "bombardier failed" }
    if ($outFile) { [IO.File]::WriteAllLines($outFile, [string[]]@($json ?? @())) }
    return ($json -join "`n") | ConvertFrom-Json
}

function Get-Median($values) {
    $s = @($values | Sort-Object)
    if ($s.Count -eq 0) { return 0 }
    $mid = [math]::Floor($s.Count / 2)
    if ($s.Count % 2) { return $s[$mid] }
    return ($s[$mid - 1] + $s[$mid]) / 2
}

function Format-MarkdownTable($rows, $columns) {
    $widths = @(
        foreach ($column in $columns) {
            $width = [math]::Max(3, $column.Header.Length)
            foreach ($row in $rows) {
                $value = [string]$row.($column.Property)
                $width = [math]::Max($width, $value.Length)
            }
            $width
        }
    )

    $headers = for ($i = 0; $i -lt $columns.Count; $i++) {
        $columns[$i].Header.PadRight($widths[$i])
    }
    "| $($headers -join ' | ') |"
    $separators = for ($i = 0; $i -lt $columns.Count; $i++) {
        if ($columns[$i].Numeric) { ('-' * ($widths[$i] - 1)) + ':' }
        else { '-' * $widths[$i] }
    }
    "| $($separators -join ' | ') |"
    foreach ($row in $rows) {
        $cells = for ($i = 0; $i -lt $columns.Count; $i++) {
            $value = [string]$row.($columns[$i].Property)
            if ($columns[$i].Numeric) { $value.PadLeft($widths[$i]) }
            else { $value.PadRight($widths[$i]) }
        }
        "| $($cells -join ' | ') |"
    }
}

# Discover apps: anything under apps/ that looks buildable.
$available = Get-ChildItem $appsDir -Directory | Where-Object { $_.Name -ne "GiraffeEndpointsBench" -and (Get-AppKind $_.FullName) }
if ($Apps) { $available = $available | Where-Object { $Apps -contains $_.Name } }
if (-not $available) { throw "No apps found to benchmark in $appsDir" }

$scenarios = @(
    @{
        Name   = "GET /"
        Url    = $url
        Suffix = ""
    }
    @{
        Name   = "GET /text"
        Url    = "${url}text"
        Suffix = "-text"
    }
    # @{
    #     Name   = "GET /json"
    #     Url    = "${url}json"
    #     Suffix = "-json"
    # }
)

$summary = New-Object System.Collections.Generic.List[object]

foreach ($app in $available) {
    $name = $app.Name
    $kind = Get-AppKind $app.FullName
    Write-Host "`n=== $name ($kind) ===" -ForegroundColor Cyan

    if ($kind -eq "go" -and -not (Get-Command go -ErrorAction SilentlyContinue)) {
        if ($BuildOnly) { throw "go not found on PATH; cannot build $name" }
        Write-Warning "go not found; skipping $name"
        continue
    }

    $artifact = Build-App $name $app.FullName $kind
    if ($BuildOnly) {
        Write-Host "Built successfully: $($artifact.Out)" -ForegroundColor Green
        continue
    }

    $sizeMb = Get-DirSizeMb $artifact.Out

    foreach ($scenario in $scenarios) {
        foreach ($conn in $Connections) {
            $started = Start-App $artifact $kind
            $proc = $started.Process
            try {
                Write-Host "Started in $($started.StartupMs) ms. Warming up $($scenario.Name) for ${WarmupSeconds}s..."
                $null = Invoke-Bombardier $scenario $conn $WarmupSeconds $null

                $runResults = @()
                for ($i = 1; $i -le $Runs; $i++) {
                    Write-Host "Run $i/$Runs of $($scenario.Name) at $conn connections for ${DurationSeconds}s"
                    $fileName = "$name$($scenario.Suffix)-c$conn-run$i.json"
                    $file = if ($Mini) { $null } else { Join-Path $resultsDir $fileName }
                    $runResults += Invoke-Bombardier $scenario $conn $DurationSeconds $file
                }

                $peak = Get-PeakMemMb $proc
                if ($IdleSeconds -gt 0) {
                    Write-Host "Idling ${IdleSeconds}s to observe memory release..."
                    Start-Sleep -Seconds $IdleSeconds
                }
                $idle = Get-MemMb $proc

                $failed = ($runResults | ForEach-Object { $_.result.req1xx + $_.result.req3xx + $_.result.req4xx + $_.result.req5xx + $_.result.others } | Measure-Object -Sum).Sum
                $rps = $runResults | ForEach-Object { $_.result.rps.mean }

                # Bombardier reports latency percentiles in microseconds.
                $p50 = $runResults | ForEach-Object { $_.result.latency.percentiles.'50' }
                $p99 = $runResults | ForEach-Object { $_.result.latency.percentiles.'99' }

                $summary.Add([pscustomobject]@{
                        App         = $name
                        Scenario    = $scenario.Name
                        Connections = $conn
                        RpsMedian   = [math]::Round((Get-Median $rps))
                        RpsMin      = [math]::Round(($rps | Measure-Object -Minimum).Minimum)
                        RpsMax      = [math]::Round(($rps | Measure-Object -Maximum).Maximum)
                        P50Ms       = [math]::Round((Get-Median $p50) / 1000, 2)
                        P99Ms       = [math]::Round((Get-Median $p99) / 1000, 2)
                        PeakMemMb   = $peak
                        IdleMemMb   = $idle
                        StartupMs   = $started.StartupMs
                        PublishMb   = $sizeMb
                        NonOkReqs   = $failed
                    })
            }
            finally {
                if (-not $proc.HasExited) { $proc.Kill($true); $proc.WaitForExit() }
            }
        }
    }
}

if ($BuildOnly) {
    Write-Host "`nBuild-only check passed for $($available.Count) app(s)." -ForegroundColor Green
    return
}

$envInfo = @(
    "- Date: $(Get-Date -Format 'u')"
    "- OS: $([System.Runtime.InteropServices.RuntimeInformation]::OSDescription)"
    "- CPU cores: $([Environment]::ProcessorCount)"
    "- .NET SDK: $(if (Get-Command dotnet -ErrorAction SilentlyContinue) { (& dotnet --version) } else { 'n/a' })"
    "- Go: $(if (Get-Command go -ErrorAction SilentlyContinue) { (& go version) } else { 'n/a' })"
    "- Duration: ${DurationSeconds}s x $Runs runs (median reported), warmup ${WarmupSeconds}s, idle ${IdleSeconds}s"
)

$table = $summary | Sort-Object Connections, Scenario, @{ Expression = "RpsMedian"; Descending = $true }
$throughputColumns = @(
    @{ Header = "App"; Property = "App" }
    @{ Header = "Scenario"; Property = "Scenario" }
    @{ Header = "Req/s median"; Property = "RpsMedian"; Numeric = $true }
    @{ Header = "min"; Property = "RpsMin"; Numeric = $true }
    @{ Header = "max"; Property = "RpsMax"; Numeric = $true }
    @{ Header = "p50 ms"; Property = "P50Ms"; Numeric = $true }
    @{ Header = "p99 ms"; Property = "P99Ms"; Numeric = $true }
)
$resourceColumns = @(
    @{ Header = "App"; Property = "App" }
    @{ Header = "Scenario"; Property = "Scenario" }
    @{ Header = "Peak MB"; Property = "PeakMemMb"; Numeric = $true }
    @{ Header = "Idle MB"; Property = "IdleMemMb"; Numeric = $true }
    @{ Header = "Startup ms"; Property = "StartupMs"; Numeric = $true }
    @{ Header = "Publish MB"; Property = "PublishMb"; Numeric = $true }
    @{ Header = "Non-200"; Property = "NonOkReqs"; Numeric = $true }
)
$md = @("# Benchmark results ($stamp)", "") + $envInfo
foreach ($group in @($table | Group-Object Connections)) {
    $md += @("", "## $($group.Name) connections", "", "Throughput and latency", "")
    $md += Format-MarkdownTable $group.Group $throughputColumns
    $md += @("", "Resources and responses", "")
    $resourceRows = $group.Group | Sort-Object PeakMemMb
    $md += Format-MarkdownTable $resourceRows $resourceColumns
}

if ($Mini) {
    $md
}
else {
    Write-Host "`n$($md -join "`n")"
}
if (-not $Mini) {
    $mdPath = Join-Path $resultsDir "summary.md"
    $jsonPath = Join-Path $resultsDir "summary.json"
    [IO.File]::WriteAllLines($mdPath, [string[]]$md)
    if ($summary.Count -gt 0) { [IO.File]::WriteAllText($jsonPath, ($summary | ConvertTo-Json) + [Environment]::NewLine) }

    Write-Host "`nResults written to $resultsDir" -ForegroundColor Green
    Write-Host "  Markdown summary: $mdPath"
    Write-Host "  JSON summary:     $jsonPath"
    Write-Host "  Raw run JSON:     $($summary.Count * $Runs) files in $resultsDir"
}
