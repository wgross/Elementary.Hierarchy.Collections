namespace Elementary.Hierarchy.Collections
{
    public interface IHierarchy<TKey, TValue>
    {
        TValue this[HierarchyPath<TKey> hierarchyPath] { set; }

        void Add(HierarchyPath<TKey> hierarchyPath, TValue value);

        /// <summary>
        /// Removes values from the hierarchy by default.
        /// If a maxDepth s spefices > 1 vaues are removed from
        /// descenadnt nodes.
        /// </summary>
        /// <param name="hierarchyPath">specifed the position of the start node for removal of values</param>
        /// <param name="maxDepth">dept of value removal. 1 removes ath te specified position only, > 1 removes at descendants, 0 removes nothing</param>
        /// <returns>true if at least one level was removed, false otherwise</returns>
        bool Remove(HierarchyPath<TKey> hierarchyPath, int? maxDepth = null);

        bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value);

        IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt);
    }
}