using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TreeViewSample
{
    public class TreeElementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NodeTemplate { get; set; }
        public DataTemplate LeafTEmplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var elem = item as TreeElementBase;
            if (elem != null)
                switch (elem.ElementType)
                {
                    case ElementType.Node:
                        return NodeTemplate;
                    case ElementType.Leaf:
                        return LeafTEmplate;
                    default:
                        break;
                }
            return base.SelectTemplate(item, container);
        }
    }
}
