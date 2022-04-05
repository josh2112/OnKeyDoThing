using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace Com.Josh2112.OnKeyDoThing
{
    public class KeyCombo : KeyGesture
    {
        [JsonIgnore]
        public int VirtualKeyCode => KeyInterop.VirtualKeyFromKey( Key );

        [JsonIgnore]
        public int ID => VirtualKeyCode + ((int)Modifiers * 0x10000);

        [JsonIgnore]
        public new string DisplayString { get; }

        public KeyCombo( Key key, ModifierKeys modifiers ) : base( key, modifiers ) =>
            DisplayString = GetDisplayStringForCulture( CultureInfo.CurrentCulture );

        public override bool Equals( object obj ) => obj is KeyCombo combo && ID == combo.ID;

        public override int GetHashCode() => HashCode.Combine( ID );
    }

    public class Mapping : ObservableObject
    {
        private bool _isEnabled, _useKeyboardHook;
        private KeyCombo _keyCombo;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty( ref _isEnabled, value );
        }

        public KeyCombo KeyCombo
        {
            get => _keyCombo;
            set => SetProperty( ref _keyCombo, value );
        }

        public bool UseKeyboardHook
        {
            get => _useKeyboardHook;
            set => SetProperty( ref _useKeyboardHook, value );
        }

        private IHotKeyRegistrar registrar;

        public ObservableCollection<IHotKeyAction> Actions { get; set; } = new ObservableCollection<IHotKeyAction>();

        public void SetEnabled( bool enabled )
        {
            if( registrar != null )
            {
                registrar.HotKeyActivated -= HotKeyActivated;
                registrar.Dispose();
                registrar = null;
            }

            if( enabled )
            {
                registrar = UseKeyboardHook ? new KeyboardHooker( KeyCombo ) : new HotKeyRegistrar( KeyCombo );
                registrar.HotKeyActivated += HotKeyActivated;
            }

            IsEnabled = enabled;
        }

        public void HotKeyActivated( object sender, EventArgs args )
        {
            App.Logger.Info( $"Detected hotkey {KeyCombo.DisplayString}" );

            foreach( var action in Actions )
            {
                try
                {
                    App.Logger.Info( $"Invoked action {action.GetType().Name}: {action.Invoke()}" );
                }
                catch( Exception ex )
                {
                    App.Logger.Error( $"Failed to invoke action {action.GetType().Name}: {ex.Message}" );
                }
            }
        }
    }
}
