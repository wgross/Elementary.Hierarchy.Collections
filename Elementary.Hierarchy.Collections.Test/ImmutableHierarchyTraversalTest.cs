using NUnit.Framework;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyTraversalTest
    {
        [Test]
        public void IMH_node_knows_its_path()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.Traverse().Path;

            // ASSERT

            Assert.AreEqual(HierarchyPath.Create<string>(), result);
        }

        [Test]
        public void IMH_hierarchy_has_no_children_if_root_has_no_children()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.Traverse().HasChildNodes;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void IMH_get_a_nodes_value()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create<string>(), "v1");

            // ACT

            var result = hierarchy.Traverse().Value;

            // ASSERT

            Assert.AreEqual("v1", result);
        }

        [Test]
        public void IMH_Get_children_of_root_node()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("b"), "v2");

            // ACT

            var result = hierarchy.Traverse().ChildNodes.ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void IMH_child_node_knows_its_path()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            // ACT

            var result = hierarchy.Traverse().ChildNodes.Single().Path;

            // ASSERT

            Assert.AreEqual(HierarchyPath.Create("a"), result);
        } 

        [Test]
        public void IMH_root_has_no_parent()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.Traverse().HasParentNode;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void IMH_children_of_root_have_root_as_parent()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var root = hierarchy.Traverse();

            // ACT

            var result = hierarchy.Traverse().Children().Single().Parent();

            // ASSERT

            Assert.AreEqual(root, result);
        }
    }
}