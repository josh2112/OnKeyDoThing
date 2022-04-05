using System;

namespace Com.Josh2112.OnKeyDoThing
{
    public class HotKeyRegistrationException : Exception
    {
        public bool WasRegistration { get; }

        public HotKeyRegistrationException( bool wasRegistration, string msg ) : base( msg )
        {
            WasRegistration = wasRegistration;
        }
    }

    public interface IHotKeyRegistrar : IDisposable
    {
        event EventHandler HotKeyActivated;
    }
}
