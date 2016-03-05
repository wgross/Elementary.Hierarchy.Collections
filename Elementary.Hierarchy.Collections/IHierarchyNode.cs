
namespace Elementary.Hierarchy.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IHierarchyNode<TKey, TItem> :
        // Support the public hierarchy traversal methods
        IHasChildNodes<TItem>,
        IHasIdentifiableChildNodes<TKey, TItem>,
        IHasParentNode<TItem>
        where TItem : IHierarchyNode<TKey, TItem>
    {
        TKey LocalPath { get; }

        TItem AddChildNode(TItem newChild);

        void ClearChildNodes();
    }
}
