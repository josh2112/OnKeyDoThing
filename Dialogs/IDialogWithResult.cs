using Nito.AsyncEx;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Josh2112.OnKeyDoThing
{
    public class DialogResult<T>
    {
        private readonly TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

        public void Set( T result ) => tcs.TrySetResult( result );

        public Task<T> WaitAsync( CancellationToken? ct = null ) =>
            ct.HasValue ? tcs.Task.WaitAsync( ct.Value ) : tcs.Task;
    }

    public interface IDialogWithResult { }

    public interface IDialogWithResult<T> : IDialogWithResult
    {
        DialogResult<T> Result { get; }
    }
}
