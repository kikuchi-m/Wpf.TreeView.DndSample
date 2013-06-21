using Lib.Prism;
using Lib.Prism.Mef;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeViewSample.Navigation;

namespace TreeViewSample
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override AggregateCatalog CreateAggregateCatalog()
        {
            var agg = base.CreateAggregateCatalog();
            agg.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            return agg;
        }

        protected override DependencyObject CreateShell()
        {
            var shell = ServiceLocator.Current.GetInstance<Shell>();
            var app = Application.Current;
            app.MainWindow = shell;
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.MainWindow.Show();
            return shell;
        }

        protected override void InitializeShell()
        {
            var vm = ((Shell)this.Shell).DataContext as IRegionInitializer;
            if (vm != null)
                vm.InitializeRegions();
        }
    }
}
