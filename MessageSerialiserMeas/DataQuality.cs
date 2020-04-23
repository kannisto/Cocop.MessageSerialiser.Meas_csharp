//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2018
// Last modified: 2/2020

using System;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents observation quality information.
    /// </summary>
    public sealed class DataQuality
    {
        /// <summary>
        /// The value of good data quality.
        /// </summary>
        public const string Good = "good";

        /// <summary>
        /// The value of bad data quality.
        /// </summary>
        public const string Bad = "bad";


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="input">A string that refers to data quality.</param>
        /// <exception cref="ArgumentException">Thrown if the value cannot be recognised as a data quality string, or if the string contains whitespace characters.</exception>
        internal DataQuality(string input)
        {
            if (!input.StartsWith(Good) && !input.StartsWith(Bad))
            {
                throw new ArgumentException("Cannot interpret data quality value \"" + input + "\"", "input");
            }

            if (XNeut.Helper.StringContainsSpace(input))
            {
                throw new ArgumentException("Data quality string must not contain whitespaces");
            }

            Value = input;
        }

        /// <summary>
        /// Creates a data quality object with the value "good".
        /// </summary>
        /// <returns>New instance.</returns>
        public static DataQuality CreateGood()
        {
            return new DataQuality(Good);
        }

        /// <summary>
        /// Creates a data quality object with the value "bad".
        /// </summary>
        /// <returns>New instance.</returns>
        public static DataQuality CreateBad()
        {
            return new DataQuality(Bad);
        }

        /// <summary>
        /// Creates a data quality object with the value "bad" and additional information why the value is bad.
        /// </summary>
        /// <param name="reason">Reason why the quality is bad. This is appended at the end of the quality string.</param>
        /// <returns>New instance.</returns>
        /// <exception cref="ArgumentException">Thrown if the string contains whitespace characters.</exception>
        public static DataQuality CreateBad(string reason)
        {
            return new DataQuality(Bad + "/" + reason);
        }
        
        /// <summary>
        /// Whether data quality is good.
        /// </summary>
        /// <returns>True if good, otherwise false.</returns>
        public bool IsGood
        {
            get
            {
                return Value.StartsWith(Good);
            }
        }

        /// <summary>
        /// The string that encloses data quality information.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Quality information as string.
        /// </summary>
        /// <returns>Quality information as string.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
