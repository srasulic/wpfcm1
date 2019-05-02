using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using wpfcm1.Settings;
using wpfcm1.Shell;

namespace wpfcm1
{
    public class AppBootstrapper : BootstrapperBase 
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
<<<<<<< HEAD
        public static string appVersion = "1.5.0.09";
=======
        //private static string appTitle = User.Default.Variation == "BIH" ? "Banja Luka" : " ";
        private static string appTitle = User.Default.AppTitle;
        public static string appVersion = appTitle + " - 1.5.0.05";
>>>>>>> feature_bih

        CompositionContainer _container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());
            _container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
            batch.AddExportedValue(catalog);

            _container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);
            if (exports.Any()) return exports.First();
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
            Log.Info("Session started! " + appVersion);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            Log.Info("Session ended!");
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error("Unhandled exception", e.Exception);
            //TODO: ovo izbrisi pod hitno
            //MessageBox.Show(
            //    Application.Current.MainWindow,
            //    "Error encountered!" + Environment.NewLine + e.Exception.Message,
            //    "Application Error",
            //    MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
