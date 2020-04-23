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
    /// Represents a time period.
    /// <para>In this module, the code has been derived from
    /// OpenGIS(r) Geography Markup Language (GML) Encoding Standard
    /// (OGC 07-036; please see the file "ref_and_license_ogc_gml.txt") and
    /// OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_TimeInstant"/>
    /// </summary>
    public sealed class Item_TimeRange : Item
    {
        /*
        This should be OK based on O&M 2.0 XML schemata and XML examples (but not the only possible way):
        <om:result xsi:type="gml:TimePeriodPropertyType">
          <gml:TimePeriod gml:id="tm876">
            <gml:beginPosition>2010-03-24T10:00:00</gml:beginPosition>
            <gml:endPosition>2010-03-25T08:50:00</gml:endPosition>
          </gml:TimePeriod>
        </om:result>
        */

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">Start time of the period. This must be in UTC.</param>
        /// <param name="end">End time of the period. This must be in UTC.</param>
        /// <exception cref="ArgumentException">Thrown if end is before start.</exception>
        /// <exception cref="XNeut.DateTimeException">Thrown if the kind of <paramref name="start"/> or <paramref name="end"/> is not UTC.</exception>
        public Item_TimeRange(DateTime start, DateTime end) : base(XNeut.Helper.TypeUri_Temporal)
        {
            XNeut.Helper.ExpectDateTimeIsUtc(start); // throws DateTimeException
            XNeut.Helper.ExpectDateTimeIsUtc(end); // throws DateTimeException
            
            Start = start;
            End = end;

            ValidateStartEnd(Start, End);
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (observation result).
        /// </summary>
        /// <param name="el">XML contents.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_TimeRange(XsdNs.TimePeriodPropertyType el) : base(XNeut.Helper.TypeUri_Temporal)
        {
            try
            {
                var startRaw = (XsdNs.TimePositionType)el.TimePeriod.Item;
                var endRaw = (XsdNs.TimePositionType)el.TimePeriod.Item1;

                Start = XNeut.Helper.DateTimeFromString(startRaw.Value);
                End = XNeut.Helper.DateTimeFromString(endRaw.Value);

                ValidateStartEnd(Start, End);
            }
            catch (ArgumentException e)
            {
                throw new XNeut.InvalidMessageException("Invalid time range values", e);
            }
            catch (InvalidCastException e)
            {
                throw new XNeut.InvalidMessageException("Invalid time range values", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Invalid time range (something missing?)", e);
            }
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (data record field).
        /// </summary>
        /// <param name="el">XML contents.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_TimeRange(XsdNs.TimeRangeType el) : base(XNeut.Helper.TypeUri_Temporal)
        {
            try
            {
                Start = el.GetStartHelper();
                End = el.GetEndHelper();

                ValidateStartEnd(Start, End);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read time range (something missing?)", e);
            }
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read time range", e);
            }
        }

        private void ValidateStartEnd(DateTime start, DateTime end)
        {
            if (start.CompareTo(end) > 0)
            {
                throw new ArgumentException("Time period: end time must not be before start");
            }
        }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return new XsdNs.TimePeriodPropertyType()
            {
                TimePeriod = GetObjectToMarshalMetadata(idPrefix + "TimeRange") // throws DateTimeException
            };
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            var retval = new XsdNs.TimeRangeType();
            retval.SetValuesHelper(Start, End); // throws DateTimeException

            // Adding an empty UOM element (the schema requires it although it is obviously needless for timestamps)
            var unitRef = new XsdNs.UnitReference();
            retval.uom = unitRef;

            return retval;
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            return string.Format("{0}..{1}",
                XNeut.Helper.DateTimeToString(Start), // throws DateTimeException
                XNeut.Helper.DateTimeToString(End)); // throws DateTimeException
        }

        /// <summary>
        /// The start time of the period.
        /// </summary>
        public DateTime Start
        {
            get;
            private set;
        }

        /// <summary>
        /// The end time of the period.
        /// </summary>
        public DateTime End
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an object for metadata item marshalling. This function was
        /// initially created to enable time periods as the phenomenon time
        /// in observations, but now time series have their dedicated XML type.
        /// </summary>
        /// <returns>Object.</returns>
        private XsdNs.TimePeriodType GetObjectToMarshalMetadata(string id)
        {
            return new XsdNs.TimePeriodType()
            {
                Item = CreateTimePositionForMarshal(Start), // throws DateTimeException
                Item1 = CreateTimePositionForMarshal(End), // throws DateTimeException
                // An ID is required; using a random ID as the default
                id = id
            };
        }

        private XsdNs.TimePositionType CreateTimePositionForMarshal(DateTime dateTime)
        {
            return new XsdNs.TimePositionType
            {
                Value = XNeut.Helper.DateTimeToString(dateTime) // throws DateTimeException
            };
        }
    }
}