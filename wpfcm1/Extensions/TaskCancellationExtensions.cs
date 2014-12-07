using System.Threading;
using System.Threading.Tasks;

namespace wpfcm1.Extensions
{
    public static class TaskCancellationExtensions
    {
        private struct Void { }

        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> orignalTask, CancellationToken ct)
        {
            var cancelTask = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
            {
                Task any = await Task.WhenAny(orignalTask, cancelTask.Task);
                if (any == cancelTask.Task) ct.ThrowIfCancellationRequested();
            }
            return await orignalTask;
        }

        public static async Task WithCancellation(this Task task, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(default(Void)), tcs))
            {
                if (await Task.WhenAny(task, tcs.Task) == tcs.Task) ct.ThrowIfCancellationRequested();
            }
            await task;
        }
    }
}
