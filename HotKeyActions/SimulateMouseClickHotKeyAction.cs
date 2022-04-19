using Com.Josh2112.OnKeyDoThing.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
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

            var pos = GetCursorPos( out Point point ) ? point : Point.Empty;
            var proc = ProcessEx.GetProcessFromPoint( pos );

            if( true == proc?.MainWindowTitle?.Contains( WindowName ) )
            {
                proc.MouseLeftButton( pos, MouseButtonState.Pressed );
                proc.MouseLeftButton( pos, MouseButtonState.Released );
                return $"Found window '{proc.MainWindowTitle}' under mouse, simulated mouse left click";
            }
            else return $"Mouse wasn't over a window matching '{WindowName}'";
        }

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool GetCursorPos( out Point point );
    }
}
