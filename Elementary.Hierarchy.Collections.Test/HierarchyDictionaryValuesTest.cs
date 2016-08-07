﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class HierarchyDictionaryValuesTest
    {
        private HierarchyDictionary<string, int> hierarchyDictionary;
        private Mock<IHierarchy<string, int>> hierarchy;

        [SetUp]
        public void ArrangeAllTests()
        {
            this.hierarchy = new Mock<IHierarchy<string, int>>();
            this.hierarchyDictionary = new HierarchyDictionary<string, int>(this.hierarchy.Object);
        }

        [Test]
        public void HierarchyDictionary_Values_publishes_values_as_dictionary()
        {
            // ACT

            Assert.IsNotNull(this.hierarchyDictionary.Values);
            Assert.IsInstanceOf<IDictionary<HierarchyPath<string>, int>>(hierarchyDictionary.Values);
        }

        [Test]
        public void HierarchyDictionary_Values_values_can_be_added()
        {
            // ACT

            this.hierarchyDictionary.Values.Add(HierarchyPath.Create("a"), 1);

            // ASSERT

            this.hierarchy.Verify(h => h.Add(HierarchyPath.Create("a"), 1), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_keyvalue_pairs_can_be_added()
        {
            // ARRANGE

            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), 1);

            // ACT

            this.hierarchyDictionary.Values.Add(kv);

            // ASSERT

            this.hierarchy.Verify(h => h.Add(kv.Key, kv.Value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_clears_underlying_hierarchy()
        {
            // ACT

            this.hierarchyDictionary.Values.Clear();

            // ASSERT

            this.hierarchy.Verify(h => h.Remove(HierarchyPath.Create<string>(), int.MaxValue));
        }

        [Test]
        public void HierarchyDictionary_Values_TryGetValue_from_underlying_hierarchy()
        {
            // ARRANGE
            int value = 1;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out value)).Returns(true);

            // ACT

            int result_value;
            var result = this.hierarchyDictionary.Values.TryGetValue(HierarchyPath.Create("a"), out result_value);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreEqual(1, result_value);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_TryGetValue_fails_if_key_isnt_there()
        {
            // ARRANGE
            int value = 1;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out value)).Returns(false);

            // ACT

            int result_value;
            var result = this.hierarchyDictionary.Values.TryGetValue(HierarchyPath.Create("a"), out result_value);

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_removes_value_from_hierarchy()
        {
            // ARRANGE

            this.hierarchy.Setup(h => h.Remove(HierarchyPath.Create("a"), 1)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsTrue(result);
            this.hierarchy.Verify(h => h.Remove(HierarchyPath.Create("a"), 1), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Remove_fails_if_key_isnt_there()
        {
            // ARRANGE

            this.hierarchy.Setup(h => h.Remove(HierarchyPath.Create("a"), 1)).Returns(false);

            // ACT

            var result = this.hierarchyDictionary.Values.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.Remove(HierarchyPath.Create("a"), 1), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_removes_keyvalue_pairs_from_hierarchy()
        {
            // ARRANGE
            var value = 1;
            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), value);

            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out value)).Returns(true);
            this.hierarchy.Setup(h => h.Remove(HierarchyPath.Create("a"), 1)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values.Remove(kv);

            // ASSERT

            Assert.IsTrue(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out value), Times.Once);
            this.hierarchy.Verify(h => h.Remove(kv.Key, 1), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_removes_keyvalue_pairs_fails_if_value_difers()
        {
            // ARRANGE

            var value = 1;
            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), value);

            var existing_value = 2;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values.Remove(kv);

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
            this.hierarchy.Verify(h => h.Remove(kv.Key, 1), Times.Never);
        }

        [Test]
        public void HierarchyDictionary_Values_removes_keyvalue_pairs_fails_if_key_doesnt_exist()
        {
            // ARRANGE

            var value = 1;
            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), value);

            var existing_value = 2;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(false);

            // ACT

            var result = this.hierarchyDictionary.Values.Remove(kv);

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
            this.hierarchy.Verify(h => h.Remove(kv.Key, 1), Times.Never);
        }

        [Test]
        public void HierarchyDictionary_Values_ContainsKey_looks_into_underlying_hierarchy()
        {
            // ARRANGE

            var existing_value = 2;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values.ContainsKey(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsTrue(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_ContainsKey_fails_if_key_isnt_there()
        {
            // ARRANGE

            var existing_value = 2;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(false);

            // ACT

            var result = this.hierarchyDictionary.Values.ContainsKey(HierarchyPath.Create("a"));

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Contains_fails_if_key_doesnt_exist()
        {
            // ARRANGE

            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), 1);
            var existing_value = 2;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(false);

            // ACT

            var result = this.hierarchyDictionary.Values.Contains(kv);

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Contains_fails_if_doesnt_match()
        {
            // ARRANGE

            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), 1);
            var existing_value = 2;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values.Contains(kv);

            // ASSERT

            Assert.IsFalse(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Contains_looks_into_underlying_hierarchy()
        {
            // ARRANGE

            var kv = new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), 1);
            var existing_value = 1;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values.Contains(kv);

            // ASSERT

            Assert.IsTrue(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out existing_value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Sets_value()
        {
            // ACT

            this.hierarchyDictionary.Values[HierarchyPath.Create("a")] = 1;

            // ASSERT

            this.hierarchy.VerifySet(h => h[HierarchyPath.Create("a")] = 1, Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Gets_value()
        {
            // ARRANGE

            int value = 1;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out value)).Returns(true);

            // ACT

            var result = this.hierarchyDictionary.Values[HierarchyPath.Create("a")];

            // ASSERT

            Assert.AreEqual(1, result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Get_value_fails_if_key_doesnt_exist()
        {
            // ARRANGE

            int value = 1;
            this.hierarchy.Setup(h => h.TryGetValue(HierarchyPath.Create("a"), out value)).Returns(false);

            // ACT
            int result_value;
            var result = Assert.Throws<KeyNotFoundException>(() => result_value = this.hierarchyDictionary.Values[HierarchyPath.Create("a")]);

            // ASSERT

            Assert.IsNotNull(result);
            this.hierarchy.Verify(h => h.TryGetValue(HierarchyPath.Create("a"), out value), Times.Once);
        }

        [Test]
        public void HierarchyDictionary_Values_Counts_nodes_with_values()
        {
            // ARRANGE

            var a = new Mock<IHierarchyNode<string, int>>();
            a.Setup(n => n.HasValue).Returns(true);
            a.Setup(n => n.Path).Returns(HierarchyPath.Create("a"));

            var b = new Mock<IHierarchyNode<string, int>>();
            b.Setup(n => n.HasValue).Returns(false);
            b.Setup(n => n.Path).Returns(HierarchyPath.Create("b"));

            var rootNode = new Mock<IHierarchyNode<string, int>>();
            rootNode.Setup(n => n.HasChildNodes).Returns(true);
            rootNode.Setup(n => n.Path).Returns(HierarchyPath.Create<string>());
            rootNode.Setup(n => n.HasValue).Returns(false);
            rootNode.Setup(n => n.ChildNodes).Returns(new[] { a.Object, b.Object });

            this.hierarchy.Setup(h => h.Traverse(HierarchyPath.Create<string>())).Returns(rootNode.Object);

            // ACT

            var result = this.hierarchyDictionary.Values.Count;

            // ASSERT

            Assert.AreEqual(1, result);
        }

        [Test]
        public void HierarchyDictionary_Values_Keys_returns_only_key_having_a_value()
        {
            // ARRANGE

            var a = new Mock<IHierarchyNode<string, int>>();
            a.Setup(n => n.HasValue).Returns(true);
            a.Setup(n => n.Path).Returns(HierarchyPath.Create("a"));

            var b = new Mock<IHierarchyNode<string, int>>();
            b.Setup(n => n.HasValue).Returns(false);
            b.Setup(n => n.Path).Returns(HierarchyPath.Create("b"));

            var rootNode = new Mock<IHierarchyNode<string, int>>();
            rootNode.Setup(n => n.HasChildNodes).Returns(true);
            rootNode.Setup(n => n.Path).Returns(HierarchyPath.Create<string>());
            rootNode.Setup(n => n.HasValue).Returns(false);
            rootNode.Setup(n => n.ChildNodes).Returns(new[] { a.Object, b.Object });

            this.hierarchy.Setup(h => h.Traverse(HierarchyPath.Create<string>())).Returns(rootNode.Object);

            // ACT

            var result = this.hierarchyDictionary.Values.Keys;

            // ASSERT

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(HierarchyPath.Create("a"), result.First());
        }

        [Test]
        public void HierarchyDictionary_Values_returns_only_values()
        {
            // ARRANGE

            var a = new Mock<IHierarchyNode<string, int>>();
            a.Setup(n => n.HasValue).Returns(true);
            a.Setup(n => n.Value).Returns(1);
            a.Setup(n => n.Path).Returns(HierarchyPath.Create("a"));

            var b = new Mock<IHierarchyNode<string, int>>();
            b.Setup(n => n.HasValue).Returns(false);
            b.Setup(n => n.Path).Returns(HierarchyPath.Create("b"));

            var rootNode = new Mock<IHierarchyNode<string, int>>();
            rootNode.Setup(n => n.HasChildNodes).Returns(true);
            rootNode.Setup(n => n.Path).Returns(HierarchyPath.Create<string>());
            rootNode.Setup(n => n.HasValue).Returns(false);
            rootNode.Setup(n => n.ChildNodes).Returns(new[] { a.Object, b.Object });

            this.hierarchy.Setup(h => h.Traverse(HierarchyPath.Create<string>())).Returns(rootNode.Object);

            // ACT

            var result = this.hierarchyDictionary.Values.Values;

            // ASSERT

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result.First());
        }

        [Test]
        public void HierarchyDictionary_Values_CopyTo_only_copies_nodes_having_values()
        {
            // ARRANGE

            var a = new Mock<IHierarchyNode<string, int>>();
            a.Setup(n => n.HasValue).Returns(true);
            a.Setup(n => n.Value).Returns(1);
            a.Setup(n => n.Path).Returns(HierarchyPath.Create("a"));

            var b = new Mock<IHierarchyNode<string, int>>();
            b.Setup(n => n.HasValue).Returns(false);
            b.Setup(n => n.Path).Returns(HierarchyPath.Create("b"));

            var rootNode = new Mock<IHierarchyNode<string, int>>();
            rootNode.Setup(n => n.HasChildNodes).Returns(true);
            rootNode.Setup(n => n.Path).Returns(HierarchyPath.Create<string>());
            rootNode.Setup(n => n.HasValue).Returns(false);
            rootNode.Setup(n => n.ChildNodes).Returns(new[] { a.Object, b.Object });

            this.hierarchy.Setup(h => h.Traverse(HierarchyPath.Create<string>())).Returns(rootNode.Object);

            // ACT

            var result = new KeyValuePair<HierarchyPath<string>, int>[2];
            this.hierarchyDictionary.Values.CopyTo(result, 1);

            // ASSERT

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(default(KeyValuePair<HierarchyPath<string>, int>), result[0]);
            Assert.AreEqual(new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), 1), result[1]);
        }

        [Test]
        public void HierarchyDictionary_Values_CopyTo_throws_on_null_array()
        {
            // ACT

            var result = Assert.Throws<ArgumentNullException>(() => this.hierarchyDictionary.Values.CopyTo(null, 1));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual("array", result.ParamName);
        }

        [Test]
        public void HierarchyDictionary_Values_GetEnumerator_returns_only_KeyValuePairs_having_values()
        {
            // ARRANGE

            var a = new Mock<IHierarchyNode<string, int>>();
            a.Setup(n => n.HasValue).Returns(true);
            a.Setup(n => n.Value).Returns(1);
            a.Setup(n => n.Path).Returns(HierarchyPath.Create("a"));

            var b = new Mock<IHierarchyNode<string, int>>();
            b.Setup(n => n.HasValue).Returns(false);
            b.Setup(n => n.Path).Returns(HierarchyPath.Create("b"));

            var rootNode = new Mock<IHierarchyNode<string, int>>();
            rootNode.Setup(n => n.HasChildNodes).Returns(true);
            rootNode.Setup(n => n.Path).Returns(HierarchyPath.Create<string>());
            rootNode.Setup(n => n.HasValue).Returns(false);
            rootNode.Setup(n => n.ChildNodes).Returns(new[] { a.Object, b.Object });

            this.hierarchy.Setup(h => h.Traverse(HierarchyPath.Create<string>())).Returns(rootNode.Object);

            // ACT

            var result = this.hierarchyDictionary.Values.ToList();

            // ASSERT

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new KeyValuePair<HierarchyPath<string>, int>(HierarchyPath.Create("a"), 1), result.First());
        }
    }
}