namespace Elementary.Hierarchy.Collections
{
    using System.Collections.Generic;

    public abstract class HierarchyNodeBase<TKey, TChildNode> : IHierarchyNode<TKey, TChildNode>
        where TChildNode : HierarchyNodeBase<TKey, TChildNode>
    {
        #region Construction and initialization of this instance

        public HierarchyNodeBase(TChildNode parent, TKey localPath)
        {
            this.ParentNode = parent;
            this.LocalPath = localPath;
        }

        public TKey LocalPath { get; private set; }

        private readonly Dictionary<TKey, TChildNode> children = new Dictionary<TKey, TChildNode>();

        #endregion Construction and initialization of this instance

        #region IHasChildNodes<TItem> Members

        public abstract bool HasChildNodes
        {
            get;
        }

        public abstract IEnumerable<TChildNode> ChildNodes
        {
            get;
        }

        #endregion IHasChildNodes<TItem> Members

        #region IHasIdentifiableChildNodes<TKey,HierarchyItem<TKey,TValue>> Members

        public abstract bool TryGetChildNode(TKey id, out TChildNode childItem);

        #endregion IHasIdentifiableChildNodes<TKey,HierarchyItem<TKey,TValue>> Members

        #region IHasParentNode<TItem> Members

        virtual public bool HasParentNode => this.ParentNode != null;

        virtual public TChildNode ParentNode
        {
            get;
        }

        #endregion IHasParentNode<TItem> Members

        #region IHierarchyNode<TKey,TItem> Members

        public void ChangeLocalPath(TKey newLocalPath)
        {
            this.LocalPath = newLocalPath;
        }

        abstract public TChildNode AddChildNode(TChildNode newChild);

        abstract public bool RemoveChildNode(TChildNode nodeToRemove);

        abstract public void ClearChildNodes();

        #endregion IHierarchyNode<TKey,TItem> Members
    }
}