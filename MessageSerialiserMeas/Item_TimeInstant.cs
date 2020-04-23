//
// Please make sure to read and understand README.md and LICENSE.txt.
//
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
// Author: Petri Kannisto, Tampere University, Finland
// Last modified: 4/2020
//
// This API has been derived from standards and XML schemata provided by the
// Open Geospatial Consortium (OGC(r)). Please make sure to read and understand
// the following legal conditions:
// (1) Copyright Notice and Disclaimers at https://www.ogc.org/ogc/legal
// (2) OGC(r) Document Notice; the most recent version is at
//     https://www.ogc.org/ogc/document and another enclosed in file
//     "ogc_document_notice.txt"
// (3) OGC(r) Software Notice; the most recent version is at
//     https://www.ogc.org/ogc/software and another enclosed in file
//     "ogc_software_notice.txt"
// (4) The license of each related standard referred to in this file.

using System;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents a timestamp either in the past or in the future.
    /// <para>In this module, the code has been derived from
    /// OpenGIS(r) Geography Markup Language (GML) Encoding Standard
    /// (OGC 07-036; please see the file "ref_and_license_ogc_gml.txt") and
    /// OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_TimeRange"/>
    /// </summary>
    public sealed class Item_TimeInstant : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dt">Timestamp value. This must be in UTC.</param>
        /// <exception cref="XNeut.DateTimeException">Thrown if <paramref name="dt"/> is not UTC.</exception>
        public Item_TimeInstant(DateTime dt) : base(XNeut.Helper.TypeUri_Temporal)
        {
            XNeut.Helper.ExpectDateTimeIsUtc(dt); // throws DateTimeException
            Value = dt;
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (phenomenon time).
        /// </summary>
        /// <param name="el">XML contents.</param>
        internal Item_TimeInstant(XsdNs.TimeInstantType el) : base(XNeut.Helper.TypeUri_Temporal)
        {
            // PhenomenonTime field uses this ctor.

            try
            {
                string valueRaw = el.timePosition.Value;
                Value = XNeut.Helper.DateTimeFromString(valueRaw);
            }
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read time instant", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read time instant (something missing?)", e);
            }
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (observation result).
        /// </summary>
        /// <param name="el">XML contents.</param>
        internal Item_TimeInstant(XsdNs.TimeInstantPropertyType el) : this(el.TimeInstant)
        {
            // The funny time-related metadata structures of O&M make this almost
            // redundant constructor about necessary.

            // ResultTime field uses this ctor, as well as Results of type TimeInstant.
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (data record field).
        /// </summary>
        /// <param name="el">XML contents.</param>
        internal Item_TimeInstant(XsdNs.TimeType1 el) : base(XNeut.Helper.TypeUri_Temporal)
        {
            // DataRecord uses this ctor.

            try
            {
                string valueRaw = el.value;
                Value = XNeut.Helper.DateTimeFromString(valueRaw);
            }
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read time instant", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read time instant (something missing?)", e);
            }
        }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            // Reusing the XML structure that appears in metadata items
            var resultTimeInstant = GetXmlObjectForMetadata(idPrefix + "TimeInst");

            return new XsdNs.TimeInstantPropertyType
            {
                TimeInstant = resultTimeInstant
            };
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return new XsdNs.TimeType1
            {
                value = XNeut.Helper.DateTimeToString(Value), // throws DateTimeException

                // Adding an empty UOM element (the schema requires it although it is obviously needless for timestamps)
                uom = new XsdNs.UnitReference()
            };
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            return XNeut.Helper.DateTimeToString(Value); // throws DateTimeException
        }

        /// <summary>
        /// The value of the timestamp.
        /// </summary>
        public DateTime Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an XML object for observation metadata. (Initially, there was a separate function for metadata.)
        /// </summary>
        /// <param name="id">An ID that is unique within the XML document.</param>
        /// <returns>Object.</returns>
        private XsdNs.TimeInstantType GetXmlObjectForMetadata(string id)
        {
            var resultTimePos = new XsdNs.TimePositionType
            {
                Value = XNeut.Helper.DateTimeToString(Value) // throws DateTimeException
            };

            var resultTimeInstant = new XsdNs.TimeInstantType
            {
                timePosition = resultTimePos,

                // An ID is required; using a random ID as the default
                id = id
            };

            return resultTimeInstant;
        }
    }
}