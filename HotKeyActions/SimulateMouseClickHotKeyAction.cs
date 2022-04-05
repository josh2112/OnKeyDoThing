using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace Com.Josh2112.OnKeyDoThing
{
    [Description( "Simulate mouse click" )]
    public class SimulateMouseClickHotKeyAction : ObservableObject, IHotKeyAction
    {
        private string _windowName;

        public string WindowName
        {
            get { return _windowName; }
            set { SetProperty( ref _windowName, value ); }
        }

        public string Invoke()
        {
            if( string.IsNullOrWhiteSpace( WindowName ) ) return "No window title to search for!";

            var pos = WindowInterface.GetMousePosition();
            var window = WindowInterface.WindowAtPosition( pos );

            if( true == window?.Caption?.Contains( WindowName ) )
            {
                window.MouseLeftButton( pos, MouseButtonState.Pressed );
                window.MouseLeftButton( pos, MouseButtonState.Released );
                return $"Found window '{window.Caption}' under mouse, simulated mouse left click";
            }
            else return $"Mouse wasn't over a window matching '{WindowName}'";
        }
    }

    public class WindowInterface
    {
        public IntPtr Hwnd;
        public string Caption;

        public static Point GetMousePosition()
        {
            GetCursorPos( out Point point );
            return point;
        }

        public static WindowInterface WindowAtPosition( Point point )
        {
            IntPtr hwnd = WindowFromPoint( point );

            var intLength = GetWindowTextLength( hwnd ) + 1;
            var sb = new StringBuilder( intLength );
            return GetWindowText( hwnd, sb, intLength ) > 0 ?
                new WindowInterface { Hwnd = hwnd, Caption = sb.ToString() } : null;
        }

        public void MouseLeftButton( Point screenPoint, MouseButtonState state )
        {
            GetWindowRect( Hwnd, out RECT windowRect );

            var clientPoint = new Point( screenPoint.X - windowRect.Left, screenPoint.Y - windowRect.Top );
            var lParam = (clientPoint.Y << 16) | clientPoint.X;

            Console.WriteLine( $"Sending left mouse {state} to window {Caption} at {clientPoint}" );
            SendMessage( Hwnd, state == MouseButtonState.Pressed ? WM_LBUTTONDOWN : WM_LBUTTONUP, 0x00000001, lParam );
        }

        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;

        [StructLayout( LayoutKind.Sequential )]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool GetCursorPos( out Point point );

        [DllImport( "user32.dll" )]
        private static extern IntPtr WindowFromPoint( Point point );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern int GetWindowTextLength( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        private static extern int GetWindowText( IntPtr hWnd, StringBuilder text, int count );

        [DllImport( "user32.dll" )]
        private static extern int SendMessage( IntPtr hWnd, int msg, int wParam, int lParam );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool GetWindowRect( IntPtr hwnd, out RECT lpRect );

        [DllImport( "dwmapi.dll" )]
        static unsafe extern int DwmGetWindowAttribute( IntPtr hWnd, int dwAttribute, void* lpRect, int cbAttribute );
    }
}
