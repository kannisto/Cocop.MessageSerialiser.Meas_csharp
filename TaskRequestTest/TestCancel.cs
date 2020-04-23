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

namespace TaskRequestTest
{
    [TestClass]
    public class TestCancel
    {
        [TestMethod]
        public void TaskCancelReq_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskCancelRequest.xml";
            var testObject = new TaskRequest(ReadFile(filepath));

            Assert.AreEqual("mytask", testObject.TaskId);
            Assert.AreEqual(TaskOperationType.Cancel, testObject.Operation);
        }

        [TestMethod]
        public void TaskCancelReq_Create()
        {
            var testObject = TaskRequest.CreateCancelRequest("sometask");
            var xmlBytes = testObject.ToXmlBytes();

            // Validating
            Validate(xmlBytes);

            var testObjectIn = new TaskRequest(xmlBytes);

            // Use separate function to make sure the correct object is asserted
            TaskCancelReq_Create_Assert(testObjectIn);
        }

        private void TaskCancelReq_Create_Assert(TaskRequest testObjectIn)
        {
            Assert.AreEqual("sometask", testObjectIn.TaskId);
            Assert.AreEqual(TaskOperationType.Cancel, testObjectIn.Operation);
        }

        [TestMethod]
        public void TaskCancelReq_Create_Params()
        {
            // Parameters are not allowed for a cancel request

            var testObject = TaskRequest.CreateCancelRequest("sometask");
            testObject.Parameters = new Item_DataRecord
            {
                { "MyParam", new Item_Count(3) }
            };

            try
            {
                testObject.ToXmlBytes();
                Assert.Fail("Expected exception");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.StartsWith("Parameters are not supported in cancel task request"));
            }
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
                validator.Validate(xmlStream, typeof(XsdNs.CancelType));
            }
        }
    }
}
