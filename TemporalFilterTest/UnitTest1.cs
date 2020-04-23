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
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace TemporalFilterTest
{
    [TestClass]
    public class UnitTest1
    {
        // This test focuses on: 
        // - Creation of filters (100)
        //   - time range included
        //   - time instant included
        //   - operator-params mismatch included
        // - Reading a basic filter from XML (2**)
        //   - After, before, during
        // - Creating and reading a basic filter (3**)
        //   - After, before, during
        // 
        // This test does *not* focus on:
        // - Reading the actual operator names in XML element names (presumably, this would result in an exception)
        // - Parsing datetime values

        [TestMethod]
        public void TempF_100_Create()
        {
            // Testing how the constructor works with invalid input

            var baseTime = DateTime.Now.ToUniversalTime();

            var timeRange1 = new Item_TimeRange(baseTime, baseTime.AddHours(1));
            var timeInstant1 = new Item_TimeInstant(baseTime);

            // These operators require a time instant as the operand
            AssertCtorException(TemporalFilter.OperatorType.After, timeRange1);
            AssertCtorException(TemporalFilter.OperatorType.Before, timeRange1);

            // These operators require a time range as the operand
            AssertCtorException(TemporalFilter.OperatorType.During, timeInstant1);
        }

        [TestMethod]
        public void TempF_210_ReadAfter()
        {
            // Testing the "after" operator

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TemporalFilter_after.xml";

            TestTimeInstantFromFile(
                filepath,
                TemporalFilter.ValueReferenceType.ResultTime,
                TemporalFilter.OperatorType.After,
                ParseDateTimeInUtc("2018-03-28T12:01:59Z")
                );
        }

        [TestMethod]
        public void TempF_220_ReadBefore()
        {
            // Testing the "before" operator

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TemporalFilter_before.xml";

            TestTimeInstantFromFile(
                filepath,
                TemporalFilter.ValueReferenceType.PhenomenonTime,
                TemporalFilter.OperatorType.Before,
                ParseDateTimeInUtc("2018-04-18T03:44:11Z")
                );
        }

        [TestMethod]
        public void TempF_230_ReadDuring()
        {
            // Testing the "during" operator

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TemporalFilter_during.xml";

            // Getting the proxy
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);
            var proxy = DeserialiseAndGetProxy(xmlBytes);

            // Creating an object from the proxy
            var testObject = new TemporalFilter(proxy);

            // Asserting value reference
            Assert.AreEqual(TemporalFilter.ValueReferenceType.PhenomenonTime, testObject.ValueReference);

            // Asserting operator
            Assert.AreEqual(TemporalFilter.OperatorType.During, testObject.Operator);

            // Expecting a time range
            var timeRange = (Item_TimeRange)testObject.Time;
            AssertDateTime(ParseDateTimeInUtc("2018-05-18T03:23:11Z"), timeRange.Start);
            AssertDateTime(ParseDateTimeInUtc("2018-05-18T03:23:29Z"), timeRange.End);
        }

        [TestMethod]
        public void TempF_310_CreateAndReadXml_after()
        {
            // Testing the "after" operator
            TestCreateAndReadTimeInstant(TemporalFilter.ValueReferenceType.ResultTime, TemporalFilter.OperatorType.After);
        }

        [TestMethod]
        public void TempF_320_CreateAndReadXml_before()
        {
            // Testing the "before" operator
            TestCreateAndReadTimeInstant(TemporalFilter.ValueReferenceType.PhenomenonTime, TemporalFilter.OperatorType.Before);
        }

        [TestMethod]
        public void TempF_330_CreateAndReadXml_during()
        {
            // Testing the "during" operator

            // Creating a time range
            var start = ParseDateTimeInUtc("2018-04-18T03:23:11Z");
            var end = ParseDateTimeInUtc("2018-05-18T03:23:11Z");
            var timeRange = new Item_TimeRange(start, end);

            var testObject = new TemporalFilter(TemporalFilter.ValueReferenceType.PhenomenonTime, TemporalFilter.OperatorType.During, timeRange);

            // Serialise and read
            var testObjectIn = SerialiseAndRead(testObject);

            // Asserting value reference
            Assert.AreEqual(TemporalFilter.ValueReferenceType.PhenomenonTime, testObjectIn.ValueReference);

            // Asserting operator
            Assert.AreEqual(TemporalFilter.OperatorType.During, testObjectIn.Operator);

            // Expecting a time range
            var timeRangeIn = (Item_TimeRange)testObjectIn.Time;
            AssertDateTime(start, timeRangeIn.Start);
            AssertDateTime(end, timeRangeIn.End);
        }

        private void TestCreateAndReadTimeInstant(TemporalFilter.ValueReferenceType valueRef, TemporalFilter.OperatorType oper)
        {
            // This general test method tests the serialisation and deserialisation of
            // a filter that has a time instant operand

            // Creating a time instant
            var time = ParseDateTimeInUtc("2018-01-18T03:23:34Z");
            var timeInstant = new Item_TimeInstant(time);

            var testObject = new TemporalFilter(valueRef, oper, timeInstant);

            // Serialise and read
            var testObjectIn = SerialiseAndRead(testObject);

            // Asserting value reference
            Assert.AreEqual(valueRef, testObjectIn.ValueReference);

            // Asserting operator
            Assert.AreEqual(oper, testObjectIn.Operator);

            // Expecting a time instant
            var timeInstantIn = (Item_TimeInstant)testObjectIn.Time;
            AssertDateTime(time, timeInstantIn.Value);
        }

        private TemporalFilter SerialiseAndRead(TemporalFilter input)
        {
            // Get proxy
            var filterProxy = input.ToXmlProxy("test_");

            // Wrap the proxy in a GetObservationRequest
            var requestProxy = new XsdNs.GetObservationType
            {
                temporalFilter = new XsdNs.GetObservationTypeTemporalFilter[] { filterProxy },
                service = "SOS",
                version = "2.0.0"
            };

            // Serialise the proxy
            byte[] xmlBytes = XNeut.Helper.ToXmlBytes(requestProxy);

            // XML validation
            Validate(xmlBytes);

            // Read XML data
            var proxyIn = DeserialiseAndGetProxy(xmlBytes);
            return new TemporalFilter(proxyIn);
        }

        private void TestTimeInstantFromFile(string filepath, TemporalFilter.ValueReferenceType valRef, TemporalFilter.OperatorType expectedOperator, DateTime expDateTime)
        {
            // Getting the proxy
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);
            var proxy = DeserialiseAndGetProxy(xmlBytes);

            // Creating an object from the proxy
            var testObject = new TemporalFilter(proxy);

            // Asserting value reference
            Assert.AreEqual(valRef, testObject.ValueReference);

            // Asserting operator
            Assert.AreEqual(expectedOperator, testObject.Operator);

            // Expecting a time instant
            var timeInstant = (Item_TimeInstant)testObject.Time;
            AssertDateTime(expDateTime, timeInstant.Value);
        }

        private void AssertCtorException(TemporalFilter.OperatorType op, Item timeItem)
        {
            try
            {
                new TemporalFilter(TemporalFilter.ValueReferenceType.PhenomenonTime, op, timeItem);
                Assert.Fail("Expected exception");
            }
            catch (ArgumentException)
            { }
        }

        private DateTime ParseDateTimeInUtc(string s)
        {
            return TestCommon.TestHelper.ParseDateTimeInUtc(s);
        }

        private void AssertDateTime(DateTime expected, DateTime actual)
        {
            TestCommon.TestHelper.AssertDateTime(expected, actual);
        }

        private XsdNs.GetObservationTypeTemporalFilter DeserialiseAndGetProxy(byte[] xmlBytes)
        {
            // The GetObservationType wraps the filter as using a bare filter in an XML document would have
            // required additional work without any value for the use cases.
            // In contrast, GetObservationType is utilised elsewhere anyway.
            var proxy = (XsdNs.GetObservationType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.GetObservationType), xmlBytes);
            return proxy.temporalFilter[0];
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
