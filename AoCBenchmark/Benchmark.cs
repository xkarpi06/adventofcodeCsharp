using AdventOfCode.Aoc2022.Day17;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AoCBenchmark
{
    public class CompareChambers
    {
        private IChamber _chamber;

        [Params(1_000_000)]
        public int N;

        [Benchmark(Baseline = true)]
        public void SlowChamber()
        {
            _chamber = new SlowChamber();
            Day17.Run(_chamber, N);
        }
        
        [Benchmark]
        public void ByteChamber()
        {
            _chamber = new ByteChamber();
            Day17.Run(_chamber, N);
        }

        [Benchmark]
        public void ByteChamber2()
        {
            _chamber = new ByteChamber2();
            Day17.Run(_chamber, N);
        }
    }

    public class Benchmark
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompareChambers>();
        }
    }
}

