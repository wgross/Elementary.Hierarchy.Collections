    BenchmarkDotNet=v0.9.2.0
    OS=Microsoft Windows NT 6.2.9200.0
    Processor=Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz, ProcessorCount=4
    Frequency=2435542 ticks, Resolution=410.5862 ns
    HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
    
    Type=MutableHierarchyInsertDelete  Mode=Throughput  TargetCount=5  
    
                                     Method |     Median |    StdDev |
    --------------------------------------- |----------- |---------- |
        InsertAndDeleteNodesDeepWithPruning | 31.0464 ms | 0.3799 ms |
                   InsertAndDeleteNodesDeep | 31.1074 ms | 0.3139 ms |
     InsertAndDeleteNodesShallowWithPruning | 29.0923 ms | 0.6980 ms |
                InsertAndDeleteNodesShallow | 28.3221 ms | 0.1278 ms |
