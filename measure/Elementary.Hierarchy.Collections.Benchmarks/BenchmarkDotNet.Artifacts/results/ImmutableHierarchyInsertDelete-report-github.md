```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=4
Frequency=2435765 ticks, Resolution=410.5486 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003131

Type=ImmutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

```
                                 Method |     Median |    StdDev |
--------------------------------------- |----------- |---------- |
               InsertAndDeleteNodesDeep | 30.4994 ms | 0.1621 ms |
    InsertAndDeleteNodesDeepWithPruning | 40.3808 ms | 1.0649 ms |
            InsertAndDeleteNodesShallow | 27.3550 ms | 1.2451 ms |
 InsertAndDeleteNodesShallowWithPruning |         NA |        NA |

Benchmarks with issues:
  ImmutableHierarchyInsertDelete_InsertAndDeleteNodesShallowWithPruning_TargetCount5
