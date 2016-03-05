using Elementary.Hierarchy.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyTest
    {
        private ImmutableHierarchy<string,string> hierarchy;

        [SetUp]
        public void ArrangeAllTests()
        {
            this.hierarchy = new ImmutableHierarchy<string,string>();
        }

        [Test]
        public void Add_value_to_root_node_doesnt_change_the_hierarchy()
        {
            // ARRANGE 
            string test = "test";

            // ACT

            var result = this.hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT

            Assert.AreSame(result, this.hierarchy);
            string value;
            Assert.IsTrue(this.hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
        }

        [Test]
        public void Add_node_to_hierarchy_returns_new_hierarchy_with_node()
        {
            // ARRANGE 
            string test = "test";
            string test1 = "test1";

            this.hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            var result = this.hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ASSERT

            Assert.AreNotSame(this.hierarchy, result);

            string value;
            // original hierarchy is the same as before
            Assert.IsTrue(this.hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsFalse(this.hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));

            // new hierarchy contains the root date and the new node.
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.AreSame(test, value);
            Assert.IsTrue(result.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.AreSame(test1, value);
        }
    }
}
