using NUnit.Framework;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class HierarchyRemoveValueTest
    {
        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_value_from_root_returns_true(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.IsTrue(result);

            string value;

            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_value_twice_from_root_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Remove(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_value_from_child_returns_true(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsTrue(result);

            string value;

            // new node has no value
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "b"), out value));
            Assert.AreSame(test2, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_value_from_root_recursive_returns_true(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>(), maxDepth: 2);

            // ASSERT

            Assert.IsTrue(result);

            string value;

            // new node has no value
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "b"), out value));
            Assert.AreSame(test2, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_false_if_no_value_was_removed(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>(), maxDepth: 2);

            // ASSERT

            Assert.IsFalse(result);

            string value;

            // new node has no value
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "b"), out value));
            Assert.AreSame(test2, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_value_from_child_twice_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);
            hierarchy.Remove(HierarchyPath.Create("a"));

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Remove_value_from_unknown_node_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void IHierarchy_Remove_value_with_pruning_reonves_empty_subnodes()
        {
            Assert.Fail("not yet imp,emented for ImmutableHierarchy.Node at least");
        }
    }
}