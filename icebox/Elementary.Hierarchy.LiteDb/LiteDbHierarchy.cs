using Elementary.Hierarchy.Collections;
using LiteDB;
using System;
using System.Reflection;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchy<TKey, TValue> : IHierarchy<TKey, TValue>
    {
#pragma warning disable RECS0108 // Warns about static fields in generic types
        private static readonly TypeInfo TValueTypeInfo = typeof(TValue).GetTypeInfo();
#pragma warning restore RECS0108 // Warns about static fields in generic types

        #region Internal Node class

        public class Node : BsonDocument
        {
            private string v;

            public Node(TKey key)
            {
                this["_id"] = new BsonValue(key);
            }

            public Node(TKey key, TValue value)
            {
                this["_id"] = new BsonValue(key);
                this["_value"] = new BsonValue(value);
            }

            public bool TryGetValue(out TValue value)
            {
                value = default(TValue);
                var bsonValue = this.Get("_value");
                if (bsonValue == null || bsonValue.IsNull)
                    return false;

                if (!TValueTypeInfo.IsAssignableFrom(bsonValue.RawValue.GetType().GetTypeInfo()))
                    return false;
                    
                value = (TValue)(bsonValue.RawValue);
                return true;
            }
        }

        #endregion Internal Node class

        public LiteDbHierarchy()
        {
        }

        public TValue this[HierarchyPath<TKey> hierarchyPath]
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(HierarchyPath<TKey> hierarchyPath, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(HierarchyPath<TKey> hierarchyPath, int? maxDepth = default(int?))
        {
            throw new NotImplementedException();
        }

        public bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse)
        {
            throw new NotImplementedException();
        }

        public IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            throw new NotImplementedException();
        }
    }
}