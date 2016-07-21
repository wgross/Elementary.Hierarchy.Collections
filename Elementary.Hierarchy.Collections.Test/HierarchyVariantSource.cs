using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyVariantSource
    {
        public static IEnumerable WithoutDefaultValue
        {
            get
            {
                yield return new ImmutableHierarchy<string, string>();
                yield return new MutableHierarchy<string, string>();
            }
        }

        public static readonly string DefaultValue = "defauöt value";

        public static IEnumerable WithDefaultValue
        {
            get
            {
                yield return new ImmutableHierarchy<string, string>(getDefaultValue:k => DefaultValue);
                yield return new MutableHierarchy<string, string>(getDefaultValue:k => DefaultValue);
            }
        }
    }
}
