//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 3/2019
// Last modified: 2/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Meas;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace TaskResponseTest
{
    [TestClass]
    public class TestGetStatus
    {
        [TestMethod]
        public void TaskGetStatusResp_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskGetStatusResponse.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.GetStatus, testObject.Operation);

            // Expecting 2 status reports
            Assert.AreEqual(2, testObject.StatusReports.Count);

            // Only asserting one field from status report because it is tested throughly elsewhere
            Assert.AreEqual(TaskStatusCodeType.Reserved, testObject.StatusReports[0].TaskStatusCode);

            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.Ok, testObject.RequestResult);
        }

        [TestMethod]
        public void TaskGetStatusResp_Read_NoStatusReports()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskGetStatusResponse_NoStatusReports.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.GetStatus, testObject.Operation);

            // Expecting no status reports
            Assert.AreEqual(0, testObject.StatusReports.Count);
            
            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.NotFound, testObject.RequestResult);
        }

        [TestMethod]
        public void TaskGetStatusResp_Read_NoExtension()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskGetStatusResponse_NoExtension.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.GetStatus, testObject.Operation);

            // Expecting 1 status report
            Assert.AreEqual(1, testObject.StatusReports.Count);

            // Only asserting one field from status report because it is tested throughly elsewhere
            Assert.AreEqual("mytask", testObject.StatusReports[0].TaskId);

            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.Unknown, testObject.RequestResult);
        }

        [TestMethod]
        public void TaskGetStatusResp_Create()
        {
            var statusReport = new TaskStatusReport(taskId: "mytask", procId: "myprocedure")
            {
                RequestStatus = TaskingRequestStatusCodeType.Pending
            };

            var statusReports = new System.Collections.Generic.List<TaskStatusReport>() { statusReport };
            var testObject = TaskResponse.CreateGetStatusResponse(statusReports);
            testObject.RequestResult = RequestResultType.NotFound;

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new TaskResponse(xmlBytes);

            // Using a separate assert function to make sure the deserialised object is asserted
            TaskGetStatusResp_Create_Assert(testObjectIn);
        }

        private void TaskGetStatusResp_Create_Assert(TaskResponse testObjectIn)
        {
            Assert.AreEqual(TaskOperationType.GetStatus, testObjectIn.Operation);

            // Only asserting one field from status report because it is tested throughly elsewhere
            Assert.AreEqual(1, testObjectIn.StatusReports.Count);
            Assert.AreEqual(TaskingRequestStatusCodeType.Pending, testObjectIn.StatusReports[0].RequestStatus);

            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.NotFound, testObjectIn.RequestResult);
        }

        private byte[] ReadFile(string filepath)
        {
            return System.IO.File.ReadAllBytes(filepath);
        }

        private void Validate(byte[] xmlBytes)
        {
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Sps_GmlSwe);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.GetStatusResponseType));
            }
        }
    }
}
