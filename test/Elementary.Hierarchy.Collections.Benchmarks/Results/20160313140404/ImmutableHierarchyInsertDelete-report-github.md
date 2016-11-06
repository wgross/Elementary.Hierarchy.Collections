```ini
BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435527 ticks, Resolution=410.5888 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=ImmutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

```
                                 Method |     Median |    StdDev |
--------------------------------------- |----------- |---------- |
    InsertAndDeleteNodesDeepWithPruning | 51.3926 ms | 1.0543 ms |
               InsertAndDeleteNodesDeep | 50.0391 ms | 0.3383 ms |
 InsertAndDeleteNodesShallowWithPruning | 46.0781 ms | 1.1184 ms |
            InsertAndDeleteNodesShallow | 45.8566 ms | 0.5330 ms |
