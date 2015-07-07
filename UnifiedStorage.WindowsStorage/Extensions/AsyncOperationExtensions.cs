/*
 * This file is copied from PCLStorage AwaitExtensions.cs
 * Copyright by Daniel Plaisted, https://github.com/dsplaisted/PCLStorage 
 * Licensed under the Ms-PL as specified in https://github.com/dsplaisted/PCLStorage/blob/master/LICENSE
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

// ReSharper disable CheckNamespace

namespace UnifiedStorage.WindowsStorage.Extensions
{
    internal static class AsyncOperationExtensions
    {
        /// <summary>
        /// Returns a task that completes when the async operation completes,
        /// but never throws to the person awaiting it.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the async operation.</typeparam>
        /// <param name="operation">The async operation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task whose result is the completed task.</returns>
        internal static Task<Task<T>> AsTaskNoThrow<T>(this IAsyncOperation<T> operation,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Task<T>>();
            var task = operation.AsTask(cancellationToken);
            task.ContinueWith((t, state) => ((TaskCompletionSource<Task<T>>) state).SetResult(t), tcs, cancellationToken);
            return tcs.Task;
        }
    }
}
