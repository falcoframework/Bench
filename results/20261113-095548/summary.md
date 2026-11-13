# Benchmark results (20261113-095548)

- Date: 2026-11-13 10:20:38Z
- OS: Ubuntu 24.04.4 LTS
- CPU cores: 16
- .NET SDK: 10.0.112
- Go: go version go1.26.0 linux/amd64
- Duration: 10s x 3 runs (median reported), warmup 5s, idle 10s

## 100 connections

Throughput and latency

| App                   | Scenario  | Req/s median | min    | max    | p50 ms | p99 ms |
| --------------------- | --------- | -----------: | -----: | -----: | -----: | -----: |
| RazorSlicesBench      | GET /     |       373701 | 369190 | 377604 |   0.21 |   1.08 |
| FalcoBench            | GET /     |       365042 | 364845 | 365992 |   0.22 |   0.99 |
| FrankBench            | GET /     |       354065 | 352856 | 354848 |   0.22 |   1.01 |
| OxpeckerBench         | GET /     |       318448 | 318294 | 328546 |   0.26 |   1.06 |
| GiraffeBench          | GET /     |       313800 | 312245 | 316203 |   0.27 |   1.09 |
| GoBench               | GET /     |       230332 | 229156 | 230597 |   0.29 |   2.09 |
| AspNetRazorPagesBench | GET /     |       197035 | 195690 | 201911 |   0.48 |    1.3 |
| AspNetMvcBench        | GET /     |       195321 | 194415 | 210097 |   0.49 |   1.32 |
| OxpeckerBench         | GET /text |       416338 | 412620 | 416545 |   0.18 |   1.06 |
| FalcoBench            | GET /text |       393375 | 389232 | 397947 |    0.2 |   1.02 |
| RazorSlicesBench      | GET /text |       390228 | 390055 | 391727 |   0.19 |   1.08 |
| GiraffeBench          | GET /text |       385628 | 385621 | 387079 |    0.2 |   1.11 |
| FrankBench            | GET /text |       369573 | 366861 | 370469 |   0.21 |   1.09 |
| AspNetMvcBench        | GET /text |       332625 | 327738 | 334541 |   0.24 |   1.04 |
| AspNetRazorPagesBench | GET /text |       328409 | 326962 | 330269 |   0.25 |   1.04 |
| GoBench               | GET /text |       215159 | 214685 | 216885 |   0.28 |   2.24 |

Resources and responses

| App                   | Scenario  | Peak MB | Idle MB | Startup ms | Publish MB | Non-200 |
| --------------------- | --------- | ------: | ------: | ---------: | ---------: | ------: |
| GoBench               | GET /     |    19.4 |    17.6 |         32 |        7.9 |       0 |
| GoBench               | GET /text |    20.1 |      18 |         33 |        7.9 |       0 |
| GiraffeBench          | GET /text |    56.9 |    48.8 |        489 |       15.1 |       0 |
| RazorSlicesBench      | GET /text |    60.6 |    55.7 |        600 |       14.9 |       0 |
| FrankBench            | GET /text |    60.6 |    52.6 |        563 |       15.6 |       0 |
| RazorSlicesBench      | GET /     |    60.8 |    52.7 |        568 |       14.9 |       0 |
| OxpeckerBench         | GET /text |      61 |    53.3 |        561 |       15.1 |       0 |
| FalcoBench            | GET /text |    62.3 |    55.5 |        518 |       15.2 |       0 |
| FalcoBench            | GET /     |    65.2 |      59 |        511 |       15.2 |       0 |
| FrankBench            | GET /     |    66.7 |    60.9 |        566 |       15.6 |       0 |
| OxpeckerBench         | GET /     |    88.8 |    88.9 |        564 |       15.1 |       0 |
| GiraffeBench          | GET /     |    90.3 |    90.3 |        499 |       15.1 |       0 |
| AspNetRazorPagesBench | GET /text |     169 |   160.7 |        447 |       49.3 |       0 |
| AspNetMvcBench        | GET /text |   174.6 |     162 |        384 |       49.3 |       0 |
| AspNetRazorPagesBench | GET /     |   190.7 |   188.2 |        399 |       49.3 |       0 |
| AspNetMvcBench        | GET /     |   199.5 |   199.5 |        388 |       49.3 |       0 |

## 250 connections

Throughput and latency

| App                   | Scenario  | Req/s median | min    | max    | p50 ms | p99 ms |
| --------------------- | --------- | -----------: | -----: | -----: | -----: | -----: |
| FalcoBench            | GET /     |       392061 | 388054 | 395650 |   0.58 |    2.3 |
| RazorSlicesBench      | GET /     |       389534 | 389182 | 392784 |   0.57 |   2.45 |
| FrankBench            | GET /     |       373355 | 370668 | 373649 |   0.61 |   2.29 |
| OxpeckerBench         | GET /     |       343972 | 343181 | 344443 |   0.67 |   2.17 |
| GiraffeBench          | GET /     |       341041 | 337686 | 344913 |   0.67 |   2.13 |
| GoBench               | GET /     |       271507 | 270634 | 271983 |   0.65 |   4.56 |
| AspNetRazorPagesBench | GET /     |       222139 | 214355 | 222469 |   1.09 |    2.7 |
| AspNetMvcBench        | GET /     |       216293 | 214336 | 217547 |   1.12 |   2.71 |
| OxpeckerBench         | GET /text |       422187 | 421223 | 423385 |   0.52 |   2.96 |
| FalcoBench            | GET /text |       411238 | 408615 | 411557 |   0.54 |   2.41 |
| GiraffeBench          | GET /text |       407842 | 399956 | 415008 |   0.55 |   2.33 |
| RazorSlicesBench      | GET /text |       403535 | 401133 | 404017 |   0.55 |    2.6 |
| FrankBench            | GET /text |       398530 | 396228 | 399303 |   0.55 |   2.58 |
| AspNetMvcBench        | GET /text |       358057 | 354822 | 361091 |   0.64 |   2.28 |
| AspNetRazorPagesBench | GET /text |       355058 | 349817 | 356562 |   0.65 |   2.21 |
| GoBench               | GET /text |       271626 | 270286 | 271660 |   0.58 |   5.42 |

Resources and responses

| App                   | Scenario  | Peak MB | Idle MB | Startup ms | Publish MB | Non-200 |
| --------------------- | --------- | ------: | ------: | ---------: | ---------: | ------: |
| GoBench               | GET /     |    26.1 |      23 |         33 |        7.9 |       0 |
| GoBench               | GET /text |    29.1 |    23.7 |         34 |        7.9 |       0 |
| GiraffeBench          | GET /text |    76.7 |    60.5 |        487 |       15.1 |       0 |
| FalcoBench            | GET /text |    77.8 |    65.6 |        516 |       15.2 |       0 |
| RazorSlicesBench      | GET /     |    81.5 |    55.4 |        593 |       14.9 |       0 |
| OxpeckerBench         | GET /text |    82.5 |    56.3 |        569 |       15.1 |       0 |
| FrankBench            | GET /text |    82.9 |    62.8 |        657 |       15.6 |       0 |
| RazorSlicesBench      | GET /text |    84.6 |    56.2 |        659 |       14.9 |       0 |
| FrankBench            | GET /     |    89.7 |    67.5 |        582 |       15.6 |       0 |
| FalcoBench            | GET /     |    96.8 |    73.9 |        513 |       15.2 |       0 |
| GiraffeBench          | GET /     |   101.9 |   101.8 |        535 |       15.1 |       0 |
| OxpeckerBench         | GET /     |   104.4 |   104.4 |        563 |       15.1 |       0 |
| AspNetMvcBench        | GET /text |   188.7 |   167.5 |        406 |       49.3 |       0 |
| AspNetRazorPagesBench | GET /text |   198.9 |   172.3 |        406 |       49.3 |       0 |
| AspNetMvcBench        | GET /     |   216.8 |   211.8 |        388 |       49.3 |       0 |
| AspNetRazorPagesBench | GET /     |   234.1 |   212.5 |        418 |       49.3 |       0 |
