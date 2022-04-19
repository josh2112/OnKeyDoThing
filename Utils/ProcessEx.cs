using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Com.Josh2112.OnKeyDoThing.Utils
{
    public static class ProcessEx
    {
        public static Process GetProcessFromPoint( Point point )
        {
            GetWindowThreadProcessId( WindowFromPoint( point ), out uint procId );
            return Process.GetProcessById( (int)procId );
        }

        public static void MouseLeftButton( this Process proc, Point screenPoint, MouseButtonState state )
        {
            DwmGetWindowAttribute( proc.MainWindowHandle, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf( typeof( RECT ) ) );

            if( rect.Right - rect.Left <= 0 )
                GetWindowRect( proc.MainWindowHandle, out rect );

            var clientPoint = new Point( screenPoint.X - rect.Left, screenPoint.Y - rect.Top );
            var lParam = (clientPoint.Y << 16) | clientPoint.X;

            Console.WriteLine( $"Sending left mouse {state} to window {proc.MainWindowTitle} at {clientPoint}" );
            SendMessage( proc.MainWindowHandle, state == MouseButtonState.Pressed ? WM_LBUTTONDOWN : WM_LBUTTONUP, 0x00000001, lParam );
        }

        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 0x9;

        [StructLayout( LayoutKind.Sequential )]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport( "user32.dll" )]
        private static extern IntPtr WindowFromPoint( Point point );

        [DllImport( "user32.dll", SetLastError = true )]
        static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

        [DllImport( "user32.dll" )]
        private static extern int SendMessage( IntPtr hWnd, int msg, int wParam, int lParam );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool GetWindowRect( IntPtr hwnd, out RECT lpRect );

        [DllImport( "dwmapi.dll" )]
        static unsafe extern int DwmGetWindowAttribute( IntPtr hWnd, int dwAttribute, out RECT pvAttribute, int cbAttribute );
    }
}
