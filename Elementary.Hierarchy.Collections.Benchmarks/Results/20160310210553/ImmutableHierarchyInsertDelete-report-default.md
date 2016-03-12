BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435521 ticks, Resolution=410.5898 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=ImmutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

                      Method |        Median |      StdDev |
---------------------------- |-------------- |------------ |
    InsertAndDeleteNodesDeep | 1,096.2574 ms | 198.1914 ms |
 InsertAndDeleteNodesShallow |   801.5948 ms | 127.6317 ms |
