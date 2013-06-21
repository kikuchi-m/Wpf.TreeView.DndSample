using Lib.Wpf;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeViewSample.DndTree
{
    /// <summary>
    /// Interaction logic for DndTreeView.xaml
    /// </summary>
    [Export(typeof(IDndTreeView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DndTreeView : UserControl, IDndTreeView
    {
        private TreeViewItem selectedContainer;

        private readonly TreeElementSelected selectedEvent;
        private readonly TreeElementDropped droppedEvent;

        public DndTreeView()
        {
            var agg = ServiceLocator.Current.GetInstance<IEventAggregator>();
            this.selectedEvent = agg.GetEvent<TreeElementSelected>();
            this.droppedEvent = agg.GetEvent<TreeElementDropped>();
            agg.GetEvent<SelectItemFormHeaderTrigger>()
                .Subscribe(header => this.DndTree.SelectItemFromHeader(header));
            agg.GetEvent<TreeElementDropDebugTrigger>()
                .Subscribe(this.DebugDropResult);
            InitializeComponent();
        }

        private void DebugDropResult(IEnumerable<TreeElementBase> before)
        {
            var visitor = new DropDebugger();
            this.DndTree.Accept(visitor);

            Debug.WriteLine("<<drop result>>");
            Debug.WriteLine(string.Format("{0} -> {1}", before.Count(), visitor.Context.Count));
            foreach (var elem in before)
            {
                Debug.WriteLine(string.Format("{0} : {1}",
                    elem.Name,
                    visitor.Context.Contains(elem)));
            }
        }

        public class DropDebugger : TreeViewItemVisitor<IList<TreeElementBase>>
        {
            public DropDebugger()
                : base(new List<TreeElementBase>())
            { }

            public override void Visit(TreeViewItem item)
            {
                this.Context.Add((TreeElementBase)item.Header);
            }
        }

        [Import]
        public IDndTreeViewModel ViewModel
        {
            get { return this.ViewModel as IDndTreeViewModel; }
            set { this.DataContext = value; }
        }

        private void TreeElement_Selected(object sender, RoutedEventArgs e)
        {
            var selectedContainer = sender as TreeViewItem;
            this.selectedContainer = selectedContainer;
            if (selectedContainer == null)
                return;
            e.Handled = true;

            var selectedHeader = selectedContainer.Header as TreeElementBase;
            var parentContainer = ItemsControl.ItemsControlFromItemContainer(selectedContainer) as TreeViewItem;
            var parentHeader = parentContainer != null ? parentContainer.Header as TreeNode : null;
            var index = ItemsControl.GetAlternationIndex(selectedContainer);

            this.Selected.Text = selectedHeader.Name;
            this.ParentOfSelected.Text = parentHeader == null ? "(null)" : parentHeader.Name;
            this.IndexToParent.Text = index.ToString();

            this.selectedEvent.Publish(new TreeElementSelectedEventArgs(selectedHeader, parentHeader, index));
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tvi = sender as TreeViewItem;
            if (tvi != null)
            {
                tvi.IsSelected = true;
                e.Handled = true;
            }
        }

        private Point lastClickPoint;
        private TreeViewItem targetContainer;
        private bool isDragStarted;

        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.lastClickPoint = e.GetPosition(this.DndTree);
            this.isDragStarted = false;
            Debug.WriteLine(string.Format("click point : {0}", this.lastClickPoint));
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(this.DndTree);

                if (!this.isDragStarted)
                    this.isDragStarted = this.CheckDragStarted(currentPosition);

                if (this.isDragStarted
                    && this.selectedContainer != null)
                {
                    //Debug.WriteLine("mouse over / drag started");
                    DragDropEffects dropEffect = DragDrop.DoDragDrop(
                        this.DndTree,
                        this.selectedContainer,
                        DragDropEffects.Move);
                }
            }
        }

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            var hoveredItem = sender as TreeViewItem;
            if (hoveredItem != null
                && this.selectedContainer != hoveredItem
                && this.isDragStarted)
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            //Debug.WriteLine(string.Format("drag over {0} / {1}",
            //    e.OriginalSource.GetType().Name,
            //    e.Source.GetType().Name));
            e.Handled = true;
        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            var origin = e.OriginalSource as UIElement;

            this.targetContainer = sender as TreeViewItem;
            if (this.targetContainer != null
                && this.targetContainer != this.selectedContainer
                && this.targetContainer.Header != this.selectedContainer.Header)
            {
                e.Effects = DragDropEffects.Move;
                var targetHeader = targetContainer.Header as TreeElementBase;
                var parentContainer = ItemsControl.ItemsControlFromItemContainer(targetContainer) as TreeViewItem;
                var parentHeader = parentContainer != null ? parentContainer.Header as TreeNode : null;
                var index = ItemsControl.GetAlternationIndex(targetContainer);

                Debug.WriteLine(string.Format("drop target : {0}({3}) / target parent : {1} / index : {2}",
                    targetHeader.Name,
                    parentHeader != null ? parentHeader.Name : "(target parent is null.)",
                    index,
                    targetHeader.ElementType));

                this.droppedEvent.Publish(new TreeElementDroppedEventArgs(targetHeader, parentHeader, index));
            }
            e.Handled = true;
        }

        private bool CheckDragStarted(Point currentPosition)
        {
            return (Math.Abs(currentPosition.X - this.lastClickPoint.X) > 10.0)
                    || (Math.Abs(currentPosition.Y - this.lastClickPoint.Y) > 10.0);
        }
    }
}
