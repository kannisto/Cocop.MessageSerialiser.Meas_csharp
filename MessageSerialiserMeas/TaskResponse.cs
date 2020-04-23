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
using System.Collections.ObjectModel;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents a task-related response.
    /// <para>In this module, the code has been derived from OGC(r) Sensor Planning Service Implementation Standard
    /// (OGC 09-000; please see the file "ref_and_license_ogc_sps.txt").</para>
    /// </summary>
    public sealed class TaskResponse : ExtensibleResponse
    {
        /*
        Relevant fields for each action type (1: exactly one, *: many):
            
                        Submit  Update  GetStatus   Cancel
        StatusReport    1       1       *           1	
        targetTask      1

        "targetTask" is redundant (?) with "taskId" in StatusReport, but the
        XML schema requires it.
        */


        #region Functions for read

        /// <summary>
        /// Constructor. Use this to deserialise the object from an XML document.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        public TaskResponse(byte[] xmlBytes)
            : base()
        {
            try
            {
                XsdNs.ExtensibleResponseType proxy = null;
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
                    case "SubmitResponse":
                        proxy = ReadSubmitXml(xmlBytes);
                        Operation = TaskOperationType.Submit;
                        break;
                    case "UpdateResponse":
                        proxy = ReadUpdateXml(xmlBytes);
                        Operation = TaskOperationType.Update;
                        break;
                    case "GetStatusResponse":
                        proxy = ReadGetStatusXml(xmlBytes);
                        Operation = TaskOperationType.GetStatus;
                        break;
                    case "CancelResponse":
                        proxy = ReadCancelXml(xmlBytes);
                        Operation = TaskOperationType.Cancel;
                        break;
                    default:
                        throw new XNeut.InvalidMessageException("Unexpected root element in task response: " + rootElName);
                }

                // Reading the data of ExtensibleResponse
                ReadExtensibleResponseItemsFromProxy(proxy);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Something expected missing from task response", e);
            }
        }

        private XsdNs.GetStatusResponseType ReadGetStatusXml(byte[] xmlBytes)
        {
            var proxy = (XsdNs.GetStatusResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.GetStatusResponseType), xmlBytes);

            // Reading status reports
            var statusReportsTemp = new List<TaskStatusReport>();

            if (proxy.status != null)
            {
                foreach (var s in proxy.status)
                {
                    var currentStatRep = new TaskStatusReport(s.StatusReport);
                    statusReportsTemp.Add(currentStatRep);
                }
            }

            StatusReports = new ReadOnlyCollection<TaskStatusReport>(statusReportsTemp);

            return proxy;
        }

        private XsdNs.CancelResponseType ReadCancelXml(byte[] xmlBytes)
        {
            var proxy = (XsdNs.CancelResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.CancelResponseType), xmlBytes);

            // Reading status report
            var oneStatusReport = new TaskStatusReport(proxy.result.StatusReport);
            AssignSingleStatusReport(oneStatusReport);

            return proxy;
        }

        private XsdNs.SubmitResponseType ReadSubmitXml(byte[] xmlBytes)
        {
            var proxy = (XsdNs.SubmitResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.SubmitResponseType), xmlBytes);

            // Reading status report
            var oneStatusReport = new TaskStatusReport(proxy.result.StatusReport);
            AssignSingleStatusReport(oneStatusReport);

            return proxy;
        }

        private XsdNs.UpdateResponseType ReadUpdateXml(byte[] xmlBytes)
        {
            var proxy = (XsdNs.UpdateResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.UpdateResponseType), xmlBytes);

            // Reading status report
            var oneStatusReport = new TaskStatusReport(proxy.result.StatusReport);
            AssignSingleStatusReport(oneStatusReport);

            // Intentionally, not processing the "targetTask" field because it is 
            // almost redundant to the task field of TaskStatusReport. However, the
            // targetTask field is obligatory for an update response per the XML schema,
            // so it must be assigned in XML document creation.

            return proxy;
        }

        private void AssignSingleStatusReport(TaskStatusReport oneStatusReport)
        {
            StatusReports = new ReadOnlyCollection<TaskStatusReport>(
                new List<TaskStatusReport>()
                {
                    oneStatusReport
                }
            );
        }

        #endregion Functions for read


        #region Functions for create

        private TaskResponse(TaskOperationType operation, List<TaskStatusReport> statRep)
            : base()
        {
            Operation = operation;
            StatusReports = new ReadOnlyCollection<TaskStatusReport>(statRep);
        }

        private TaskResponse(TaskOperationType operation, TaskStatusReport statRep)
            : this(operation, new List<TaskStatusReport>() { statRep })
        {
            // Empty ctor body (another ctor called)
        }

        /// <summary>
        /// Creates a response to a submit request.
        /// </summary>
        /// <param name="statRep">Status report.</param>
        /// <returns>Response object.</returns>
        public static TaskResponse CreateSubmitResponse(TaskStatusReport statRep)
        {
            return new TaskResponse(TaskOperationType.Submit, statRep);
        }

        /// <summary>
        /// Creates a response to an update request.
        /// </summary>
        /// <param name="statRep">Status report.</param>
        /// <returns>Response object.</returns>
        public static TaskResponse CreateUpdateResponse(TaskStatusReport statRep)
        {
            return new TaskResponse(TaskOperationType.Update, statRep);
        }

        /// <summary>
        /// Creates a response to a GetStatus request.
        /// </summary>
        /// <param name="statRep">Status report.</param>
        /// <returns>Response object.</returns>
        public static TaskResponse CreateGetStatusResponse(List<TaskStatusReport> statRep)
        {
            return new TaskResponse(TaskOperationType.GetStatus, statRep);
        }

        /// <summary>
        /// Creates a response to a cancel request.
        /// </summary>
        /// <param name="statRep">Status report.</param>
        /// <returns>Response object.</returns>
        public static TaskResponse CreateCancelResponse(TaskStatusReport statRep)
        {
            return new TaskResponse(TaskOperationType.Cancel, statRep);
        }

        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns>Object in XML.</returns>
        public byte[] ToXmlBytes()
        {
            // There must be exactly 1 status report if the operation is not GetStatus
            if (Operation != TaskOperationType.GetStatus && StatusReports.Count != 1)
            {
                throw new ArgumentException("Expected exactly one status report for response to " + Operation.ToString());
            }

            XsdNs.ExtensibleResponseType proxy = null;

            switch (Operation)
            {
                case TaskOperationType.Submit:
                    proxy = GetSubmitResponseProxy();
                    break;

                case TaskOperationType.Update:
                    proxy = GetUpdateResponseProxy();
                    break;

                case TaskOperationType.GetStatus:
                    proxy = GetGetStatusResponseProxy();
                    break;
                    
                case TaskOperationType.Cancel:
                    proxy = GetCancelResponseProxy();
                    break;

                default:
                    throw new NotSupportedException("Unsupported task operation " + Operation.ToString());
            }

            // Populating the items of ExtensibleResponse
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

        private XsdNs.GetStatusResponseType GetGetStatusResponseProxy()
        {
            // Creating a proxy to be serialised
            var proxy = new XsdNs.GetStatusResponseType()
            {
                status = new XsdNs.GetStatusResponseTypeStatus[StatusReports.Count]
            };
            
            for (int a = 0; a < StatusReports.Count; ++a)
            {
                var currentReport = StatusReports[a];
                var IdPrefix = string.Format("StatRep{0}-", (a+1));

                proxy.status[a] = new XsdNs.GetStatusResponseTypeStatus()
                {
                    StatusReport = currentReport.ToXmlProxy(IdPrefix)
                };
            }

            return proxy;
        }

        private XsdNs.SubmitResponseType GetSubmitResponseProxy()
        {
            var proxySubmit = new XsdNs.SubmitResponseType();

            // Adding the fields common with "update"
            PopulateTaskingResponseProxy(proxySubmit);

            return proxySubmit;
        }

        private XsdNs.UpdateResponseType GetUpdateResponseProxy()
        {
            // Creating the proxy object
            var proxyUpdate = new XsdNs.UpdateResponseType()
            {
                // This targetTask field is required in the XML schema although it
                // seems redundant
                targetTask = StatusReports[0].TaskId
            };

            // Adding the fields common with "submit"
            PopulateTaskingResponseProxy(proxyUpdate);

            return proxyUpdate;
        }

        private XsdNs.CancelResponseType GetCancelResponseProxy()
        {
            var statusRep = StatusReports[0];

            return new XsdNs.CancelResponseType()
            {
                result = new XsdNs.CancelResponseTypeResult()
                {
                    StatusReport = statusRep.ToXmlProxy("CancelResp-")
                }
            };
        }

        private void PopulateTaskingResponseProxy(XsdNs.TaskingResponseType proxy)
        {
            // Intentionally, not processing the "targetTask" field because it is almost redundant to
            // the task field of TaskStatusReport
            
            // Adding status report
            var statusRep = StatusReports[0];
            proxy.result = new XsdNs.TaskingResponseTypeResult()
            {
                StatusReport = statusRep.ToXmlProxy("TaskingResp-")
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
        /// Status reports. For "GetStatus" responses, this can be many, whereas other
        /// request types can only hold one. A "GetStatus" response can return all
        /// status values of the task to show its progress thoroughly.
        /// </summary>
        public ReadOnlyCollection<TaskStatusReport> StatusReports
        {
            get;
            private set;
        }
    }
}
