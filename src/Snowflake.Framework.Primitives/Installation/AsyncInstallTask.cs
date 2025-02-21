﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Snowflake.Installation.TaskResult;

namespace Snowflake.Installation
{
    /// <summary>
    /// Represents an installation task that runs at most once, and yields a single result of type 
    /// <typeparamref name="T"/>. It can also be passed to other installation tasks as a placeholder
    /// for the final result of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the yielded result.</typeparam>
    public abstract class AsyncInstallTask<T>
    {
        private Task<T> BaseTask => CachedTask.Value;
        private Lazy<Task<T>> CachedTask { get; }

        /// <summary>
        /// Base constructor for <see cref="AsyncInstallTask{T}"/>.
        /// </summary>
        protected AsyncInstallTask() => this.CachedTask = new Lazy<Task<T>>(() => this.ExecuteOnce());

        /// <summary>
        /// Runs when the <see cref="AsyncInstallTask{T}"/> is evaluated.
        /// </summary>
        /// <returns>The result of the <see cref="AsyncInstallTask{T}"/>.</returns>
        protected abstract Task<T> ExecuteOnce();

        /// <summary>
        /// A string identifier for the task.
        /// </summary>
        protected abstract string TaskName { get; }

        /// <summary>
        /// Gets the awaiter for the <see cref="AsyncInstallTask{T}"/>.
        /// This awaiter is always configured to not continue on captured context.
        /// <see cref="AsyncInstallTask{T}"/> must be context free.
        /// </summary>
        /// <returns>The awaiter that results the <see cref="AsyncInstallTask{T}"/>.</returns>
        public ConfiguredTaskAwaitable<TaskResult<T>>.ConfiguredTaskAwaiter GetAwaiter()
        {
            return this.BaseTask
                .ContinueWith(f => f.Exception == null ?
                     Success(this.TaskName, f) : Failure(this.TaskName, f, f.Exception))
                .ConfigureAwait(false)
                .GetAwaiter();
        }
    }
}
