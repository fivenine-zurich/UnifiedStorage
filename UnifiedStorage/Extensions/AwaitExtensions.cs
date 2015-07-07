/*
 * This file is copied from PCLStorage AwaitExtensions.cs
 * Copyright by Daniel Plaisted, https://github.com/dsplaisted/PCLStorage 
 * Licensed under the Ms-PL as specified in https://github.com/dsplaisted/PCLStorage/blob/master/LICENSE
 */

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace UnifiedStorage.Extensions
{
    /// <summary>
    /// Extensions for working with the task asynchronous pattern (async/await).
    /// </summary>
    public static class AwaitExtensions
    {
        /// <summary>
        /// Causes the caller who awaits this method to
        /// switch off the Main thread. It has no effect if
        /// the caller is already off the main thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable that does the thread switching magic.</returns>
        public static TaskSchedulerAwaiter SwitchOffMainThreadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new TaskSchedulerAwaiter(
                SynchronizationContext.Current != null ? TaskScheduler.Default : null,
                cancellationToken);
        }
        
        /// <summary>
        /// An awaitable task scheduler.
        /// </summary>
        public struct TaskSchedulerAwaiter : INotifyCompletion
        {
            private readonly TaskScheduler _taskScheduler;
            private CancellationToken _cancellationToken;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskSchedulerAwaiter"/> structure.
            /// </summary>
            /// <param name="taskScheduler">The task scheduler.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            internal TaskSchedulerAwaiter(TaskScheduler taskScheduler, CancellationToken cancellationToken)
            {
                _taskScheduler = taskScheduler;
                _cancellationToken = cancellationToken;
            }

            /// <summary>
            /// Gets the awaiter.
            /// </summary>
            /// <returns></returns>
            public TaskSchedulerAwaiter GetAwaiter()
            {
                return this;
            }

            /// <summary>
            /// Gets a value indicating whether this instance is completed.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is completed; otherwise, <c>false</c>.
            /// </value>
            public bool IsCompleted
            {
                [DebuggerStepThrough] get { return _taskScheduler == null; }
            }

            /// <summary>
            /// Schedules the continuation action that's invoked when the instance completes.
            /// </summary>
            /// <param name="continuation">The action to invoke when the operation completes.</param>
            /// <exception cref="System.InvalidOperationException">IsCompleted is true, so this is unexpected.</exception>
            public void OnCompleted(Action continuation)
            {
                if (_taskScheduler == null)
                {
                    throw new InvalidOperationException("IsCompleted is true, so this is unexpected.");
                }

                Task.Factory.StartNew(
                    continuation,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    _taskScheduler);
            }

            /// <summary>
            /// Gets the result.
            /// </summary>
            public void GetResult()
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
