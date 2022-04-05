using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace Com.Josh2112.OnKeyDoThing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static NLog.Logger Logger { get; } = NLog.LogManager.GetCurrentClassLogger();

        private readonly bool isDebugMode;

        private TaskbarIcon taskbarIcon;

        private ObservableCollection<Mapping> mappings;

        public string ConfigPath { get; }

        public RelayCommand ShowWindowCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public App()
        {
            System.Diagnostics.Debug.Assert( isDebugMode = true );

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
                MainWindow = MainWindow ?? new MainWindow( mappings );
                MainWindow.Show();
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
}
