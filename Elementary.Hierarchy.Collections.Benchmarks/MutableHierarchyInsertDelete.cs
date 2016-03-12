using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using System;

namespace Elementary.Hierarchy.Collections.Benchmarks
{
    [Config(typeof(Config))]
    public class MutableHierarchyInsertDelete
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(Job.Default.WithTargetCount(5));
            }
        }

        private int[] values = new int[500006];
        private const int loops = 2500;

        private MutableHierarchy<int, int> hierarchy = new MutableHierarchy<int, int>();

        [Setup]
        public void ConfigPayload()
        {
            // create an array to take 50000 keys of size 5 from
            var rand = new Random();
            for (int i = 0; i < 500006; i++)
                this.values[i] = rand.Next();
        }

        [Benchmark(OperationsPerInvoke = 5)]
        public void InsertAndDeleteNodesDeep()
        {
            for (int i = 0; i < loops; i++)
                this.hierarchy = this.hierarchy.Add(
                    HierarchyPath.Create(this.values[i], this.values[i + 1], this.values[i + 2], this.values[i + 3], this.values[i + 4], this.values[i + 5], this.values[i + 6]),
                    value: i);

            for (int i = 0; i < loops; i++)
                this.hierarchy = this.hierarchy.Remove(
                    HierarchyPath.Create(this.values[i], this.values[i + 1], this.values[i + 2], this.values[i + 3], this.values[i + 4], this.values[i + 5], this.values[i + 6]));
        }

        [Benchmark(OperationsPerInvoke = 5)]
        public void InsertAndDeleteNodesShallow()
        {
            for (int i = 0; i < loops; i++)
                this.hierarchy = this.hierarchy.Add(
                    HierarchyPath.Create(this.values[i], this.values[i + 1]),
                    value: i);

            for (int i = 0; i < loops; i++)
                this.hierarchy = this.hierarchy.Remove(
                    HierarchyPath.Create(this.values[i], this.values[i + 1]));
        }
    }
}