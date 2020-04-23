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
    /// Base class for extensible response types.
    /// <para>In this module, the code has been derived from
    /// OpenGIS(r) SWE Service Model Implementation Standard
    /// (OGC 09-001; please see the file "ref_and_license_ogc_swes.txt").</para>
    /// </summary>
    public abstract class ExtensibleResponse
    {
        private const string IDENTIFIER = "cocop/requestresult";
        private const string RESULT_FIELD_NAME = "result";
        private const string MESSAGE_FIELD_NAME = "message";

        private string m_requestResultMessage = "";


        /// <summary>
        /// Constructor.
        /// </summary>
        protected ExtensibleResponse()
        {
            RequestResult = RequestResultType.Ok;
        }
        
        /// <summary>
        /// The result of the related operation.
        /// </summary>
        public RequestResultType RequestResult
        {
            get;
            set;
        }

        /// <summary>
        /// The result message of the operation (if any).
        /// </summary>
        public string RequestResultMessage
        {
            get
            {
                // Return "" if null
                return m_requestResultMessage ?? "";
            }
            set
            {
                // Assign "" if null
                m_requestResultMessage = value ?? "";
            }
        }

        /// <summary>
        /// This method populates the base class data to the given proxy.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        protected void PopulateExtensibleResponseToProxy(XsdNs.ExtensibleResponseType proxy)
        {
            // Creating a data record to enclose result information
            var record = new Item_DataRecord();
            var resultItem = new Item_Category(RequestResult.ToString());
            record.Add(RESULT_FIELD_NAME, resultItem);
            var messageItem = new Item_Text(RequestResultMessage);
            record.Add(MESSAGE_FIELD_NAME, messageItem);
            
            // Creating a proxy and adding it as an extension
            proxy.Extension = new object[1];
            var recordProp = (XsdNs.DataRecordPropertyType)record.GetObjectForXml_Result("ExtResp_");
            recordProp.DataRecord.identifier = IDENTIFIER;
            proxy.Extension[0] = recordProp;
        }

        /// <summary>
        /// Reads values from the XML proxy.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        protected void ReadExtensibleResponseItemsFromProxy(XsdNs.ExtensibleResponseType proxy)
        {
            // The default result is "unknown" when reading from XML
            RequestResult = RequestResultType.Unknown;

            // Checking if the status data record is there
            if (proxy.Extension != null && proxy.Extension.Length > 0)
            {
                foreach (var ext in proxy.Extension)
                {
                    // Is it a data record?
                    if (!(ext is XsdNs.DataRecordPropertyType dataRecordProp))
                    {
                        continue;
                    }

                    // Does the data record have a body with the expected identifier?
                    if (dataRecordProp.DataRecord == null ||
                        dataRecordProp.DataRecord.identifier == null ||
                        !dataRecordProp.DataRecord.identifier.Equals(IDENTIFIER))
                    {
                        continue;
                    }

                    // Processing the data record
                    var dataRecord = new Item_DataRecord(dataRecordProp);
                    
                    // Getting status information
                    if (dataRecord.ItemNames.Contains(RESULT_FIELD_NAME))
                    {
                        RequestResult = ParseRequestResult(dataRecord[RESULT_FIELD_NAME]);
                    }
                    else
                    {
                        RequestResult = RequestResultType.Unknown;
                    }

                    // Getting status message
                    if (dataRecord.ItemNames.Contains(MESSAGE_FIELD_NAME))
                    {
                        RequestResultMessage = GetStatusMessage(dataRecord[MESSAGE_FIELD_NAME]);
                    }
                }
            }
        }
        
        private RequestResultType ParseRequestResult(Item fieldItem)
        {
            try
            {
                // Parsing status value
                var categoryItem = (Item_Category)fieldItem;
                return (RequestResultType)Enum.Parse(typeof(RequestResultType), categoryItem.Value);
            }
            catch (ArgumentException e)
            {
                throw new XNeut.InvalidMessageException("Unexpected request result value", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Something is missing from the result", e);
            }
            catch (InvalidCastException e)
            {
                throw new XNeut.InvalidMessageException("Unexpected type of result field", e);
            }
        }

        private string GetStatusMessage(Item fieldItem)
        {
            try
            {
                var textItem = (Item_Text)fieldItem;
                return textItem.Value;
            }
            catch (InvalidCastException e)
            {
                throw new XNeut.InvalidMessageException("Unexpected type of status message field", e);
            }
        }
    }
}
