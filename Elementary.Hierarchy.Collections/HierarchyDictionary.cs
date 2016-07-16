using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections
{
    public class HierarchyDictionary<K, V>
    {
        #region Implements IDictionary interface based on the hierarchical value storage

        private class ValuesAsDictionaryFacade : IDictionary<HierarchyPath<K>, V>
        {
            #region Initialization of this instance

            public ValuesAsDictionaryFacade(IHierarchy<K, V> hierarchy)
            {
                this.hierarchy = hierarchy;
            }

            private readonly IHierarchy<K, V> hierarchy;

            #endregion Initialization of this instance

            public V this[HierarchyPath<K> key]
            {
                get
                {
                    V value;
                    if (this.hierarchy.TryGetValue(key, out value))
                        return value;
                    throw new KeyNotFoundException("The given key was not present in the dictionary.");
                }

                set
                {
                    this.hierarchy[key] = value;
                }
            }

            public int Count => this.TraverseValueNodesFromRoot().Count();

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<HierarchyPath<K>> Keys => this.TraverseValueNodesFromRoot().Select(n => n.Path).ToList();

            public ICollection<V> Values => this.TraverseValueNodesFromRoot().Select(n => n.Value).ToList();

            public void Add(KeyValuePair<HierarchyPath<K>, V> item)
            {
                this.hierarchy.Add(item.Key, item.Value);
            }

            public void Add(HierarchyPath<K> key, V value)
            {
                this.hierarchy.Add(key, value);
            }

            public void Clear()
            {
                this.hierarchy.Remove(HierarchyPath.Create<K>(), int.MaxValue);
            }

            public bool Contains(KeyValuePair<HierarchyPath<K>, V> item)
            {
                V value;
                if (this.hierarchy.TryGetValue(item.Key, out value))
                    if (EqualityComparer<V>.Default.Equals(item.Value, value))
                        return true;
                return false;
            }

            public bool ContainsKey(HierarchyPath<K> key)
            {
                V value;
                return this.hierarchy.TryGetValue(key, out value);
            }

            public void CopyTo(KeyValuePair<HierarchyPath<K>, V>[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                this.TraverseValueNodesFromRoot()
                    .Select(n => new KeyValuePair<HierarchyPath<K>, V>(n.Path, n.Value))
                    .ToList()
                    .CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<HierarchyPath<K>, V>> GetEnumerator()
            {
                return TraverseValueNodesFromRoot()
                  .Select(n => new KeyValuePair<HierarchyPath<K>, V>(n.Path, n.Value))
                  .GetEnumerator();
            }

            private IEnumerable<IHierarchyNode<K, V>> TraverseValueNodesFromRoot()
            {
                return this.hierarchy
                    .Traverse(HierarchyPath.Create<K>())
                    .DescendantsOrSelf()
                    .Where(n => n.HasValue);
            }

            public bool Remove(KeyValuePair<HierarchyPath<K>, V> item)
            {
                V existingValue;
                if (this.hierarchy.TryGetValue(item.Key, out existingValue))
                    if (EqualityComparer<V>.Default.Equals(item.Value, existingValue))
                        return this.hierarchy.Remove(item.Key, 1);
                return false;
            }

            public bool Remove(HierarchyPath<K> key)
            {
                return this.hierarchy.Remove(key, 1);
            }

            public bool TryGetValue(HierarchyPath<K> key, out V value)
            {
                return this.hierarchy.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion Implements IDictionary interface based on the hierarchical value storage

        #region Initialization of this instance

        public HierarchyDictionary(IHierarchy<K, V> hierarchyImplementation)
        {
            this.hierarchy = hierarchyImplementation;
            this.values = new ValuesAsDictionaryFacade(this.hierarchy);
        }

        private readonly IDictionary<HierarchyPath<K>, V> values;
        private readonly IHierarchy<K, V> hierarchy;

        #endregion Initialization of this instance

        public object Nodes { get; set; }
        public IDictionary<HierarchyPath<K>, V> Values => this.values;
    }
}