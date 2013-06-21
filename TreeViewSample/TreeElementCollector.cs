using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeViewSample
{
    public interface ITreeElementVisitor
    {
        void Visit(TreeNode node);
        void Visit(TreeLeaf leaf);
    }

    public class TreeElementCollector : ITreeElementVisitor
    {
        public IList<TreeElementBase> Context { get; private set; }

        public TreeElementCollector()
        {
            this.Context = new List<TreeElementBase>();
        }

        public void Visit(TreeNode node)
        {
            this.Context.Add(node);
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public void Visit(TreeLeaf leaf)
        {
            this.Context.Add(leaf);
        }
    }
}
