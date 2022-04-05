using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Com.Josh2112.OnKeyDoThing
{
    /// <summary>
    /// Interaction logic for KeyComboDialog.xaml
    /// </summary>
    public partial class KeyComboDialog : IDialogWithResult<KeyCombo>
    {
        public class ViewModel : ObservableObject
        {
            private KeyCombo _keyCombo;
            private string _errorMessage;

            public KeyCombo KeyCombo
            {
                get => _keyCombo;
                set => SetProperty( ref _keyCombo, value );
            }

            public string ErrorMessage
            {
                get => _errorMessage;
                set => SetProperty( ref _errorMessage, value );
            }

            internal void ValidateKeyCombo( List<KeyCombo> existingKeyCombos )
            {
                if( KeyCombo != null )
                {
                    if( existingKeyCombos.Contains( KeyCombo ) )
                        ErrorMessage = "A thing with that key combo already exists";
                    else if( KeyCombo.Key == System.Windows.Input.Key.LWin || KeyCombo.Key == System.Windows.Input.Key.RWin )
                        ErrorMessage = "Windows keys aren't allowed in the key combo";
                    else ErrorMessage = null;
                }
                else ErrorMessage = null;
            }
        }

        public ViewModel Model { get; } = new ViewModel();

        public DialogResult<KeyCombo> Result { get; } = new DialogResult<KeyCombo>(); 

        public RelayCommand AcceptKeyComboEntryCommand { get; }

        public KeyComboDialog( KeyCombo keyCombo, List<KeyCombo> existingKeyCombos )
        {
            AcceptKeyComboEntryCommand = new RelayCommand( () => Result.Set( Model.KeyCombo ),
                () => Model.KeyCombo != null && Model.ErrorMessage == null );

            Model.KeyCombo = keyCombo;

            Model.PropertyChanged += ( s, e ) =>
            {
                if( e.PropertyName == nameof( Model.KeyCombo ))
                {
                    Model.ValidateKeyCombo( existingKeyCombos );
                    AcceptKeyComboEntryCommand.NotifyCanExecuteChanged();
                }
            };

            InitializeComponent();

            keyComboBehavior.KeyInputBegan += ( s, e ) => Model.ValidateKeyCombo( existingKeyCombos );

            keyComboBehavior.InvalidKeyComboDetected += (s,ex) =>
            {
                Model.ErrorMessage = "Invalid key combo";
                AcceptKeyComboEntryCommand.NotifyCanExecuteChanged();
            };
        }

        private void CancelButton_Click( object sender, RoutedEventArgs e ) => Result.Set( null );
    }
}
