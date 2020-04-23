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
    public class TestCancel
    {
        [TestMethod]
        public void TaskCancelResp_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskCancelResponse.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.Cancel, testObject.Operation);

            // Only asserting one field from status report (because it is tested thorougly elsewhere)
            Assert.AreEqual(1, testObject.StatusReports.Count);
            Assert.AreEqual(TaskingRequestStatusCodeType.Accepted, testObject.StatusReports[0].RequestStatus);

            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.Ok, testObject.RequestResult);
        }

        [TestMethod]
        public void TaskCancelResp_Read_NoExtension()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskCancelResponse_NoExtension.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.Cancel, testObject.Operation);

            // Only asserting one field from status report (because it is tested thorougly elsewhere)
            Assert.AreEqual(1, testObject.StatusReports.Count);
            Assert.AreEqual(TaskingRequestStatusCodeType.Accepted, testObject.StatusReports[0].RequestStatus);

            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.Unknown, testObject.RequestResult);
        }

        [TestMethod]
        public void TaskCancelResp_Create()
        {
            var statusReport = new TaskStatusReport(taskId: "mytask", procId: "myprocedure")
            {
                RequestStatus = TaskingRequestStatusCodeType.Pending
            };
            var testObject = TaskResponse.CreateCancelResponse(statusReport);
            testObject.RequestResult = RequestResultType.Conflict;

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new TaskResponse(xmlBytes);

            // Using a separate assert function to make sure the deserialised object is asserted
            TaskCancelResp_Create_Assert(testObjectIn);
        }

        private void TaskCancelResp_Create_Assert(TaskResponse testObjectIn)
        {
            Assert.AreEqual(TaskOperationType.Cancel, testObjectIn.Operation);

            // Only asserting one field from status report (because it is tested thorougly elsewhere)
            Assert.AreEqual(1, testObjectIn.StatusReports.Count);
            Assert.AreEqual(TaskingRequestStatusCodeType.Pending, testObjectIn.StatusReports[0].RequestStatus);

            // This is from ExtensibleResponse
            Assert.AreEqual(RequestResultType.Conflict, testObjectIn.RequestResult);
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
                validator.Validate(xmlStream, typeof(XsdNs.CancelResponseType));
            }
        }
    }
}
