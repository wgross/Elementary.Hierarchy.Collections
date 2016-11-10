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
    InsertAndDeleteNodesDeepWithPruning | 49.1232 ms | 3.9917 ms |
               InsertAndDeleteNodesDeep | 50.0804 ms | 1.4417 ms |
 InsertAndDeleteNodesShallowWithPruning | 44.6469 ms | 0.7124 ms |
            InsertAndDeleteNodesShallow | 43.9915 ms | 0.1425 ms |
