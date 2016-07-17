using NUnit.Framework;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class MutableHierarchyTestRemoveNode
    {
        [Test]
        public void MH_RemoveNode_root_removes_value_but_not_the_node()
        {
            // ARRANGE

            string test = "test";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create<string>(), false);

            // ASSERT

            Assert.IsTrue(result);

            // value is removed
            string value;
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));

            // node is still there
            Assert.IsNotNull(hierarchy.Traverse(HierarchyPath.Create<string>()));
            Assert.IsFalse(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
        }

        [TestCase("", "a")] // root with direct subnode
        [TestCase("", "a/b")] // root with indirect subnode
        [TestCase("a", "a/b")] // sub node with direct subnode
        [TestCase("a", "a/b/c")] // subnode with indirect subnode
        public void MH_RemoveNode_non_recursive_fails_if_a_childnode_is_present(string nodePath, string subNodePath)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Parse(nodePath, "/"), test);
            hierarchy.Add(HierarchyPath.Parse(subNodePath, "/"), test1);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Parse(nodePath, "/"), recurse: false);

            // ASSERT

            Assert.IsFalse(result);

            string value;

            // node has no value
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Parse(nodePath, "/"), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Parse(subNodePath, "/"), out value));
            Assert.AreSame(test1, value);
        }

        [TestCase("a", true)]
        [TestCase("a", false)]
        [TestCase("a/b", true)]
        [TestCase("a/b", false)]
        public void MH_RemoveNode_removes_node_from_hierarchy_completely(string pathToDelete, bool recurse)
        {
            // ARRANGE

            var node = HierarchyPath.Parse(pathToDelete, "/");

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(node, pathToDelete);

            // ACT

            var result = hierarchy.RemoveNode(node, recurse: recurse);

            // ASSERT

            Assert.IsTrue(result);

            // node has no value
            string value;
            Assert.IsFalse(hierarchy.TryGetValue(node, out value));

            // nodes are no longer present
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(node));
        }

        [TestCase("")]
        [TestCase("a")]
        [TestCase("a/b")]
        public void MH_RemoveNode_removes_node_from_hierarchy_completely_and_all_descendants(string nodeToDelete)
        {
            // ARRANGE
            var node = HierarchyPath.Parse(nodeToDelete, "/");
            var subNode1 = node.Join("subNode");

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(node, node.ToString());
            hierarchy.Add(subNode1, subNode1.ToString());

            // ACT

            var result = hierarchy.RemoveNode(node, recurse: true);

            // ASSERT

            Assert.IsTrue(result);

            // new node has no value
            string value;
            Assert.IsFalse(hierarchy.TryGetValue(node, out value));
            Assert.IsFalse(hierarchy.TryGetValue(subNode1, out value));

            // nodes are no longer present
            if(!node.IsRoot) Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(node));
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(subNode1));
        }

        [TestCase("", true)]
        [TestCase("", false)]
        [TestCase("a", true)]
        [TestCase("a", false)]
        public void MH_RemoveNode_twice_returns_false(string path, bool recurse)
        {
            // ARRANGE
            string test = "test";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Parse(path, "/"), test);
            hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void MH_RemoveNode_unknown_node_returns_false()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create("a"), recurse: false);

            // ASSERT

            Assert.IsFalse(result);
        }
    }
}