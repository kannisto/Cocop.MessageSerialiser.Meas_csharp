//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 3/2020

using System;
using System.Linq;
using SysColl = System.Collections.Generic;
using SXml = System.Xml;

// Using a "neutral" namespace. This namespace does not refer to any other namespace of
// the project, and all of the other namespaces can refer to it. This way, there are no
// bi-directional dependencies between namespaces.
namespace Cocop.MessageSerialiser.Meas.Neutral
{
    /// <summary>
    /// Contains static members that help XML processing.
    /// </summary>
    internal static class Helper
    {
        // These URIs are based on the Observations and Measurements standard
        private const string TypeUri_Start              = "http://www.opengis.net/def/observationType/OGC-OM/2.0/";
        public const string TypeUri_Category            = TypeUri_Start + "OM_CategoryObservation";
	    public const string TypeUri_Complex             = TypeUri_Start + "OM_ComplexObservation";
	    public const string TypeUri_Count               = TypeUri_Start + "OM_CountObservation";
	    public const string TypeUri_Measurement         = TypeUri_Start + "OM_Measurement";
        public const string TypeUri_Temporal            = TypeUri_Start + "OM_TemporalObservation";
	    public const string TypeUri_Truth               = TypeUri_Start + "OM_TruthObservation";

        // These type URIs are custom-made for COCOP
        private const string TypeUri_CustomStart       = "cocop/observationType/";
        public const string TypeUri_CategoryRange      = TypeUri_CustomStart + "CategoryRange";
        public const string TypeUri_CountRange         = TypeUri_CustomStart + "CountRange";
        public const string TypeUri_MeasurementRange   = TypeUri_CustomStart + "MeasurementRange";
        public const string TypeUri_Text               = TypeUri_CustomStart + "Text";
        public const string TypeUri_TimeSeriesFlexible = TypeUri_CustomStart + "TimeSeriesFlexible";
        public const string TypeUri_TimeSeriesConstant = TypeUri_CustomStart + "TimeSeriesRegular";
        
        // These are used to cache serialisers. The framework supposedly does not always
        // cache it, at least if overriding attributes, which reduces performance. Also,
        // the serialiser object is supposedly thread-safe.
        private static SerializerCache m_serialiserCache = new SerializerCache();

        
        /// <summary>
        /// Serialises an object to XML.
        /// </summary>
        /// <param name="obj">The object to be serialised.</param>
        /// <returns>XML data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serialisation fails.</exception>
        internal static byte[] ToXmlBytes(object obj)
        {
            // Creating an XmlTextReader from the XML document
            using (var xmlStream = new System.IO.MemoryStream())
            {
                using (var xmlWriter = new SXml.XmlTextWriter(xmlStream, System.Text.Encoding.UTF8))
                {
                    // Serialising
                    var serializer = m_serialiserCache.GetSerializer(obj.GetType());
                    serializer.Serialize(xmlWriter, obj);

                    // Returning a byte array
                    return xmlStream.ToArray();
                }
            }
        }
        
        /// <summary>
        /// Deserialises an object from XML.
        /// </summary>
        /// <param name="type">Proxy class type.</param>
        /// <param name="xmlBytes">XML data.</param>
        /// <returns>Deserialised proxy object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serialisation fails.</exception>
        internal static object DeserialiseFromXml(Type type, byte[] xmlBytes)
        {
            var serializer = m_serialiserCache.GetSerializer(type);

            // Creating an XmlTextReader from the XML document
            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                using (var reader = SXml.XmlReader.Create(xmlStream))
                {
                    // Deserialising
                    return serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Checks if a string contains a whitespace character in it.
        /// </summary>
        /// <param name="str">String.</param>
        /// <returns>True if whitespaces found, otherwise false.</returns>
        internal static bool StringContainsSpace(string str)
        {
            return str.Any(x => char.IsWhiteSpace(x));
        }

        /// <summary>
        /// Resolves the local name of the root element in an XML document.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <returns>Local name of root element.</returns>
        /// <exception cref="InvalidOperationException">Thrown if XML parsing fails.</exception>
        internal static string GetRootElNameFromXmlDoc(byte[] xmlBytes)
        {
            // Parsing the document to resolving the name of the root element
            SXml.XmlDocument xmlDoc = new SXml.XmlDocument();

            using (var ms = new System.IO.MemoryStream(xmlBytes))
            {
                try
                {
                    xmlDoc.Load(ms);
                }
                catch (SXml.XmlException e)
                {
                    throw new InvalidOperationException("Failed to parse XML document", e);
                }
            }

            // Mapping the names of root elements and message classes
            return xmlDoc.DocumentElement.LocalName;
        }


        #region Encoding of datatypes

        /// <summary>
        /// Checks that a DateTime value has UTC as the kind.
        /// </summary>
        /// <param name="dt">DateTime value to be checked.</param>
        /// <exception cref="DateTimeException">Thrown if DateTime kind is not UTC.</exception>
        internal static void ExpectDateTimeIsUtc(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
                throw new DateTimeException("DateTime kind must be UTC");
            }
        }

        /// <summary>
        /// Converts a datetime value to an XML datetime string.
        /// </summary>
        /// <param name="dt">DateTime.</param>
        /// <returns>XML datetime string.</returns>
        /// <exception cref="DateTimeException">Thrown if DateTime kind is not UTC.</exception>
        internal static string DateTimeToString(DateTime dt)
        {
            ExpectDateTimeIsUtc(dt);
            return SXml.XmlConvert.ToString(dt, SXml.XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// Converts an XML datetime string to a datetime value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>DateTime.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static DateTime DateTimeFromString(string s)
        {
            return (DateTime)TryParse((datetimeStr) => // Throws ArgumentException
            {
                // First, parse to resolve if timezone information exists
                var firstParse = SXml.XmlConvert.ToDateTime(datetimeStr, SXml.XmlDateTimeSerializationMode.RoundtripKind);

                // Always parse in UTC if time zone information is there.
                // Otherwise, do not try to guess the time zone.
                var serialisationMode = firstParse.Kind != DateTimeKind.Unspecified ?
                    SXml.XmlDateTimeSerializationMode.Utc :
                    SXml.XmlDateTimeSerializationMode.Unspecified;
                return SXml.XmlConvert.ToDateTime(datetimeStr, serialisationMode);
            },
            "DateTime", s);
        }

        /// <summary>
        /// If DateTime has a local kind, this method converts it to UTC.
        /// Otherwise, the kind will remain intact.
        /// </summary>
        /// <returns>DateTime in either unspecified or UTC kind.</returns>
        internal static DateTime DateTimeToUtcIfPossible(DateTime dt)
        {
            switch (dt.Kind)
            {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Utc:
                    return dt;
                case DateTimeKind.Local:
                    return dt.ToUniversalTime();
                default:
                    // Default not expected
                    throw new ArgumentException("Unexpected DateTime kind " + dt.Kind.ToString());
            }
        }

        /// <summary>
        /// Converts a TimeSpan value to an XML period string.
        /// </summary>
        /// <param name="ts">TimeSpan.</param>
        /// <returns>XML period string.</returns>
        internal static string TimeSpanToString(TimeSpan ts)
        {
            return SXml.XmlConvert.ToString(ts);
        }

        /// <summary>
        /// Converts an XML period string to a TimeSpan value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>TimeSpan.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static TimeSpan TimeSpanFromString(string s)
        {
            return (TimeSpan)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToTimeSpan(a);
            },
            "TimeSpan", s);
        }

        /// <summary>
        /// Converts a boolean to XML boolean.
        /// </summary>
        /// <param name="b">Input.</param>
        /// <returns>XML boolean.</returns>
        internal static string BoolToString(bool b)
        {
            return SXml.XmlConvert.ToString(b);
        }

        /// <summary>
        /// Converts an XML boolean string to a boolean value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Boolean.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static bool BoolFromString(string s)
        {
            return (bool)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToBoolean(a);
            },
            "boolean", s);
        }

        /// <summary>
        /// Converts a double to XML double.
        /// </summary>
        /// <param name="d">Input.</param>
        /// <returns>XML double.</returns>
        internal static string DoubleToString(double d)
        {
            return SXml.XmlConvert.ToString(d);
        }

        /// <summary>
        /// Converts an XML double string to a double value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Double.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static double DoubleFromString(string s)
        {
            return (double)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToDouble(a);
            },
            "double", s);
        }

        /// <summary>
        /// Converts a long to XML long.
        /// </summary>
        /// <param name="l">Input.</param>
        /// <returns>XML long.</returns>
        internal static string LongToString(long l)
        {
            return SXml.XmlConvert.ToString(l);
        }

        /// <summary>
        /// Converts an XML long value to a long.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Long.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static long LongFromString(string s)
        {
            return (long)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToInt64(a);
            },
            "long", s);
        }

        /// <summary>
        /// Converts an int to XML int.
        /// </summary>
        /// <param name="i">Input.</param>
        /// <returns>XML int.</returns>
        internal static string IntToString(int i)
        {
            return SXml.XmlConvert.ToString(i);
        }

        /// <summary>
        /// Converts an XML int to an int value.
        /// </summary>
        /// <param name="s">Input.</param>
        /// <returns>Int.</returns>
        /// <exception cref="ArgumentException">Thrown if parsing fails.</exception>
        internal static int IntFromString(string s)
        {
            return (int)TryParse((a) => // Throws ArgumentException
            {
                return SXml.XmlConvert.ToInt32(a);
            },
            "Int32", s);
        }
        
        // Delegate for the generic TryParse function
        private delegate object ParserDelegate(string inputString);

        // Generic parsing function to re-use error handling
        private static object TryParse(ParserDelegate del, string typeName, string inputString)
        {
            try
            {
                return del(inputString);
            }
            catch (ArgumentException e) // Will also catch ArgumentNullException
            {
                var msg = BuildParsingErrorMessage(typeName, inputString);
                throw new ArgumentException(msg, e);
            }
            catch (FormatException e)
            {
                var msg = BuildParsingErrorMessage(typeName, inputString);
                throw new ArgumentException(msg, e);
            }
            catch (OverflowException e)
            {
                var msg = BuildParsingErrorMessage(typeName, inputString);
                throw new ArgumentException(msg, e);
            }
        }

        private static string BuildParsingErrorMessage(string typeName, string inputString)
        {
            return string.Format("Failed to parse {0} from \"{1}\"", typeName, inputString);
        }

        #endregion Parsing of datatypes


        /// <summary>
        /// Serialisers are cached for a better performance.
        /// </summary>
        private class SerializerCache
        {
            // The framework supposedly does not always cache the serialiser, at least if overriding attributes,
            // which reduces performance. Also, the serialiser object is supposedly thread-safe, so it can be
            // shared among applications.
            
            private readonly object m_lockObject = new object();
            private readonly SysColl.Dictionary<string, SXml.Serialization.XmlSerializer> m_cache = new SysColl.Dictionary<string, SXml.Serialization.XmlSerializer>();

            public SerializerCache()
            {
                // Empty ctor body
            }

            public SXml.Serialization.XmlSerializer GetSerializer(Type type)
            {
                var key = type.FullName;
                
                // The creation of a serialiser can take long, but no can do.
                // This will then block any other users.
                lock (m_lockObject)
                {
                    if (!m_cache.ContainsKey(key))
                    {
                        // Creating the serialiser object
                        m_cache[key] = CreateSerialiser(type);
                    }

                    return m_cache[key];
                }
            }

            private static SXml.Serialization.XmlSerializer CreateSerialiser(Type type)
            {
                return new SXml.Serialization.XmlSerializer(type);
            }
        }
    }
}
