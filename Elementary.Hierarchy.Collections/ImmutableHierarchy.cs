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
    /// An immutable hierarchy holds a set of values but never changes its nodes in place.
    /// Any change of the node structure or the value of a node causes a copy of the node and rebuilding
    /// of the path of tha ancestors including the root node to create a new hierarchy referencing both the
    /// old unchanged nodes and the new changed nodes.
    /// In a multithreaded environment reading is still possible while a change is happening in parallel
    /// </summary>
    /// <typeparam name="TKey">type of the identifier of the stored data</typeparam>s
    /// <typeparam name="TNode"></typeparam>
    public class ImmutableHierarchy<TKey, TValue> : IHierarchy<TKey, TValue>
    {
        #region Internal Node class

        /// <summary>
        /// Internal node class: holds a value and child nodes.
        /// </summary>
        [DebuggerDisplay("key={key},hasValue={HasValue},value={value}")]
        public sealed class Node : IHasIdentifiableChildNodes<TKey, Node>
        {
            /// <summary>
            /// Null value marker
            /// </summary>
            private static readonly object ValueNotSet = new object();

            #region Construction and initialization of this instance

            public Node(TKey key)
            {
                this.key = key;
                this.value = ValueNotSet;
                this.childNodes = new Node[0];
            }

            public Node(TKey key, object value)
            {
                this.key = key;
                this.value = value;
                this.childNodes = new Node[0];
            }

            public Node(TKey key, object value, IEnumerable<Node> childNodes)
            {
                this.key = key;
                this.value = value;
                this.childNodes = childNodes.ToArray();
            }

            public readonly TKey key;

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
                childNode = this.childNodes.SingleOrDefault(n => EqualityComparer<TKey>.Default.Equals(n.key, id));
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
                return new Node(this.key, this.value, newChildNodes);
            }

            public Node SetChildNode(Node newChildNode)
            {
                Node[] newChildNodes = new Node[this.childNodes.Length];
                Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);

                for (int i = 0; i < newChildNodes.Length; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(newChildNodes[i].key, newChildNode.key))
                    {
                        // the node is already child of this node -> just return this node as changed node.
                        if (object.ReferenceEquals(newChildNodes[i], newChildNode))
                            return this;

                        //substitute the existing child node with the new one.
                        newChildNodes[i] = newChildNode;

                        // return a sunstitut for this node contains the new child node.
                        return new Node(this.key, this.value, newChildNodes);
                    }
                }
                throw new InvalidOperationException($"The node (id={newChildNode.key}) doesn't substutite any of the existing child nodes in (id={this.key})");
            }

            public Node SetValue(TValue value)
            {
                // equality comparer fails with exception if ValueNotSet is compares woth string value.
                // therfore check first of there is a value at all.
                if (this.HasValue && EqualityComparer<TValue>.Default.Equals((TValue)this.value, value))
                    return this;
                else
                    return new Node(this.key, (object)value, this.childNodes);
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
                        return new Node(this.key, ValueNotSet);

                // return a clone of this node, changed only at its value

                return new Node(this.key, ValueNotSet, this.childNodes);
            }
        }

        #endregion Internal Node class

        #region Construction and initialization of this instance

        public ImmutableHierarchy()
            : this(new Node(default(TKey)), pruneOnUnsetValue: false, getDefaultValue: null)
        {
        }

        public ImmutableHierarchy(Func<HierarchyPath<TKey>, TValue> getDefaultValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: false, getDefaultValue: getDefaultValue)
        {
        }

        public ImmutableHierarchy(bool pruneOnUnsetValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: pruneOnUnsetValue, getDefaultValue: null)
        {
        }

        public ImmutableHierarchy(bool pruneOnUnsetValue, Func<HierarchyPath<TKey>, TValue> getDefaultValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: pruneOnUnsetValue, getDefaultValue: getDefaultValue)
        {
        }

        private ImmutableHierarchy(Node rootNode, bool pruneOnUnsetValue, Func<HierarchyPath<TKey>, TValue> getDefaultValue)
        {
            this.getDefaultValue = getDefaultValue;

            if (this.getDefaultValue != null)
                this.rootNode = rootNode.SetValue(this.getDefaultValue(HierarchyPath.Create<TKey>()));
            else
                this.rootNode = rootNode;

            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private Node rootNode;

        private readonly bool pruneOnUnsetValue;

        private readonly Func<HierarchyPath<TKey>, TValue> getDefaultValue;

        // MSDN: Do not store SpinLock instances in readonly fields.
        private SpinLock writeLock = new SpinLock();

        private ImmutableHierarchy<TKey, TValue> CreateIfRootHasChanged(Node newRoot)
        {
            if (object.ReferenceEquals(this.rootNode, newRoot))
                return this;

            return new ImmutableHierarchy<TKey, TValue>(newRoot, this.pruneOnUnsetValue, this.getDefaultValue);
        }

        #endregion Construction and initialization of this instance

        #region Hierarchy Node Traversal

        public sealed class Traverser : IHierarchyNode<TKey, TValue>
        {
            private readonly Traverser parentTraverser;
            private readonly Node node;
            private readonly Lazy<HierarchyPath<TKey>> path;

            public Traverser(Node rootNode)
            {
                this.parentTraverser = null;
                this.node = rootNode;
                this.path = new Lazy<HierarchyPath<TKey>>(() => HierarchyPath.Create<TKey>(), isThreadSafe: false);
            }

            public Traverser(Traverser parentTraverser, Node rootNode)
            {
                if (parentTraverser == null)
                    throw new ArgumentNullException(nameof(parentTraverser));

                this.parentTraverser = parentTraverser;
                this.node = rootNode;
                this.path = new Lazy<HierarchyPath<TKey>>(() => this.parentTraverser.Path.Join(this.node.key), isThreadSafe: false);
            }

            public IEnumerable<IHierarchyNode<TKey, TValue>> ChildNodes => this.node.Children().Select(c => new Traverser(this, c));

            public bool HasChildNodes => this.node.HasChildNodes;

            public bool HasParentNode => this.parentTraverser != null;

            public IHierarchyNode<TKey, TValue> ParentNode => this.parentTraverser;

            public HierarchyPath<TKey> Path => this.path.Value;

            public bool HasValue => this.node.HasValue;

            public TValue Value => (TValue)this.node.value;

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                    return true;

                var objAsTraverser = obj as Traverser;
                if (objAsTraverser == null)
                    return false;

                // Traversers are equals if the point to the same node
                return object.ReferenceEquals(this.node, objAsTraverser.node);
            }

            public override int GetHashCode()
            {
                return this.node.GetHashCode();
            }
        }

        /// <summary>
        /// Starts a traversal of the hierarchy at the root node.
        /// </summary>
        /// <returns>A traversable representation of the root node</returns>
        public IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt)
        {
            Traverser startNode = new Traverser(this.rootNode);

            // Descend along the soecifed path and buidl ap teh chain of ancestors of the start node.
            // if the start node can't be reached because it doesn't exist in the hierarchy a
            // KeyNotFound exception is thrown

            this.rootNode.DescendantAt(tryGetChildNode: delegate (Node parent, TKey key, out Node child)
            {
                child = null;
                if (!parent.TryGetChildNode(key, out child))
                    throw new KeyNotFoundException($"node '{startAt}'  doesn't exist");
                startNode = new Traverser(startNode, child);
                return true;
            }, key: startAt);

            // Travesal was successul.
            // just return wwhat is now in 'startNode'
            return startNode;
        }

        #endregion Hierarchy Node Traversal

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
                    this.writeLock.Enter(ref isLocked);

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
        /// Adds a value to the immutable hierarchy at the specified position.
        /// The result is a new ImmutableHiarachy contains the value. The
        /// old one is unchanged.
        /// If the value is equal to the value already stored at the position the hierarchy remains unchanged.
        /// </summary>
        /// <param name="path">Specifies where to set the value</param>
        /// <param name="value">the value to keep</param>
        /// <returns>Am immutable hierach which contains the specified value</returns>
        public void Add(HierarchyPath<TKey> path, TValue value)
        {
            bool isLocked = false;
            try
            {
                this.writeLock.Enter(ref isLocked);

                // Set the value at the destination node. The clone may substitute the current node.

                Stack<Node> nodesAlongPath;
                var currentNode = this.GetOrCreateNode(path, out nodesAlongPath);

                // if the node has already a value, add gails woth argument exception

                if (currentNode.HasValue)
                    throw new ArgumentException($"Node at '{path}' already has a value", nameof(path));

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
            var currentPosition = HierarchyPath.Create<TKey>();

            // Descend in to the tree, create nodes along the way if they are missing

            var result = this.rootNode.DescendantAt(delegate (Node parentNode, TKey key, out Node childNode)
            {
                currentPosition = currentPosition.Join(key);

                // get or create child. If created the clone of the parent node substitutes the
                // old parent node.

                if (!parentNode.TryGetChildNode(key, out childNode))
                {
                    if (currentPosition.Items.Count() < hierarchyPath.Items.Count() && this.getDefaultValue != null)
                    {
                        // new inner node ist initialized with default value.
                        parentNode = parentNode.AddChildNode(childNode = new Node(key, this.getDefaultValue(currentPosition)));
                    }
                    else
                    {
                        // final node is taken as it is
                        parentNode = parentNode.AddChildNode(childNode = new Node(key));
                    }
                }

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
        /// Retrieves a nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>true, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            Node descendantNode;
            Stack<Node> nodesAlongPath;

            if (this.TryGetNode(hierarchyPath, out nodesAlongPath, out descendantNode))
                return descendantNode.TryGetValue(out value);

            value = default(TValue);
            return false;
        }

        public bool Remove(HierarchyPath<TKey> hierarchyPath, int? maxDepth = null)
        {
            if (maxDepth == null || maxDepth == 1)
            {
                return this.RemoveAtSingleNode(hierarchyPath);
            }
            else if (maxDepth > 1)
            {
                bool removed = false;
                foreach (var node in this.Traverse(hierarchyPath)
                    .DescendantsOrSelf(depthFirst: false, maxDepth: maxDepth).ToList())
                {
                    removed = this.RemoveAtSingleNode(node.Path) || removed;
                }
                return removed;
            }
            return false;
        }

        private bool RemoveAtSingleNode(HierarchyPath<TKey> hierarchyPath)
        {
            Stack<Node> nodesAlongPath = new Stack<Node>();
            Node currentNode;

            // try to get the node to remove values from

            if (!this.TryGetNode(hierarchyPath, out nodesAlongPath, out currentNode))
                return false;

            // If node hasn't a value Remove is finshed. Just return false

            if (!currentNode.HasValue)
                return false;

            // last node must be the root node: create new hierarchy if root node has changed

            bool isLocked = false;
            try
            {
                this.writeLock.Enter(ref isLocked);
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

        public bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse)
        {
            throw new NotImplementedException();
        }
    }
}