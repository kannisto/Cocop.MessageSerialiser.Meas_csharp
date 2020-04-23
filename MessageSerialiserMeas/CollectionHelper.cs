//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2018
// Last modified: 3/2020

using System;
using System.Linq;
using System.Collections.Generic;

// Using a "neutral" namespace. This namespace does not refer to any other namespace of
// the project, and all of the other namespaces can refer to it. This way, there are no
// bi-directional dependencies between namespaces.
namespace Cocop.MessageSerialiser.Meas.Neutral
{
    /// <summary>
    /// Helper functions to overcome the problems of the code from "xsd.exe".
    /// To keep the design in line with previous design, this class should
    /// only be called from the partial classes made for the XML proxies.
    /// </summary>
    internal static class CollectionHelper
    {
        /// <summary>
        /// Converts an array of strings to string.
        /// </summary>
        /// <param name="arr">String array.</param>
        /// <returns>String.</returns>
        /// <exception cref="ArgumentException">Thrown if any of the strings contains a whitespace.</exception>
        public static string StringCollectionToString(List<string> arr)
        {
            return CollectionToString(arr,
                (obj) =>
                {
                    var currentString = (string)obj;

                    if (StringContainsSpace(currentString))
                    {
                        throw new ArgumentException("A string in XML list must not contain a whitespace");
                    }

                    return currentString;
                });
        }
        
        /// <summary>
        /// Parse a string array from a string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>String array.</returns>
        public static List<string> StringToStringCollection(string input)
        {
            if (input == null || input.Trim().Length < 1)
            {
                // Empty value
                return new List<string>();
            }

            input = input.Trim();

            // Null parameter will split the string by whitespace
            var retval = input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(retval);
        }

        /// <summary>
        /// Converts an array of doubles to string.
        /// </summary>
        /// <param name="arr">Doubles.</param>
        /// <returns>String.</returns>
        public static string DoubleCollectionToString(List<double> arr)
        {
            return CollectionToString(arr,
                (obj) =>
                {
                    return Helper.DoubleToString((double)obj);
                });
        }

        /// <summary>
        /// Parses doubles from a string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Doubles.</returns>
        public static List<double> StringTooDoubleCollection(string input)
        {
            // In the name, "too" is with a double "o" to avoid appearance in searches for "to-do" items
            
            // Split the string
            var stringArray = StringToStringCollection(input);

            var retval = new List<double>();

            for (int a = 0; a < stringArray.Count; ++a)
            {
                retval.Add(Helper.DoubleFromString(stringArray[a]));
            }

            return retval;
        }

        /// <summary>
        /// Converts an array of datetimes to string.
        /// </summary>
        /// <param name="arr">Array.</param>
        /// <returns>String.</returns>
        /// <exception cref="DateTimeException">Thrown if DateTime kind is invalid.</exception>
        public static string DateTimeCollectionToString(List<DateTime> arr)
        {
            return CollectionToString(arr,
                (obj) =>
                {
                    // throws DateTimeException
                    return Helper.DateTimeToString((DateTime)obj);
                });
        }

        /// <summary>
        /// Parses datetimes from a string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Datetimes.</returns>
        /// <exception cref="FormatException">Thrown if the values are invalid.</exception>
        public static List<DateTime> StringToDateTimeCollection(string input)
        {
            // Split the string
            var stringArray = StringToStringCollection(input);
            
            var retval = new List<DateTime>();

            for (int a = 0; a < stringArray.Count; ++a)
            {
                retval.Add(Helper.DateTimeFromString(stringArray[a]));
            }

            return retval;
        }

        /// <summary>
        /// Converts an array of longs to string.
        /// </summary>
        /// <param name="arr">Array.</param>
        /// <returns>String.</returns>
        public static string LongCollectionToString(List<long> arr)
        {
            return CollectionToString(arr,
                (obj) =>
                {
                    return Helper.LongToString((long)obj);
                });
        }

        /// <summary>
        /// Parses longs from a string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Longs.</returns>
        public static List<long> StringToLongCollection(string input)
        {
            // Split the string
            var stringArray = StringToStringCollection(input);

            var retval = new List<long>();

            for (int a = 0; a < stringArray.Count; ++a)
            {
                retval.Add(Helper.LongFromString(stringArray[a]));
            }

            return retval;
        }

        private static string CollectionToString(System.Collections.ICollection items, Func<object, string> convert)
        {
            // Converts a collection of items to string using a converter method
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool first = true;

            foreach (object o in items)
            {
                if (!first)
                {
                    // Separate with spaces
                    sb.Append(" ");
                }

                sb.Append(convert(o));
                first = false;
            }

            return sb.ToString();
        }

        private static bool StringContainsSpace(string str)
        {
            return str.Any(x => char.IsWhiteSpace(x));
        }
    }
}
