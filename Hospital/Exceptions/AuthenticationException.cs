﻿using System;

namespace Hospital.Exceptions
{
    /// <summary>
    /// Represents errors that occur during authentication operations.
    /// </summary>
    public class AuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthenticationException(string message)
            : base(message)
        {
        }
    }
}
