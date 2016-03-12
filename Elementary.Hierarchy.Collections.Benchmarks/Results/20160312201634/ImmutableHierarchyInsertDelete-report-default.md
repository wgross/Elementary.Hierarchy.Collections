BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435535 ticks, Resolution=410.5874 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=ImmutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

                      Method |        Median |      StdDev |
---------------------------- |-------------- |------------ |
    InsertAndDeleteNodesDeep | 1,099.9738 ms | 225.0200 ms |
 InsertAndDeleteNodesShallow |   795.6559 ms | 119.8189 ms |
