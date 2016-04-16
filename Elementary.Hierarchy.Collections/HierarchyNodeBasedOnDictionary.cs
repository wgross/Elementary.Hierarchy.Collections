//namespace Elementary.Hierarchy.Collections
//{
//    using System.Collections.Generic;
//    using System.Linq;

//    public abstract class HierarchyNodeBasedOnDictionary<TKey, TItem> : HierarchyNodeBase<TKey, TItem>
//        where TItem : HierarchyNodeBase<TKey, TItem>
//    {
//        #region Construction and initialization of this instance

//        public HierarchyNodeBasedOnDictionary(TItem parent, TKey localPath)
//            : base(parent, localPath)
//        { }

//        private readonly Dictionary<TKey, TItem> children = new Dictionary<TKey, TItem>();

//        #endregion Construction and initialization of this instance

//        #region IHasChildNodes<TItem> Members

//        override public bool HasChildNodes
//        {
//            get { return this.children.Any(); }
//        }

//        override public IEnumerable<TItem> ChildNodes
//        {
//            get { return this.children.Values; }
//        }

//        #endregion IHasChildNodes<TItem> Members

//        #region IHasIdentifiableChildNodes<TKey,HierarchyItem<TKey,TValue>> Members

//        override public bool TryGetChildNode(TKey id, out TItem childItem)
//        {
//            return this.children.TryGetValue(id, out childItem);
//        }

//        #endregion IHasIdentifiableChildNodes<TKey,HierarchyItem<TKey,TValue>> Members

//        override public TItem AddChildNode(TItem newChild)
//        {
//            this.children[newChild.LocalPath] = newChild;
//            return newChild;
//        }

//        override public bool RemoveChildNode(TItem nodeToRemove)
//        {
//            return this.children.Remove(nodeToRemove.LocalPath);
//        }

//        override public void ClearChildNodes()
//        {
//            this.children.Clear();
//        }
//    }
//}