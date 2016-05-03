using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbraco.Core
{
    /// <summary>
    /// Represents the result of an operation attempt.
    /// </summary>
    /// <typeparam name="T">The type of the attempted operation result.</typeparam>
#if DNX46
    [Serializable]
#endif
    public struct Attempt<T>
    {
        private readonly bool _success;
        private readonly T _result;
        private readonly Exception _exception;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Attempt{T}"/> was successful.
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        /// Gets the exception associated with an unsuccessful attempt.
        /// </summary>
        public Exception Exception { get { return _exception; } }

        /// <summary>
        /// Gets the exception associated with an unsuccessful attempt.
        /// </summary>
        /// <remarks>Keep it for backward compatibility sake.</remarks>
        [Obsolete(".Error is obsolete, you should use .Exception instead.", false)]
        public Exception Error { get { return _exception; } }

        /// <summary>
        /// Gets the attempt result.
        /// </summary>
        public T Result
        {
            get { return _result; }
        }

        // optimize, use a singleton failed attempt
        private static readonly Attempt<T> Failed = new Attempt<T>(false, default(T), null);

        /// <summary>
        /// Represents an unsuccessful attempt.
        /// </summary>
        /// <remarks>Keep it for backward compatibility sake.</remarks>
        [Obsolete(".Failed is obsolete, you should use Attempt<T>.Fail() instead.", false)]
        public static readonly Attempt<T> False = Failed;

        // private - use Succeed() or Fail() methods to create attempts
        private Attempt(bool success, T result, Exception exception)
        {
            _success = success;
            _result = result;
            _exception = exception;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Attempt{T}"/> struct with a result.
        /// </summary>
        /// <param name="success">A value indicating whether the attempt is successful.</param>
        /// <param name="result">The result of the attempt.</param>
        /// <remarks>Keep it for backward compatibility sake.</remarks>
        [Obsolete("Attempt ctors are obsolete, you should use Attempt<T>.Succeed(), Attempt<T>.Fail() or Attempt<T>.If() instead.", false)]
        public Attempt(bool success, T result)
            : this(success, result, null)
        { }

        /// <summary>
        /// Initialize a new instance of the <see cref="Attempt{T}"/> struct representing a failed attempt, with an exception.
        /// </summary>
        /// <param name="exception">The exception causing the failure of the attempt.</param>
        /// <remarks>Keep it for backward compatibility sake.</remarks>
        [Obsolete("Attempt ctors are obsolete, you should use Attempt<T>.Succeed(), Attempt<T>.Fail() or Attempt<T>.If() instead.", false)]
        public Attempt(Exception exception)
            : this(false, default(T), exception)
        { }

        /// <summary>
        /// Creates a successful attempt.
        /// </summary>
        /// <returns>The successful attempt.</returns>
        public static Attempt<T> Succeed()
        {
            return new Attempt<T>(true, default(T), null);
        }

        /// <summary>
        /// Creates a successful attempt with a result.
        /// </summary>
        /// <param name="result">The result of the attempt.</param>
        /// <returns>The successful attempt.</returns>
        public static Attempt<T> Succeed(T result)
        {
            return new Attempt<T>(true, result, null);
        }

        /// <summary>
        /// Creates a failed attempt.
        /// </summary>
        /// <returns>The failed attempt.</returns>
        public static Attempt<T> Fail()
        {
            return Failed;
        }

        /// <summary>
        /// Creates a failed attempt with an exception.
        /// </summary>
        /// <param name="exception">The exception causing the failure of the attempt.</param>
        /// <returns>The failed attempt.</returns>
        public static Attempt<T> Fail(Exception exception)
        {
            return new Attempt<T>(false, default(T), exception);
        }

        /// <summary>
        /// Creates a failed attempt with a result.
        /// </summary>
        /// <param name="result">The result of the attempt.</param>
        /// <returns>The failed attempt.</returns>
        public static Attempt<T> Fail(T result)
        {
            return new Attempt<T>(false, result, null);
        }

        /// <summary>
        /// Creates a failed attempt with a result and an exception.
        /// </summary>
        /// <param name="result">The result of the attempt.</param>
        /// <param name="exception">The exception causing the failure of the attempt.</param>
        /// <returns>The failed attempt.</returns>
        public static Attempt<T> Fail(T result, Exception exception)
        {
            return new Attempt<T>(false, result, exception);
        }

        /// <summary>
        /// Creates a successful or a failed attempt.
        /// </summary>
        /// <param name="condition">A value indicating whether the attempt is successful.</param>
        /// <returns>The attempt.</returns>
        public static Attempt<T> SucceedIf(bool condition)
        {
            return condition ? new Attempt<T>(true, default(T), null) : Failed;
        }

        /// <summary>
        /// Creates a successful or a failed attempt, with a result.
        /// </summary>
        /// <param name="condition">A value indicating whether the attempt is successful.</param>
        /// <param name="result">The result of the attempt.</param>
        /// <returns>The attempt.</returns>
        public static Attempt<T> SucceedIf(bool condition, T result)
        {
            return new Attempt<T>(condition, result, null);
        }

        /// <summary>
        /// Implicity operator to check if the attempt was successful without having to access the 'success' property
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static implicit operator bool(Attempt<T> a)
        {
            return a.Success;
        }
    }

    /// <summary>
    /// Provides ways to create attempts.
    /// </summary>
    public static class Attempt
    {
        /// <summary>
        /// Creates a successful attempt with a result.
        /// </summary>
        /// <typeparam name="T">The type of the attempted operation result.</typeparam>
        /// <param name="result">The result of the attempt.</param>
        /// <returns>The successful attempt.</returns>
        public static Attempt<T> Succeed<T>(T result)
        {
            return Attempt<T>.Succeed(result);
        }

        /// <summary>
        /// Creates a failed attempt with a result.
        /// </summary>
        /// <typeparam name="T">The type of the attempted operation result.</typeparam>
        /// <param name="result">The result of the attempt.</param>
        /// <returns>The failed attempt.</returns>
        public static Attempt<T> Fail<T>(T result)
        {
            return Attempt<T>.Fail(result);
        }

        /// <summary>
        /// Creates a failed attempt with a result and an exception.
        /// </summary>
        /// <typeparam name="T">The type of the attempted operation result.</typeparam>
        /// <param name="result">The result of the attempt.</param>
        /// <param name="exception">The exception causing the failure of the attempt.</param>
        /// <returns>The failed attempt.</returns>
        public static Attempt<T> Fail<T>(T result, Exception exception)
        {
            return Attempt<T>.Fail(result, exception);
        }

        /// <summary>
        /// Creates a successful or a failed attempt, with a result.
        /// </summary>
        /// <typeparam name="T">The type of the attempted operation result.</typeparam>
        /// <param name="success">A value indicating whether the attempt is successful.</param>
        /// <param name="result">The result of the attempt.</param>
        /// <returns>The attempt.</returns>
        public static Attempt<T> If<T>(bool success, T result)
        {
            return Attempt<T>.SucceedIf(success, result);
        }


        /// <summary>
        /// Executes an attempt function, with callbacks.
        /// </summary>
        /// <typeparam name="T">The type of the attempted operation result.</typeparam>
        /// <param name="attempt">The attempt returned by the attempt function.</param>
        /// <param name="onSuccess">An action to execute in case the attempt succeeds.</param>
        /// <param name="onFail">An action to execute in case the attempt fails.</param>
        /// <returns>The outcome of the attempt.</returns>
        /// <remarks>Runs <paramref name="onSuccess"/> or <paramref name="onFail"/> depending on the
        /// whether the attempt function reports a success or a failure.</remarks>
        public static Outcome Try<T>(Attempt<T> attempt, Action<T> onSuccess, Action<Exception> onFail = null)
        {
            if (attempt.Success)
            {
                onSuccess(attempt.Result);
                return Outcome.Success;
            }

            if (onFail != null)
            {
                onFail(attempt.Exception);
            }

            return Outcome.Failure;
        }

        /// <summary>
        /// Represents the outcome of an attempt.
        /// </summary>
        /// <remarks>Can be a success or a failure, and allows for attempts chaining.</remarks>
        public struct Outcome
        {
            private readonly bool _success;

            /// <summary>
            /// Gets an outcome representing a success.
            /// </summary>
            public static readonly Outcome Success = new Outcome(true);

            /// <summary>
            /// Gets an outcome representing a failure.
            /// </summary>
            public static readonly Outcome Failure = new Outcome(false);

            private Outcome(bool success)
            {
                _success = success;
            }

            /// <summary>
            /// Executes another attempt function, if the previous one failed, with callbacks.
            /// </summary>
            /// <typeparam name="T">The type of the attempted operation result.</typeparam>
            /// <param name="nextFunction">The attempt function to execute, returning an attempt.</param>
            /// <param name="onSuccess">An action to execute in case the attempt succeeds.</param>
            /// <param name="onFail">An action to execute in case the attempt fails.</param>
            /// <returns>If it executes, returns the outcome of the attempt, else returns a success outcome.</returns>
            /// <remarks>
            /// <para>Executes only if the previous attempt failed, else does not execute and return a success outcome.</para>
            /// <para>If it executes, then runs <paramref name="onSuccess"/> or <paramref name="onFail"/> depending on the
            /// whether the attempt function reports a success or a failure.</para>
            /// </remarks>
            public Outcome OnFailure<T>(Func<Attempt<T>> nextFunction, Action<T> onSuccess, Action<Exception> onFail = null)
            {
                return _success
                    ? Success
                    : ExecuteNextFunction(nextFunction, onSuccess, onFail);
            }

            /// <summary>
            /// Executes another attempt function, if the previous one succeeded, with callbacks.
            /// </summary>
            /// <typeparam name="T">The type of the attempted operation result.</typeparam>
            /// <param name="nextFunction">The attempt function to execute, returning an attempt.</param>
            /// <param name="onSuccess">An action to execute in case the attempt succeeds.</param>
            /// <param name="onFail">An action to execute in case the attempt fails.</param>
            /// <returns>If it executes, returns the outcome of the attempt, else returns a failed outcome.</returns>
            /// <remarks>
            /// <para>Executes only if the previous attempt succeeded, else does not execute and return a success outcome.</para>
            /// <para>If it executes, then runs <paramref name="onSuccess"/> or <paramref name="onFail"/> depending on the
            /// whether the attempt function reports a success or a failure.</para>
            /// </remarks>
            public Outcome OnSuccess<T>(Func<Attempt<T>> nextFunction, Action<T> onSuccess, Action<Exception> onFail = null)
            {
                return _success
                    ? ExecuteNextFunction(nextFunction, onSuccess, onFail)
                    : Failure;
            }

            private static Outcome ExecuteNextFunction<T>(Func<Attempt<T>> nextFunction, Action<T> onSuccess, Action<Exception> onFail = null)
            {
                var attempt = nextFunction();

                if (attempt.Success)
                {
                    onSuccess(attempt.Result);
                    return Success;
                }

                if (onFail != null)
                {
                    onFail(attempt.Exception);
                }

                return Failure;
            }
        }

    }
}
