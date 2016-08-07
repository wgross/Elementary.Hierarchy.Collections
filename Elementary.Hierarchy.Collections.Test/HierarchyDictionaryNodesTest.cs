using Moq;
using NUnit.Framework;

namespace Elementary.Hierarchy.Collections.Test
{
    [TestFixture]
    public class HierarchyDictionaryNodesTest
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
        public void HierarchyDictionary_Nodes_publishes_values_as_dictionary()
        {
            // ACT

            Assert.IsNotNull(this.hierarchyDictionary.Root);
            //Assert.IsInstanceOf<IDictionary<HierarchyPath<string>, IHierarchyNode<string,int>>>(hierarchyDictionary.Nodes);
            Assert.IsInstanceOf<IHierarchyNode<string, int>>(hierarchyDictionary.Root);
        }
    }
}