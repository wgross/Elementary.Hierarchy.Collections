﻿using NUnit.Framework;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class MutableHierarchyTestRemoveValue
    {
        [Test]
        public void MH_Remove_value_from_root_doesnt_create_new_hierarchy()
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.AreSame(hierarchy, result);

            string value;

            // node has no value
            Assert.IsFalse(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test]
        public void MH_Remove_value_twice_from_root_doesnt_create_new_hierarchy()
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy = hierarchy.Remove(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.AreSame(hierarchy, result);
        }

        [Test]
        public void MH_Remove_value_from_child_doesnt_create_new_hierarchy()
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a", "b"), test2);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.AreSame(hierarchy, result);

            string value;

            // node has no value
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsFalse(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a", "b"), out value));
            Assert.AreSame(test2, value);
        }

        [Test]
        public void MH_Remove_value_from_child_twice_doesnt_create_new_hierarchy()
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            var hierarchy = new MutableHierarchy<string, string>();
            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a", "b"), test2);
            hierarchy = hierarchy.Remove(HierarchyPath.Create("a"));

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.AreSame(hierarchy, result);
        }

        [Test]
        public void MH_Remove_value_from_unknown_node_throws()
        {
            // ARRANGE

            var hierarchy = new MutableHierarchy<string, string>();

            // ACT & ASSERT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchy.Remove(HierarchyPath.Create("a")));

            Assert.That(result.Message.Contains("'a'"));
        }
    }
}