namespace Elementary.Hierarchy.Collections
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// An immutable hierachy holds a set of value but never changes ists structure.
    /// Any strcuture change like adding or removing a node produces a new hierarchy.
    /// The data is copied wothe the nodes. If TValue is a reference type all hierechay reference the same data
    /// If TValue is a value type thevalues are cpied with their nodes.
    /// </summary>
    /// <typeparam name="TKey">type of the indetifier of the stires data</typeparam>s
    /// <typeparam name="TNode"></typeparam>
    public class MutableHierarchy<TKey, TValue>
    {
        /// <summary>
        /// Internal node class: holds a value and child nodes.
        /// </summary>
        [DebuggerDisplay("id={id},hasValue={HasValue},value={value}")]
        public sealed class Node : IHasIdentifiableChildNodes<TKey, Node>
        {
            /// <summary>
            /// Null value marker
            /// </summary>
            private static readonly object ValueNotSet = new object();

            #region Construction and initialization of this instance

            public Node(TKey id)
            {
                this.id = id;
                this.value = ValueNotSet;
                this.childNodes = new Node[0];
            }

            public Node(TKey id, object value)
            {
                this.id = id;
                this.value = value;
                this.childNodes = new Node[0];
            }

            public Node(TKey id, object value, IEnumerable<Node> childNodes)
            {
                this.id = id;
                this.value = value;
                this.childNodes = childNodes.ToArray();
            }

            public readonly TKey id;

            public object value = ValueNotSet;

            private Node[] childNodes;

            #endregion Construction and initialization of this instance

            #region IHasChildNodes Members

            public bool HasChildNodes => this.childNodes.Any();

            public IEnumerable<Node> ChildNodes => this.childNodes;

            #endregion IHasChildNodes Members

            #region IHasIdentifiableChildNodes Members

            public bool TryGetChildNode(TKey id, out Node childNode)
            {
                childNode = this.childNodes.SingleOrDefault(n => EqualityComparer<TKey>.Default.Equals(n.id, id));
                return childNode != null;
            }

            #endregion IHasIdentifiableChildNodes Members

            public bool HasValue => this.value != ValueNotSet;

            public Node AddChildNode(Node newChildNode)
            {
                // copy the existing children to a new array, and append the new one.
                Node[] newChildNodes = new Node[this.childNodes.Length + 1];
                Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);
                newChildNodes[this.childNodes.Length] = newChildNode;
                // set nw chile array instead of current child node array
                this.childNodes = newChildNodes;
                return this;
            }

            public Node SetChildNode(Node newChildNode)
            {
                for (int i = 0; i < this.childNodes.Length; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(this.childNodes[i].id, newChildNode.id))
                    {
                        //substitute the existing child node with the new one.
                        this.childNodes[i] = newChildNode;
                        return this;
                    }
                }
                throw new InvalidOperationException($"The node (id={newChildNode.id}) doesn't substutite any of the existing child nodes in (id={this.id})");
            }

            public Node SetValue(TValue value)
            {
                this.value = value;
                return this;
            }

            /// <summary>
            /// Gets value from node. If value is set treu is returned and the out parameter ist set.
            /// </summary>
            /// <param name="value"></param>
            /// <returns>true if node has a value</returns>
            public bool TryGetValue(out TValue value)
            {
                value = default(TValue);
                if (!this.HasValue)
                    return false;

                value = (TValue)this.value;
                return true;
            }

            /// <summary>
            /// Unset ths value of this this node instance.
            /// </summary>
            /// <returns></returns>
            public Node UnsetValue(bool prune = false)
            {
                this.value = ValueNotSet;

                // pruning is swtched on.
                // if none of the descandant has a value anymore, the children are deleted

                if (prune)
                    if (!this.Descendants().Any(d => d.HasValue))
                        this.ClearChildNodes();

                return this;
            }

            /// <summary>
            /// Forgets all child nodes.
            /// </summary>
            private void ClearChildNodes()
            {
                this.childNodes = new Node[] { };
            }
        }

        #region Construction and initialization of this instance

        public MutableHierarchy()
            : this(new Node(default(TKey)), pruneOnUnsetValue: false)
        {
        }

        public MutableHierarchy(bool pruneOnUnsetValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: pruneOnUnsetValue)
        {
        }

        private MutableHierarchy(Node rootNode, bool pruneOnUnsetValue)
        {
            this.rootNode = rootNode;
            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private readonly Node rootNode;

        private readonly bool pruneOnUnsetValue;

        #endregion Construction and initialization of this instance

        #region Add/Set a hierarchy nodes value

        /// <summary>
        /// Set the value of the specified node of teh hierarchy.
        /// if the node doesn't exist, it is created.
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns></returns>
        public TValue this[HierarchyPath<TKey> hierarchyPath]
        {
            set
            {
                this.GetOrCreateNode(hierarchyPath).SetValue(value);
            }
        }

        /// <summary>
        /// Adds a value to the immutable hierarchy at the specified position.
        /// </summary>
        /// <param name="hierarchyPath">Specifies where to set the value</param>
        /// <param name="value">the value to keep</param>
        /// <returns>returns this</returns>
        public void Add(HierarchyPath<TKey> hierarchyPath, TValue value)
        {
            var nodeToSetValueAt = this.GetOrCreateNode(hierarchyPath);

            if (nodeToSetValueAt.HasValue)
                throw new ArgumentException($"Node at '{hierarchyPath}' has already a value");

            nodeToSetValueAt.SetValue(value);
        }

        private Node GetOrCreateNode(HierarchyPath<TKey> hierarchyPath)
        {
            return this.rootNode.DescendantAt(delegate (Node current, TKey key, out Node child)
            {
                // if the chiiled ic not found, just create a new one on-the-fly
                if (!current.TryGetChildNode(key, out child))
                    current.AddChildNode(child = new Node(key));
                return true;
            }, hierarchyPath);
        }

        #endregion Add/Set a hierarchy nodes value

        /// <summary>
        /// Retrieves the nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            Node descendantNode;
            if (this.rootNode.TryGetDescendantAt(hierarchyPath, out descendantNode))
                return descendantNode.TryGetValue(out value);

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the value from the specified node in hierarchy.
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns>true if value was removed, false otherwise</returns>
        public bool Remove(HierarchyPath<TKey> hierarchyPath)
        {
            Node node;
            if (!this.rootNode.TryGetDescendantAt(hierarchyPath, out node))
                return false;

            if (!node.HasValue)
                return false;

            node.UnsetValue(prune: this.pruneOnUnsetValue);
            return true;
        }
    }
}