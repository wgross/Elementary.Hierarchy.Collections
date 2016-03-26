# Elementary.Hierarchy.Collections

Provides a set of hierarchical collections based on the interfaces defined in [Elementary.Hierarchy](https://github.com/wgross/Elementary.Hierarchy).

# Installation 

Elementary.Hierarchy.Collections is easily installed using NuGet
```
Install-Package Elementary.Hierarchy.Collections
```

# Hierarchical Collections provided

## MutableHierarchy<TKey,TValue>

A straightforward implementaion of a hierarchical key-value store following the example of the Frameworks Dictionary<TKey,TValue>.

## ImmutableHierarchy<TKey,TValue>

A hierarchical key-value store targeted to multithreaded environments. Changes are not done inplace but cause copy of the part of the hierachy which is changed.
The 'copy-on-write' scheme is continued until the root node of the ImmutableHierarchy is exchanged with a modified clone referencing a hierarchy consisting of the old uncganged nodes and the new modified ones.

