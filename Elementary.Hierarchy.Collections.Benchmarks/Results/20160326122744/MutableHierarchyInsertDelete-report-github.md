```ini
BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435542 ticks, Resolution=410.5862 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=MutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

```
                                 Method |     Median |    StdDev |
--------------------------------------- |----------- |---------- |
    InsertAndDeleteNodesDeepWithPruning | 32.5297 ms | 0.7030 ms |
               InsertAndDeleteNodesDeep | 30.4470 ms | 0.1702 ms |
 InsertAndDeleteNodesShallowWithPruning | 28.7095 ms | 0.4031 ms |
            InsertAndDeleteNodesShallow | 29.1791 ms | 0.6269 ms |
