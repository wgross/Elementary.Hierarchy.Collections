using NUnit.Framework;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class MutableHierarchyNodeTest
    {
        [Test]
        public void MHN_TryGetValue_is_false_if_node_has_no_value()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id");

            // ACT

            string value;
            var result = node.TryGetValue(out value);

            // ASSERT

            Assert.IsFalse(result);
        }

        [Test]
        public void MHN_TryGetValue_is_true_if_node_has_value()
        {
            // ARRANGE

            var value = "value";
            var node = new MutableHierarchy<string, string>.Node("id", value);

            // ACT

            string value_result;
            var result = node.TryGetValue(out value_result);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreSame(value, value_result);
        }

        [Test]
        public void MHN_Setting_node_value_creates_a_node_node()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id", "value");

            // ACT

            var result = node.SetValue("newValue");

            // ASSERT

            Assert.AreSame(node, result);
            Assert.AreEqual("newValue", result.value);
        }

        [Test]
        public void MHN_Setting_node_value_twice_doesnt_create_new_node()
        {
            // ARRANGE

            string value = "value";
            var node = new MutableHierarchy<string, string>.Node("id", value);

            // ACT

            var result = node.SetValue(value);

            // ASSERT

            Assert.AreSame(node, result);
            Assert.AreSame(value, result.value);
        }

        [Test]
        public void MHN_Adding_child_doesnt_create_new_node()
        {
            // ARRANGE

            var childNode = new MutableHierarchy<string, string>.Node("id2", "value2");
            var node = new MutableHierarchy<string, string>.Node("id", "value");

            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.AreSame(node, result);

            // node contains child
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(childNode, result.ChildNodes.ElementAt(0));
            Assert.AreEqual("value2", result.ChildNodes.ElementAt(0).value);
        }

        [Test]
        public void MHN_Add_child_twice_doesnt_create_new_node()
        {
            // ARRANGE

            var childNode = new MutableHierarchy<string, string>.Node("id2", "value2");
            var node = new MutableHierarchy<string, string>.Node("id", "value");
            node = node.AddChildNode(childNode);

            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.AreSame(node, result);

            // node contains child twice.
            Assert.AreSame(childNode, result.ChildNodes.ElementAt(0));
            Assert.AreSame(childNode, result.ChildNodes.ElementAt(1));
        }

        [Test]
        public void MHN_Set_child_node_exchanges_child_and_doesnt_create_new_node()
        {
            // ARRANGE

            var childNode = new MutableHierarchy<string, string>.Node("child", "value2");
            var node = new MutableHierarchy<string, string>.Node("id", "value", new[] { childNode });

            // ACT

            var childNode2 = new MutableHierarchy<string, string>.Node("child", "value");
            var result = node.SetChildNode(childNode2);

            // ASSERT

            Assert.AreSame(node, result);

            // new node contains the new child node
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(childNode2, result.ChildNodes.ElementAt(0));
        }

        [Test]
        public void MHN_Unset_value_of_node_doesnt_create_new_node()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id", "value", new[] {
                new MutableHierarchy<string,string>.Node("child", "value2")
            });

            // ACT

            var result = node.UnsetValue();

            // ASSERT

            Assert.AreSame(node, result);

            // node has no value
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(node.ChildNodes.ElementAt(0), result.ChildNodes.ElementAt(0));
        }

        [Test]
        public void MHN_Unset_value_twice_doesnt_create_new_node()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id", "value", new[] {
                new MutableHierarchy<string,string>.Node("child", "value2")
            });

            node = node.UnsetValue();

            // ACT

            var result = node.UnsetValue();

            // ASSERT

            Assert.AreSame(node, result);
        }

        [Test]
        public void MHN_Unset_value_of_parent_node_removes_empty_child_on_UnsetValue_with_prune()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id", "value", new[] {
                new MutableHierarchy<string,string>.Node("child")
            });

            // ACT

            var result = node.UnsetValue(prune: true);

            // ASSERT

            Assert.AreSame(node, result);

            // node has no value
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(0, result.ChildNodes.Count());
        }

        [Test]
        public void MHN_Unset_value_of_parent_node_keeps_non_empty_child_on_UnsetValue_with_prune()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id", "value", new[] {
                new MutableHierarchy<string,string>.Node("child","value2")
            });

            // ACT

            var result = node.UnsetValue(prune: true);

            // ASSERT

            Assert.AreSame(node, result);

            // node has no value
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(node.ChildNodes.ElementAt(0), result.ChildNodes.ElementAt(0));
        }

        [Test]
        public void MHN_Remove_child_from_node()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, string>.Node("id", "value", new[] {
                new MutableHierarchy<string,string>.Node("child","value2")
            });

            // ACT

            node.RemoveChildNode(node.ChildNodes.Single());

            // ASSERT

            Assert.IsTrue(node.HasValue);
            string value;
            Assert.IsTrue(node.TryGetValue(out value));
            Assert.AreEqual("value", value);
            Assert.AreEqual("id", node.key);
        }
    }
}