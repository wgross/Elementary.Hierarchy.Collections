using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyNodeTest
    {
        [Test]
        public void Setting_node_value_creates_a_node_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.ImmutableHierarchyNode("id", "value");

            // ACT

            var result = node.SetValue("newValue");

            // ASSERT

            Assert.AreNotSame(node, result);
            Assert.AreEqual("newValue", result.value);
        }

        [Test]
        public void Setting_node_value_twice_doesnt_create_new_node()
        {
            // ARRANGE

            string value = "value";
            var node = new ImmutableHierarchy<string, string>.ImmutableHierarchyNode("id", value);

            // ACT

            var result = node.SetValue(value);

            // ASSERT

            Assert.AreSame(node, result);
            Assert.AreSame(value, result.value);
        }
    }
}
