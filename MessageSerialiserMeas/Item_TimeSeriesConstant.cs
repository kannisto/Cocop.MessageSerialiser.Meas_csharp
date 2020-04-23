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
    /// A class to serialise time series with a constant sampling interval.
    /// <para>In this module, the code has been derived from
    /// TimeseriesML 1.0 – XML Encoding of the Timeseries Profile of Observations and Measurements
    /// (OGC 15-042r3; please see the file "ref_and_license_ogc_tsml.txt").</para>
    /// <seealso cref="Item_Array"/>
    /// <seealso cref="Item_TimeSeriesFlexible"/>
    /// </summary>
    public sealed class Item_TimeSeriesConstant : Item_TimeSeries, IEnumerable<Item_TimeSeriesConstant.TimeSeriesConstItem>
    {
        /// <summary>
        /// Represents an item in a time series. This class was implemented to enable the IEnumerable interface,
        /// which enables the collection initialiser.
        /// </summary>
        public sealed class TimeSeriesConstItem
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="v">Value.</param>
            /// <param name="dq">Data quality.</param>
            internal TimeSeriesConstItem(double v, DataQuality dq)
            {
                Value = v;
                DataQualityObj = dq;
            }
            
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


        private List<TimeSeriesConstItem> m_items = new List<TimeSeriesConstItem>();


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uom">Unit of measure.</param>
        /// <param name="baseTime">The base time of the time series (i.e., the time of the first sample). This must be in UTC.</param>
        /// <param name="spacing">The constant time interval between samples.</param>
        /// <exception cref="XNeut.DateTimeException">Thrown if the kind of <paramref name="baseTime" /> is not UTC.</exception>
        public Item_TimeSeriesConstant(string uom, DateTime baseTime, TimeSpan spacing) : base(XNeut.Helper.TypeUri_TimeSeriesConstant, uom)
        {
            XNeut.Helper.ExpectDateTimeIsUtc(baseTime); // throws DateTimeException
            
            BaseTime = baseTime;
            Spacing = spacing;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy"></param>
        internal Item_TimeSeriesConstant(XsdNs.TimeseriesDomainRangeType proxy) : base(XNeut.Helper.TypeUri_TimeSeriesConstant, proxy)
        {
            ReadValuesFromProxy(proxy);
        }


        #region Properties

        /// <summary>
        /// The base time of the time series (i.e., the time of the first sample).
        /// </summary>
        public DateTime BaseTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The constant time interval between samples.
        /// </summary>
        public TimeSpan Spacing
        {
            get;
            private set;
        }
        
        #endregion


        #region Public methods and properties
        
        /// <summary>
        /// Gets data quality.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Data quality.</returns>
        public override DataQuality GetDataQuality(int index)
        {
            return m_items[index].DataQualityObj;
        }

        /// <summary>
        /// Gets a value.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>Value.</returns>
        public override double GetValue(int index)
        {
            return m_items[index].Value;
        }

        /// <summary>
        /// The total count of values.
        /// </summary>
        public override int ValueCount
        {
            get { return m_items.Count; }
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


        #region Protected or private methods

        /// <summary>
        /// Adds subclass-specific values to the proxy.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        /// <param name="uid">Unique identifier for XML IDs.</param>
        protected override void AddSubclassDataToProxy(XsdNs.TimeseriesDomainRangeType proxy, string uid)
        {
            // (1) Creating a metadata element if it does not exist
            if (proxy.metadata1 == null || proxy.metadata1.Length == 0)
            {
                proxy.metadata1 = new XsdNs.TimeseriesMetadataExtensionPropertyType[]
                {
                    new XsdNs.TimeseriesMetadataExtensionPropertyType()
                };
            }
            else if (proxy.metadata1.Length > 1)
            {
                throw new ArgumentException("Cannot populate subclass data: expected exactly one metadata element");
            }

            if (proxy.metadata1[0].TimeseriesMetadataExtension == null)
            {
                proxy.metadata1[0].TimeseriesMetadataExtension = new XsdNs.TimeseriesMetadataExtensionType();
            }

            if (proxy.metadata1[0].TimeseriesMetadataExtension.timeseriesMetadata == null)
            {
                proxy.metadata1[0].TimeseriesMetadataExtension.timeseriesMetadata = new XsdNs.TimeseriesMetadataExtensionTypeTimeseriesMetadata();
            }
            
            if (proxy.metadata1[0].TimeseriesMetadataExtension.timeseriesMetadata.TimeseriesMetadata == null)
            {
                proxy.metadata1[0].TimeseriesMetadataExtension.timeseriesMetadata.TimeseriesMetadata = new XsdNs.TimeseriesMetadataType();
            }

            // (2) Setting timing information
            var metadataElement = proxy.metadata1[0].TimeseriesMetadataExtension.timeseriesMetadata.TimeseriesMetadata;
            metadataElement.baseTime = new XsdNs.TimePositionType
            {
                Value = XNeut.Helper.DateTimeToString(BaseTime) // throws DateTimeException
            };
            metadataElement.spacing = XNeut.Helper.TimeSpanToString(Spacing);
        }

        private void ReadValuesFromProxy(XsdNs.TimeseriesDomainRangeType proxy)
        {
            const string ERROR_MSG = "Failed to read the data of constant-interval time series";

            if (proxy.metadata1.Length != 1)
            {
                throw new XNeut.InvalidMessageException("Expected exactly one tsml:metadata element in a constant-interval time series");
            }

            try
            {
                var timeseriesMetadata = proxy.metadata1[0].TimeseriesMetadataExtension.timeseriesMetadata.TimeseriesMetadata;
                
                BaseTime = XNeut.Helper.DateTimeFromString(timeseriesMetadata.baseTime.Value);
                Spacing = XNeut.Helper.TimeSpanFromString(timeseriesMetadata.spacing);
            }
            // Missing fields
            catch (IndexOutOfRangeException e)
            {
                throw new XNeut.InvalidMessageException(ERROR_MSG + " (required item missing?)", e);
            }
            // Missing fields
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException(ERROR_MSG + " (required item missing?)", e);
            }
            // Datetime values, timespan values
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException(ERROR_MSG + " (invalid value formatting?)", e);
            }

            // Adding deserialised values from the base class
            for (int a = 0; a < DeserialisedValues.Count; ++a)
            {
                Add(DeserialisedValues[a], DeserialiesdDataQualities[a]);
            }
        }

        #endregion


        #region IEnumerable and others required for collection initialiser
        
        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<TimeSeriesConstItem> GetEnumerator()
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
        /// Adds an item to the time series.
        /// </summary>
        /// <param name="value">Measurement value.</param>
        /// <param name="dq">Data quality information.</param>
        public void Add(double value, DataQuality dq)
        {
            m_items.Add(new TimeSeriesConstItem(value, dq));
        }

        /// <summary>
        /// Adds an item to the time series.
        /// </summary>
        /// <param name="value">Measurement value.</param>
        public void Add(double value)
        {
            Add(value, DataQuality.CreateGood());
        }

        #endregion
    }
}
