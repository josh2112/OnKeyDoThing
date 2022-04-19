using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Com.Josh2112.OnKeyDoThing
{
    [Description( "Minimize windows" )]
    public class MinimizeAllWindowsHotKeyAction : ObservableObject, IHotKeyAction
    {
        private string _windowName;

        public string WindowName
        {
            get => _windowName;
            set => SetProperty( ref _windowName, value );
        }

        public string Invoke()
        {
            if( string.IsNullOrWhiteSpace( WindowName ) ) return "No window title to search for!";

            var result = new List<string>();
            foreach( var proc in Process.GetProcesses().Where( proc => proc.MainWindowTitle.Contains( WindowName ) ) )
            {
                ShowWindow( proc.MainWindowHandle, SW_MINIMIZE );
                result.Add( $"Minimized window with title '{proc.MainWindowTitle}'" );
            }
            if( result.Any() ) return string.Join( "\n", result );
            else return $"Found no windows with title containing '{WindowName}'";
        }

        private const int SW_MINIMIZE = 6;

        [DllImport( "USER32.DLL" )]
        static extern bool ShowWindow( IntPtr hWnd, int cmdShow );

    }
}
