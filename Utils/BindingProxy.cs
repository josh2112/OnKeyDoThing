using System.Windows;

namespace Com.Josh2112.OnKeyDoThing
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register( nameof( DataContext ), typeof( object ),
            typeof( BindingProxy ), new UIPropertyMetadata( null ) );

        public object DataContext
        {
            get { return GetValue( DataContextProperty ); }
            set { SetValue( DataContextProperty, value ); }
        }

        protected override Freezable CreateInstanceCore() { return new BindingProxy(); }
    }
}
