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
    public class TestSubmit
    {
        // There are no separate test cases for minimal documents because all supported elements are mandatory.

        [TestMethod]
        public void TaskSubmitReq_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskSubmitRequest.xml";
            var testObject = new TaskRequest(ReadFile(filepath));

            // Not asserting the contents of the data record (tested elsewhere)
            Assert.AreEqual(2, testObject.Parameters.ItemNames.Count);
            Assert.AreEqual("http://cocop/myproc", testObject.ProcedureId);
            Assert.AreEqual(TaskOperationType.Submit, testObject.Operation);
        }
        
        [TestMethod]
        public void TaskSubmitReq_Create()
        {
            var testObject = TaskRequest.CreateSubmitRequest(procId: "myprocedure");
            testObject.Parameters.Add("myparam", new Item_Count(4));
            var xmlBytes = testObject.ToXmlBytes();

            // Validating
            Validate(xmlBytes);

            var testObjectIn = new TaskRequest(xmlBytes);
            
            // Using a separated assert function to make sure the deserialised object is asserted
            TaskSubmitReq_Create_Assert(testObjectIn);
        }

        private void TaskSubmitReq_Create_Assert(TaskRequest testObjectIn)
        {
            Assert.AreEqual(1, testObjectIn.Parameters.ItemNames.Count);
            Assert.IsTrue(testObjectIn.Parameters.ItemNames.Contains("myparam"));
            Assert.AreEqual("myprocedure", testObjectIn.ProcedureId);
            Assert.AreEqual(TaskOperationType.Submit, testObjectIn.Operation);
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
                validator.Validate(xmlStream, typeof(XsdNs.SubmitType));
            }
        }
    }
}
