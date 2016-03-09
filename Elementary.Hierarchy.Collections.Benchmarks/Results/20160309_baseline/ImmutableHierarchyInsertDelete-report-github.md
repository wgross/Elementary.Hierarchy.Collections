```ini
BenchmarkDotNet=v0.9.2.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
Frequency=2435552 ticks, Resolution=410.5845 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE

Type=ImmutableHierarchyInsertDelete  Mode=Throughput  

```
               Method |   Median |   StdDev |
--------------------- |--------- |--------- |
 InsertAndDeleteNodes | 1.8286 s | 0.0224 s |
