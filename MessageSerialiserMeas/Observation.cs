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
    /// Observation class to associate metadata to actual measurement objects.
    /// <para>In this module, the code has been derived from
    /// Observations and Measurements - XML Implementation
    /// (OGC 10-025r1; please see the file "ref_and_license_ogc_om.txt").</para>
    /// </summary>
    public sealed class Observation
    {
        private const string ErrorMsgPopulation = "Failed to populate observation from XML data";
        
        private DateTime m_phenomenonTime = DateTime.MinValue.AddDays(1).ToUniversalTime(); // Note that setDefaults() overrides this
        private DateTime m_resultTime = DateTime.MinValue.AddDays(1).ToUniversalTime(); // Note that setDefaults() overrides this


        #region Constructors

        /// <summary>
        /// Constructor. Use this populate the observation manually.
        /// </summary>
        /// <param name="r">Result object.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        public Observation(Item r)
        {
            SetDefaults();

            Result = r;
        }
        
        /// <summary>
        /// Constructor to populate the information from XML.
        /// </summary>
        /// <param name="xmlBytes">Serialised XML document.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        public Observation(byte[] xmlBytes)
        {
            SetDefaults();

            XsdNs.OM_ObservationType proxy = null;

            try
            {
                // Deserialising
                proxy = (XsdNs.OM_ObservationType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.OM_ObservationType), xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation, e);
            }

            // Reading values from XML
            ReadFieldValuesFromXmlDoc(proxy);
        }

        /// <summary>
        /// Constructor to populate the object from an XML proxy object.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Observation(XsdNs.OM_ObservationType proxy)
        {
            ReadFieldValuesFromXmlDoc(proxy);
        }

        private void SetDefaults()
        {
            // For the sake of consistency, all defaults are now set here.
            Description = null;
            Name = null;
            PhenomenonTime = DateTime.Now.ToUniversalTime();
            ResultTime = PhenomenonTime;
            Procedure = "";
            ObservedProperty = "";
            FeatureOfInterest = "";
            ResultQuality = DataQuality.CreateGood();
            Result = null;
        }

        #endregion


        #region Properties
        
        /// <summary>
        /// Gets observation name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Phenomenon time.
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">Thrown if there is an attempt to set a non-UTC value.</exception>
        public DateTime PhenomenonTime
        {
            get { return m_phenomenonTime; }
            set
            {
                XNeut.Helper.ExpectDateTimeIsUtc(value); // throws DateTimeException
                m_phenomenonTime = value;
            }
        }

        /// <summary>
        /// Result time.
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">Thrown if there is an attempt to set a non-UTC value.</exception>
        public DateTime ResultTime
        {
            get { return m_resultTime; }
            set
            {
                XNeut.Helper.ExpectDateTimeIsUtc(value); // throws DateTimeException
                m_resultTime = value;
            }
        }

        /// <summary>
        /// Procedure.
        /// </summary>
        public string Procedure
        {
            get;
            set;
        }

        /// <summary>
        /// Observed property.
        /// </summary>
        public string ObservedProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Feature of interest.
        /// </summary>
        public string FeatureOfInterest
        {
            get;
            set;
        }

        /// <summary>
        /// The details of the feature of interest.
        /// </summary>
        public Item_DataRecord FeatureOfInterestDetails
        {
            get;
            set;
        }

        /// <summary>
        /// Result quality.
        /// </summary>
        public DataQuality ResultQuality
        {
            get;
            set;
        }

        /// <summary>
        /// Result.
        /// </summary>
        public Item Result
        {
            get;
            private set;
        }

        #endregion
        

        #region Other public or internal functions
        
        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns>Object as XML.</returns>
        public byte[] ToXmlBytes()
	    {
            var proxy = ToXmlProxy(""); // Empty ID prefix because the observation is the root element in the doc

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

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
        /// require an ID. For certain XML elements, the schema requires an ID that is unique within
        /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
        /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
        /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
        /// <returns>Proxy.</returns>
        internal XsdNs.OM_ObservationType ToXmlProxy(string idPrefix)
        {
            // No exception handling because an exception would indicate a bug

            // Creating an ID prefix to enable unique IDs within the XML doc
            var myUniqueId = idPrefix + "Obs";

            // Result quality
            var resultQuality = new XsdNs.DQ_Element_PropertyType { Title = ResultQuality.Value };

            if (PhenomenonTime == null)
            {
                // Default: using result time as the phenomenon time
                PhenomenonTime = ResultTime;
            }

            // Phenomenon time
            var phenoInstant = new Item_TimeInstant(PhenomenonTime);
            var phenoProp = (XsdNs.TimeInstantPropertyType)phenoInstant.GetObjectForXml_Result(myUniqueId + "_phenoTime");

            // Result time
            var resultTimeInstant = new Item_TimeInstant(ResultTime);
            var resultTimeProp = (XsdNs.TimeInstantPropertyType)resultTimeInstant.GetObjectForXml_Result(myUniqueId + "_resTime");

            // Assigning values to a raw observation object
            var obsToSerialise = new XsdNs.OM_ObservationType
            {
                id = myUniqueId,
                type = new XsdNs.ReferenceType { Href = Result.ObservationTypeUri },
                procedure = new XsdNs.OM_ProcessPropertyType { Title = Procedure },
                PhenomenonTime = phenoProp,
                resultTime = resultTimeProp,
                observedProperty = new XsdNs.ReferenceType { Title = ObservedProperty },
                featureOfInterest = WriteFeatureOfInterest(myUniqueId + "_complexFeature"),
                resultQuality = new XsdNs.DQ_Element_PropertyType[] { resultQuality },
                result = Result.GetObjectForXml_Result(myUniqueId + "_result_")
            };
            
            // Name
            if (!string.IsNullOrEmpty(Name))
            {
                var nameCodeObj = new XsdNs.CodeType { Value = Name };
                obsToSerialise.name = new XsdNs.CodeType[] { nameCodeObj };
            }

            // Description
            if (!string.IsNullOrEmpty(Description))
            {
                XsdNs.StringOrRefType desc = new XsdNs.StringOrRefType { Value = Description };
                obsToSerialise.description = desc;
            }
            
            return obsToSerialise;
        }

        #endregion


        #region Private functions for write

        private XsdNs.FeaturePropertyType WriteFeatureOfInterest(string idPrefix)
        {
            if (FeatureOfInterestDetails == null || FeatureOfInterestDetails.ItemNames.Count < 1)
            {
                // Simple feature of interest
                return new XsdNs.FeaturePropertyType { Title = FeatureOfInterest };
            }
            else
            {
                // Complex feature of interest
                FeatureOfInterestDetails.Identifier = FeatureOfInterest;
                
                return new XsdNs.FeaturePropertyType
                {
                    AbstractFeature = new XsdNs.SweDataComponentAsFeatureType()
                    {
                        DataRecord = FeatureOfInterestDetails.ToXmlProxy(idPrefix)
                    },
                    AbstractFeatureTypeInfo = XsdNs.AbstractFeatureTypeType.SweDataComponentAsFeatureType
                };
            }
        }
        
        #endregion


        #region Private functions for read
        
        private void ReadFieldValuesFromXmlDoc(XsdNs.OM_ObservationType observationRaw)
        {
            try
            {
                // Getting observation type Uri
                var typeUri = GetTypeUri(observationRaw);

                // Name specified?
                var nameList = observationRaw.name;

                Name = (nameList != null && nameList.Length > 0) ?
                    nameList[0].Value : null;

                // Description
                Description = (observationRaw.description != null && observationRaw.description.Value != null) ?
                    observationRaw.description.Value : null;

                // Phenomenon time; this is a required field
                PhenomenonTime = new Item_TimeInstant(observationRaw.PhenomenonTime).Value;

                // Result time; this is a required field
                ResultTime = new Item_TimeInstant(observationRaw.resultTime).Value;

                // These fields are required per the schema, but this processing enables non-specified values
                Procedure = (observationRaw.procedure != null && observationRaw.procedure.Title != null) ?
                    observationRaw.procedure.Title : null;
                ObservedProperty = (observationRaw.observedProperty != null && observationRaw.observedProperty.Title != null) ?
                    observationRaw.observedProperty.Title : null;
                ReadFeatureOfInterest(observationRaw);
                
                // Explicit result quality defined?
                if (observationRaw.resultQuality != null && observationRaw.resultQuality.Length > 0)
                {
                    var qualityStringRaw = observationRaw.resultQuality[0].Title;
                    ResultQuality = new DataQuality(qualityStringRaw);
                }
                else
                {
                    ResultQuality = DataQuality.CreateGood();
                }
                
                // Processing result information
                Result = ResultTypeManager.BuildResultFromXml(typeUri, observationRaw.result);
            }
            // If the document lacks required data:
            catch (ArgumentNullException e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation + " (required data missing?)", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation + " (required data missing?)", e);
            }
            // new DataQuality:
            catch (ArgumentException e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation, e);
            }
            // ResultTypeManager.buildResultFromXml:
            catch (NotSupportedException e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation + " (XML typing error?)", e);
            }
            catch (InvalidCastException e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation + " (XML typing error?)", e);
            }
        }
        
        private string GetTypeUri(XsdNs.OM_ObservationType observationRaw)
        {
            try
            {
                return observationRaw.type.Href;
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to recognise observation type", e);
            }
        }

        private void ReadFeatureOfInterest(XsdNs.OM_ObservationType observationRaw)
        {
            var featureProxy = observationRaw.featureOfInterest;

            // Feature of interest specified?
            if (featureProxy == null)
            {
                // No feature of interest specified at all
                FeatureOfInterest = null;
                FeatureOfInterestDetails = new Item_DataRecord();
                return;
            }

            // Consistency check: there must not be both xlink attrs and a complex feature of interest
            if (featureProxy.AbstractFeature != null && featureProxy.XLinkHelperObj.XLinkSpecified)
            {
                throw new XNeut.InvalidMessageException("Inconsistent feature of interest: complex feature must not have xlink attributes");
            }

            // Simple feature of interest?
            if (featureProxy.Title != null)
            {
                // No additional metadata expected, just reading the xlink:title attribute
                FeatureOfInterest = featureProxy.Title;
                FeatureOfInterestDetails = new Item_DataRecord();
                return;
            }

            if (featureProxy.AbstractFeature == null)
            {
                // No feature of interest specified at all
                FeatureOfInterest = null;
                FeatureOfInterestDetails = new Item_DataRecord();
                return;
            }

            try
            {
                // Processing complex feature of interest
                var featureCasted = (XsdNs.SweDataComponentAsFeatureType)featureProxy.AbstractFeature;
                var dataRecordRaw = featureCasted.DataRecord;
                
                FeatureOfInterestDetails = new Item_DataRecord(dataRecordRaw);
                FeatureOfInterest = FeatureOfInterestDetails.Identifier;
            }
            catch (Exception e)
            {
                throw new XNeut.InvalidMessageException(ErrorMsgPopulation + " (failed to process complex feature)", e);
            }
        }

        #endregion
    }
}
