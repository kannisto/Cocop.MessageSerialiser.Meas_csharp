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
    public class TestSubmit
    {
        [TestMethod]
        public void TaskSubmitResp_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskSubmitResponse.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.Submit, testObject.Operation);

            // From ExtensibleResponse; this assertion is a kind of integration test
            Assert.AreEqual(RequestResultType.Ok, testObject.RequestResult);

            // From TaskStatusReport; only asserting one field because TaskStatusReport
            // is tested thoroughly elsewhere
            Assert.AreEqual(1, testObject.StatusReports.Count);
            Assert.AreEqual("myproc-1", testObject.StatusReports[0].ProcedureId);
        }

        [TestMethod]
        public void TaskSubmitResp_Read_NoExtension()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskSubmitResponse_NoExtension.xml";
            var testObject = new TaskResponse(ReadFile(filepath));

            Assert.AreEqual(TaskOperationType.Submit, testObject.Operation);

            // From ExtensibleResponse; this assertion is a kind of integration test
            Assert.AreEqual(RequestResultType.Unknown, testObject.RequestResult);

            // From TaskStatusReport; only asserting one field because TaskStatusReport
            // is tested thoroughly elsewhere
            Assert.AreEqual(1, testObject.StatusReports.Count);
            Assert.AreEqual("myproc-1", testObject.StatusReports[0].ProcedureId);
        }

        [TestMethod]
        public void TaskSubmitResp_Create()
        {
            var statusReportIn = new TaskStatusReport(taskId: "mytask", procId: "myproc")
            {
                RequestStatus = TaskingRequestStatusCodeType.Accepted
            };

            var testObject = TaskResponse.CreateSubmitResponse(statusReportIn);

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new TaskResponse(xmlBytes);
            
            // Using a separate assert function to make sure the deserialised object is asserted
            TaskSubmitResp_Create_Assert(testObjectIn);
        }

        private void TaskSubmitResp_Create_Assert(TaskResponse testObjectIn)
        {
            Assert.AreEqual(TaskOperationType.Submit, testObjectIn.Operation);

            // From ExtensibleResponse; this assertion is a kind of integration test
            Assert.AreEqual(RequestResultType.Ok, testObjectIn.RequestResult);

            // From TaskStatusReport; only asserting one field because TaskStatusReport
            // is tested thoroughly elsewhere
            Assert.AreEqual(1, testObjectIn.StatusReports.Count);
            Assert.AreEqual("myproc", testObjectIn.StatusReports[0].ProcedureId);
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
                validator.Validate(xmlStream, typeof(XsdNs.SubmitResponseType));
            }
        }
    }
}
