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
    /// A class to process "get observation" responses.
    /// <para>In this module, the code has been derived from
    /// OGC(r) Sensor Observation Service Interface Standard
    /// (OGC 12-006; please see the file "ref_and_license_ogc_sos.txt").</para>
    /// </summary>
    public sealed class GetObservationResponse : ExtensibleResponse
    {
        /// <summary>
        /// Constructor. Use this to create a response to be submitted.
        /// </summary>
        public GetObservationResponse()
            : base()
        {
            Observations = new List<Observation>();
        }

        /// <summary>
        /// Constructor. Use this to process an incoming response object.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the message is not valid.</exception>
        public GetObservationResponse(byte[] xmlBytes)
            : base()
        {
            try
            {
                // Deserialising the document
                var proxy = (XsdNs.GetObservationResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.GetObservationResponseType), xmlBytes);
                ReadExtensibleResponseItemsFromProxy(proxy);

                // Reading content
                Observations = new List<Observation>();
                ReadFieldValueFromXmlDoc(proxy);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise XML data", e);
            }
        }


        #region Properties

        /// <summary>
        /// Observation data.
        /// </summary>
        public List<Observation> Observations
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
        
        private void ReadFieldValueFromXmlDoc(XsdNs.GetObservationResponseType proxy)
        {
            // Reading observation data
            if (proxy.observationData != null)
            {
                foreach (var item in proxy.observationData)
                {
                    var newObs = new Observation(item.OM_Observation);
                    Observations.Add(newObs);
                }
            }
        }

        private object ToXmlProxy()
        {
            var proxy = new XsdNs.GetObservationResponseType();

            // Populating data from the base class
            PopulateExtensibleResponseToProxy(proxy);

            // These enable unique identifiers within the XML document
            var idPrefix = "GetObsResp_i";
            var idCounter = 1;

            // Adding observations
            if (Observations.Count > 0)
            {
                proxy.observationData = new XsdNs.GetObservationResponseTypeObservationData[Observations.Count];
                
                for (int a = 0; a < Observations.Count; ++a)
                {
                    proxy.observationData[a] = new XsdNs.GetObservationResponseTypeObservationData();
                    var idPrefWithCounter = BuildIdPrefix(idPrefix, idCounter);
                    proxy.observationData[a].OM_Observation = Observations[a].ToXmlProxy(idPrefWithCounter);

                    ++idCounter;
                }
            }
            
            return proxy;
        }

        private string BuildIdPrefix(string idPrefix, int idCounter)
        {
            // Would give, e.g., "GetObsResp_i3-"
            return idPrefix + idCounter + "-";
        }

        #endregion
    }
}
