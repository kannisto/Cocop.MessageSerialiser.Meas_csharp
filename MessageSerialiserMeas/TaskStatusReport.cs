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
    /// Represents a status report for a task.
    /// <para>In this module, the code has been derived from OGC(r) Sensor Planning Service Implementation Standard
    /// (OGC 09-000; please see the file "ref_and_license_ogc_sps.txt").</para>
    /// </summary>
    public sealed class TaskStatusReport
    {
        private DateTime m_updateTime;
        private DateTime? m_estimatedTimeOfCompletion;


        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="taskId">The ID of the related task.</param>
        /// <param name="procId">The ID of the procedure being executed or asked to be executed.</param>
        public TaskStatusReport(string taskId, string procId)
        {
            // #1 in XML schema: task
            // The schema requires a value for this field.
            TaskId = taskId;

            // #5 in XML schema: procedure
            // The schema requires a value for this field.
            ProcedureId = procId;


            // Setting defaults (in the order the fields appear in the XML schema)

            // #2 in XML schema: estimatedToC
            EstimatedTimeOfCompletion = null;

            // #3 in XML schema: event (not implemented)

            // #4 in XML schema: percentCompletion
            PercentCompletion = null;

            // #6 in XML schema: requestStatus
            // The schema requires a value for this field.
            RequestStatus = TaskingRequestStatusCodeType.Accepted;

            // #7 in XML schema: statusMessage
            StatusMessages = new System.Collections.Generic.List<string>();

            // #8 in XML schema: taskStatus
            TaskStatusCode = TaskStatusCodeType.Unknown;

            // #9 in XML schema: updateTime
            // The schema requires a value for this field.
            UpdateTime = DateTime.Now.ToUniversalTime();

            // #10 in XML schema: alternative (not implemented)

            // #11 in XML schema: taskingParameters
            TaskingParameters = new Item_DataRecord();
        }

        /// <summary>
        /// Constructor. Use this to deserialise the object from XML.
        /// </summary>
        /// <param name="xmlBytes">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown in an error is encountered.</exception>
        public TaskStatusReport(byte[] xmlBytes)
        {
            XsdNs.StatusReportType proxy = null;

            try
            {
                // Deserialising
                proxy = (XsdNs.StatusReportType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.StatusReportType), xmlBytes);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("Failed to deserialise TaskStatusReport from XML", e);
            }

            // Reading values from XML
            ReadFieldValuesFromXmlDoc(proxy);
        }

        /// <summary>
        /// Constructor. Use this to populate the object from a proxy.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown in an error is encountered.</exception>
        internal TaskStatusReport(XsdNs.StatusReportType proxy)
        {
            // Reading values from XML
            ReadFieldValuesFromXmlDoc(proxy);
        }
        
        #endregion Constructors


        #region Properties

        /// <summary>
        /// The identifier of the task.
        /// </summary>
        public string TaskId
        {
            // #1 in XML schema: task
            get;
            private set;
        }

        /// <summary>
        /// The estimated time when the task will be completed (optional). The default is null (this is a nullable value).
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">When setting the value, thrown if DateTime kind is not UTC.</exception>
        public DateTime? EstimatedTimeOfCompletion
        {
            // #2 in XML schema: estimatedToC
            get
            {
                return m_estimatedTimeOfCompletion;
            }
            set
            {
                if (value != null && value.HasValue)
                {
                    XNeut.Helper.ExpectDateTimeIsUtc(value.Value); // throws DateTimeException
                }

                m_estimatedTimeOfCompletion = value;
            }
        }

        /// <summary>
        /// The progress made in task completion; 0..100 (optional). The default is null (this is a nullable value).
        /// </summary>
        public double? PercentCompletion
        {
            // #4 in XML schema: percentCompletion
            get;
            set;
        }

        /// <summary>
        /// The identifier of the procedure.
        /// </summary>
        public string ProcedureId
        {
            // #5 in XML schema: procedure
            get;
            private set;
        }

        /// <summary>
        /// Request status. The default is "accepted".
        /// </summary>
        public TaskingRequestStatusCodeType RequestStatus
        {
            // #6 in XML schema: requestStatus
            get;
            set;
        }

        /// <summary>
        /// Any status messages from the server (optional).
        /// </summary>
        public System.Collections.Generic.List<string> StatusMessages
        {
            // #7 in XML schema: statusMessage
            get;
            set;
        }

        /// <summary>
        /// Status code.
        /// </summary>
        public TaskStatusCodeType TaskStatusCode
        {
            // #8 in XML schema: taskStatus
            get;
            set;
        }

        /// <summary>
        /// The time when the task reached the current state. Mandatory;
        /// if not assigned, this will be the creation time of the message.
        /// </summary>
        /// <exception cref="XNeut.DateTimeException">Thrown if trying to set a value where kind is not UTC.</exception>
        public DateTime UpdateTime
        {
            // #9 in XML schema: updateTime
            get
            {
                return m_updateTime;
            }
            set
            {
                XNeut.Helper.ExpectDateTimeIsUtc(value);
                m_updateTime = value;
            }
        }

        /// <summary>
        /// Tasking parameters.
        /// </summary>
        public Item_DataRecord TaskingParameters
        {
            // #11 in XML schema: taskingParameters
            get;
            set;
        }

        #endregion Properties


        #region Public or internal methods

        /// <summary>
        /// Serialises the object to XML.
        /// </summary>
        /// <returns>Object as XML.</returns>
        public byte[] ToXmlBytes()
        {
            var proxy = ToXmlProxy(""); // Empty ID prefix because the status report is the root element in the doc

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

        /// <summary>
        /// Creates an XML proxy from the object.
        /// </summary>
        /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
        /// require an ID. For certain XML elements, the schema requires an ID that is unique within
        /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
        /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
        /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
        /// <returns>Proxy.</returns>
        public XsdNs.StatusReportType ToXmlProxy(string idPrefix)
        {
            var proxy = new XsdNs.StatusReportType()
            {
                // #1 in XML schema: task
                task = TaskId,

                // #3 in XML schema: event (not implemented)

                // #5 in XML schema: procedure
                procedure = ProcedureId,

                // #6 in XML schema: requestStatus
                requestStatus = RequestStatus.ToString(),

                // #9 in XML schema: updateTime
                updateTime = UpdateTime

                // #10 in XML schema: alternative (not implemented)
            };

            // #2 in XML schema: estimatedToC
            // Assign if defined
            if (EstimatedTimeOfCompletion.HasValue)
            {
                // UTC not expected because already supposedly checked in the setter, but check just in case
                XNeut.Helper.ExpectDateTimeIsUtc(EstimatedTimeOfCompletion.Value); // throws DateTimeException
                proxy.estimatedToC = EstimatedTimeOfCompletion.Value;
            }
            proxy.estimatedToCSpecified = EstimatedTimeOfCompletion.HasValue;

            // #4 in XML schema: percentCompletion
            if (PercentCompletion.HasValue)
            {
                proxy.percentCompletion = PercentCompletion.Value;
            }
            proxy.percentCompletionSpecified = PercentCompletion.HasValue;

            // #7 in XML schema: statusMessage
            if (StatusMessages != null && StatusMessages.Count > 0)
            {
                proxy.statusMessage = new XsdNs.LanguageStringType[StatusMessages.Count];

                for (int a = 0; a < StatusMessages.Count; ++a)
                {
                    proxy.statusMessage[a] = new XsdNs.LanguageStringType
                    {
                        Value = StatusMessages[a]
                    };
                }
            }

            // Assign status code only if known
            // #8 in XML schema: taskStatus
            if (TaskStatusCode != TaskStatusCodeType.Unknown)
            {
                proxy.taskStatus = TaskStatusCode.ToString();
            }

            // Assign tasking parameters if defined
            // #11 in XML schema: taskingParameters
            if (TaskingParameters.ItemNames.Count > 0)
            {
                var paramsProc = new TaskingParameterProcessor(TaskingParameters);

                proxy.taskingParameters = new XsdNs.ParameterDataPropertyType()
                {
                    ParameterData = paramsProc.ToXmlProxy(idPrefix + "TaskPar-")
                };
            }

            return proxy;
        }

        #endregion Public or internal methods


        #region Private methods

        private void ReadFieldValuesFromXmlDoc(XsdNs.StatusReportType proxy)
        {
            try
            {
                // #1 in XML schema: task
                // The schema requires a value for this field.
                TaskId = proxy.task;

                // #2 in XML schema: estimatedToC
                if (proxy.estimatedToCSpecified)
                {
                    EstimatedTimeOfCompletion = XNeut.Helper.DateTimeToUtcIfPossible(proxy.estimatedToC);
                }
                else
                {
                    EstimatedTimeOfCompletion = null;
                }

                // #3 in XML schema: event (not implemented)

                // #4 in XML schema: percentCompletion
                if (proxy.percentCompletionSpecified)
                {
                    PercentCompletion = proxy.percentCompletion;
                }
                else
                {
                    PercentCompletion = null;
                }

                // #5 in XML schema: procedure
                // The schema requires a value for this field.
                ProcedureId = proxy.procedure;

                // #6 in XML schema: requestStatus
                // The schema requires a value for this field.
                try
                {
                    RequestStatus = (TaskingRequestStatusCodeType)Enum.Parse(typeof(TaskingRequestStatusCodeType), proxy.requestStatus);
                }
                catch (ArgumentException e)
                {
                    throw new XNeut.InvalidMessageException("Failed to parse request status", e);
                }

                // #7 in XML schema: statusMessage
                StatusMessages = new System.Collections.Generic.List<string>();

                if (proxy.statusMessage != null)
                {
                    foreach (var s in proxy.statusMessage)
                    {
                        StatusMessages.Add(s.Value.Trim());
                    }
                }

                // #8 in XML schema: taskStatus
                try
                {
                    if (!string.IsNullOrEmpty(proxy.taskStatus))
                    {
                        TaskStatusCode = (TaskStatusCodeType)Enum.Parse(typeof(TaskStatusCodeType), proxy.taskStatus);
                    }
                    else
                    {
                        // Default
                        TaskStatusCode = TaskStatusCodeType.Unknown;
                    }
                }
                catch (ArgumentException e)
                {
                    throw new XNeut.InvalidMessageException("Failed to parse task status", e);
                }

                // #9 in XML schema: updateTime
                // The schema requires a value for this field.
                UpdateTime = XNeut.Helper.DateTimeToUtcIfPossible(proxy.updateTime);

                // #10 in XML schema: alternative (not implemented)

                // Tasking parameters defined?
                // #11 in XML schema: taskingParameters
                if (proxy.taskingParameters != null &&
                    proxy.taskingParameters.ParameterData != null)
                {
                    var paramsProxy = proxy.taskingParameters.ParameterData;
                    TaskingParameters = new TaskingParameterProcessor(paramsProxy).Parameters;
                }
                else
                {
                    TaskingParameters = new Item_DataRecord();
                }
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read status report from XML (something required missing?)", e);
            }
        }
        
        #endregion Private methods
    }
}
