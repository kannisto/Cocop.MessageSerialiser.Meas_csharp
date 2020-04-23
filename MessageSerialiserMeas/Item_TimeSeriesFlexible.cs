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
using System.Collections;
using System.Collections.Generic;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// A time series of measurement values. In this time series type, there is no requirement for 
    /// constant time intervals between measurement points.
    /// <para>In this module, the code has been derived from
    /// TimeseriesML 1.0 – XML Encoding of the Timeseries Profile of Observations and Measurements
    /// (OGC 15-042r3; please see the file "ref_and_license_ogc_tsml.txt").</para>
    /// <seealso cref="Item_Array"/>
    /// <seealso cref="Item_TimeSeriesConstant"/>
    /// </summary>
    public sealed class Item_TimeSeriesFlexible : Item_TimeSeries, IEnumerable<Item_TimeSeriesFlexible.TimeSeriesFlexItem>
    {
        /// <summary>
        /// Represents an item in a time series. This class was implemented to enable the IEnumerable interface,
        /// which enables the collection initialiser.
        /// </summary>
        public sealed class TimeSeriesFlexItem
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="dt">Timestamp. This must be in UTC.</param>
            /// <param name="v">Value.</param>
            /// <param name="dq">Data quality.</param>
            /// <exception cref="XNeut.DateTimeException">Thrown if <paramref name="dt"/> kind is not UTC.</exception>
            internal TimeSeriesFlexItem(DateTime dt, double v, DataQuality dq)
            {
                XNeut.Helper.ExpectDateTimeIsUtc(dt); // throws DateTimeException
                
                Timestamp = dt;
                Value = v;
                DataQualityObj = dq;
            }

            /// <summary>
            /// Timestamp.
            /// </summary>
            public DateTime Timestamp
            { get; private set; }

            /// <summary>
            /// Value.
            /// </summary>
            public double Value
            { get; private set; }

            /// <summary>
            /// Data quality.
            /// </summary>
            public DataQuality DataQualityObj
            { get; private set; }
        }
        
        private List<TimeSeriesFlexItem> m_items = new List<TimeSeriesFlexItem>();
        

        /// <summary>
        /// Constructor. Use this for manual population.
        /// </summary>
        /// <param name="uom">Unit of measure.</param>
        public Item_TimeSeriesFlexible(string uom) : base(XNeut.Helper.TypeUri_TimeSeriesFlexible, uom)
        {
            // Empty ctor body
        }

        /// <summary>
        /// Constructor. Use to populate from XML proxy.
        /// </summary>
        /// <param name="proxy">Proxy object.</param>
        internal Item_TimeSeriesFlexible(XsdNs.TimeseriesDomainRangeType proxy) : base(XNeut.Helper.TypeUri_TimeSeriesFlexible, proxy)
        {
            // Reading values from XML
            ReadFieldValuesFromXmlDoc(proxy);
        }


        #region Public methods and properties

        /// <summary>
        /// The total count of values.
        /// </summary>
        public override int ValueCount
        {
            get { return m_items.Count; }
        }
        
        /// <summary>
        /// Gets the timestamp of an item.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Timestamp.</returns>
        public DateTime GetTimestamp(int index)
        {
            return m_items[index].Timestamp;
        }

        /// <summary>
        /// Gets the value of an item.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Value.</returns>
        public override double GetValue(int index)
        {
            return m_items[index].Value;
        }

        /// <summary>
        /// Gets the data quality of an item.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Data quality.</returns>
        public override DataQuality GetDataQuality(int index)
        {
            return m_items[index].DataQualityObj;
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            return string.Format("Time series ({0} values)", m_items.Count);
        }
        
        #endregion


        #region Private and protected methods
        
        private void ReadFieldValuesFromXmlDoc(XsdNs.TimeseriesDomainRangeType proxy)
        {
            string partName = "timestamps";
            var timestampsList = new List<DateTime>();

            try
            {
                // May throw FormatException
                foreach (var dt in proxy.domainSet.TimePositionList.GetTimestampsHelper())
                {
                    timestampsList.Add(dt.ToUniversalTime());
                }
            }
            // Missing fields
            catch (IndexOutOfRangeException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read " + partName + " from time series XML (required item missing?)", e);
            }
            // Missing fields
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read " + partName + " from time series XML (required item missing?)", e);
            }
            // Datetime values
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read " + partName + " from time series XML (invalid value formatting?)", e);
            }

            // Checking that the size of each collection matches
            if (timestampsList.Count != DeserialisedValues.Count)
            {
                throw new XNeut.InvalidMessageException("The sizes of collections do not match in the XML document (something missing or too many)");
            }

            // Adding deserialised values
            for (int a = 0; a < timestampsList.Count; ++a)
            {
                Add(timestampsList[a], DeserialisedValues[a], DeserialiesdDataQualities[a]);
            }
        }
        
        /// <summary>
        /// Adds values to proxy.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        /// <param name="uid">Unique ID (for XML IDs).</param>
        protected override void AddSubclassDataToProxy(XsdNs.TimeseriesDomainRangeType proxy, string uid)
        {
            // No exception handling here as exceptions indicate bugs in this function
            
            var myUniqueId = uid + "_flex";

            // Checking in case the base class has already added this
            if (proxy.domainSet == null)
            {
                var domainSet = new XsdNs.DomainSetType();
                proxy.domainSet = domainSet;
            }

            // Adding time positions

            var timestampsList = new List<DateTime>();

            foreach (var obj in m_items)
            {
                timestampsList.Add(obj.Timestamp);
            }
            
            // domainSet/TimePositionList
            proxy.domainSet.TimePositionList = new XsdNs.TimePositionListType
            {
                id = myUniqueId + "_timestamps" // Required by the schema
            };
            proxy.domainSet.TimePositionList.SetTimestampsHelper(timestampsList); // throws DateTimeException
        }

        #endregion


        #region IEnumerable and add methods for collection initialisers

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<TimeSeriesFlexItem> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a value to the time series. The timestamp of the item must be after the timestamp of earlier added values.
        /// </summary>
        /// <param name="time">Timestamp. This must be in UTC.</param>
        /// <param name="value">Measurement value.</param>
        /// <param name="dq">Data quality information.</param>
        /// <exception cref="ArgumentException">Thrown if the timestamp is before the timestamp of an already existing value.</exception>
        /// <exception cref="XNeut.DateTimeException">Thrown if <paramref name="time"/> is not in UTC.</exception>
        public void Add(DateTime time, double value, DataQuality dq)
        {
            var newItem = new TimeSeriesFlexItem(time, value, dq); // throws DateTimeException

            // Checking in case of incorrect item ordering
            if (m_items.Count > 0)
            {
                DateTime previous = m_items[m_items.Count - 1].Timestamp;

                if (DateTime.Compare(previous, newItem.Timestamp) > 0)
                {
                    throw new ArgumentException("Time series items must be added in the time order");
                }
            }
            
            m_items.Add(newItem);
        }

        /// <summary>
        /// Adds a value to the time series. The timestamp of the item must be after the timestamp of earlier added values.
        /// The related data quality is assumed to be "good".
        /// </summary>
        /// <param name="time">Timestamp. This must be in UTC.</param>
        /// <param name="value">Measurement value.</param>
        /// <exception cref="ArgumentException">Thrown if the timestamp is before the timestamp of an already existing value.</exception>
        /// <exception cref="XNeut.DateTimeException">Thrown if <paramref name="time"/> is not in UTC.</exception>
        public void Add(DateTime time, double value)
        {
            // throws ArgumentException
            // throws DateTimeException
            Add(time, value, DataQuality.CreateGood());
        }
        
        #endregion
    }
}
