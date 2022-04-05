using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Com.Josh2112.OnKeyDoThing
{
    [Description( "Activate window" )]
    public class ActivateWindowHotKeyAction : ObservableObject, IHotKeyAction
    {
        private string _windowName;

        [DllImport( "USER32.DLL" )]
        public static extern bool SetForegroundWindow( IntPtr hWnd );

        public string WindowName
        {
            get { return _windowName; }
            set { SetProperty( ref _windowName, value ); }
        }

        public string Invoke()
        {
            if( string.IsNullOrWhiteSpace( WindowName ) ) return "No window title to search for!";

            var process = Process.GetProcesses().FirstOrDefault( proc => proc.MainWindowTitle.Contains( WindowName ) );
            if( process != null )
            {
                SetForegroundWindow( process.MainWindowHandle );
                return $"Activated window with title '{process.MainWindowTitle}'";
            }
            else return $"Found no windows with title containing '{WindowName}'";
        }
    }
}
