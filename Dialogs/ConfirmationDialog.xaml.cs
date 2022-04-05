using System.Windows;

namespace Com.Josh2112.OnKeyDoThing
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : IDialogWithResult<bool>
    {
        public string Title { get; }
        public string Message { get; }
        
        public DialogResult<bool> Result { get; } = new DialogResult<bool>(); 

        public ConfirmationDialog( string title, string message )
        {
            Title = title; Message = message;

            InitializeComponent();
        }

        private void OkButton_Click( object sender, RoutedEventArgs e ) => Result.Set( true );
    }
}
