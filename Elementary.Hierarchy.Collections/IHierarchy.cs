namespace Elementary.Hierarchy.Collections
{
    public interface IHierarchy<TKey, TValue>
    {
        TValue this[HierarchyPath<TKey> hierarchyPath] { set; }

        void Add(HierarchyPath<TKey> hierarchyPath, TValue value);

        bool Remove(HierarchyPath<TKey> hierarchyPath);

        bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value);

        IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt);
    }
}