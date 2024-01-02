# adventofcode
Advent of Code coding challenge (https://adventofcode.com)
# BenchmarkDotNet
to compare several implementations, benchmark can be used (https://github.com/dotnet/BenchmarkDotNet)

to run benchmark, first install BenchmarkDotNet package
- Navigate to AoCBenchmark directory in terminal
- run: dotnet add package BenchmarkDotNet
  
run
- dotnet run -c Release
# Benchmark results
16.12. - YEAR 2022, DAY 17
| Method       | N       | Mean       | Error    | StdDev   | Ratio |
|------------- |-------- |-----------:|---------:|---------:|------:|
| SlowChamber  | 1000000 | 5,601.4 ms | 29.80 ms | 27.88 ms |  1.00 |
| ByteChamber  | 1000000 |   796.0 ms |  3.37 ms |  2.99 ms |  0.14 |
| ByteChamber2 | 1000000 |   812.6 ms |  7.32 ms |  6.85 ms |  0.15 |
| ByteChamber3 | 1000000 |   151.7 ms |  2.47 ms |  2.19 ms |  0.03 |

17.12. - YEAR 2022, DAY 17
(removed some code from Part2, that helps ByteChamber3- and hurts ByteChamber4+)
| Method       | N       | Mean        | Error     | StdDev    | Ratio |
|------------- |-------- |------------:|----------:|----------:|------:|
| SlowChamber  | 1000000 | 5,520.94 ms | 34.664 ms | 30.729 ms |  1.00 |
| ByteChamber  | 1000000 | 1,027.82 ms |  8.595 ms |  7.177 ms |  0.19 |
| ByteChamber2 | 1000000 | 1,003.65 ms |  6.742 ms |  6.306 ms |  0.18 |
| ByteChamber3 | 1000000 |   168.08 ms |  2.246 ms |  1.991 ms |  0.03 |
| ByteChamber4 | 1000000 |    78.77 ms |  1.305 ms |  1.912 ms |  0.01 |
| ByteChamber5 | 1000000 |    72.13 ms |  0.460 ms |  0.408 ms |  0.01 |

02.01. - YEAR 2022, DAY 17
MemoryDiagnoser (Same implementation)
| Method       | N       | Mean        | Error     | StdDev    | Ratio | Gen0         | Gen1        | Gen2       | Allocated  | Alloc Ratio |
|------------- |-------- |------------:|----------:|----------:|------:|-------------:|------------:|-----------:|-----------:|------------:|
| SlowChamber  | 1000000 | 5,132.67 ms | 20.447 ms | 18.126 ms |  1.00 | 1516000.0000 | 152000.0000 | 79000.0000 | 4451.09 MB |       1.000 |
| ByteChamber  | 1000000 | 1,010.51 ms | 14.921 ms | 13.227 ms |  0.20 |  239000.0000 | 161000.0000 |          - | 1118.17 MB |       0.251 |
| ByteChamber2 | 1000000 |   941.66 ms |  7.531 ms |  6.289 ms |  0.18 |  431000.0000 | 200000.0000 |          - | 1746.53 MB |       0.392 |
| ByteChamber3 | 1000000 |   162.19 ms |  2.948 ms |  3.395 ms |  0.03 |     500.0000 |    500.0000 |   500.0000 |    5.41 MB |       0.001 |
| ByteChamber4 | 1000000 |    75.44 ms |  0.889 ms |  0.788 ms |  0.01 |     857.1429 |    714.2857 |   714.2857 |    5.41 MB |       0.001 |
| ByteChamber5 | 1000000 |    69.65 ms |  0.609 ms |  0.540 ms |  0.01 |     857.1429 |    857.1429 |   857.1429 |    4.06 MB |       0.001 |