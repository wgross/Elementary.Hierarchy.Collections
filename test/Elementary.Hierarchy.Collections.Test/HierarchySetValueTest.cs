using NUnit.Framework;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class HierarchySetValueTest
    {
        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Set_value_at_root_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";

            // ACT

            hierarchy[HierarchyPath.Create<string>()] = test;

            // ASSERT

            string value;

            // hierarchy contains the value
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Set_value_at_root_node_twice_overwrites_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test2 = "test2";

            hierarchy[HierarchyPath.Create<string>()] = test;

            // ACT & ASSERT

            hierarchy[HierarchyPath.Create<string>()] = test2;

            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test2, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Set_child_sets_value_at_child_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy[HierarchyPath.Create<string>()] = test;

            // ACT

            hierarchy[HierarchyPath.Create("a")] = test1;

            // ASSERT

            string value;

            // hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Set_child_twice_throws_ArgumentException(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy[HierarchyPath.Create<string>()] = test;
            hierarchy[HierarchyPath.Create("a")] = test1;

            // ACT

            hierarchy[HierarchyPath.Create("a")] = test1;

            // ASSERT

            string value;

            // hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Set_value_at_child_sibling(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy[HierarchyPath.Create<string>()] = test;
            hierarchy[HierarchyPath.Create("a")] = test1;

            // ACT

            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ASSERT

            string value;

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Set_value_at_grandchild(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy[HierarchyPath.Create<string>()] = test;
            hierarchy[HierarchyPath.Create("a")] = test1;
            hierarchy[HierarchyPath.Create("b")] = test2;

            // ACT

            hierarchy[HierarchyPath.Create("a", "c")] = test3;

            // ASSERT

            string value;

            // hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out value));
            Assert.AreSame(test3, value);
        }
    }
}