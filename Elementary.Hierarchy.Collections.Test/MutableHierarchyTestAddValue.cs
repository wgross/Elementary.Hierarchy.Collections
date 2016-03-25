﻿using NUnit.Framework;

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
        public void MH_Add_value_to_root_node_doesnt_create_new_hierarchy()
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
        public void MH_Add_same_value_to_root_node_doesnt_create_new_hierarchy()
        {
            // ARRANGE
            var hierarchy = new MutableHierarchy<string, string>();

            string test = "test";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT
            
            string value;
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test]
        public void MH_Add_child_returns_same_hierarchy_with_same_values()
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
        public void MH_Add_child_sibling_returns_same_hierachy_with_same_values()
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
        public void MH_Add_grandchild_returns_same_hierachy_with_same_values()
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
    }
}