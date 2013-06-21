using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeViewSample
{
    public interface ITreeElement
    {
        void Accept(ITreeElementVisitor visitor);
    }

    public abstract class TreeElementBase : ITreeElement
    {
        public string Name { get; protected set; }
        public ElementType ElementType { get; protected set; }

        protected TreeElementBase(string name, ElementType type)
        {
            this.Name = name;
            this.ElementType = type;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public abstract void Accept(ITreeElementVisitor visitor);
    }

    public class TreeNode : TreeElementBase
    {
        private ObservableCollection<TreeElementBase> children = new ObservableCollection<TreeElementBase>();
        public ObservableCollection<TreeElementBase> Children { get { return this.children; } }

        public TreeNode(string name) : base(name, ElementType.Node) { }

        public void Append(TreeElementBase elem)
        {
            this.children.Add(elem);
        }

        public void InsertAt(int i, TreeElementBase elem)
        {
            this.children.Insert(i, elem);
        }

        public void Remove(TreeElementBase elem)
        {
            this.children.Remove(elem);
        }

        public void RemoveAt(int i)
        {
            this.children.RemoveAt(i);
        }

        public override void Accept(ITreeElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class TreeLeaf : TreeElementBase
    {
        public string Image { get; private set; }

        public TreeLeaf(string name)
            : base(name, ElementType.Leaf) { }
        public TreeLeaf(string name, string path)
            : this(name)
        {
            this.Image = path;
        }

        public override void Accept(ITreeElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public enum ElementType
    {
        Node,
        Leaf
    }
}
