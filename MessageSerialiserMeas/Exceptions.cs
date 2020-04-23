//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 2/2020

using System;

namespace Cocop.MessageSerialiser.Meas.Neutral
{
    /// <summary>
    /// Thrown when a message-related error has been encountered.
    /// </summary>
    public sealed class InvalidMessageException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Error message.</param>
        internal InvalidMessageException(string msg) : base(msg)
        {
            // Empty ctor body
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Error message.</param>
        /// <param name="inner">Inner exception.</param>
        internal InvalidMessageException(string msg, Exception inner) : base(msg, inner)
        {
            // Empty ctor body
        }
    }

    /// <summary>
    /// Indicates an invalid DateTime value.
    /// </summary>
    public sealed class DateTimeException : ArgumentException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Message.</param>
        internal DateTimeException(string msg)
            : base(msg)
        {
            // Empty ctor body
        }
    }
}
