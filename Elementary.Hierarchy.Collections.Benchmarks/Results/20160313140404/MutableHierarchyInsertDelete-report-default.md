BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435527 ticks, Resolution=410.5888 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=MutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

                                 Method |     Median |    StdDev |
--------------------------------------- |----------- |---------- |
    InsertAndDeleteNodesDeepWithPruning | 32.2229 ms | 0.5806 ms |
               InsertAndDeleteNodesDeep | 31.4727 ms | 0.4286 ms |
 InsertAndDeleteNodesShallowWithPruning | 31.1127 ms | 0.5943 ms |
            InsertAndDeleteNodesShallow | 29.5190 ms | 0.2633 ms |
