using Lib.Prism;
using Lib.Prism.Mef;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeViewSample.Navigation;

namespace TreeViewSample
{
    [Export(typeof(IShellViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShellViewModel : NotificationObject, IShellViewModel, IRegionInitializer
    {
        public IRegionManager RegionManager { get; private set; }

        [ImportingConstructor]
        public ShellViewModel(
            IRegionManager regioManger,
            IEventAggregator eventAggregator)
        {
            this.RegionManager = regioManger;

            this.ViewList = new List<string>(RegionName.GetViewKeys(RegionKey.MainRegion));
        }

        public void InitializeRegions()
        {
            this.InitializeRegion(RegionName.MainRegion, RegionName.GetViewTypePairs(RegionKey.MainRegion));
        }

        private IList<string> viewList;
        public IList<string> ViewList
        {
            get { return this.viewList; }
            set
            {
                if (this.viewList != value)
                {
                    this.viewList = value;
                    this.RaisePropertyChanged(() => this.ViewList);
                }
            }
        }

        private int selectedViewIndex;
        public int SelectedViewIndex
        {
            get { return this.selectedViewIndex; }
            set
            {
                if (this.selectedViewIndex != value)
                {
                    this.selectedViewIndex = value;
                    this.RaisePropertyChanged(() => this.SelectedViewIndex);
                    this.Navigate(value);
                }
            }
        }

        private void Navigate(int i)
        {
            this.RegionManager.RequestNavigate(RegionName.MainRegion, ViewKey.GetViewUri(this.ViewList[i]));
        }

        private DelegateCommand reloadCommand;
        public DelegateCommand ReloadCommand
        {
            get
            {
                if (this.reloadCommand == null)
                {
                    this.reloadCommand = new DelegateCommand
                      (() =>
                      {
                          this.Navigate(this.SelectedViewIndex);
                      }, () => true);
                }
                return this.reloadCommand;
            }
        }
    }
}
