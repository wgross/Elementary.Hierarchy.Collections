using BenchmarkDotNet.Running;

namespace Elementary.Hierarchy.Collections.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<ImmutableHierarchyInsertDelete>();
            BenchmarkRunner.Run<MutableHierarchyInsertDelete>();
        }
    }
}