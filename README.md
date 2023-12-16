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
