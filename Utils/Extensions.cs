using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System.Windows;

namespace Com.Josh2112.OnKeyDoThing
{
    public static class Extensions
    {
        public static async Task<T> ShowDialogForResultAsync<T>( this DialogHost dialogHost, IDialogWithResult<T> dialog )
        {
            dialogHost.DialogContent = dialog;
            dialogHost.IsOpen = true;

            var result = await dialog.Result.WaitAsync();

            dialogHost.IsOpen = false;

            return result;
        }

        public static T GetDataContext<T>( this DependencyObject obj ) where T : class
        {
            if( obj is FrameworkElement fe ) return fe.DataContext as T;
            else if( obj is FrameworkContentElement fce ) return fce.DataContext as T;
            else return null;
        }
    }
}
