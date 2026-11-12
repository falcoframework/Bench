# Benchmarks

Requirements
- dotnet >= 10.0
- go >= 1.25
- PowerShell 7
- bombardier >= 1.2

Run the benchmark script with PowerShell 7, with `bombardier`, `dotnet`, and
`go` on PATH (only the runtimes needed by the selected apps are required):

```powershell
.\scripts\RunBenchmarks.ps1
```

For example, to benchmark specific apps and connection counts:

```powershell
.\scripts\RunBenchmarks.ps1 -Apps AspNetMvcBench,AspNetRazorPages,FalcoBench,FrankBench,GiraffeBench,GiraffeEndpointsBench,GoBench,OxpeckerBench,RazorSlicesBench -Connections 64,256 -Runs 5
```

`AspNetMvcBench` benchmarks ASP.NET Core MVC, and `AspNetRazorPages` benchmarks
ASP.NET Core Razor Pages. Like the other apps, both serve a “Hello World!” page
with the current UTC time at `/`. Their publish trimming is disabled because
ASP.NET Core MVC and Razor Pages do not support trimming.

Results are saved under `results\<timestamp>\`. The script prints
the full paths to `summary.md` and `summary.json`; each measured run also has
a raw `<app>-c<connections>-run<run>.json` file in the same directory.
The Markdown summary groups results by connection count and separates
throughput and latency from resources and response counts for readability.

For a quick, non-persistent run, use `-Mini`. It runs 64 connections for one
30-second measurement after a one-second warmup, skips the idle wait, and
prints the summary without creating files under `results\`.

To verify that the selected apps can be built/published without starting them
or running a load test, use `-BuildOnly`. This does not require `bombardier` and
does not create benchmark result files:

```powershell
.\scripts\RunBenchmarks.ps1 -BuildOnly
```

The script rebuilds selected apps under `build\`. It runs the load
generator locally, so the results also reflect contention for local resources.