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
    public class TestInsertObsRequest
    {
        // This test focuses on:
        // - Reading an "insert" request (120)
        // - The creation of an "insert" request (130) with non-default values
        // - Inclusion of the actual payload (observations) (120, 130)
        // - Creating a request without offerings specified (131)
        // - Creating a request without any observation (error) (132)
        //
        // This test does *not* focus on:
        // - Content of observations or any other included items (they are tested elsewhere)


        [TestMethod]
        public void InsertObsReq_120_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\InsertObservationRequest.xml";
            var testObject = new InsertObservationRequest(ReadFile(filepath));

            // Asserting offering
            Assert.AreEqual("some-offering", testObject.Offering[0]);

            // Asserting the enclosed observations (not field by field because it is in the tests of Observation class)
            Assert.AreEqual(2, testObject.Observations.Count);

            var result1 = (Item_Measurement)testObject.Observations[0].Result;
            var result2 = (Item_Measurement)testObject.Observations[1].Result;

            Assert.AreEqual(20.3, result1.Value, 0.001);
            Assert.AreEqual(20.5, result2.Value, 0.001);
        }

        [TestMethod]
        public void InsertObsReq_130_Create()
        {
            var testObject = CreateTestObjectFor130();

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new InsertObservationRequest(xmlBytes);

            // Asserting observations
            Assert.AreEqual(2, testObjectIn.Observations.Count);

            var result1In = (Item_Measurement)testObjectIn.Observations[0].Result;
            var result2In = (Item_Measurement)testObjectIn.Observations[1].Result;

            Assert.AreEqual(1.11, result1In.Value, 0.0001);
            Assert.AreEqual(1.28, result2In.Value, 0.0001);

            // Asserting offerings
            Assert.AreEqual(2, testObjectIn.Offering.Count);
            Assert.AreEqual("off1", testObjectIn.Offering[0]);
            Assert.AreEqual("off2", testObjectIn.Offering[1]);
        }

        private InsertObservationRequest CreateTestObjectFor130()
        {
            // Creation is separated to this function to make sure the correct object is asserted
            
            // Creating the object to be serialised
            var testObject = new InsertObservationRequest();

            // Adding observations
            var result1 = new Item_Measurement("m", 1.11);
            var result2 = new Item_Measurement("m", 1.28);
            testObject.Observations.Add(new Observation(result1));
            testObject.Observations.Add(new Observation(result2));

            // Adding offerings
            testObject.Offering.Add("off1");
            testObject.Offering.Add("off2");

            return testObject;
        }

        [TestMethod]
        public void InsertObsReq_131_Create_NoOffering()
        {
            var testObject = CreateTestObjectFor131();

            // Serialising, validating and deserialising
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new InsertObservationRequest(xmlBytes);

            // Not asserting observations. Offerings are in focus here.
            
            // Asserting offerings. Expecting an empty offering to appear
            // because the initial test object contained none.
            Assert.AreEqual(1, testObjectIn.Offering.Count);
            Assert.IsTrue(testObjectIn.Offering[0].Length == 0);
        }

        private InsertObservationRequest CreateTestObjectFor131()
        {
            // Creation is separated to this function to make sure the correct object is asserted

            // Creating the object to be serialised
            var testObject = new InsertObservationRequest();

            // Adding observations
            var result1 = new Item_Measurement("m", 1.11);
            testObject.Observations.Add(new Observation(result1));

            // Not adding any offerings
            
            return testObject;
        }

        [TestMethod]
        public void InsertObsReq_132_Create_NoObservation()
        {
            var testObject = new InsertObservationRequest();

            try
            {
                // No observations! This is supposed to fail.
                testObject.ToXmlBytes();
                Assert.Fail("Expected exception");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("InsertObservationRequest must contain at least one observation", e.Message);
            }
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
                validator.Validate(xmlStream, typeof(XsdNs.InsertObservationType));
            }
        }
    }
}
