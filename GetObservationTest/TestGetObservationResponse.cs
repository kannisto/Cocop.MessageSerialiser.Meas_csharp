//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2018
// Last modified: 2/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Meas;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace GetObservationTest
{
    [TestClass]
    public class TestGetObservationResponse
    {
        // This test focuses on:
        // - Reading a "get" response (210, 211)
        // - The creation of a "get" response (220)
        // - Inclusion of the actual payload (observations, time series...) (210, 220)
        //
        // This test does *not* focus on:
        // - Content of observations or any other included items (they are tested elsewhere)


        [TestMethod]
        public void GetObsResp_210_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\GetObservationResponse.xml";
            var testObject = new GetObservationResponse(ReadFile(filepath));

            // Asserting observations
            Assert.AreEqual(2, testObject.Observations.Count);
            var obs1 = testObject.Observations[0];
            var obs2 = testObject.Observations[1];
            var meas1 = (Item_Measurement)obs1.Result;
            var meas2 = (Item_Measurement)obs2.Result;
            Assert.AreEqual(20.3, meas1.Value, 0.0001);
            Assert.AreEqual(20.5, meas2.Value, 0.0001);

            // A simple assertion for request result information
            // (to check that it is processed; an exhaustive test
            // is performed for the ExtensibleResponse class)
            Assert.AreEqual(RequestResultType.Ok, testObject.RequestResult);
            Assert.AreEqual("It's OK", testObject.RequestResultMessage);
        }

        [TestMethod]
        public void GetObsResp_211_Read_NoExtension()
        {
            // Testing that deserialisation does not fail when there is no extension
            
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\GetObservationResponse_NoExtension.xml";
            var testObject = new GetObservationResponse(ReadFile(filepath));

            // Asserting observations (no thoroughly because this is tested elsewhere)
            Assert.AreEqual(2, testObject.Observations.Count);

            // Only asserting a single field to see that deserialisation has succeeded in general
            Assert.AreEqual(RequestResultType.Unknown, testObject.RequestResult);
        }

        [TestMethod]
        public void GetObsResp_220_Create()
        {
            var objectOut = new GetObservationResponse()
            {
                RequestResultMessage = "Success",
                RequestResult = RequestResultType.Ok
            };

            // Setting observation data
            var observation1 = new Observation(new Item_Measurement("s", 2.2));
            var observation2 = new Observation(new Item_Measurement("s", 2.4));
            objectOut.Observations.Add(observation1);
            objectOut.Observations.Add(observation2);

            // Serialising, validating and deserialising the object
            var xmlBytes = objectOut.ToXmlBytes();
            Validate(xmlBytes);
            var objectIn = new GetObservationResponse(xmlBytes);

            // Asserting observations (only some values)
            var resultIn1 = (Item_Measurement)objectIn.Observations[0].Result;
            var resultIn2 = (Item_Measurement)objectIn.Observations[1].Result;
            Assert.AreEqual(2.2, resultIn1.Value, 0.0001);
            Assert.AreEqual(2.4, resultIn2.Value, 0.0001);

            // A simple assertion for request result information
            // (to check that it is processed; an exhaustive test
            // is performed for the ExtensibleResponse class)
            Assert.AreEqual(RequestResultType.Ok, objectIn.RequestResult);
            Assert.AreEqual("Success", objectIn.RequestResultMessage);
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
                validator.Validate(xmlStream, typeof(XsdNs.GetObservationResponseType));
            }
        }
    }
}
