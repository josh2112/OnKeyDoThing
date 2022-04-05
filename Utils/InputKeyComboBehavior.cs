using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Com.Josh2112.OnKeyDoThing
{
    public class InputKeyComboBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty KeyComboProperty = DependencyProperty.Register( nameof( KeyCombo ), typeof( KeyCombo ),
            typeof( InputKeyComboBehavior ), new FrameworkPropertyMetadata( null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                ( s, e ) => ((InputKeyComboBehavior)s).OnKeyComboChanged() ) );

        public KeyCombo KeyCombo
        {
            get => (KeyCombo)GetValue( KeyComboProperty );
            set => SetValue( KeyComboProperty, value );
        }

        public event EventHandler KeyInputBegan;
        public event EventHandler<Exception> InvalidKeyComboDetected;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            AssociatedObject.PreviewKeyDown += TextBox_PreviewKeyDown;

            if( KeyCombo != null ) AssociatedObject.Text = KeyCombo.DisplayString;
        }

        private void TextBox_GotKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
        {
            KeyCombo = null;
            KeyInputBegan?.Invoke( this, EventArgs.Empty );
        }

        private void OnKeyComboChanged()
        {
            if( AssociatedObject != null ) AssociatedObject.Text = KeyCombo?.DisplayString ?? "";
        }

        private Key[] modifiers = new[] { Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl,
            Key.LeftAlt, Key.RightAlt, Key.LWin, Key.RWin };

        private void TextBox_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if( !modifiers.Contains( e.Key ))
            {
                try
                {
                    KeyCombo = new KeyCombo( e.Key, e.KeyboardDevice.Modifiers );
                }
                catch( NotSupportedException ex )
                {
                    KeyCombo = null;
                    InvalidKeyComboDetected?.Invoke( this, ex );
                }
                Keyboard.ClearFocus();
            }
        }
    }
}
