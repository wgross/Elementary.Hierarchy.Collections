namespace Elementary.Hierarchy.Collections
{
    public interface IHierarchyNode<TKey> :
        IHasChildNodes<IHierarchyNode<TKey>>,
        IHasParentNode<IHierarchyNode<TKey>>
    {
        HierarchyPath<TKey> Path { get; }
    }
}