using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class MutableHierarchyTraversalTest
    {
        [Test]
        public void MH_Rootnode_has_default_value_on_traversal()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>(p => "default value");

            // ACT & ASSERT

            Assert.IsTrue(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
            Assert.AreEqual("default value", hierarchy.Traverse(HierarchyPath.Create<string>()).Value);
        }

        [Test]
        public void MH_node_knows_its_path()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Path;

            // ASSERT

            Assert.AreEqual(HierarchyPath.Create<string>(), result);
        }

        [Test]
        public void MH_hierarchy_has_no_children_if_root_has_no_children()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasChildNodes;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void MH_get_a_nodes_value()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create<string>(), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Value;

            // ASSERT

            Assert.AreEqual("v1", result);
        }

        [Test]
        public void MH_Get_children_of_root_node()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("b"), "v2");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void MH_child_node_knows_its_path()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.Single().Path;

            // ASSERT

            Assert.AreEqual(HierarchyPath.Create("a"), result);
        }

        [Test]
        public void MH_root_has_no_parent()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasParentNode;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void MH_children_of_root_have_root_as_parent()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var root = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().Single().Parent();

            // ASSERT

            Assert.AreEqual(root, result);
        }

        [Test]
        public void MH_Start_traversal_at_child_of_root()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("a", "b"), "v2");
            hierarchy.Add(HierarchyPath.Create("a", "c"), "v3");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().First();

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(HierarchyPath.Create("a"), result.Path);
        }

        [Test]
        public void MH_start_at_inner_node_stil_allows_to_ascend()
        {
            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("a", "b", "c"), "v2");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create("a", "b", "c"));

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new[] {
                    HierarchyPath.Create("a","b"),
                    HierarchyPath.Create("a"),
                    HierarchyPath.Create<string>(),
                },
                result.Ancestors().Select(n => n.Path));
        }

        [Test]
        public void MH_throw_if_start_path_doesnt_exist()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().First();

            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(HierarchyPath.Create("b")));

            // ASSERT

            Assert.IsTrue(result.Message.Contains("'b'"));
        }
    }
}