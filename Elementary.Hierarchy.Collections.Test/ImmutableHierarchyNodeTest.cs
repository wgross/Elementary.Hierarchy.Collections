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
        public void TryGetValue_is_false_if_node_has_no_value()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id");

            // ACT

            string value;
            var result = node.TryGetValue(out value);

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void TryGetValue_is_true_if_node_has_value()
        {
            // ARRANGE

            var value = "value";
            var node = new ImmutableHierarchy<string, string>.Node("id",value);

            // ACT

            string value_result;
            var result = node.TryGetValue(out value_result);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreSame(value, value_result);
        }

        [Test]
        public void Setting_node_value_creates_a_node_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value");

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
            var node = new ImmutableHierarchy<string, string>.Node("id", value);

            // ACT

            var result = node.SetValue(value);

            // ASSERT

            Assert.AreSame(node, result);
            Assert.AreSame(value, result.value);
        }

        [Test]
        public void Adding_child_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("id2", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value");
            
            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.AreNotSame(node, result);
            
            // old node is unchanged
            Assert.AreEqual(0, node.childNodes.Length);
            Assert.AreEqual("value", node.value);

            // new node contains child
            Assert.AreEqual(1, result.childNodes.Length);
            Assert.AreSame(childNode, result.childNodes[0]);
            Assert.AreEqual("value2", result.childNodes[0].value);
        }

        [Test]
        public void Add_child_twice_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("id2", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value");
            node = node.AddChildNode(childNode);

            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.AreNotSame(node, result);

            // old node is unchanged
            Assert.AreEqual(1, node.childNodes.Length);
            Assert.AreSame(childNode, node.childNodes[0]);
            
            // new node contains child twice.
            Assert.AreSame(childNode, result.childNodes[0]);
            Assert.AreSame(childNode, result.childNodes[1]);
        }

        [Test]
        public void Set_child_node_exchanges_child_and_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("child", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] { childNode });

            // ACT

            var childNode2 = new ImmutableHierarchy<string, string>.Node("child", "value");
            var result = node.SetChildNode(childNode2);

            // ASSERT

            Assert.AreNotSame(node, result);
            
            // old node is unchanged
            Assert.AreEqual(1, node.childNodes.Length);
            Assert.AreSame(childNode, node.childNodes[0]);

            // new node contains the noew child node
            Assert.AreEqual(1, result.childNodes.Length);
            Assert.AreSame(childNode2, result.childNodes[0]);
        }
    }
}
