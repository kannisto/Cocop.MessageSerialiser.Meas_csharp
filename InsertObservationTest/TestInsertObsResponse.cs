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

namespace InsertObservationTest
{
    [TestClass]
    public class TestInsertObsResponse
    {
        // This test focuses on:
        // - Reading an "insert" response (210)
        // - Creating an "insert" response (220)
        //
        // This test does *not* focus on:
        // - Observations or any other included items (they are tested elsewhere)


        [TestMethod]
        public void InsertObsResp_210_Read()
        {
            // This test is very simple, because ExtensibleResponse provides
            // almost all the functionality of InsertObservationResponse and
            // ExtensibleResponse is tested elsewhere.

            string filepath = TestCommon.TestHelper.TestFileFolder + @"\InsertObservationResponse_ok.xml";
            var testObject = new InsertObservationResponse(ReadFile(filepath));

            Assert.AreEqual(RequestResultType.Ok, testObject.RequestResult);
        }

        [TestMethod]
        public void InsertObsResp_211_Read_NoExtension()
        {
            // Testing that deserialisation works when the extension does not exist.
            // Only asserting a single field to see that deserialisation has succeeded in general.

            string filepath = TestCommon.TestHelper.TestFileFolder + @"\InsertObservationResponse_empty.xml";
            var testObject = new InsertObservationResponse(ReadFile(filepath));

            Assert.AreEqual(RequestResultType.Unknown, testObject.RequestResult);
        }

        [TestMethod]
        public void InsertObsResp_220_Create()
        {
            // This test is very simple, because ExtensibleResponse provides
            // almost all the functionality of InsertObservationResponse and
            // ExtensibleResponse is tested elsewhere.

            var testObject = new InsertObservationResponse()
            {
                RequestResult = RequestResultType.NotFound
            };

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new InsertObservationResponse(xmlBytes);

            Assert.AreEqual(RequestResultType.NotFound, testObjectIn.RequestResult);
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
