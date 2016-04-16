namespace Elementary.Hierarchy.Collections
{
    public interface IHierarchyNode<TKey,TValue> :
        IHasChildNodes<IHierarchyNode<TKey,TValue>>,
        IHasParentNode<IHierarchyNode<TKey,TValue>>
    {
        HierarchyPath<TKey> Path { get; }

        bool HasValue { get; }

        TValue Value { get;}
    }
}