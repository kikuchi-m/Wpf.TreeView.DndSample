using Lib.Misc;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewSample;

namespace TreeViewSample.DndTree
{
    [Export(typeof(IDndTreeViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DndTreeViewModel : NotificationObject, IDndTreeViewModel
    {
        private TreeElementBase selectedElement;
        private TreeNode selectedElementParent;
        private int selectedIndexToParent;

        private readonly SelectItemFormHeaderTrigger selectItemTrigger;
        private readonly TreeElementDropDebugTrigger dropDebugTrigger;

        [ImportingConstructor]
        public DndTreeViewModel(IEventAggregator eventAggregator)
        {
            GenerateInitial();

            this.selectItemTrigger = eventAggregator.GetEvent<SelectItemFormHeaderTrigger>();
            this.dropDebugTrigger = eventAggregator.GetEvent<TreeElementDropDebugTrigger>();
            eventAggregator.GetEvent<TreeElementSelected>()
                .Subscribe(this.TreeElementSelected);
            eventAggregator.GetEvent<TreeElementDropped>()
                .Subscribe(this.TreeElementDropped);
        }

        private void TreeElementDropped(TreeElementDroppedEventArgs args)
        {
            if (args.DropTarget != null)
            {

                var checker = new LoopChecker(args.DropTarget);
                this.selectedElement.Accept(checker);
                if (checker.ContainsTarget)
                    return;

                var visitor = new TreeElementCollector();
                this.TreeSource.ForEach(elem => elem.Accept(visitor));

                ObservableCollection<TreeElementBase> insertTargetCollection, removeTargetCollection;
                int insertPosition, removePosition;
                var removeTarget = this.selectedElement;

                insertTargetCollection = args.DropTargetParent != null ? args.DropTargetParent.Children : this.TreeSource;
                removeTargetCollection = this.selectedElementParent != null ? this.selectedElementParent.Children : this.TreeSource;
                removePosition = this.selectedIndexToParent;
                if (insertTargetCollection == removeTargetCollection)
                    insertPosition = args.TargetIndexToParent
                        + (this.selectedIndexToParent > args.TargetIndexToParent
                          ? 1
                          : 0);
                else
                    insertPosition = args.TargetIndexToParent + 1;

                Debug.WriteLine(string.Format("drag source : {0}({3}) / source parent : {1} / idnex : {2}",
                    removeTarget.Name,
                    this.selectedElementParent != null ? this.selectedElementParent.Name : "(selected parent is null.)",
                    this.selectedIndexToParent,
                    removeTarget.ElementType));
                try
                {
                    removeTargetCollection.RemoveAt(removePosition);
                    insertTargetCollection.Insert(insertPosition, removeTarget);
                    this.selectItemTrigger.Publish(removeTarget);
                }
                catch (Exception e)
                {
                    throw;
                }
                this.dropDebugTrigger.Publish(visitor.Context);
            }
        }

        private void TreeElementSelected(TreeElementSelectedEventArgs args)
        {
            this.selectedElement = args.SelectedElement;
            this.selectedElementParent = args.ParentElement;
            this.selectedIndexToParent = args.IndexToParent;
        }

        private void GenerateInitial()
        {
            var elem0 = new TreeNode("elem 0");
            {
                var elem00 = new TreeNode("elm 00");
                {
                    var elem000 = new TreeLeaf("elm 000");
                    var elem001 = new TreeLeaf("elm 001");

                    elem00.Append(elem000);
                    elem00.Append(elem001);
                }
                var elem01 = new TreeNode("elm 01");
                {
                    var elem010 = new TreeLeaf("elm 010");
                    var elem011 = new TreeLeaf("elm 011");

                    elem01.Append(elem010);
                    elem01.Append(elem011);
                }
                var elem02 = new TreeLeaf("elm 02");

                elem0.Append(elem00);
                elem0.Append(elem01);
                elem0.Append(elem02);
            }
            var elem1 = new TreeNode("elem 1");
            {
                var elem100 = new TreeLeaf("elm 100");
                var elem101 = new TreeLeaf("elm 101");

                elem1.Append(elem100);
                elem1.Append(elem101);
            }

            var source = new ObservableCollection<TreeElementBase>
            {
                elem0,
                elem1
            };

            this.TreeSource = source;
        }

        private ObservableCollection<TreeElementBase> treeSource;
        public ObservableCollection<TreeElementBase> TreeSource
        {
            get { return this.treeSource; }
            set
            {
                if (this.treeSource != value)
                {
                    this.treeSource = value;
                    this.RaisePropertyChanged(() => this.TreeSource);
                }
            }
        }

    }
}
