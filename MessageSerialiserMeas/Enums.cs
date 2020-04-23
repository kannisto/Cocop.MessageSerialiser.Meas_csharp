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

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Specifies a task-related operation.
    /// </summary>
    public enum TaskOperationType
    {
        /// <summary>
        /// Submit (i.e., create) a new task.
        /// </summary>
        Submit = 0,
        /// <summary>
        /// Update an existing task.
        /// </summary>
        Update = 1,
        /// <summary>
        /// Get the status of a task.
        /// </summary>
        GetStatus = 2,
        /// <summary>
        /// Cancel a task.
        /// </summary>
        Cancel = 3
    }

    /// <summary>
    /// Represents the result of a request.
    /// </summary>
    public enum RequestResultType
    {
        /// <summary>
        /// The result is unknown. This should only be used to indicate that the result has not been specified.
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// The request has been processed successfully.
        /// </summary>
        Ok = 1,
        
        /// <summary>
        /// The server encountered an internal error while processing the request.
        /// </summary>
        ServerError = 2,

        /// <summary>
        /// The requested resource was not found in the server.
        /// </summary>
        NotFound = 3,

        /// <summary>
        /// The server failed to understand the request.
        /// </summary>
        BadRequest = 4,

        /// <summary>
        /// The requested action cannot be performed due to a conflict.
        /// </summary>
        Conflict = 5
    }

    /// <summary>
    /// Represents the status of a tasking request.
    /// <para>In this module, the code has been derived from OGC(r) Sensor Planning Service Implementation Standard
    /// (OGC 09-000; please see the file "ref_and_license_ogc_sps.txt").</para>
    /// </summary>
    public enum TaskingRequestStatusCodeType
    {
        /// <summary>
        /// Accepted.
        /// </summary>
        Accepted = 0,
        /// <summary>
        /// Pending.
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Rejected.
        /// </summary>
        Rejected = 2
    }

    /// <summary>
    /// Represents the status of a task.
    /// <para>In this module, the code has been derived from OGC(r) Sensor Planning Service Implementation Standard
    /// (OGC 09-000; please see the file "ref_and_license_ogc_sps.txt").</para>
    /// </summary>
    public enum TaskStatusCodeType
    {
        /// <summary>
        /// Unknown. Use this only when the status is truly unknown!
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Cancelled.
        /// </summary>
        Cancelled = 1,
        /// <summary>
        /// Completed.
        /// </summary>
        Completed = 2,
        /// <summary>
        /// Failed.
        /// </summary>
        Failed = 3,
        /// <summary>
        /// In execution.
        /// </summary>
        InExecution = 4,
        /// <summary>
        /// Reserved but not in execution.
        /// </summary>
        Reserved = 5,
        /// <summary>
        /// Task reservation expired.
        /// </summary>
        Expired = 6
    }
}
