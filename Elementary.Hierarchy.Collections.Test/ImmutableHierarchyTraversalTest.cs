using NUnit.Framework;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyTraversalTest
    {
        [Test]
        public void IMH_hierarchy_has_no_children_if_root_has_no_children()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.StartTraversal().HasChildNodes;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void IMH_Get_children_of_root_node()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("b"), "v2");

            // ACT

            var result = hierarchy.StartTraversal().ChildNodes.ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void IMH_root_has_no_parent()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            // ACT

            var result = hierarchy.StartTraversal().HasParentNode;

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void IMH_children_of_root_have_root_as_parent()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var root = hierarchy.StartTraversal();

            // ACT

            var result = hierarchy.StartTraversal().Children().Single().Parent();

            // ASSERT

            Assert.AreEqual(root, result);
        }
    }
}