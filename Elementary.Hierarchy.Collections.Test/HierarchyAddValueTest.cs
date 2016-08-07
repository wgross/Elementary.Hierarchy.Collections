using NUnit.Framework;
using System;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class HierarchyAddValueTest
    {
        [Test,TestCaseSource(typeof(HierarchyVariantSource),nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Rootnode_has_no_value(IHierarchy<string,string> hierarchy)
        {
            // ACT & ASSERT

            string value;
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.IsFalse(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithDefaultValue))]
        public void IHierarchy_Rootnode_has_default_value(IHierarchy<string,string> hierarchy)
        {
            // ACT & ASSERT

            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreEqual(HierarchyVariantSource.DefaultValue, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Add_value_to_root_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            
            string test = "test";

            // ACT

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT

            string value;

            // new hierarchy contains all values
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Add_same_value_to_root_twice_throws_ArgumentException(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            
            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create<string>(), test1));

            // ASSERT

            Assert.That(result.Message.Contains("already has a value"));
            Assert.That(result.Message.Contains("''"));

            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.AreEqual("path", result.ParamName);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Add_child_sets_value_at_child_node(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ASSERT

            string value;

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Add_child_value_twice_throws_ArgumentException(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create("a"), test2));

            // ASSERT

            Assert.That(result.Message.Contains("already has a value"));
            Assert.That(result.Message.Contains("'a'"));

            string value;

            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.AreEqual("path", result.ParamName);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Add_child_sibling(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

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
        public void IHierarchy_Add_child_sibling_value_twice_throws_ArgumentException(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create("b"), test3));

            // ASSERT

            Assert.That(result.Message.Contains("already has a value"));
            Assert.That(result.Message.Contains("'b'"));

            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
            Assert.AreEqual("path", result.ParamName);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithoutDefaultValue))]
        public void IHierarchy_Add_grandchild_under_existing_nodes_returns_new_hierachy_with_same_values(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ACT

            hierarchy.Add(HierarchyPath.Create("a", "c"), test3);

            // ASSERT

            string value;

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out value));
            Assert.AreSame(test3, value);
        }

        [Test, TestCaseSource(typeof(HierarchyVariantSource), nameof(HierarchyVariantSource.WithDefaultValue))]
        public void IHierarchy_Add_grandchild_as_first_node_returns_new_hierachy_with_same_values(IHierarchy<string,string> hierarchy)
        {
            // ARRANGE
            
            string test3 = "test3";

            // ACT

            hierarchy.Add(HierarchyPath.Create("a", "c"), test3);

            // ASSERT

            string value;

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(HierarchyVariantSource.DefaultValue, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(HierarchyVariantSource.DefaultValue, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out value));
            Assert.AreSame(test3, value);
        }
    }
}