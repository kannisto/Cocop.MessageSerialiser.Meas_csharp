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
    /// Represents a task-related request.
    /// <para>In this module, the code has been derived from OGC(r) Sensor Planning Service Implementation Standard
    /// (OGC 09-000; please see the file "ref_and_license_ogc_sps.txt").</para>
    /// </summary>
    public sealed class TaskRequest
    {
        /*
        Relevant fields for each operation type (each field with 'x' is required):
            
                          Submit  Update  GetStatus Cancel
        taskingParameters      x       x
        procedure              x       x
        targetTask/task                x          x      x
        */

        
        private const string PROXY_SERVICE = "SPS";
        private const string PROXY_VERSION = "2.0.0";


        #region Functions for read

        /// <summary>
        /// Constructor. Use this to deserialise a message from XML.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        public TaskRequest(byte[] xmlBytes)
        {
            string rootElName;

            try
            {
                // Mapping the names of root elements and message classes
                rootElName = XNeut.Helper.GetRootElNameFromXmlDoc(xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Fail to parse task-related request", e);
            }

            switch (rootElName)
            {
                case "Submit":
                    Operation = TaskOperationType.Submit;
                    ReadSubmitXml(xmlBytes);
                    break;
                case "Update":
                    Operation = TaskOperationType.Update;
                    ReadUpdateXml(xmlBytes);
                    break;
                case "Cancel":
                    Operation = TaskOperationType.Cancel;
                    ReadCancelXml(xmlBytes);
                    break;
                case "GetStatus":
                    Operation = TaskOperationType.GetStatus;
                    ReadGetStatusXml(xmlBytes);
                    break;
                default:
                    throw new XNeut.InvalidMessageException("Unexpected root element " + rootElName);
            }
        }

        private void ReadCancelXml(byte[] xmlBytes)
        {
            try
            {
                // Deserialising
                var proxy = (XsdNs.CancelType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.CancelType), xmlBytes);
                TaskId = proxy.task;
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise cancel request", e);
            }
        }

        private void ReadGetStatusXml(byte[] xmlBytes)
        {
            try
            {
                // Deserialising
                var proxy = (XsdNs.GetStatusType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.GetStatusType), xmlBytes);
                TaskId = proxy.task;
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise get status request", e);
            }
        }

        private void ReadSubmitXml(byte[] xmlBytes)
        {
            XsdNs.SubmitType proxy = null;

            try
            {
                // Deserialising
                proxy = (XsdNs.SubmitType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.SubmitType), xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise submit task request from XML", e);
            }

            ReadTaskingRequestItemsFromProxy(proxy);
            
            // Nothing else to read here
        }

        private void ReadUpdateXml(byte[] xmlBytes)
        {
            XsdNs.UpdateType proxy = null;

            try
            {
                // Deserialising
                proxy = (XsdNs.UpdateType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.UpdateType), xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise update task request from XML", e);
            }

            ReadTaskingRequestItemsFromProxy(proxy);

            // Assigning the fields specific to this subclass
            TaskId = proxy.targetTask;
        }

        private void ReadTaskingRequestItemsFromProxy(XsdNs.TaskingRequestType proxy)
        {
            // Setting defaults
            Parameters = new Item_DataRecord();

            // Reading procedure
            ProcedureId = proxy.procedure;

            // Parameters defined in the proxy?
            if (proxy.taskingParameters != null &&
                proxy.taskingParameters.ParameterData != null)
            {
                Parameters = new TaskingParameterProcessor(proxy.taskingParameters.ParameterData).Parameters;
            }
        }

        #endregion Functions for read


        #region Functions for create

        private TaskRequest(TaskOperationType operation)
        {
            Operation = operation;
        }

        /// <summary>
        /// Creates a request to submit (create) a new task.
        /// </summary>
        /// <param name="procId">The procedure to be executed.</param>
        /// <returns>Submit request.</returns>
        public static TaskRequest CreateSubmitRequest(string procId)
        {
            return new TaskRequest(TaskOperationType.Submit)
            {
                ProcedureId = procId,
                Parameters = new Item_DataRecord()
            };
        }

        /// <summary>
        /// Creates a request to update an existing task.
        /// </summary>
        /// <param name="taskId">Task ID.</param>
        /// <param name="procId">The procedure to be executed.</param>
        /// <returns>Update request.</returns>
        public static TaskRequest CreateUpdateRequest(string taskId, string procId)
        {
            return new TaskRequest(TaskOperationType.Update)
            {
                ProcedureId = procId,
                TaskId = taskId,
                Parameters = new Item_DataRecord()
            };
        }
        
        /// <summary>
        /// Creates a request to get the status of a task.
        /// </summary>
        /// <param name="taskId">Task ID.</param>
        /// <returns>GetStatus request.</returns>
        public static TaskRequest CreateGetStatusRequest(string taskId)
        {
            return new TaskRequest(TaskOperationType.GetStatus)
            {
                TaskId = taskId
            };
        }

        /// <summary>
        /// Creates a request to cancel a task.
        /// </summary>
        /// <param name="taskId">Task ID.</param>
        /// <returns>Cancel request.</returns>
        public static TaskRequest CreateCancelRequest(string taskId)
        {
            return new TaskRequest(TaskOperationType.Cancel)
            {
                TaskId = taskId
            };
        }

        #endregion Functions for create
        

        /// <summary>
        /// The requested operation.
        /// </summary>
        public TaskOperationType Operation
        {
            get;
            private set;
        }

        /// <summary>
        /// The URI (or identifier) of the procedure to be executed. Only supported when the action is either "submit" or "update".
        /// </summary>
        public string ProcedureId
        {
            get;
            private set;
        }

        /// <summary>
        /// The identifier of the related task. Not supported when the action is "submit", because submit creates a new task.
        /// </summary>
        public string TaskId
        {
            get;
            private set;
        }

        /// <summary>
        /// Tasking parameters. Only supported when the operation is either "submit" or "update".
        /// </summary>
        public Item_DataRecord Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns></returns>
        public byte[] ToXmlBytes()
        {
            // It is an error if parameters are defined for Cancel or GetStatus request

            object proxy = null;

            switch (Operation)
            {
                case TaskOperationType.Submit:

                    // Creating the proxy object
                    var proxySubmit = new XsdNs.SubmitType();

                    // Nothing to add to proxy here!
                    
                    // Adding the fields common with "update"
                    PopulateTaskingRequestProxy(proxySubmit);
                    proxy = proxySubmit;
                    break;

                case TaskOperationType.Update:

                    // Creating the proxy object
                    var proxyUpdate = new XsdNs.UpdateType()
                    {
                        targetTask = TaskId
                    };

                    // Adding the fields common with "submit"
                    PopulateTaskingRequestProxy(proxyUpdate);
                    proxy = proxyUpdate;
                    break;

                case TaskOperationType.GetStatus:

                    proxy = CreateGetStatusProxy();
                    break;
                    
                case TaskOperationType.Cancel:

                    proxy = CreateCancelProxy();
                    break;
                    
                default:
                    throw new NotSupportedException("Unsupported task operation " + Operation.ToString());
            }

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

        private object CreateGetStatusProxy()
        {
            // It is an error if parameters are defined for Cancel or GetStatus request
            if (Parameters != null && Parameters.ItemNames.Count > 0)
            {
                throw new ArgumentException("Parameters are not supported in get task status request");
            }

            return new XsdNs.GetStatusType()
            {
                // Assigning mandatory attributes specified in the SWES standard.
                // These values are after the SPS specification.
                service = PROXY_SERVICE,
                version = PROXY_VERSION,
                task = TaskId
            };
        }

        private XsdNs.CancelType CreateCancelProxy()
        {
            // It is an error if parameters are defined for Cancel or GetStatus request
            if (Parameters != null && Parameters.ItemNames.Count > 0)
            {
                throw new ArgumentException("Parameters are not supported in cancel task request");
            }

            return new XsdNs.CancelType()
            {
                // Assigning mandatory attributes specified in the SWES standard.
                // These values are after the SPS specification.
                service = PROXY_SERVICE,
                version = PROXY_VERSION,
                task = TaskId
            };
        }

        private void PopulateTaskingRequestProxy(XsdNs.TaskingRequestType proxy)
        {
            // Assigning to fields that are common to both request types

            // Assigning mandatory attributes specified in the SWES standard.
            // These values are after the SPS specification.
            proxy.service = PROXY_SERVICE;
            proxy.version = PROXY_VERSION;

            proxy.procedure = ProcedureId;

            // According to the SWE schema, a data record should not be empty
            // -> omitting it if no parameters specified
            if (Parameters.ItemNames.Count > 0)
            {
                var serialisableParams = new TaskingParameterProcessor(Parameters);

                proxy.taskingParameters = new XsdNs.TaskingRequestTypeTaskingParameters()
                {
                    ParameterData = serialisableParams.ToXmlProxy("TaskReqParams-")
                };
            }
        }
    }
}
