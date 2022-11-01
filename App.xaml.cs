using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Mvvm.Input;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Com.Josh2112.OnKeyDoThing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        private bool isDebugMode;

        private TaskbarIcon taskbarIcon;

        private ObservableCollection<Mapping> mappings;

        public string ConfigPath { get; }

        public RelayCommand ShowWindowCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public App()
        {
            var singleInstanceHelper = new SingleInstanceHelper( "{809c1153-0ad6-47e8-8050-f5356887f525}",
                () => ShowWindowCommand.Execute( null ), () => Shutdown() );
            Exit += ( s, e ) => singleInstanceHelper.Dispose();

            Debug.Assert( isDebugMode = true );

            var company = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            var product = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            var appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
            var path = Path.Combine( appDataPath, company, product );
            Directory.CreateDirectory( path );

            ConfigPath = Path.Combine( path, "hotkeys.json" );
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            ShowWindowCommand = new RelayCommand( () =>
            {
                MainWindow ??= new MainWindow( mappings );
                MainWindow.Show();
                MainWindow.Activate();
            } );

            ExitCommand = new RelayCommand( () => Shutdown() );

            taskbarIcon = (TaskbarIcon)FindResource( "taskbarIcon" );
            taskbarIcon.DataContext = this;

            if( File.Exists( ConfigPath ) )
            {
                mappings = JsonSerializer.Deserialize<ObservableCollection<Mapping>>( File.ReadAllText( ConfigPath ),
                    new JsonSerializerOptions { Converters = { new HotKeyActionJsonConverter() } } );

                bool success = true;

                foreach( var m in mappings.Where( m => m.IsEnabled ) )
                {
                    try { m.SetEnabled( true ); }
                    catch( Exception ) { success = false; }
                }

                if( !success )
                    taskbarIcon.ShowBalloonTip( "On Key, Do Thing", "Some hotkeys could not be activated", BalloonIcon.None );
            }
            else
                mappings = new ObservableCollection<Mapping>();

            if( isDebugMode ) ShowWindowCommand.Execute( null );
        }
    }

    public class SingleInstanceHelper
    {
        public class AnotherInstanceOpenException : Exception { }

        private EventWaitHandle eventWaitHandle;

        public SingleInstanceHelper( string appGuid, Action showWindowAction, Action shutdownAction )
        {
            try
            {
                eventWaitHandle = EventWaitHandle.OpenExisting( appGuid );
                eventWaitHandle.Set();

                shutdownAction();
                throw new AnotherInstanceOpenException();
            }
            catch( WaitHandleCannotBeOpenedException )
            {
                // No instance found, create a new one
                eventWaitHandle = new EventWaitHandle( false, EventResetMode.AutoReset, appGuid );
            }

            // Watch for the handle to be signaled (by another instance starting up and create/open main window
            new Task( () => {
                while( eventWaitHandle.WaitOne() )
                    Application.Current.Dispatcher.BeginInvoke( showWindowAction );
            } ).Start();
        }

        public void Dispose() => eventWaitHandle.Close();
    }
}
