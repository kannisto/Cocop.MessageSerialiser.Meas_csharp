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
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Processes tasking parameters.
    /// <para>In this module, the code has been derived from
    /// OGC(r) Sensor Planning Service Implementation Standard
    /// (OGC 09-000; please see the file "ref_and_license_ogc_sps.txt") and
    /// OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// </summary>
    internal class TaskingParameterProcessor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pm">Parameter values.</param>
        public TaskingParameterProcessor(Item_DataRecord pm)
        {
            Parameters = pm;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="proxy">Proxy with parameter data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        public TaskingParameterProcessor(XsdNs.ParameterDataType proxy)
        {
            // Setting defaults
            Parameters = new Item_DataRecord();
            
            // Parameters defined in the proxy?
            // The values are mandatory but, for robustness, just skipping if empty
            if (proxy.values != null)
            {
                // Reading parameters; expecting a data record
                if (!(proxy.values is XsdNs.DataRecordPropertyType paramProxy))
                {
                    throw new XNeut.InvalidMessageException("Unexpected type of tasking parameters; expected swe:DataRecord");
                }
                else
                {
                    Parameters = new Item_DataRecord(paramProxy);
                }
            }
        }

        /// <summary>
        /// Parameter values.
        /// </summary>
        public Item_DataRecord Parameters
        {
            get;
            set;
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
        internal XsdNs.ParameterDataType ToXmlProxy(string idPrefix)
        {
            return new XsdNs.ParameterDataType()
            {
                // The "encoding" element is mandatory in the schema
                encoding = new XsdNs.ParameterDataTypeEncoding()
                {
                    Item = new XsdNs.XMLEncodingType()
                },
                // The "values" element is mandatory in the schema
                values = Parameters.ToXmlProxyWithWrapper(idPrefix + "TaskPar-")
            };
        }
    }
}
