```ini
BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435542 ticks, Resolution=410.5862 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=ImmutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

```
                                 Method |     Median |    StdDev |
--------------------------------------- |----------- |---------- |
    InsertAndDeleteNodesDeepWithPruning | 51.0661 ms | 1.7943 ms |
               InsertAndDeleteNodesDeep | 49.9014 ms | 0.6155 ms |
 InsertAndDeleteNodesShallowWithPruning | 45.1150 ms | 1.2673 ms |
            InsertAndDeleteNodesShallow | 45.8256 ms | 0.6477 ms |
