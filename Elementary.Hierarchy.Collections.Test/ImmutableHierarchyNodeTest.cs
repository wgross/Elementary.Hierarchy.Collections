using NUnit.Framework;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class ImmutableHierarchyNodeTest
    {
        [Test]
        public void IMHN_TryGetValue_is_false_if_node_has_no_value()
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
        public void IMHN_TryGetValue_is_true_if_node_has_value()
        {
            // ARRANGE

            var value = "value";
            var node = new ImmutableHierarchy<string, string>.Node("id", value);

            // ACT

            string value_result;
            var result = node.TryGetValue(out value_result);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreSame(value, value_result);
        }

        [Test]
        public void IMHN_Setting_node_value_creates_a_node_node()
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
        public void IMHN_Setting_node_value_twice_doesnt_create_new_node()
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
        public void IMHN_Adding_child_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("id2", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value");

            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.AreNotSame(node, result);

            // old node is unchanged
            Assert.AreEqual(0, node.ChildNodes.Count());
            Assert.AreEqual("value", node.value);

            // new node contains child
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(childNode, result.ChildNodes.ElementAt(0));
            Assert.AreEqual("value2", result.ChildNodes.ElementAt(0).value);
        }

        [Test]
        public void IMHN_Add_child_twice_creates_new_node()
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
            Assert.AreEqual(1, node.ChildNodes.Count());
            Assert.AreSame(childNode, node.ChildNodes.ElementAt(0));

            // new node contains child twice.
            Assert.AreSame(childNode, result.ChildNodes.ElementAt(0));
            Assert.AreSame(childNode, result.ChildNodes.ElementAt(1));
        }

        [Test]
        public void IMHN_Set_child_node_exchanges_child_and_creates_new_node()
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
            Assert.AreEqual(1, node.ChildNodes.Count());
            Assert.AreSame(childNode, node.ChildNodes.ElementAt(0));

            // new node contains the noew child node
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(childNode2, result.ChildNodes.ElementAt(0));
        }

        [Test]
        public void IMHN_Unset_value_of_node_create_new_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child", "value2")
            });

            // ACT

            var result = node.UnsetValue();

            // ASSERT

            Assert.AreNotSame(node, result);

            // old node remains unchanged
            Assert.IsTrue(node.HasValue);
            Assert.AreEqual(1, node.ChildNodes.Count());

            // new node has no value
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(node.ChildNodes.ElementAt(0), result.ChildNodes.ElementAt(0));
        }

        [Test]
        public void IMHN_Unset_value_twice_doesnt_create_new_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child", "value2")
            });

            node = node.UnsetValue();

            // ACT

            var result = node.UnsetValue();

            // ASSERT

            Assert.AreSame(node, result);
        }

        [Test]
        public void IMHN_Unset_value_of_parent_node_removes_empty_child_On_UnsetVaue_with_prune()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child")
            });

            // ACT

            var result = node.UnsetValue(prune: true);

            // ASSERT

            Assert.AreNotSame(node, result);

            // old node remains unchanged
            Assert.IsTrue(node.HasValue);
            Assert.AreEqual(1, node.ChildNodes.Count());

            // new node has no value
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(0, result.ChildNodes.Count());
        }

        [Test]
        public void IMHN_Unset_value_of_parent_node_keeps_non_empty_child_OnUnsetValue_with_prune()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child","value2")
            });

            // ACT

            var result = node.UnsetValue(prune: true);

            // ASSERT

            Assert.AreNotSame(node, result);

            // old node remains unchanged
            Assert.IsTrue(node.HasValue);
            Assert.AreEqual(1, node.ChildNodes.Count());

            // new node has no value
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(1, result.ChildNodes.Count());
            Assert.AreSame(node.ChildNodes.ElementAt(0), result.ChildNodes.ElementAt(0));
        }
    }
}