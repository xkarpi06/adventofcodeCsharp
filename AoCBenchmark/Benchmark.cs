using AdventOfCode.Aoc2022.Day17;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace AoCBenchmark
{
    [MemoryDiagnoser]
    [Config(typeof(CustomConfig))]
    public class CompareChambers
    {
        private class CustomConfig : ManualConfig
        {
            public CustomConfig()
            {
                Add(MemoryDiagnoser.Default);
            }
        }

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

        [Benchmark]
        public void ByteChamber3()
        {
            _chamber = new ByteChamber3();
            Day17.Run(_chamber, N, 100_000_000); // doesn't have to be reduced so often
        }

        [Benchmark]
        public void ByteChamber4()
        {
            _chamber = new ByteChamber4();
            Day17.Run(_chamber, N, 100_000_000); // doesn't have to be reduced so often
        }

        [Benchmark]
        public void ByteChamber5()
        {
            Day17.RunInChamber(new ByteChamber5(), N);
        }
    }

    public class AllocateVsCopy
    {

        [Params(1_000_000)]
        public int N;

        [Benchmark(Baseline = true)]
        public void Allocation()
        {
            byte[] bytes;
            for (int i = 0; i < N; i++)
            {
                bytes =
                [
                    0b00001000,
                    0b00001000,
                    0b00001000,
                ];
            }
        }

        [Benchmark]
        public void Copy()
        {
            byte[] bytes =
            [
                0b00001000,
                0b00001000,
                0b00001000,
            ];
            byte[] x = bytes;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < bytes.Length; j++)
                {
                    x[j] = bytes[j];
                }
            }
        }
    }

    public class ElementAtVsIterate
    {

        [Params(1_000_000)]
        public int N;

        public List<byte> list;

        [GlobalSetup]
        public void Setup()
        {
            list = new List<byte>();
            for (int i = 0; i < N; i++)
            {
                list.Add(0);
            }
        }

        [Benchmark(Baseline = true)]
        public void ElementAt()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0 + N / 2; j < 3 + N / 2; j++)
                {
                    var x = list.ElementAt(j);
                }
            }
        }

        [Benchmark]
        public void Index()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0 + N / 2; j < 3 + N / 2; j++)
                {
                    var x = list[j];
                }
            }
        }
    }

    public class ListVsLinkedList
    {

        [Params(10_000)]
        public int N;

        public List<byte> list;
        public LinkedList<byte> llist;

        [IterationSetup]
        public void Setup()
        {
            list = new List<byte>();
            llist = new LinkedList<byte>();
            for (int i = 0; i < N; i++)
            {
                list.Add(0);
                llist.AddLast(0);
            }
        }

        [Benchmark(Baseline = true)]
        public void ListInsertFirst()
        {
            for (int i = 0; i < N; i++)
            {
                list.Insert(0, 0);
            }
        }

        [Benchmark]
        public void LinkedListInsertFirst()
        {
            for (int i = 0; i < N; i++)
            {
                llist.AddFirst(0);
            }
        }

        [Benchmark]
        public void ListAccess()
        {
            for (int i = 0; i < N; i++)
            {
                var x = list[i];
            }
        }

        [Benchmark]
        public void LinkedListAccess()
        {
            for (int i = 0; i < N; i++)
            {
                var x = llist.ElementAt(i);
            }
        }
    }

    public class Benchmark
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompareChambers>();
            // var summary = BenchmarkRunner.Run<AllocateVsCopy>();
            // var summary = BenchmarkRunner.Run<ElementAtVsIterate>();
            // var summary = BenchmarkRunner.Run<ListVsLinkedList>();
        }
    }
}

