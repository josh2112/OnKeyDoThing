using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Com.Josh2112.OnKeyDoThing
{
    public class KeyboardHooker : IHotKeyRegistrar
    {
        private delegate IntPtr KeyboardHookHandler( int nCode, IntPtr wParam, IntPtr lParam );
        private KeyboardHookHandler hookHandler;

        public event EventHandler HotKeyActivated;

        private IntPtr hookID;

        private int keyCode;
        private bool keyState;

        public KeyboardHooker( KeyCombo keyCombo )
        {
            keyCode = keyCombo.VirtualKeyCode;

            hookHandler = HookFunc;
            using( ProcessModule module = Process.GetCurrentProcess().MainModule )
                hookID = SetWindowsHookEx( 13, hookHandler, GetModuleHandle( module.ModuleName ), 0 );
        }

        private IntPtr HookFunc( int nCode, IntPtr wParam, IntPtr lParam )
        {
            var key = Marshal.ReadInt32( lParam );

            if( nCode >= 0 && keyCode == key )
            {
                int iwParam = wParam.ToInt32();

                if( (iwParam == WM_KEYDOWN || iwParam == WM_SYSKEYDOWN) && !keyState )
                {
                    // key down
                    keyState = true;
                }
                else if( (iwParam == WM_KEYUP || iwParam == WM_SYSKEYUP) && keyState )
                {
                    keyState = false;
                    HotKeyActivated?.Invoke( this, EventArgs.Empty );
                }
            }

            return CallNextHookEx( hookID, nCode, wParam, lParam );
        }

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;


        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern IntPtr GetModuleHandle( string lpModuleName );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern IntPtr SetWindowsHookEx( int idHook, KeyboardHookHandler lpfn, IntPtr hMod, uint dwThreadId );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern IntPtr CallNextHookEx( IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool UnhookWindowsHookEx( IntPtr hhk );

        private bool disposedValue;

        protected virtual void Dispose( bool disposing )
        {
            if( !disposedValue )
            {
                UnhookWindowsHookEx( hookID );
                disposedValue = true;
            }
        }

        ~KeyboardHooker() => Dispose( disposing: false );

        public void Dispose()
        {
            Dispose( disposing: true );
            GC.SuppressFinalize( this );
        }
    }
}
