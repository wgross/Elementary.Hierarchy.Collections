using NUnit.Framework;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class MutableHierarchyTraverserTest
    {
        [Test]
        public void MH_Same_traverser_are_equal()
        {
            // ARRANGE

            var a = new MutableHierarchy<string, int>.Traverser(new MutableHierarchy<string, int>.Node(""));

            // ACT

            var result = a.Equals(a);

            // ASSERT

            Assert.IsTrue(result);
        }

        [Test]
        public void MH_Traversers_are_equal_if_node_is_same()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, int>.Node("");
            var a = new MutableHierarchy<string, int>.Traverser(node);
            var b = new MutableHierarchy<string, int>.Traverser(node);

            // ACT

            var result = a.Equals(b);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}