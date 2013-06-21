using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewSample;

namespace TreeViewSample
{
    namespace DndTree
    {
        public class TreeElementSelected : CompositePresentationEvent<TreeElementSelectedEventArgs> { }
        public class TreeElementSelectedEventArgs
        {
            public TreeElementBase SelectedElement { get; private set; }
            public TreeNode ParentElement { get; private set; }
            public int IndexToParent { get; private set; }

            public TreeElementSelectedEventArgs(TreeElementBase selected, TreeNode parent, int index)
            {
                this.SelectedElement = selected;
                this.ParentElement = parent;
                this.IndexToParent = index;
            }
        }

        public class TreeElementDropped : CompositePresentationEvent<TreeElementDroppedEventArgs> { }
        public class TreeElementDroppedEventArgs
        {
            public TreeElementBase DropTarget { get; private set; }
            public TreeNode DropTargetParent { get; private set; }
            public int TargetIndexToParent { get; private set; }

            public TreeElementDroppedEventArgs(TreeElementBase target, TreeNode parent, int index)
            {
                this.DropTarget = target;
                this.DropTargetParent = parent;
                this.TargetIndexToParent = index;
            }
        }

        public class SelectItemFormHeaderTrigger : CompositePresentationEvent<TreeElementBase> { }
        public class TreeElementDropDebugTrigger : CompositePresentationEvent<IEnumerable<TreeElementBase>> { }
    }
}
