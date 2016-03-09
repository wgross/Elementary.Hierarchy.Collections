using BenchmarkDotNet.Attributes;
using Elementary.Hierarchy;
using Elementary.Hierarchy.Collections;
using System;

namespace Elementary.Hierarchy.Collections.Benchmarks
{
    public class ImmutableHierarchyInsertDelete
    {
        private int[] values = new int[500006];

        private ImmutableHierarchy<int, int> hierarchy = new ImmutableHierarchy<int, int>();

        [Setup]
        public void ConfigPayload()
        {
            // create an array to take 50000 keys of size 5 from
            var rand = new Random();
            for (int i = 0; i < 500006; i++)
                this.values[i] = rand.Next();
        }

        [Benchmark]
        public void InsertAndDeleteNodes()
        {
            for (int i = 0; i < 500000; i++)
                this.hierarchy.Add(
                    HierarchyPath.Create(this.values[i], this.values[i + 1], this.values[i + 2], this.values[i + 3], this.values[i + 4], this.values[i + 5], this.values[i + 6]),
                    value: i);
        }
    }
}