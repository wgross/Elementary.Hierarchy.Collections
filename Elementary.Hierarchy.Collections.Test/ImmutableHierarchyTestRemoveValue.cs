using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyTestRemoveValue
    {
        [Test]
        public void Remove_value_from_root_creates_new_hierarchy()
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.AreNotSame(hierarchy, result);

            string value;

            // old node is unchanged
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);

            // new node has no value
            Assert.IsFalse(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }

        [Test]
        public void Remove_value_twice_from_root_doesnt_creates_new_hierarchy()
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            var hierarchy = new ImmutableHierarchy<string, string>();
            hierarchy = hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy = hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy = hierarchy.Remove(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.AreSame(hierarchy, result);
        }
    }
}
