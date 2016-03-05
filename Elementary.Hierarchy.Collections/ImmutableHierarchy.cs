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
        public sealed class ImmutableHierarchyNode : IHasIdentifiableChildNodes<TKey, ImmutableHierarchyNode>
        {
            #region Construction and initialization of this instance

            public ImmutableHierarchyNode(TKey id)
            {
                this.id = id;
                this.childNodes = new ImmutableHierarchyNode[0];
            }

            public ImmutableHierarchyNode(TKey id, IEnumerable<ImmutableHierarchyNode> childNodes)
            {
                this.id = id;
                this.childNodes = childNodes.ToArray();
            }

            private readonly TKey id;
            private readonly ImmutableHierarchyNode[] childNodes;

            public bool HasChildNodes
            {
                get
                {
                    return this.ChildNodes.Any();
                }
            }

            #endregion Construction and initialization of this instance

            public object Value { get; set; }

            #region IHasChildNodes Members

            public IEnumerable<ImmutableHierarchyNode> ChildNodes
            {
                get
                {
                    return this.childNodes;
                }
            }

            #endregion IHasChildNodes Members

            #region IHasIdentifiableChildNodes Members

            public bool TryGetChildNode(TKey id, out ImmutableHierarchyNode childNode)
            {
                childNode = this.childNodes.SingleOrDefault(n => EqualityComparer<TKey>.Default.Equals(n.id, id));
                return childNode != null;
            }

            public ImmutableHierarchyNode AddChildNode(ImmutableHierarchyNode newChildNode)
            {
                // copy the existing children to a new array, and append the new one.
                ImmutableHierarchyNode[] newChildNodes = new ImmutableHierarchyNode[this.childNodes.Length + 1];
                Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);
                newChildNodes[this.childNodes.Length] = newChildNode;

                // return a new node as susbtitute for this node to add to the new parent
                return new ImmutableHierarchyNode(this.id, newChildNodes) { Value = this.Value };
            }

            public ImmutableHierarchyNode SetChildNode(ImmutableHierarchyNode newChildNode)
            {
                ImmutableHierarchyNode[] newChildNodes = new ImmutableHierarchyNode[this.childNodes.Length];
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
                        return new ImmutableHierarchyNode(this.id, newChildNodes) { Value = this.Value };
                    }
                }
                throw new InvalidOperationException($"The node (id={newChildNode.id}) doesn't substutite any of the existing child nodes in (id={this.id})");
            }

            #endregion IHasIdentifiableChildNodes Members
        }

        #region Construction and initialization of this instance

        public ImmutableHierarchy()
            : this(new ImmutableHierarchyNode(default(TKey)))
        {
        }

        private ImmutableHierarchy(ImmutableHierarchyNode rootNode)
        {
            this.rootNode = rootNode;
        }

        private readonly ImmutableHierarchyNode rootNode;

        #endregion Construction and initialization of this instance

        public ImmutableHierarchy<TKey, TValue> Add(HierarchyPath<TKey> hierarchyPath, TValue value)
        {
            Stack<ImmutableHierarchyNode> nodesAlongPath = new Stack<ImmutableHierarchyNode>();
            
            var currentNode = this.rootNode;

            foreach (var currentKey in hierarchyPath.Items)
            {
                ImmutableHierarchyNode nextNode = null;

                if (currentNode.TryGetChildNode(currentKey, out nextNode))
                {
                    // child exists, just descend further
                    nodesAlongPath.Push(currentNode);
                }
                else
                {
                    // child nodes doesn't exist -> create new one
                    nextNode = new ImmutableHierarchyNode(currentKey);
                    nodesAlongPath.Push(currentNode.AddChildNode(nextNode));
                }
                currentNode = nextNode;
            }

            // at the last node of the descendent, set the value.
            currentNode.Value = value;

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
            if (valueNode == null)
                return false;

            value = (TValue)(valueNode.Value);
            return true;
        }
    }
}