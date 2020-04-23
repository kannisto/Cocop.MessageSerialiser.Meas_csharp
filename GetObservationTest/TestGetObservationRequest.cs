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
    public class TestGetObservationRequest
    {
        // This test focuses on:
        // - Reading a "get" request (111)
        // - The creation of a "get" request (121) with default values
        // - The creation of a "get" request (122) with non-default values
        //
        // This test does *not* focus on:
        // - Content of temporal filters or any other included items (they are tested elsewhere)


        [TestMethod]
        public void GetObsReq_111_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\GetObservationRequest.xml";
            var testObject = new GetObservationRequest(ReadFile(filepath));

            // Asserting
            Assert.AreEqual(1, testObject.FeaturesOfInterest.Count);
            Assert.AreEqual(1, testObject.ObservedProperties.Count);
            Assert.IsTrue(testObject.ObservedProperties.Contains("304-TI-101"));
            Assert.IsTrue(testObject.FeaturesOfInterest.Contains("cocop/somefeature"));

            // Asserting extension (a data record)
            Assert.AreEqual(1, testObject.Items.Count);
            var extension = testObject.Items[0];
            var measurementItem = (Item_Measurement)extension["MyDataRecordItem"];
            Assert.AreEqual(324.23, measurementItem.Value, 0.0001);


            Assert.AreEqual(2, testObject.TemporalFilters.Count);

            // Asserting temporal filter 1 (the assertion has a low coverage because filters are tested elsewhere)
            var filter1 = testObject.TemporalFilters[0];
            Assert.AreEqual(TemporalFilter.ValueReferenceType.ResultTime, filter1.ValueReference);
            Assert.AreEqual(TemporalFilter.OperatorType.After, filter1.Operator);

            // Asserting temporal filter 2
            var filter2 = testObject.TemporalFilters[1];
            Assert.AreEqual(TemporalFilter.ValueReferenceType.PhenomenonTime, filter2.ValueReference);
            Assert.AreEqual(TemporalFilter.OperatorType.During, filter2.Operator);
        }
        
        [TestMethod]
        public void GetObsReq_121_CreateWithDefault()
        {
            // This test exists mainly to make sure that no errors occur the default values

            var testObject = new GetObservationRequest();

            // Serialising and deserialising request object
            var testObjectIn = new GetObservationRequest(testObject.ToXmlBytes());

            // Asserting
            Assert.AreEqual(0, testObjectIn.FeaturesOfInterest.Count);
            Assert.AreEqual(0, testObjectIn.Items.Count);
            Assert.AreEqual(0, testObjectIn.ObservedProperties.Count);
            Assert.AreEqual(0, testObjectIn.TemporalFilters.Count);
        }

        [TestMethod]
        public void GetObsReq_122_Create()
        {
            // Creating request object
            var testObject = new GetObservationRequest();
            testObject.FeaturesOfInterest.Add("myfeature");
            testObject.ObservedProperties.Add("myproperty");

            // Adding a data record
            var extensionObj = new Item_DataRecord
            {
                { "MyMeasurement", new Item_Measurement("s", 0.453) }
            };
            testObject.Items.Add(extensionObj);

            // Adding temporal filters
            var baseTime = DateTime.Now.ToUniversalTime();
            var tempFilter1 = new TemporalFilter(
                TemporalFilter.ValueReferenceType.ResultTime,
                TemporalFilter.OperatorType.During,
                new Item_TimeRange(baseTime, baseTime.AddHours(2))
                );
            testObject.TemporalFilters.Add(tempFilter1);
            var tempFilter2 = new TemporalFilter(
                TemporalFilter.ValueReferenceType.PhenomenonTime,
                TemporalFilter.OperatorType.Before,
                new Item_TimeInstant(baseTime)
                );
            testObject.TemporalFilters.Add(tempFilter2);

            // Serialising, validating and deserialising request object
            var xmlBytes = testObject.ToXmlBytes();
            Validate(xmlBytes);
            var testObjectIn = new GetObservationRequest(xmlBytes);

            // Asserting
            Assert.AreEqual(1, testObjectIn.FeaturesOfInterest.Count);
            Assert.AreEqual(1, testObjectIn.ObservedProperties.Count);
            Assert.IsTrue(testObjectIn.FeaturesOfInterest.Contains("myfeature"));
            Assert.IsTrue(testObjectIn.ObservedProperties.Contains("myproperty"));

            // Asserting extension (a data record)
            Assert.AreEqual(1, testObjectIn.Items.Count);
            var extension = testObjectIn.Items[0];
            var measurementItem = (Item_Measurement)extension["MyMeasurement"];
            Assert.AreEqual(0.453, measurementItem.Value, 0.0001);


            Assert.AreEqual(2, testObject.TemporalFilters.Count);

            // Asserting temporal filter 1 (the assertion has a low coverage because filters are tested elsewhere)
            var filter1 = testObjectIn.TemporalFilters[0];
            Assert.AreEqual(TemporalFilter.ValueReferenceType.ResultTime, filter1.ValueReference);
            Assert.AreEqual(TemporalFilter.OperatorType.During, filter1.Operator);

            // Asserting temporal filter 2
            var filter2 = testObjectIn.TemporalFilters[1];
            Assert.AreEqual(TemporalFilter.ValueReferenceType.PhenomenonTime, filter2.ValueReference);
            Assert.AreEqual(TemporalFilter.OperatorType.Before, filter2.Operator);
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
                validator.Validate(xmlStream, typeof(XsdNs.GetObservationType));
            }
        }
    }
}
