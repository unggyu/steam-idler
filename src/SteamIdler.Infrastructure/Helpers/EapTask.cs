using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Helpers
{
    public static class TaskExt
    {
        public static EapTask<TEventArgs, EventHandler<TEventArgs>> FromEvent<TEventArgs>()
        {
            var tcs = new TaskCompletionSource<TEventArgs>();
            var handler = new EventHandler<TEventArgs>((s, e) =>
            {
                Debug.WriteLine($"[TaskExt.cs] TrySetResult: {e}");
                tcs.TrySetResult(e);
            });
            return new EapTask<TEventArgs, EventHandler<TEventArgs>>(tcs, handler);
        }
    }


    public sealed class EapTask<TEventArgs, TEventHandler>
        where TEventHandler : class
    {
        private readonly TaskCompletionSource<TEventArgs> _completionSource;
        private readonly TEventHandler _eventHandler;

        public EapTask(
            TaskCompletionSource<TEventArgs> completionSource,
            TEventHandler eventHandler)
        {
            _completionSource = completionSource;
            _eventHandler = eventHandler;
        }

        public EapTask<TEventArgs, TOtherEventHandler> WithHandlerConversion<TOtherEventHandler>(
            Converter<TEventHandler, TOtherEventHandler> converter)
            where TOtherEventHandler : class
        {
            return new EapTask<TEventArgs, TOtherEventHandler>(
                _completionSource, converter(_eventHandler));
        }

        public async Task<TEventArgs> Start(
            Action<TEventHandler> subscribe,
            Action action,
            Action<TEventHandler> unsubscribe,
            CancellationToken cancellationToken = default)
        {
            subscribe(_eventHandler);
            try
            {
                using (cancellationToken.Register(() => _completionSource.SetCanceled()))
                {
                    action();
                    return await _completionSource.Task;
                }
            }
            finally
            {
                unsubscribe(_eventHandler);
            }
        }
    }
}
