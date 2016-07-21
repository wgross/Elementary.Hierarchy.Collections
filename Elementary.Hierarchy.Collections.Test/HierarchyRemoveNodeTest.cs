using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Collections.Test
{
    public class RemoveNodeTestCaseSource
    {
        public static IEnumerable One
        {
            get
            {
                yield return new TestCaseData("", "a", new MutableHierarchy<string, string>()); // root with direct subnode
                yield return new TestCaseData("", "a/b", new MutableHierarchy<string, string>()); // root with indirect subnode
                yield return new TestCaseData("a", "a/b", new MutableHierarchy<string, string>()); // sub node with direct subnode
                yield return new TestCaseData("a", "a/b/c", new MutableHierarchy<string, string>()); // subnode with indirect subnode
            }
        }

        public static IEnumerable Two
        {
            get
            {
                yield return new TestCaseData("a", true, new MutableHierarchy<string, string>());
                yield return new TestCaseData("a", false, new MutableHierarchy<string, string>());
                yield return new TestCaseData("a/b", true, new MutableHierarchy<string, string>());
                yield return new TestCaseData("a/b", false, new MutableHierarchy<string, string>());
            }
        }

        public static IEnumerable Three
        {
            get
            {
                yield return new TestCaseData("", new MutableHierarchy<string, string>());
                yield return new TestCaseData("a", new MutableHierarchy<string, string>());
                yield return new TestCaseData("a/b", new MutableHierarchy<string, string>());
            }
        }

        public static IEnumerable Four
        {
            get
            {
                yield return new TestCaseData("", true, new MutableHierarchy<string, string>());
                yield return new TestCaseData("", false, new MutableHierarchy<string, string>());
                yield return new TestCaseData("a", true, new MutableHierarchy<string, string>());
                yield return new TestCaseData("a", false, new MutableHierarchy<string, string>());
            }
        }
    }

    [TestFixture]
    public class HierarchyRemoveNodeTest
    {
        [Test]
        public void IHierarchy_RemoveNode_root_removes_value_but_not_the_node()
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
        
        [Test,TestCaseSource(typeof(RemoveNodeTestCaseSource), nameof(RemoveNodeTestCaseSource.One))]
        public void IHierarchy_RemoveNode_non_recursive_fails_if_a_childnode_is_present(string nodePath, string subNodePath, IHierarchy<string,string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
        
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

        [Test, TestCaseSource(typeof(RemoveNodeTestCaseSource), nameof(RemoveNodeTestCaseSource.Two))]
        public void IHierarchy_RemoveNode_removes_node_from_hierarchy_completely(string pathToDelete, bool recurse, IHierarchy<string,string> hierarchy)
        {
            // ARRANGE

            var node = HierarchyPath.Parse(pathToDelete, "/");
            
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

        [Test, TestCaseSource(typeof(RemoveNodeTestCaseSource), nameof(RemoveNodeTestCaseSource.Three))]
        public void IHierarchy_RemoveNode_removes_node_from_hierarchy_completely_and_all_descendants(string nodeToDelete, IHierarchy<string,string> hierarchy)
        {
            // ARRANGE

            var node = HierarchyPath.Parse(nodeToDelete, "/");
            var subNode1 = node.Join("subNode");

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
            if (!node.IsRoot) Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(node));
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(subNode1));
        }

        [Test, TestCaseSource(typeof(RemoveNodeTestCaseSource), nameof(RemoveNodeTestCaseSource.Four))]
        public void IHierarchy_RemoveNode_twice_returns_false(string path, bool recurse, IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            string test = "test";

            hierarchy.Add(HierarchyPath.Parse(path, "/"), test);
            hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_RemoveNode_unknown_node_returns_false(IHierarchy<string,string> hierarchy)
        {
            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create("a"), recurse: false);

            // ASSERT

            Assert.IsFalse(result);
        }
    }
}