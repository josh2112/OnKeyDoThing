using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Com.Josh2112.OnKeyDoThing
{
    
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : UserControl
    {
        private static LogViewer instance;

        public class ViewModel : ObservableObject
        {
            private Queue<string> logEvents = new Queue<string>();

            public string Log { get; private set; }

            internal void AddLogEvent( string message )
            {
                logEvents.Enqueue( $"{DateTime.Now}: {message}" );
                if( logEvents.Count > 20 ) logEvents.Dequeue();

                Log = string.Join( Environment.NewLine, logEvents );
                OnPropertyChanged( nameof( Log ) );
            }
        }

        public ViewModel Model { get; } = new ViewModel();

        public LogViewer()
        {
            instance = this;

            InitializeComponent();
        }

        public static void OnLogMessageReceived( string message ) => instance?.Model?.AddLogEvent( message );   
    }
}
