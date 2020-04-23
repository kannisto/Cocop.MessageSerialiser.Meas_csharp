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
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// A class to process "get observation" requests.
    /// <para>In this module, the code has been derived from
    /// OGC(r) Sensor Observation Service Interface Standard
    /// (OGC 12-006; please see the file "ref_and_license_ogc_sos.txt").</para>
    /// </summary>
    public sealed class GetObservationRequest
    {
        private const string ErrorMsgPopulation = "Failed to populate request object from XML data";

        
        #region Constructors

        /// <summary>
        /// Constructor. Use to create requests to be submitted.
        /// </summary>
        public GetObservationRequest()
        {
            FeaturesOfInterest = new HashSet<string>();
            ObservedProperties = new HashSet<string>();
            TemporalFilters = new List<TemporalFilter>();
            Items = new List<Item_DataRecord>();
        }

        /// <summary>
        /// Constructor. Use to process incoming requests.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the message is not valid.</exception>
        public GetObservationRequest(byte[] xmlBytes)
            : this()
        {
            XsdNs.GetObservationType proxy = null;

            try
            {
                // Deserialising
                proxy = (XsdNs.GetObservationType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.GetObservationType), xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise XML data", e);
            }

            // Reading values from the proxy
            ReadFieldValuesFromXmlDoc(proxy);
        }

        #endregion


        #region Properties

        /// <summary>
        /// The collection of features of interest.
        /// </summary>
        public HashSet<string> FeaturesOfInterest
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of observed properties.
        /// </summary>
        public HashSet<string> ObservedProperties
        {
            get;
            private set;
        }

        /// <summary>
        /// The temporal filters of the request.
        /// </summary>
        public List<TemporalFilter> TemporalFilters
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of items in the request.
        /// </summary>
        public List<Item_DataRecord> Items
        {
            get;
            private set;
        }

        #endregion


        #region Other public methods

        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns>XML data.</returns>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the message is not valid.</exception>
        public byte[] ToXmlBytes()
        {
            var proxy = ToXmlProxy();

            try
            {
                // Serialising
                return XNeut.Helper.ToXmlBytes(proxy);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("XML serialisation failed", e);
            }
        }

        #endregion


        #region Private methods

        private XsdNs.GetObservationType ToXmlProxy()
        {
            // Using this to enable unique IDs within the XML document
            var idPrefix = "GetObsReq_i";

            var toSerialise = new XsdNs.GetObservationType
            {
                // Assigning mandatory attributes specified in the SWES standard.
                // These value follow the SOS specification.
                service = "SOS",
                version = "2.0.0",
                
                // Assigning measurement point information
                featureOfInterest = StringCollectionToStringArray(FeaturesOfInterest),
                observedProperty = StringCollectionToStringArray(ObservedProperties),

                // Assigning extensions
                extension = new object[Items.Count]
            };

            int prefixCounter = 0;

            for (var a = 0; a < Items.Count; ++a)
            {
                var ext = Items[a];

                if (ext == null)
                {
                    throw new NullReferenceException("Item is null");
                }
                
                // Assigning the proxy object
                var idWithCounter = idPrefix + (prefixCounter + 1) + "-"; // This gives, e.g., "GetObsReq_i3-"
                toSerialise.extension[a] = ext.ToXmlProxyWithWrapper(idWithCounter);

                ++prefixCounter;
            }

            // Assigning temporal filters
            toSerialise.temporalFilter = new XsdNs.GetObservationTypeTemporalFilter[TemporalFilters.Count];
            
            for (int a = 0; a < TemporalFilters.Count; ++a)
            {
                // Create a proxy object for serialisation
                var idWithCounter = idPrefix + (prefixCounter + 1) + "-"; // This gives, e.g., "GetObsReq_i3-"
                var filterProxy = TemporalFilters[a].ToXmlProxy(idWithCounter);

                toSerialise.temporalFilter[a] = filterProxy;

                ++prefixCounter;
            }

            return toSerialise;
        }
        
        private string[] StringCollectionToStringArray(HashSet<string> input)
        {
            string[] retval = new string[input.Count];
            int counter = 0;

            foreach (var item in input)
            {
                retval[counter] = item;
                ++counter;
            }

            return retval;
        }
        
        private void ReadFieldValuesFromXmlDoc(XsdNs.GetObservationType requestObjectRaw)
        {
            // Reading features of interest
            if (requestObjectRaw.featureOfInterest != null && requestObjectRaw.featureOfInterest.Length > 0)
            {
                FeaturesOfInterest = new HashSet<string>(requestObjectRaw.featureOfInterest);
            }

            // Reading observed properties
            if (requestObjectRaw.observedProperty != null && requestObjectRaw.observedProperty.Length > 0)
            {
                ObservedProperties = new HashSet<string>(requestObjectRaw.observedProperty);
            }

            // Reading extensions
            if (requestObjectRaw.extension != null)
            {
                foreach (var ext in requestObjectRaw.extension)
                {
                    if (ext.GetType() != typeof(XsdNs.DataRecordPropertyType))
                    {
                        throw new XNeut.InvalidMessageException("The type of the extension object is not supported: " + ext.GetType().ToString());
                    }

                    var dataRecordRaw = (XsdNs.DataRecordPropertyType)ext;
                    Items.Add(new Item_DataRecord(dataRecordRaw.DataRecord));
                }
            }

            // Reading temporal filters
            if (requestObjectRaw.temporalFilter != null)
            {
                foreach (var filterEl in requestObjectRaw.temporalFilter)
                {
                    var filterObj = new TemporalFilter(filterEl);
                    TemporalFilters.Add(filterObj);
                }
            }
        }

        #endregion
    }
}
