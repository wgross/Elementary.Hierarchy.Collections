﻿using NUnit.Framework;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyTestAddValue
    {
        [Test]
        public void Rootnode_has_no_value()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            // ACT & ASSERT

            string value;
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
        }

        [Test]
        public void Add_value_to_root_node_creates_new_hierarchy()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT

            Assert.AreNotSame(hierarchy, result);

            string value;

            // old hierarchy is empty
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));

            // new hierachy contains the value
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test]
        public void Add_same_value_to_root_node_doesnt_create_new_hierarchy()
        {
            // ARRANGE
            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";

            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT

            Assert.AreSame(result, hierarchy);

            string value;
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test]
        public void Add_child_returns_new_hierarchy_with_same_values()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";

            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ASSERT

            Assert.AreNotSame(hierarchy, result);

            string value;
            // original hierarchy is the same as before
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test]
        public void Add_child_value_twice_doesnt_return_new_hierarchy()
        {
            // ARRANGE

            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";

            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ASSERT

            Assert.AreSame(hierarchy, result);
        }

        [Test]
        public void Add_child_sibling_returns_new_hierachy_with_same_values()
        {
            // ARRANGE
            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ASSERT

            Assert.AreNotSame(hierarchy, result);

            string value;
            // original hierarchy is the same as before
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create("b"), out value));

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
        }

        [Test]
        public void Add_child_sibling_value_twice_doesnt_return_new_hierachy()
        {
            // ARRANGE
            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy = hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ASSERT

            Assert.AreSame(hierarchy, result);
        }

        [Test]
        public void Add_grandchild_returns_new_hierachy_with_same_values()
        {
            // ARRANGE
            var hierarchy = new ImmutableHierarchy<string, string>();

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy = hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ACT

            var result = hierarchy.Add(HierarchyPath.Create("a", "c"), test3);

            // ASSERT

            Assert.AreNotSame(hierarchy, result);

            string value;
            // original hierarchy is the same as before
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
            Assert.IsFalse(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out value));

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("b"), out value));
            Assert.AreSame(test2, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a", "c"), out value));
            Assert.AreSame(test3, value);
        }
    }
}