using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Com.Josh2112.OnKeyDoThing
{
    public class HotKeyRegistrar : IHotKeyRegistrar
    {
        private KeyCombo keyCombo;

        public event EventHandler HotKeyActivated;

        public HotKeyRegistrar( KeyCombo keyCombo )
        {
            this.keyCombo = keyCombo;

            if( RegisterHotKey( IntPtr.Zero, keyCombo.ID, (int)keyCombo.Modifiers, keyCombo.VirtualKeyCode ))
                ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
            else
                throw new HotKeyRegistrationException( true, new Win32Exception().Message );
        }

        private const int WM_HOTKEY = 0x0312;

        private void ComponentDispatcher_ThreadFilterMessage( ref MSG msg, ref bool handled )
        {
            if( !handled )
            {
                if( msg.message == WM_HOTKEY )
                {
                    var key = KeyInterop.KeyFromVirtualKey( ((int)msg.lParam >> 16) & 0xFFFF );
                    var modifiers = (ModifierKeys)((int)msg.lParam & 0xFFFF);
                    var keyCombo = new KeyCombo( key, modifiers );

                    HotKeyActivated?.Invoke( this, EventArgs.Empty );

                    //mappings.FirstOrDefault( hk => hk.KeyCombo.Equals( keyCombo ) )?.OnHotKeyActivated();
                }
            }
        }

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool RegisterHotKey( IntPtr hWnd, int id, int fsModifiers, int vk );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool UnregisterHotKey( IntPtr hWnd, int id );

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose( bool disposing )
        {
            if( !disposedValue )
            {
                ComponentDispatcher.ThreadFilterMessage -= ComponentDispatcher_ThreadFilterMessage;
                UnregisterHotKey( IntPtr.Zero, keyCombo.ID );

                disposedValue = true;
            }
        }

        ~HotKeyRegistrar() => Dispose(disposing: false);

        public void Dispose()
        {
            Dispose( disposing: true );
            GC.SuppressFinalize( this );
        }

        #endregion IDisposable
    }
}
