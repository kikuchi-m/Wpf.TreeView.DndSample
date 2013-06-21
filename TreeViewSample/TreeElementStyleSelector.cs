using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TreeViewSample
{
    public class TreeElementStyleSelector : StyleSelector
    {
        public Style NodeStyle { get; set; }
        public Style LeafStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var elem = item as TreeElementBase;
            if (elem != null)
                switch (elem.ElementType)
                {
                    case ElementType.Node:
                        return NodeStyle;
                    case ElementType.Leaf:
                        return LeafStyle;
                    default:
                        break;
                }
            return base.SelectStyle(item, container);
        }
    }
}
