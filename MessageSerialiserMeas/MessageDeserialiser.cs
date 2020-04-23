//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 8/2018
// Last modified: 2/2020

using System;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Helper class for the deserialisation of messages. If you need this class,
    /// it may indicate bad design, because you should usually know which message
    /// type to expect.
    /// </summary>
    public static class MessageDeserialiser
    {
        /// <summary>
        /// Recognises message type and deserialises it from given XML data.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <returns>Deserialised message.</returns>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        public static object Deserialise(byte[] xmlBytes)
        {
            // Resolving the name of the root element
            string rootName;

            try
            {
                rootName = XNeut.Helper.GetRootElNameFromXmlDoc(xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("XML parsing failed", e);
            }
            
            // Mapping the names of root elements and message classes
            switch (rootName)
            {
                case "GetObservation":
                    return new GetObservationRequest(xmlBytes);
                case "GetObservationResponse":
                    return new GetObservationResponse(xmlBytes);

                case "InsertObservation":
                    return new InsertObservationRequest(xmlBytes);
                case "InsertObservationResponse":
                    return new InsertObservationResponse(xmlBytes);

                case "OM_Observation":
                    return new Observation(xmlBytes);
                    
                case "StatusReport":
                    return new TaskStatusReport(xmlBytes);

                case "Submit":
                case "Update":
                case "Cancel":
                case "GetStatus":
                    return new TaskRequest(xmlBytes);

                case "SubmitResponse":
                case "UpdateResponse":
                case "CancelResponse":
                case "GetStatusResponse":
                    return new TaskResponse(xmlBytes);
                    
                default:
                    throw new XNeut.InvalidMessageException("Unexpected root element \"" + rootName + "\"");
            }
        }
    }
}
