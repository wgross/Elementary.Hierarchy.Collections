using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class HierarchyTraversalTest
    {
        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithDefaultValue))]
        public void IHierarchy_Rootnode_has_default_value_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            Assert.IsTrue(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
            Assert.AreEqual(HierarchyVariantSource.DefaultValue, hierarchy.Traverse(HierarchyPath.Create<string>()).Value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_node_knows_its_path(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(HierarchyPath.Create<string>(), result.Path);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_hierarchy_has_no_children_if_root_has_no_children(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasChildNodes;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_get_a_nodes_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create<string>(), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Value;

            // ASSERT

            Assert.AreEqual("v1", result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Get_children_of_root_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("b"), "v2");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Length);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_child_node_knows_its_path(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.Single().Path;

            // ASSERT

            Assert.AreEqual(HierarchyPath.Create("a"), result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_root_has_no_parent(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasParentNode;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_children_of_root_have_root_as_parent(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var root = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().Single().Parent();

            // ASSERT

            Assert.AreEqual(root, result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Start_traversal_at_child_of_root(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().First();

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(HierarchyPath.Create("a"), result.Path);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_start_at_inner_node_stil_allows_to_ascend(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

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

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_throw_if_start_path_doesnt_exist(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().First();

            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(HierarchyPath.Create("b")));

            // ASSERT

            Assert.IsTrue(result.Message.Contains("'b'"));
        }
    }
}