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
        public static IEnumerable WithouthDefaultValue
        {
            get
            {
                yield return new ImmutableHierarchy<string, string>();
                yield return new MutableHierarchy<string, string>();
            }
        }
    }
}
