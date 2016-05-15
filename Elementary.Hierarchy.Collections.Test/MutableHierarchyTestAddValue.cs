using NUnit.Framework;
using System;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class MutableHierarchyTestAddValue
    {
        [Test]
        public void MH_Rootnode_has_no_value()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            // ACT & ASSERT

            string value;
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
        }

        [Test]
        public void MH_Rootnode_has_default_value()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>(p=>"default value");

            // ACT & ASSERT

            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreEqual("default value", value);
        }

        [Test]
        public void MH_Add_value_to_root_node()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            string test = "test";

            // ACT

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT

            string value;

            // hierachy contains the value
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test]
        public void MH_Add_same_value_to_root_node_throws_ArgumentException()
        {
            // ARRANGE
            var hierarchy = new MutableHierarchy<string, string>();

            string test = "test";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create<string>(), "test2"));

            Assert.That(() => result.Message.Contains("already has a value"));

            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.AreEqual("path", result.ParamName);
        }

        [Test]
        public void MH_Add_child_sets_value_at_child_node()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ASSERT

            string value;

            // hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test]
        public void MH_Add_child_twice_throws_ArgumentException()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create("a"), test1));

            Assert.That(result.Message.Contains("already has a value"));
            Assert.That(result.Message.Contains("'a'"));
            Assert.AreEqual("path", result.ParamName);

            string value;

            // hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test]
        public void MH_Add_child_sibling()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

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

        [Test]
        public void MH_Add_grandchild_under_existing_nodes_returns_new_hierachy_with_same_values()
        {
            // ARRANGE
            var hierarchy = new MutableHierarchy<string, string>();

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

        [Test]
        public void MH_Add_grandchild_as_first_node_returns_new_hierachy_with_same_values()
        {
            // ARRANGE
            var hierarchy = new MutableHierarchy<string, string>(p => string.Empty);

            string test3 = "test3";

            // ACT

            hierarchy.Add(HierarchyPath.Create("a", "c"), test3);

            // ASSERT

            string value;

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(string.Empty, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(string.Empty, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out value));
            Assert.AreSame(test3, value);
        }
    }
}