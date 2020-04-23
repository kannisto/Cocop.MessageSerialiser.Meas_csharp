//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2/2020
// Last modified: 3/2020

using System;
using MsgMeas = Cocop.MessageSerialiser.Meas;

namespace Examples
{
    // This example shows how to communicate observations using GetObservationRequest and
    // GetObservationResponse.
    
    class GetObservationClient
    {
        public void Get()
        {
            // Creating a request object
            var requestObj = new MsgMeas.GetObservationRequest();

            // Setting the conditions of the request
            requestObj.FeaturesOfInterest.Add("myplant/myprocess/mytemperature");

            // Adding a data record; use this to whatever additional parameters you need.
            // This naturally depends on the server.
            var extensionObj = new MsgMeas.Item_DataRecord()
            {
                { "parameter_x", new MsgMeas.Item_Count(3) }
            };
            requestObj.Items.Add(extensionObj);

            // Adding a temporal filter: retrieve all observation after given time
            var tempFilter1 = new MsgMeas.TemporalFilter(
                MsgMeas.TemporalFilter.ValueReferenceType.PhenomenonTime,
                MsgMeas.TemporalFilter.OperatorType.After,
                new MsgMeas.Item_TimeInstant(DateTime.Parse("2018-05-18T08:05:44Z").ToUniversalTime())
            );
            requestObj.TemporalFilters.Add(tempFilter1);

            // Serialising the request object
            byte[] requestXml = requestObj.ToXmlBytes();

            // Sending the requestXml to the server and waiting for a response...
            SendRequest(requestXml);
            byte[] responseXml = WaitForResponse();

            // Processing the response
            MsgMeas.GetObservationResponse responseObj;
            try
            {
                responseObj = new MsgMeas.GetObservationResponse(responseXml);
            }
            catch (MsgMeas.Neutral.InvalidMessageException e)
            {
                throw new InvalidOperationException("Failed to read server response: " + e.Message, e);
            }

            if (responseObj.RequestResult != MsgMeas.RequestResultType.Ok)
            {
                throw new InvalidOperationException("Unexpected response from server");
            }

            foreach (var obs in responseObj.Observations)
            {
                // Processing the observations...
                // ...
            }
        }

        private void SendRequest(byte[] requestXml)
        {
            // In real life, you will have to consider where to send.
            // This depends on the protocol and network structure.
            throw new NotImplementedException();
        }

        private byte[] WaitForResponse()
        {
            throw new NotImplementedException();
        }
    }
    
    class GetObservationServer
    {
        public void Serve()
        {
            while (true)
            {
                // Waiting for a request to come from the network
                byte[] requestXml = WaitForRequest();
                MsgMeas.GetObservationRequest requestObj;
                try
                {
                    requestObj = new MsgMeas.GetObservationRequest(requestXml);
                }
                catch (MsgMeas.Neutral.InvalidMessageException e)
                {
                    Console.WriteLine("Failed to read request: " + e.Message);
                    continue;
                }

                // Assuming a certain feature of interest
                if (!requestObj.FeaturesOfInterest.Contains("myplant/myprocess/mytemperature"))
                {
                    // Error!
                    var errorResponse = new MsgMeas.GetObservationResponse()
                    {
                        RequestResult = MsgMeas.RequestResultType.NotFound
                    };
                    SendResponse(errorResponse);

                    continue;
                }

                // Looking at the temporal filter received from the client
                var temporalFilter = requestObj.TemporalFilters[0];
                
                // Retrieving data (e.g., from a database). Filtering with the temporal filter.
                // ...
                
                // Building a response object
                var responseObj = new MsgMeas.GetObservationResponse()
                {
                    RequestResult = MsgMeas.RequestResultType.Ok
                };

                // Setting observation data. Suppose these have been retrieved from a database, for instance.
                var observation1 = new MsgMeas.Observation(new MsgMeas.Item_Measurement("s", 2.2));
                var observation2 = new MsgMeas.Observation(new MsgMeas.Item_Measurement("s", 2.4));
                responseObj.Observations.Add(observation1);
                responseObj.Observations.Add(observation2);

                // Sending the response
                SendResponse(responseObj);
            }
        }

        private byte[] WaitForRequest()
        {
            throw new NotImplementedException();
        }

        private void SendResponse(MsgMeas.GetObservationResponse responseObj)
        {
            // Serialising the object
            byte[] responseXml = responseObj.ToXmlBytes();

            // Sending the response to the client...
            // In real life, you will have to consider where to send.
            // This depends on the protocol and network structure.
            throw new NotImplementedException();
        }
    }
}
