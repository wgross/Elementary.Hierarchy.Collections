namespace Elementary.Hierarchy.Collections
{
    using Elementary.Hierarchy;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            public readonly Node[] childNodes;

            public bool HasChildNodes
            {
                get
                {
                    return this.ChildNodes.Any();
                }
            }

            #endregion Construction and initialization of this instance

            #region IHasChildNodes Members

            public IEnumerable<Node> ChildNodes
            {
                get
                {
                    return this.childNodes;
                }
            }

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
            /// </summary>
            /// <returns></returns>
            public Node UnsetValue()
            {
                if(this.HasValue)
                    return new Node(this.id, ValueNotSet, this.childNodes);

                return this;
            }
        }

        #region Construction and initialization of this instance

        public ImmutableHierarchy()
            : this(new Node(default(TKey)))
        {
        }

        private ImmutableHierarchy(Node rootNode)
        {
            this.rootNode = rootNode;
        }

        private readonly Node rootNode;

        private ImmutableHierarchy<TKey, TValue> CreateIfRootHasChanged(Node newRoot)
        {
            if (object.ReferenceEquals(this.rootNode, newRoot))
                return this;

            return new ImmutableHierarchy<TKey, TValue>(newRoot);
        }

        #endregion Construction and initialization of this instance

        /// <summary>
        /// Adds a value to the immutable hierachy at the specified position.
        /// The result is a new ImmutableHiarachy contains the value. The
        /// old one is unchanged.
        /// If the value is equal to the value already stored at the position the hierachy remains unchanged.
        /// </summary>
        /// <param name="hierarchyPath">Specifies where to set the value</param>
        /// <param name="value">the value to keep</param>
        /// <returns>Am immutable hierach which contains the specified value</returns>
        public ImmutableHierarchy<TKey, TValue> Add(HierarchyPath<TKey> hierarchyPath, TValue value)
        {
            // if the path has no items, tthe root node is changed
            if (!hierarchyPath.Items.Any())
                return this.CreateIfRootHasChanged(this.rootNode.SetValue(value));

            // make a snapshot of the path items for easier handling
            var hierarchyPathItems = hierarchyPath.Items.ToArray();
            var hierarchyPathItemsLength = hierarchyPathItems.Length;

            // Create the new value node with the ngiven value and the leaf id.

            Stack<Node> nodesAlongPath = new Stack<Node>();

            var currentNode = this.rootNode;

            // descend until the parent of the valueNode is reached
            for (int currentHierarchyLevel = 0; currentHierarchyLevel < hierarchyPathItemsLength; currentHierarchyLevel++)
            {
                Node nextNode = null;

                if (currentNode.TryGetChildNode(hierarchyPathItems[currentHierarchyLevel], out nextNode))
                {
                    // child exists, just descend further
                    nodesAlongPath.Push(currentNode);
                }
                else
                {
                    // child nodes doesn't exist -> create new one
                    if (currentHierarchyLevel < hierarchyPathItemsLength - 1)
                    {
                        // parent of new node isn't ready yet. Just another node.
                        nextNode = new Node(id: hierarchyPathItems[currentHierarchyLevel]);
                    }
                    else
                    {
                        // this is the parent node of the value node.
                        nextNode = new Node(id: hierarchyPathItems[currentHierarchyLevel], value: value);
                    }

                    nodesAlongPath.Push(currentNode.AddChildNode(nextNode));
                }
                currentNode = nextNode;
            }

            // new ascend agin to the root and clone new parnet node for the newly created child nodes.
            while (nodesAlongPath.Any())
            {
                // the next (parent node) get the current child node as a substitute.
                currentNode = nodesAlongPath.Peek().SetChildNode(currentNode);
                nodesAlongPath.Pop();
            }

            // this ist the new immutable hierachy root.
            if (object.ReferenceEquals(this.rootNode, currentNode))
                return this;

            return new ImmutableHierarchy<TKey, TValue>(currentNode);
        }

        /// <summary>
        /// Retrieves the nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            value = default(TValue);
            var valueNode = this.rootNode.DescendantAtOrDefault(hierarchyPath);
            if (valueNode == null || !valueNode.HasValue)
                return false;

            return valueNode.TryGetValue(out value);
        }

        /// <summary>
        /// Removes the value from the specified node in hierarchy.
        /// Value and nodes on under the specified nde remain unchanged
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns>true if value was removed</returns>
        public ImmutableHierarchy<TKey, TValue> Remove(HierarchyPath<TKey> hierarchyPath)
        {
            // if the path has no items, the root node is changed
            if (!hierarchyPath.Items.Any())
                return this.CreateIfRootHasChanged(this.rootNode.UnsetValue());

            // now find the the value node and the path to reach it
            Stack<Node> nodesAlongPath = new Stack<Node>(this.rootNode.DescentAlongPath(hierarchyPath));
            if(nodesAlongPath.Count != hierarchyPath.Items.Count())
            {
                // the value node doesn't exist: keep hierarchy as it is
                throw new KeyNotFoundException($"Could not find node '{hierarchyPath.Items.ElementAt(nodesAlongPath.Count)}' under '{HierarchyPath.Create(hierarchyPath.Items.Take(nodesAlongPath.Count)).ToString()}'");
            }

            // unset the value at the value node...
            var currentNode = nodesAlongPath.Pop().UnsetValue();
            
            // ... ascend again to the root and copy-on-change the ancestor nodes.
            while (nodesAlongPath.Any())
                currentNode = nodesAlongPath.Pop().SetChildNode(currentNode);
           
            // create new hierachy if root node has changed
            return this.CreateIfRootHasChanged(this.rootNode.SetChildNode(currentNode));
        }
    }
}