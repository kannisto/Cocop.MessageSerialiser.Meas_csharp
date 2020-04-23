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
    /// A class to process "insert observation" requests.
    /// <para>In this module, the code has been derived from
    /// OGC(r) Sensor Observation Service Interface Standard
    /// (OGC 12-006; please see the file "ref_and_license_ogc_sos.txt").</para>
    /// </summary>
    public sealed class InsertObservationRequest
    {
        /// <summary>
        /// Constructor. Use this to create a request to be submitted.
        /// </summary>
        public InsertObservationRequest()
        {
            Offering = new List<string>();
            Observations = new List<Observation>();
        }

        /// <summary>
        /// Constructor. Use this to process an incoming request object.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the message is not valid.</exception>
        public InsertObservationRequest(byte[] xmlBytes)
            : this()
        {
            XsdNs.InsertObservationType proxy = null;

            try
            {
                // Deserialising
                proxy = (XsdNs.InsertObservationType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.InsertObservationType), xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise XML data", e);
            }

            // Reading values from the proxy
            ReadFieldValuesFromXmlDoc(proxy);
        }


        #region Properties

        /// <summary>
        /// These are "pointers to an ObservationOffering to which the observation shall be added."
        /// </summary>
        public List<string> Offering
        {
            get;
            private set;
        }

        /// <summary>
        /// Observation data.
        /// </summary>
        public List<Observation> Observations
        {
            get;
            private set;
        }

        #endregion


        #region Public methods

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

        private void ReadFieldValuesFromXmlDoc(XsdNs.InsertObservationType proxy)
        {
            // Reading offerings
            if (proxy.offering != null)
            {
                foreach (string s in proxy.offering)
                {
                    Offering.Add(s);
                }
            }
            
            // Reading observations
            if (proxy.observation != null && proxy.observation.Length > 0)
            {
                foreach (var obsRaw in proxy.observation)
                {
                    var observation = new Observation(obsRaw.OM_Observation);
                    Observations.Add(observation);
                }
            }
        }

        private XsdNs.InsertObservationType ToXmlProxy()
        {
            var proxy = new XsdNs.InsertObservationType()
            {
                // Assigning mandatory attributes specified in the SWES standard.
                // These values are after the SOS specification.
                service = "SOS",
                version = "2.0.0",

                observation = new XsdNs.InsertObservationTypeObservation[Observations.Count]
            };

            // Populating offerings. There must be at least one value.
            // Therefore, adding an empty value if none have been specified.
            // Offerings are of the XML type "anyUri", which allows even an empty value.
            if (Offering.Count == 0)
            {
                Offering.Add("");
            }

            proxy.offering = new string[Offering.Count];

            for (int a = 0; a < Offering.Count; ++a)
            {
                proxy.offering[a] = Offering[a];
            }

            // XML Schema requires at least one observation
            if (Observations.Count < 1)
            {
                throw new ArgumentException("InsertObservationRequest must contain at least one observation");
            }

            // Populating observation data to proxy
            var index = 0;
            const string idPrefix = "InsertObsReq_i";

            foreach (var obs in Observations)
            {
                // To have unique IDs in the XML doc:
                var idPrefixForThis = BuildIdPrefix(idPrefix, index + 1);

                // Creating an observation proxy
                proxy.observation[index] = new XsdNs.InsertObservationTypeObservation
                {
                    OM_Observation = obs.ToXmlProxy(idPrefixForThis)
                };

                ++index;
            }

            return proxy;
        }

        private string BuildIdPrefix(string idPrefix, int idCounter)
        {
            // Would give, e.g., "InsertObsReq_i3-"
            return idPrefix + idCounter + "-";
        }

        #endregion
    }
}
