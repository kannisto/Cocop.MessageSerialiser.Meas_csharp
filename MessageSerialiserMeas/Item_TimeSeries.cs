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
using System.Collections.Generic;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Abstract base class for time series. A common base class exists, because the XML structures of time series
    /// resemble each other very much. However, in C#, not all structures can be reused. This applies especially
    /// because of collection initialisers that require the implementation of the <c>IEnumerable</c> interface.
    /// <para>In this module, the code has been derived from
    /// TimeseriesML 1.0 – XML Encoding of the Timeseries Profile of Observations and Measurements
    /// (OGC 15-042r3; please see the file "ref_and_license_ogc_tsml.txt").</para>
    /// <seealso cref="Item_Array"/>
    /// </summary>
    public abstract class Item_TimeSeries : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="typeUri">Type URI.</param>
        /// <param name="uom">Unit of measure.</param>
        protected Item_TimeSeries(string typeUri, string uom) : base(typeUri)
        {
            UnitOfMeasure = uom;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="typeUri">Type URI.</param>
        /// <param name="proxy">Proxy object.</param>
        protected Item_TimeSeries(string typeUri, XsdNs.TimeseriesDomainRangeType proxy) : base(typeUri)
        {
            UnitOfMeasure = ReadFieldValuesFromXmlDoc(proxy);
        }


        #region Properties

        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// The unit of measure.
        /// </summary>
        public string UnitOfMeasure
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of items in the time series.
        /// </summary>
        public abstract int ValueCount
        {
            get;
        }

        /// <summary>
        /// Values deserialised from XML.
        /// </summary>
        protected List<double> DeserialisedValues
        {
            get;
            private set;
        }

        /// <summary>
        /// Data quality values deserialised from XML.
        /// </summary>
        protected List<DataQuality> DeserialiesdDataQualities
        {
            get;
            private set;
        }
        
        #endregion


        #region Public and protected methods

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return ToXmlProxy(idPrefix);
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            var proxyFromContents = ToXmlProxy(idPrefix);

            var retval = new XsdNs.AbstractGmlAsSweDataComponentType()
            {
                AbstractGML = proxyFromContents,
                AbstractGMLTypeInfo = XsdNs.AbstractGMLTypeType.TimeseriesDomainRangeType
            };
            return retval;
        }

        internal override bool SupportsDataQualityInDataRecord
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the value of an item.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Value.</returns>
        public abstract double GetValue(int index);
        
        /// <summary>
        /// Gets the data quality of an item.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns>Data quality.</returns>
        public abstract DataQuality GetDataQuality(int index);
        
        /// <summary>
        /// Adds subclass data to a proxy object. In the implementation, the subclass *must not*
        /// re-instantiate any proxy class members if they already exist, as it would erase
        /// the data set by the superclass.
        /// </summary>
        /// <param name="proxy">Proxy object.</param>
        /// <param name="uid">Unique ID (for XML IDs).</param>
        protected abstract void AddSubclassDataToProxy(XsdNs.TimeseriesDomainRangeType proxy, string uid);

        #endregion


        #region Private methods

        private string ReadFieldValuesFromXmlDoc(XsdNs.TimeseriesDomainRangeType proxy)
        {
            string unitOfMeasure = null;
            string partName = "measurement values";

            DeserialisedValues = new List<double>();
            DeserialiesdDataQualities = new List<DataQuality>();

            try
            {
                // Reading description
                if (proxy.description != null)
                {
                    Description = proxy.description.Value;
                }

                var measValues = (XsdNs.MeasureOrNilReasonListType)proxy.rangeSet.Items[0];

                // Reading the unit of measure
                unitOfMeasure = measValues.uom;

                // Getting measurement values
                foreach (var d in measValues.ValuesHelper)
                {
                    DeserialisedValues.Add(d);
                }
                
                // Reading data qualities
                partName = "data qualities";

                if (proxy.metadata1.Length != 1)
                {
                    throw new XNeut.InvalidMessageException("Expected exactly one tsml:metadata element in a time series");
                }

                /*
                The OGC example "timeseries-domain-range-example.xml" uses the gmlcov:metadata
                field instead of this. However, this approach is better typed (no extension
                elements used) and has one less of XML element layers.
                */
                // Path: proxy.metadata1.TimeseriesMetadataExtension.annotation[0].AnnotationCoverage.rangeSet.CategoryList
                var metadataExt = proxy.metadata1[0].TimeseriesMetadataExtension;
                var rangeSet = metadataExt.annotation[0].AnnotationCoverage.rangeSet;
                var qualityListRaw = (XsdNs.CodeOrNilReasonListType)rangeSet.Items[0];

                // The first item in the quality list contains the quality values
                var qualityItemsRaw = qualityListRaw.TextAsSingleArrayHelper;

                // Parsing data quality values
                foreach (var s in qualityItemsRaw)
                {
                    DeserialiesdDataQualities.Add(new DataQuality(s));
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
            if (DeserialiesdDataQualities.Count != DeserialisedValues.Count)
            {
                throw new XNeut.InvalidMessageException("The sizes of collections do not match in the XML document (something missing or too many)");
            }

            return unitOfMeasure;
        }

        private XsdNs.TimeseriesDomainRangeType ToXmlProxy(string idPrefix)
        {
            // No exception handling here as exceptions indicate bugs in this function

            var myUniqueId = idPrefix + "TimeSer";

            // Initialising objects
            var proxy = new XsdNs.TimeseriesDomainRangeType
            {
                id = myUniqueId, // Required by the schema

                // Adding a rangeType element as it is required by the XML schema
                rangeType = new XsdNs.DataRecordPropertyType(),

                description = new XsdNs.StringOrRefType(),

                // Adding an empty domain set (required by gml:AbstractCoverageType)
                domainSet = new XsdNs.DomainSetType()
            };
            proxy.description.Value = Description;

            List<double> valuesForDeser = new List<double>();

            for (int a = 0; a < ValueCount; ++a)
            {
                valuesForDeser.Add(GetValue(a));
            }

            // 1) Adding item values
            // rangeSet/QuantityList
            var measValues = new XsdNs.MeasureOrNilReasonListType
            {
                uom = UnitOfMeasure,
                ValuesHelper = valuesForDeser
            };

            // rangeSet
            var rangeSetValues = new XsdNs.RangeSetType
            {
                Items = new object[] { measValues }
            };
            proxy.rangeSet = rangeSetValues;

            // 2) Adding data quality information
            var qualityStrings = new List<string>();

            for (int a = 0; a < ValueCount; ++a)
            {
                qualityStrings.Add(GetDataQuality(a).Value);
            }
            
            var qualityList = new XsdNs.CodeOrNilReasonListType
            {
                codeSpace = "http://cocop",
                TextAsSingleArrayHelper = qualityStrings
            };

            /*
            The OGC example "timeseries-domain-range-example.xml" uses the gmlcov:metadata
            field instead of this. However, this approach is better typed (no extension 
            elements used) and has one less of XML element layers.
            */
            // tsml:metadata/TimeseriesMetadataExtension/defaultPointMetadata/annotation/AnnotationCoverage/rangeSet (/CategoryList)
            var rangeSetQual = new XsdNs.RangeSetType
            {
                Items = new object[] { qualityList }
            };

            // tsml:metadata/TimeseriesMetadataExtension/defaultPointMetadata/annotation/AnnotationCoverage
            var annotationCoverage = new XsdNs.AnnotationCoverageType
            {
                id = myUniqueId + "_qualCov", // Required by the schema (inherited from AbstractGMLType)
                domainSet = new XsdNs.DomainSetType(), // Required by the schema
                rangeSet = rangeSetQual,
                rangeType = new XsdNs.DataRecordPropertyType() // Required by the schema
            };
            var annotationCoverageProp = new XsdNs.AnnotationCoveragePropertyType
            {
                AnnotationCoverage = annotationCoverage
            };

            // tsml:metadata/TimeseriesMetadataExtension
            var tsMetadataExt = new XsdNs.TimeseriesMetadataExtensionType
            {
                annotation = new XsdNs.AnnotationCoveragePropertyType[1]
            };
            tsMetadataExt.annotation[0] = annotationCoverageProp;

            // tsml:metadata
            var metadata = new XsdNs.TimeseriesMetadataExtensionPropertyType
            {
                TimeseriesMetadataExtension = tsMetadataExt
            };
            proxy.metadata1 = new XsdNs.TimeseriesMetadataExtensionPropertyType[] { metadata };

            // Adding items from the subclass
            AddSubclassDataToProxy(proxy, myUniqueId);

            return proxy;
        }
        #endregion
    }
}
