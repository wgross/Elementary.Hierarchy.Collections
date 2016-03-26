namespace Elementary.Hierarchy.Collections
{
    using Elementary.Hierarchy;
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// An immutable hierachy holds a set of value but never changes ists structure.
    /// Any strcuture change like adding or removing a node produces a new hierarchy.
    /// The data is copied wothe the nodes. If TValue is a reference type all hierechay reference the same data
    /// If TValue is a value type thevalues are cpied with their nodes.
    /// </summary>
    /// <typeparam name="TKey">type of the indetifier of the stires data</typeparam>s
    /// <typeparam name="TNode"></typeparam>
    public class ImmutableHierarchy<TKey, TValue>
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

            public readonly object value = ValueNotSet;

            private readonly Node[] childNodes;

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

                // return a new node as susbtitute for this node to add to the new parent
                return new Node(this.id, this.value, newChildNodes);
            }

            public Node SetChildNode(Node newChildNode)
            {
                Node[] newChildNodes = new Node[this.childNodes.Length];
                Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);

                for (int i = 0; i < newChildNodes.Length; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(newChildNodes[i].id, newChildNode.id))
                    {
                        // the node is already child of this node -> just return this node as changed node.
                        if (object.ReferenceEquals(newChildNodes[i], newChildNode))
                            return this;

                        //substitute the existing child node with the new one.
                        newChildNodes[i] = newChildNode;

                        // return a sunstitut for this node contains the new child node.
                        return new Node(this.id, this.value, newChildNodes);
                    }
                }
                throw new InvalidOperationException($"The node (id={newChildNode.id}) doesn't substutite any of the existing child nodes in (id={this.id})");
            }

            public Node SetValue(TValue value)
            {
                // equality comparer fails with exception if ValueNotSet is compares woth string value.
                // therfore check first of there is a value at all.
                if (this.HasValue && EqualityComparer<TValue>.Default.Equals((TValue)this.value, value))
                    return this;
                else
                    return new Node(this.id, (object)value, this.childNodes);
            }

            public bool TryGetValue(out TValue value)
            {
                value = default(TValue);
                if (!this.HasValue)
                    return false;

                value = (TValue)this.value;
                return true;
            }

            /// <summary>
            /// Creates a new node as clone of this node without a value
            /// If prune is enabled, the child nodes are abandoned if non of them has a value.
            /// </summary>
            /// <returns></returns>
            public Node UnsetValue(bool prune = false)
            {
                if (!this.HasValue)
                    return this;

                // pruning is swtched on.
                // if none of the descandant has a value anymore, the children are deleted

                if (prune)
                    if (!this.Descendants().Any(c => c.HasValue))
                        return new Node(this.id, ValueNotSet);

                // return a clone of this node, changed only at its value

                return new Node(this.id, ValueNotSet, this.childNodes);
            }
        }

        #region Construction and initialization of this instance

        public ImmutableHierarchy()
            : this(new Node(default(TKey)), pruneOnUnsetValue: false)
        {
        }

        public ImmutableHierarchy(bool pruneOnUnsetValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: pruneOnUnsetValue)
        {
        }

        private ImmutableHierarchy(Node rootNode, bool pruneOnUnsetValue)
        {
            this.rootNode = rootNode;
            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private Node rootNode;

        private readonly bool pruneOnUnsetValue;

        // MSDN: Do not store SpinLock instances in readonly fields.
        private SpinLock writeLock = new SpinLock();

        private ImmutableHierarchy<TKey, TValue> CreateIfRootHasChanged(Node newRoot)
        {
            if (object.ReferenceEquals(this.rootNode, newRoot))
                return this;

            return new ImmutableHierarchy<TKey, TValue>(newRoot, this.pruneOnUnsetValue);
        }

        #endregion Construction and initialization of this instance

        #region Add/Set value

        /// <summary>
        /// Set the value of the specified node of the hierarchy.
        /// if the node doesn't exist, it is created.
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns></returns>
        public TValue this[HierarchyPath<TKey> hierarchyPath]
        {
            set
            {
                bool isLocked = false;
                try
                {
                    // this.writeLock.Enter(ref isLocked);

                    Stack<Node> nodesAlongPath;
                    this.rootNode = this.RebuildAscendingPathAfterChange(
                        this.GetOrCreateNode(hierarchyPath, out nodesAlongPath).SetValue(value), nodesAlongPath);
                }
                finally
                {
                    if (isLocked)
                        this.writeLock.Exit();
                }
            }
        }

        /// <summary>
        /// Adds a value to the immutable hierachy at the specified position.
        /// The result is a new ImmutableHiarachy contains the value. The
        /// old one is unchanged.
        /// If the value is equal to the value already stored at the position the hierachy remains unchanged.
        /// </summary>
        /// <param name="hierarchyPath">Specifies where to set the value</param>
        /// <param name="value">the value to keep</param>
        /// <returns>Am immutable hierach which contains the specified value</returns>
        public void Add(HierarchyPath<TKey> hierarchyPath, TValue value)
        {
            bool isLocked = false;
            try
            {
                // this.writeLock.Enter(ref isLocked);

                // Set the value at the destination node. The clone may substitute the current node.

                Stack<Node> nodesAlongPath;
                var currentNode = this.GetOrCreateNode(hierarchyPath, out nodesAlongPath);

                // if the node has already a value, add gails woth argument exception

                if (currentNode.HasValue)
                    throw new ArgumentException($"Node at '{hierarchyPath}' already has a value");

                // now ascend again to the root and clone new parent node for the newly created child nodes.

                this.rootNode = this.RebuildAscendingPathAfterChange(currentNode.SetValue(value), nodesAlongPath);
            }
            finally
            {
                if (isLocked)
                    this.writeLock.Exit();
            }
        }

        private Node GetOrCreateNode(HierarchyPath<TKey> hierarchyPath, out Stack<Node> nodesAlongPath)
        {
            nodesAlongPath = null;

            // Create the new value node with the given value and the leaf id.

            var tmp = new Stack<Node>();

            // Descend in to the tree, create nodes along the way if they are missing

            var result = this.rootNode.DescendantAt(delegate (Node parentNode, TKey key, out Node childNode)
           {
               // get or create child. If created the clone of the parent node substitutes the
               // old parent node.

               if (!parentNode.TryGetChildNode(key, out childNode))
                   parentNode = parentNode.AddChildNode(childNode = new Node(key));

               // Remember all the nodes passing along for later rebuild of the changed
               // hierarchy path

               tmp.Push(parentNode);
               return true;
           },
            hierarchyPath);

            nodesAlongPath = tmp;
            return result;
        }

        private Node RebuildAscendingPathAfterChange(Node changedChildNode, Stack<Node> nodesAlongPath)
        {
            var reconnectedParentNode = changedChildNode;

            // ascend to the root and copy-on-change the ancestor nodes.

            while (nodesAlongPath.Any())
                reconnectedParentNode = nodesAlongPath.Pop().SetChildNode(reconnectedParentNode);

            // this is the new (or unchanged node)
            return reconnectedParentNode;
        }

        #endregion Add/Set value

        /// <summary>
        /// Retrieves the nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            Node descendantNode;
            Stack<Node> nodesAlongPath;

            if (this.TryGetNode(hierarchyPath, out nodesAlongPath, out descendantNode))
                return descendantNode.TryGetValue(out value);

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the value from the specified node in hierarchy.
        /// Value and nodes on under the specified nde remain unchanged
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns>true if value was removed</returns>
        public bool Remove(HierarchyPath<TKey> hierarchyPath)
        {
            Stack<Node> nodesAlongPath = new Stack<Node>();
            Node currentNode;

            // try to get the node to remove values from

            if (!this.TryGetNode(hierarchyPath, out nodesAlongPath, out currentNode))
                return false;

            // If node hasn't a value Remove is finshed. Just return false

            if (!currentNode.HasValue)
                return false;

            // last node must be the root node: create new hierachy if root node has changed
            bool isLocked = false;
            try
            {
                // this.writeLock.Enter(ref isLocked);
                this.rootNode = this.RebuildAscendingPathAfterChange(currentNode.UnsetValue(prune: this.pruneOnUnsetValue), nodesAlongPath);
                return true;
            }
            finally
            {
                if (isLocked)
                    this.writeLock.Exit();
            }
        }

        private bool TryGetNode(HierarchyPath<TKey> hierarchyPath, out Stack<Node> nodesAlongPath, out Node node)
        {
            node = null;
            Stack<Node> tmp = new Stack<Node>();

            // now find the the value node and the path to reach it

            bool found = this.rootNode.TryGetDescendantAt(tryGetChildNode: delegate (Node parent, TKey key, out Node child)
            {
                // from parent node try to retrieve the child with the local id from key

                child = null;
                if (!parent.TryGetChildNode(key, out child))
                    return false;

                // remember parent was passed on the way down

                tmp.Push(parent);
                return true;
            },
            key: hierarchyPath, descendantAt: out node);

            nodesAlongPath = tmp;
            return found;
        }
    }
}