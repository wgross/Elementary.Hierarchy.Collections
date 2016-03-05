namespace Elementary.Hierarchy.Collections
{
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
        private sealed class ImmutableHierarchyNode
        {
            #region Construction and initialization of this instance

            public ImmutableHierarchyNode(ImmutableHierarchyNode parent, IEnumerable<ImmutableHierarchyNode> children)
            {
                this.parent = parent;
            }
            
            private readonly ImmutableHierarchyNode parent;
            private readonly IEnumerable<ImmutableHierarchyNode> children;

            #endregion Construction and initialization of this instance
        }

        #region Construction and initialization of this instance

        public ImmutableHierarchy()
        {
            this.rootNode = new ImmutableHierarchyNode(null, Enumerable.Empty<ImmutableHierarchyNode>());
        }

        private readonly ImmutableHierarchyNode rootNode;
        public TNode RootNode { get; private set; }

        #endregion Construction and initialization of this instance

        #region Manage nodes in hierarchy

        public int Count { get; private set; } = 0;

        public bool TryGetNode(HierarchyPath<TKey> path, out TNode node)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            node = this.RootNode.DescendantAtOrDefault(path);
            return (node != null);
        }

        public TNode GetOrCreate(HierarchyPath<TKey> hierarchyPath, Func<TNode, HierarchyPath<TKey>, TNode> createNode)
        {
            TNode nextNode, currentNode = this.RootNode;

            HierarchyPath<TKey> breadcrumb = HierarchyPath.Create<TKey>();

            foreach (var pathItem in hierarchyPath.Items)
            {
                breadcrumb = breadcrumb.Join(pathItem);
                if (!currentNode.TryGetChildNode(pathItem, out nextNode))
                {
                    currentNode = currentNode.AddChildNode(createNode(currentNode, breadcrumb));
                    if (currentNode != null)
                        this.Count++; // increment node count, if create&add performed successful
                }
                else
                {
                    currentNode = nextNode;
                }
            }
            return currentNode;
        }

        /// <summary>
        /// Removes the specified node from the hierachy. This includes its child nodes.
        /// </summary>
        /// <param name="pathToRemove"></param>
        /// <param name="ifTrue">predicate to decide if the node can be deleted, default n =&gt; true</param>
        /// <returns></returns>
        public bool Remove(HierarchyPath<TKey> pathToRemove, Predicate<TNode> ifTrue = null)
        {
            var nodeToRemove = this.RootNode.DescendantAtOrDefault(pathToRemove);
            if (nodeToRemove == null)
                return false;

            // remove node only if it the predicate allows it (or take default predicate)
            if (!(ifTrue ?? (n => true))(nodeToRemove))
                return false;

            // the node has no parent.
            // removing the root means to clear the hierarchy
            if (nodeToRemove.HasParentNode)
            {
                var nodesInChild = nodeToRemove.DescendantsOrSelf().Count();
                if (nodeToRemove.Parent().RemoveChildNode(nodeToRemove))
                {
                    this.Count -= nodesInChild;
                    return true;
                }
            }

            // nodeTo Remove has no parent node -> is root node
            this.RootNode.ClearChildNodes();
            this.Count = 0;
            return true;
        }

        public HierarchyPath<TKey> Move(HierarchyPath<TKey> sourcePath, HierarchyPath<TKey> destinationPath, Func<TNode, HierarchyPath<TKey>, TNode> createNode)
        {
            if (sourcePath.IsRoot)
                throw new ArgumentException("Root node can't be moved.");

            // get source node: operation fails if source doesn exist.
            var sourceNode = this.RootNode.DescendantAt(sourcePath);
            // get destination node
            var destinationNode = this.RootNode.DescendantAtOrDefault(destinationPath);
            if (destinationNode == null)
            {
                // source node is the new destination node. The ancestor node(s) are created on demand
                var destinationParent = this.GetOrCreate(destinationPath.Parent(), createNode);

                // remove the source node from the tree and add it to the destination node without renaming
                if (!sourceNode.Parent().RemoveChildNode(sourceNode))
                    throw new InvalidOperationException($"Couldn't remove sourceNode {sourcePath} from parent node");

                // unlink source node from its parent and attach to the destination parent
                sourceNode.ChangeLocalPath(destinationPath.Items.Last());
                destinationParent.AddChildNode(sourceNode);
                sourceNode.ChangeParentNode(destinationParent);
            }
            else
            {
                // destinationPath is already taken. Try to put source under destination node
                var effectiveDestinationPath = destinationPath.Join(sourcePath.Leaf());
                TNode node;
                if (destinationNode.TryGetChildNode(sourcePath.Items.Last(), out node))
                    throw new InvalidOperationException($"New place of sourceNode({sourcePath}) is already taken: {destinationPath.Join(sourcePath.Leaf())}");

                // remove the source node from the tree and add it to the destination node without renaming
                if (!sourceNode.Parent().RemoveChildNode(sourceNode))
                    throw new InvalidOperationException($"Couldn't remove sourceNode {sourcePath} from parent node");

                destinationNode.AddChildNode(sourceNode);
                sourceNode.ChangeParentNode(destinationNode);
            }

            return HierarchyPath.Create(sourceNode.AncestorsOrSelf().Select(n => n.LocalPath).Reverse().Skip(1).ToArray());
        }

        #endregion Manage nodes in hierarchy
    }
}