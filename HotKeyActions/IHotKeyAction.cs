using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace Com.Josh2112.OnKeyDoThing
{
    public static class HotKeyActionRegistry
    {
        private static List<Tuple<string, Type>> _actionTypes;

        public static IReadOnlyList<Tuple<string, Type>> ActionTypes
        {
            get
            {
                if( _actionTypes == null )
                {
                    var intf = typeof( IHotKeyAction );
                    var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany( s => s.GetTypes() )
                        .Where( p => intf.IsAssignableFrom( p ) && !p.IsInterface );
                    _actionTypes = types.Select( t => Tuple.Create(
                        (t.GetCustomAttributes( typeof( DescriptionAttribute ), true ).FirstOrDefault() as DescriptionAttribute)?.Description ?? t.Name,
                        t ) ).ToList();
                }
                return _actionTypes;
            }
        }
    }

    public sealed class HotKeyActionJsonConverter : JsonPolymorphicConverter<IHotKeyAction>
    {
        protected override string DataPropertyName => "properties";

        protected override string DescriptorFromValue( IHotKeyAction value ) => value.GetType().Name;

        protected override IHotKeyAction ReadFromDescriptor( ref Utf8JsonReader reader, string typeDescriptor )
        {
            var type = HotKeyActionRegistry.ActionTypes.FirstOrDefault( at => at.Item2.Name == typeDescriptor )?.Item2;
            return (IHotKeyAction)JsonSerializer.Deserialize( ref reader, type );
        }
    }

    public interface IHotKeyAction
    {
        string Invoke();
    }
}
