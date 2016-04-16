using NUnit.Framework;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyTraverserTest
    {
        [Test]
        public void IMH_Same_traverser_are_equal()
        {
            // ARRANGE

            var a = new ImmutableHierarchy<string, int>.Traverser(null, new ImmutableHierarchy<string, int>.Node(""));

            // ACT

            var result = a.Equals(a);

            // ASSERT

            Assert.IsTrue(result);
        }

        [Test]
        public void IMH_Travers_are_equal_if_node_is_same()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, int>.Node("");
            var a = new ImmutableHierarchy<string, int>.Traverser(null, node);
            var b = new ImmutableHierarchy<string, int>.Traverser(null, node);

            // ACT

            var result = a.Equals(b);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}