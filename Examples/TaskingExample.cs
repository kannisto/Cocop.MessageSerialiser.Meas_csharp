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
using SysColl = System.Collections.Generic;
using MsgMeas = Cocop.MessageSerialiser.Meas;

namespace Examples
{
    // This example shows how to implement the communication of a tasking client
    // and a tasking server.

    class TaskingClient
    {
        // This idenfities the supported procedure. There could be multiple of these.
        private const string MySupportedProcedure = "temperature-measurement";
        private string m_currentTaskId = "";

        
        // Send a request to the tasking server
        public void SendRequest(MsgMeas.TaskOperationType operation)
        {
            MsgMeas.TaskRequest request = null;

            switch (operation)
            {
                case MsgMeas.TaskOperationType.Submit: // Create a new task for "myproc"
                    request = MsgMeas.TaskRequest.CreateSubmitRequest(MySupportedProcedure);
                    break;
                case MsgMeas.TaskOperationType.Update: // Update an existing task
                    request = MsgMeas.TaskRequest.CreateUpdateRequest(MySupportedProcedure, m_currentTaskId);
                    break;
                case MsgMeas.TaskOperationType.GetStatus: // Get the status of an existing task
                    request = MsgMeas.TaskRequest.CreateGetStatusRequest(m_currentTaskId);
                    break;
                case MsgMeas.TaskOperationType.Cancel: // Cancel a task
                    request = MsgMeas.TaskRequest.CreateCancelRequest(m_currentTaskId);
                    break;
                default:
                    throw new ArgumentException("Unexpected operation " + operation.ToString());
            }

            // Sending the request
            SendRequest(request.ToXmlBytes());

            // Waiting for a response
            var responseXml = WaitForResponse();

            MsgMeas.TaskResponse response;
            try
            {
                response = new MsgMeas.TaskResponse(responseXml);
            }
            catch (MsgMeas.Neutral.InvalidMessageException e)
            {
                throw new InvalidOperationException("Failed to read response: " + e.Message);
            }
            
            if (response.RequestResult != MsgMeas.RequestResultType.Ok)
            {
                // Request failed!
                throw new InvalidOperationException("Unexpected request result " + response.RequestResult.ToString());
            }

            // Processing the request
            m_currentTaskId = response.StatusReports[0].TaskId; // Task ID is assigned by the server

            // Do whatever you want with the response...
        }

        // Listen to status updates from the server
        public void ListenToStatusUpdates()
        {
            // If your communication protocol is capable of publish-subscribe communication
            // (such as AMQP or MQTT), you can listen to status information instead of
            // requiring the client to request for status repeatedly.

            var messageXml = WaitForStatus();
            var statusReport = new MsgMeas.TaskStatusReport(messageXml);

            var statusToPrint = string.Format("Task {0}: {1}", statusReport.TaskId, statusReport.TaskStatusCode.ToString());
            Console.WriteLine(statusToPrint);
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

        private byte[] WaitForStatus()
        {
            throw new NotImplementedException();
        }
    }

    class TaskingServer
    {
        // This idenfities the procedure supported by this server.
        // There could be multiple of these.
        private const string MySupportedProcedure = "temperature-measurement";
        
        
        // Serve tasking clients
        public void Serve()
        {
            while (true)
            {
                var xmlBytes = ReceiveFromNetwork();
                MsgMeas.TaskRequest request;
                try
                {
                    request = new MsgMeas.TaskRequest(xmlBytes);
                }
                catch (MsgMeas.Neutral.InvalidMessageException e)
                {
                    Console.WriteLine("Failed to read request: " + e.Message);
                    continue;
                }

                MsgMeas.TaskResponse response = null;

                try
                {
                    if (request.Operation != MsgMeas.TaskOperationType.Submit &&
                        request.TaskId != CurrentTaskId)
                    {
                        // Unknown task ID
                        response = CreateFailureResponse(request.Operation);
                    }
                    else if ((request.Operation == MsgMeas.TaskOperationType.Submit ||
                        request.Operation == MsgMeas.TaskOperationType.Update) &&
                        request.ProcedureId != MySupportedProcedure)
                    {
                        // Cannot start a task or update an existing task, because the procedure is unknown.
                        response = CreateFailureResponse(request.Operation);
                    }
                    else
                    {
                        response = CreateOkResponse(request.Operation);
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Failed to process request: " + e.Message);
                    continue;
                }
                
                SendToNetwork(response.ToXmlBytes());
            }
        }

        // Notify tasking clients about task status
        public void NotifyStatus()
        {
            // If your communication protocol is capable of publish-subscribe communication
            // (such as AMQP or MQTT), you can send status information regularly instead of
            // requiring the client to request for status.

            // Get status and send
            var statusReport = GetStatusReport();
            byte[] xmlBytes = statusReport.ToXmlBytes();
            SendToNetwork(xmlBytes);
        }


        private MsgMeas.TaskResponse CreateFailureResponse(MsgMeas.TaskOperationType operation)
        {
            var statusReport = new MsgMeas.TaskStatusReport("?", "?");
            MsgMeas.TaskResponse retval = null;

            // Creating a response object
            switch (operation)
            {
                case MsgMeas.TaskOperationType.Submit:
                    retval = MsgMeas.TaskResponse.CreateSubmitResponse(statusReport);
                    break;

                case MsgMeas.TaskOperationType.Update:
                    retval = MsgMeas.TaskResponse.CreateUpdateResponse(statusReport);
                    break;

                case MsgMeas.TaskOperationType.GetStatus:
                    retval = MsgMeas.TaskResponse.CreateGetStatusResponse(
                        new SysColl.List<MsgMeas.TaskStatusReport> { statusReport }
                        );
                    break;

                case MsgMeas.TaskOperationType.Cancel:
                    retval = MsgMeas.TaskResponse.CreateCancelResponse(statusReport);
                    break;

                default:
                    throw new InvalidOperationException("Unexpected operation " + operation.ToString());
            }

            // This indicates the server did not understand the request
            retval.RequestResult = MsgMeas.RequestResultType.BadRequest;
            return retval;
        }

        private MsgMeas.TaskResponse CreateOkResponse(MsgMeas.TaskOperationType operation)
        {
            // Creating a response object
            switch (operation)
            {
                case MsgMeas.TaskOperationType.Submit:
                    // Processing a create request...
                    // ...
                    CurrentTaskStatus = MsgMeas.TaskStatusCodeType.InExecution;
                    CurrentTaskId = Guid.NewGuid().ToString();
                    return MsgMeas.TaskResponse.CreateSubmitResponse(GetStatusReport());

                case MsgMeas.TaskOperationType.Update:
                    // Processing an update request...
                    // ...
                    return MsgMeas.TaskResponse.CreateUpdateResponse(GetStatusReport());

                case MsgMeas.TaskOperationType.GetStatus:
                    // Processing a get status request...
                    // ...
                    return MsgMeas.TaskResponse.CreateGetStatusResponse(
                        new SysColl.List<MsgMeas.TaskStatusReport>()
                        { GetStatusReport() }
                        );

                case MsgMeas.TaskOperationType.Cancel:
                    // Processing a cancel request...
                    // ...
                    CurrentTaskStatus = MsgMeas.TaskStatusCodeType.Cancelled;
                    return MsgMeas.TaskResponse.CreateCancelResponse(GetStatusReport());

                default:
                    throw new InvalidOperationException("Unexpected operation " + operation.ToString());
            }
        }

        private MsgMeas.TaskStatusReport GetStatusReport()
        {
            var taskingParams = new MsgMeas.Item_DataRecord
            {
                { "myparam", new MsgMeas.Item_Category("myctg") }
            };

            var statusReport = new MsgMeas.TaskStatusReport(CurrentTaskId, MySupportedProcedure)
            {
                EstimatedTimeOfCompletion = DateTime.Now.AddMinutes(4).ToUniversalTime(),
                PercentCompletion = 3.2,
                RequestStatus = MsgMeas.TaskingRequestStatusCodeType.Accepted,
                StatusMessages = new SysColl.List<string>()
                {
                    "Creation was successful", "Start was successful"
                },
                
                TaskStatusCode = CurrentTaskStatus,
                UpdateTime = DateTime.Now.ToUniversalTime(),
                TaskingParameters = taskingParams
            };

            return statusReport;
        }
        
        private byte[] ReceiveFromNetwork()
        {
            throw new NotImplementedException();
        }

        private void SendToNetwork(byte[] xmlBytes)
        {
            // In real life, you will have to consider where to send.
            // This depends on the protocol and network structure.
            throw new NotImplementedException();
        }

        private MsgMeas.TaskStatusCodeType CurrentTaskStatus
        {
            // This status code should indicate task status: 
            // - completed
            // - in execution
            // - cancelled
            // - ...
            //
            // In a real server, this property likely requires thread synchronisation, i.e.,
            // cache flush and mutual exclusion. Depending on the case, the "lock" statement
            // can be sufficient. Not implemented here!
            //
            // Also, consider that there could be multiple tasks run in parallel or at least
            // some tasks queued.
            get;
            set;
        }

        private string CurrentTaskId
        {
            // Your server may have to support multiple tasks.
            // In such situations, a single variable for the
            // task ID can be insufficient.
            get;
            set;
        }
    }
}
