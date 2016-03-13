# Elementary.Hierarchy.Collections

The Elementary.Hierarchy.Collections provides data strautures for storeing arbitrary data in an tree-structured data strcuture.
The structure of the tree isn't balanced but instead follows the path-like identifier like a file system does. 
Therefore I named it 'Hierarchy' and not 'tree'.

# Dependencies

Elementary.Hierarchy.Collections depends on the [Elementary.Hierarchy project](https://github.com/wgross/Elementary.Hierarchy). Elementary.Hierarchy provides basic interfaces (IHasChildNodes, IHasParentNode etc.) and data structures (HierarchyPath<T>) for implementation of trees and also provides tree-traversal algorithms like DescendantAt(..).

