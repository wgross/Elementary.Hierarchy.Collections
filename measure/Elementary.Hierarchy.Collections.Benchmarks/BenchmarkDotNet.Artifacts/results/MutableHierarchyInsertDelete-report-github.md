```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=4
Frequency=2435765 ticks, Resolution=410.5486 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003131

Type=MutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  

```
                                 Method | Median | StdDev |
--------------------------------------- |------- |------- |
               InsertAndDeleteNodesDeep |     NA |     NA |
    InsertAndDeleteNodesDeepWithPruning |     NA |     NA |
            InsertAndDeleteNodesShallow |     NA |     NA |
 InsertAndDeleteNodesShallowWithPruning |     NA |     NA |

Benchmarks with issues:
  MutableHierarchyInsertDelete_InsertAndDeleteNodesDeep_TargetCount5
  MutableHierarchyInsertDelete_InsertAndDeleteNodesDeepWithPruning_TargetCount5
  MutableHierarchyInsertDelete_InsertAndDeleteNodesShallow_TargetCount5
  MutableHierarchyInsertDelete_InsertAndDeleteNodesShallowWithPruning_TargetCount5
