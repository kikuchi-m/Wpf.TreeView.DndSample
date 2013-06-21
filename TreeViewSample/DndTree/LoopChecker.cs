using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeViewSample.DndTree
{
    public class LoopChecker : ITreeElementVisitor
    {
        public bool ContainsTarget { get; private set; }
        public TreeElementBase Target { get; private set; }

        public LoopChecker(TreeElementBase target)
        {
            this.Target = target;
        }

        public void Visit(TreeNode node)
        {
            if (node == this.Target)
                this.ContainsTarget = true;
            else
                foreach (var child in node.Children)
                    child.Accept(this);
        }

        public void Visit(TreeLeaf leaf)
        {
            if (leaf == this.Target)
                this.ContainsTarget = true;
        }
    }
}
