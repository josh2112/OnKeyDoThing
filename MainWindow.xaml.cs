using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Com.Josh2112.OnKeyDoThing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Mapping> mappings;

        public ListCollectionView MappingCollection { get; }

        public RelayCommand AddMappingCommand { get; }
        public RelayCommand<Mapping> DeleteMappingCommand { get; }

        public RelayCommand<IHotKeyAction> RemoveActionCommand { get; }
        public RelayCommand<IHotKeyAction> TestActionCommand { get; }

        public MainWindow( ObservableCollection<Mapping> mappings )
        {
            AddMappingCommand = new RelayCommand( async () =>
            {
                var m = new Mapping();
                await ChangeKeyComboAsync( m );
                if( m.IsEnabled ) mappings.Add( m );
            } );

            DeleteMappingCommand = new RelayCommand<Mapping>( hk =>
            {
                hk.SetEnabled( false );
                this.mappings.Remove( hk );
            } );

            RemoveActionCommand = new RelayCommand<IHotKeyAction>(
                hka => MappingContainingAction( hka ).Actions.Remove( hka ) );

            TestActionCommand = new RelayCommand<IHotKeyAction>(
                hka => App.Logger.Info( hka.Invoke() ) );

            this.mappings = mappings;

            MappingCollection = CollectionViewSource.GetDefaultView( mappings ) as ListCollectionView;

            mappings.CollectionChanged += Mappings_CollectionChanged;

            foreach( var hk in mappings ) SubscribeToChanges( hk );

            InitializeComponent();
        }

        private void Mappings_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
        {
            Save();

            if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
            {
                foreach( var m in e.NewItems.Cast<Mapping>() ) SubscribeToChanges( m );
            }
            else if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
            {
                foreach( var m in e.OldItems.Cast<Mapping>() ) UnsubscribeFromChanges( m );
            }
        }

        private void SubscribeToChanges( Mapping mapping )
        {
            mapping.PropertyChanged += ( s, e2 ) => Save();
            mapping.Actions.CollectionChanged += ( s, e2 ) => Save();
        }

        private void UnsubscribeFromChanges( Mapping mapping )
        {
            mapping.PropertyChanged -= ( s, e2 ) => Save();
            mapping.Actions.CollectionChanged -= ( s, e2 ) => Save();
        }

        private void HotKeyAction_PropertyChanged( object sender, DataTransferEventArgs e ) => Save();

        private Mapping MappingContainingAction( IHotKeyAction hka ) => mappings.FirstOrDefault( m => m.Actions.Contains( hka ) );

        private void AddActionButton_Click( object sender, RoutedEventArgs e )
        {
            MappingCollection.MoveCurrentTo( (sender as FrameworkElement).DataContext as Mapping );

            var menu = (sender as FrameworkElement).ContextMenu;
            menu.Placement = PlacementMode.Bottom;
            menu.PlacementTarget = sender as FrameworkElement;
            menu.IsOpen = true;
        }

        private void Save() => File.WriteAllText( (Application.Current as App).ConfigPath, JsonSerializer.Serialize( mappings,
            new JsonSerializerOptions { WriteIndented = true, Converters = { new HotKeyActionJsonConverter() } } ) );

        private void AddActionComboBox_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var comboBox = sender as ComboBox;
            if( comboBox.SelectedItem is Tuple<string, Type> action )
                (comboBox.DataContext as Mapping).Actions.Add( Activator.CreateInstance( action.Item2 ) as IHotKeyAction );

            (sender as ComboBox).SelectedItem = null;
        }

        private async void KeyComboHyperlink_Click( object sender, RoutedEventArgs e ) =>
            await ChangeKeyComboAsync( (sender as DependencyObject).GetDataContext<Mapping>() );

        private async Task ChangeKeyComboAsync( Mapping mapping )
        {
            var newKeyCombo = await dialogHost.ShowDialogForResultAsync( new KeyComboDialog(
                mapping.KeyCombo, mappings.Where( m => m != mapping ).Select( m => m.KeyCombo ).ToList() ) );

            if( newKeyCombo != null )
            {
                mapping.KeyCombo = newKeyCombo;
                await SetMappingEnabledAsync( mapping, true );
            }
        }

        private async void IsEnabledButton_CheckChanged( object sender, RoutedEventArgs e )
        {
            var btn = sender as ToggleButton;
            await SetMappingEnabledAsync( btn.DataContext as Mapping, true == btn.IsChecked );
        }

        private async void UseKeyboardHookCheckbox_SourceChanged( object sender, DataTransferEventArgs e )
        {
            var btn = sender as ToggleButton;
            var mapping = btn.DataContext as Mapping;
            await SetMappingEnabledAsync( mapping, mapping.IsEnabled );
        }

        private async Task SetMappingEnabledAsync( Mapping mapping, bool enabled )
        {
            try
            {
                 mapping.SetEnabled( enabled );
            }
            catch( HotKeyRegistrationException ex )
            {
                App.Logger.Error( $"Couldn't {(ex.WasRegistration ? "" : "un")}register hotkey: {ex.Message}" );

                await dialogHost.ShowDialogForResultAsync( new ConfirmationDialog(
                    $"Couldn't {(ex.WasRegistration ? "" : "un")}register hotkey", ex.Message ) );
            }
        }
    }
}
