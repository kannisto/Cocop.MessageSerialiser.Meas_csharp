//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 8/2018
// Last modified: 2/2020


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Meas;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace ExtensibleResponseTest
{
    [TestClass]
    public class UnitTest1
    {
        // Testing the ExtensibleResponse class requires a subclass.
        // Here, the subclass "InsertObservationResponse" is utilised
        // instead of creating a stub class.
        //
        // The InsertObservationResponse class does not have a dedicated
        // test. If this test is modified to use another class than
        // InsertObservationResponse, please make sure to create a test
        // for InsertObservationResponse!

        // This test focuses on:
        // - Reading a response (101, 102, 103)
        // - Trying to read a badly formed response (111)
        // - The creation of a response (222, 223)
        

        [TestMethod]
        public void ExtResp_101_ReadXml_Unknown()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\InsertObservationResponse_empty.xml";
            var testObject = new InsertObservationResponseForTest(ReadFile(filepath));

            Assert.AreEqual(RequestResultType.Unknown, testObject.RequestResult);
            Assert.IsTrue(string.IsNullOrEmpty(testObject.RequestResultMessage));
        }

        [TestMethod]
        public void ExtResp_102_ReadXml_Ok()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\InsertObservationResponse_ok.xml";
            var testObject = new InsertObservationResponseForTest(ReadFile(filepath));

            Assert.AreEqual(RequestResultType.Ok, testObject.RequestResult);
            Assert.IsTrue(string.IsNullOrEmpty(testObject.RequestResultMessage));
        }

        [TestMethod]
        public void ExtResp_103_ReadXml_BadRequest()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\InsertObservationResponse_badrequest.xml";
            var testObject = new InsertObservationResponseForTest(ReadFile(filepath));

            Assert.AreEqual(RequestResultType.BadRequest, testObject.RequestResult);
            Assert.AreEqual("The server did not understand the request", testObject.RequestResultMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.InvalidMessageException))]
        public void ExtResp_111_ReadXml_Unexpected()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\Neg_InsertObservationResponse_unexpected.xml";

            // Exception expected
            new InsertObservationResponseForTest(ReadFile(filepath));
        }

        [TestMethod]
        public void ExtResp_201_CreateXml_Ok()
        {
            var testObject = new InsertObservationResponseForTest()
            {
                RequestResult = RequestResultType.Ok
            };

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new InsertObservationResponseForTest(xmlBytes);

            // Asserting
            Assert.AreEqual(RequestResultType.Ok, testObjectIn.RequestResult);
            Assert.IsTrue(string.IsNullOrEmpty(testObject.RequestResultMessage));
        }

        [TestMethod]
        public void ExtResp_202_CreateXml_ServerError()
        {
            var testObject = new InsertObservationResponseForTest()
            {
                RequestResult = RequestResultType.ServerError,
                RequestResultMessage = "The server failed"
            };

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new InsertObservationResponseForTest(testObject.ToXmlBytes());

            // Asserting
            Assert.AreEqual(RequestResultType.ServerError, testObjectIn.RequestResult);
            Assert.AreEqual("The server failed", testObjectIn.RequestResultMessage);
        }

        private byte[] ReadFile(string filepath)
        {
            return System.IO.File.ReadAllBytes(filepath);
        }

        private void Validate(byte[] xmlBytes)
        {
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Sos_GmlOmSwe);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.InsertObservationResponseType));
            }
        }
    }
}
