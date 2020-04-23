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
    /// A class to process "insert observation" responses.
    /// <para>In this module, the code has been derived from
    /// OGC(r) Sensor Observation Service Interface Standard
    /// (OGC 12-006; please see the file "ref_and_license_ogc_sos.txt").</para>
    /// </summary>
    public sealed class InsertObservationResponse : ExtensibleResponse
    {
        /// <summary>
        /// Constructor. Use this to create a response to be submitted.
        /// </summary>
        public InsertObservationResponse()
            : base()
        {
            // Empty ctor body
        }

        /// <summary>
        /// Constructor. Use this to process an incoming response object.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the message is not valid.</exception>
        public InsertObservationResponse(byte[] xmlBytes)
            : base()
        {
            try
            {
                // Deserialising the document
                var proxy = (XsdNs.InsertObservationResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.InsertObservationResponseType), xmlBytes);
                ReadExtensibleResponseItemsFromProxy(proxy);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise XML data", e);
            }
        }

        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns>XML data.</returns>
        public byte[] ToXmlBytes()
        {
            var proxy = new XsdNs.InsertObservationResponseType();

            // Populating data from the base class
            PopulateExtensibleResponseToProxy(proxy);

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
    }
}
