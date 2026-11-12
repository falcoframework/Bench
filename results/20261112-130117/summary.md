# Benchmark results (20261112-130117)

- Date: 2026-11-12 13:41:42Z
- OS: Microsoft Windows 10.0.19045
- CPU cores: 4
- .NET SDK: 10.0.115
- Go: go version go1.26.0 windows/amd64
- Duration: 30s x 3 runs (median reported), warmup 10s, idle 30s

## 64 connections

Throughput and latency

| App                   | Req/s median | min   | max   | p50 ms | p99 ms |
| --------------------- | -----------: | ----: | ----: | -----: | -----: |
| FalcoBench            |        81013 | 80731 | 81761 |      1 |   1.99 |
| GiraffeEndpointsBench |        80046 | 79274 | 81714 |      1 |   1.99 |
| GiraffeBench          |        79509 | 76717 | 80828 |      1 |   1.99 |
| OxpeckerBench         |        79169 | 78768 | 80053 |      1 |   1.99 |
| RazorSlicesBench      |        78115 | 77790 | 79700 |      1 |   1.99 |
| FrankBench            |        77982 | 69833 | 80146 |      1 |      2 |
| AspNetRazorPagesBench |        48785 | 48601 | 49265 |      1 |      2 |
| AspNetMvcBench        |        48719 | 48464 | 49220 |      1 |      2 |
| GoBench               |        48287 | 48098 | 49172 |   1.16 |   3.74 |

Resources and responses

| App                   | Peak MB | Idle MB | Startup ms | Publish MB | Non-200 |
| --------------------- | ------: | ------: | ---------: | ---------: | ------: |
| GoBench               |    17.4 |    15.6 |       1378 |        8.2 |       0 |
| GiraffeBench          |    48.5 |    39.6 |       1235 |       13.1 |       0 |
| FalcoBench            |    49.3 |    40.3 |       1277 |       13.1 |       0 |
| GiraffeEndpointsBench |    49.5 |      40 |       1274 |       13.1 |       0 |
| RazorSlicesBench      |    49.9 |    41.1 |       1298 |         13 |       0 |
| OxpeckerBench         |    50.2 |    41.5 |       1266 |       13.1 |       0 |
| FrankBench            |    51.4 |      45 |       1323 |       13.6 |       0 |
| AspNetRazorPagesBench |   173.9 |     157 |        763 |         48 |       0 |
| AspNetMvcBench        |   176.7 |   168.7 |        741 |         48 |       0 |

## 256 connections

Throughput and latency

| App                   | Req/s median | min   | max   | p50 ms | p99 ms |
| --------------------- | -----------: | ----: | ----: | -----: | -----: |
| FalcoBench            |        64860 | 64764 | 65123 |   3.99 |   6.98 |
| OxpeckerBench         |        64809 | 59423 | 65597 |   3.99 |   6.98 |
| FrankBench            |        64759 | 63863 | 65715 |   3.99 |   6.98 |
| RazorSlicesBench      |        64575 | 64324 | 65402 |   3.99 |   5.98 |
| GiraffeBench          |        64363 | 63555 | 65498 |   3.99 |   6.98 |
| GiraffeEndpointsBench |        64073 | 63950 | 65835 |   3.99 |   6.98 |
| GoBench               |        46609 | 46608 | 47203 |   4.74 |  14.96 |
| AspNetRazorPagesBench |        44359 | 43985 | 45382 |   4.99 |   8.98 |
| AspNetMvcBench        |        42176 | 31492 | 42378 |   4.99 |  11.97 |

Resources and responses

| App                   | Peak MB | Idle MB | Startup ms | Publish MB | Non-200 |
| --------------------- | ------: | ------: | ---------: | ---------: | ------: |
| GoBench               |    24.8 |    23.8 |        519 |        8.2 |       0 |
| OxpeckerBench         |    70.6 |    48.2 |       1236 |       13.1 |       0 |
| FalcoBench            |    70.8 |    44.5 |       1195 |       13.1 |       0 |
| FrankBench            |    70.9 |    47.2 |       1262 |       13.6 |       0 |
| GiraffeEndpointsBench |    73.7 |    44.8 |       1184 |       13.1 |       0 |
| RazorSlicesBench      |    75.5 |    45.8 |       1220 |         13 |       0 |
| GiraffeBench          |      76 |    44.3 |       1161 |       13.1 |       0 |
| AspNetRazorPagesBench |   187.4 |   165.7 |        624 |         48 |       0 |
| AspNetMvcBench        |   191.3 |   164.7 |        637 |         48 |       0 |
