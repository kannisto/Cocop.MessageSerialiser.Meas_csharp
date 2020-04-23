//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 2/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Meas;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace ObservationTest
{
    [TestClass]
    public class UnitTest1
    {
        // This test covers:
        // - (De)serialisation of observation metadata fields (all test cases)
        // - Reading a typical XML document (10)
        // - Reading a minimal XML document (11)
        // - Creating an XML document with default values (20)
        // - Creating an XML document with typical values (21)
        // - Creating an XML document with a bad data quality value (22)
        // 
        // This test does *not* pay particular attention on:
        // - The serialisation of result types
        // - (De)serialisation of datetime values
        // - (De)serialisation of URI values
        // - (De)serialisation of data quality values (tested elsewhere)

        [TestMethod]
        public void Obs_10_Read()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\Observation_typical.xml";

            // Processing XML data
            var testObject = new Observation(ReadFile(filepath));

            // Checking string values
            Assert.AreEqual("FSF batch mass", testObject.Name);
            Assert.AreEqual("The mass of a batch a crane has received from FSF", testObject.Description);

            // Checking URI values
            Assert.AreEqual("cocop/copper/crane/loadmassmeasurement", testObject.Procedure);
            Assert.AreEqual("mass", testObject.ObservedProperty);
            Assert.AreEqual("cocop/copper/fsf/batch", testObject.FeatureOfInterest);

            // Checking datetime values
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2018-02-05T12:10:13.00Z"), testObject.PhenomenonTime);
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2018-02-05T12:31:53.00Z"), testObject.ResultTime);

            // Asserting good data quality
            Assert.IsTrue(testObject.ResultQuality.IsGood);
        }

        [TestMethod]
        public void Obs_11_Read_MinimalXmlDoc()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\Observation_minimal.xml";

            // Processing XML data
            var testObject = new Observation(ReadFile(filepath));

            // Checking string values
            Assert.IsTrue(string.IsNullOrEmpty(testObject.Name));
            Assert.IsTrue(string.IsNullOrEmpty(testObject.Description));

            // Checking URI values
            Assert.IsTrue(string.IsNullOrEmpty(testObject.Procedure));
            Assert.IsTrue(string.IsNullOrEmpty(testObject.ObservedProperty));
            Assert.IsTrue(string.IsNullOrEmpty(testObject.FeatureOfInterest));

            // Checking datetime values
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2018-02-05T12:10:13.00Z"), testObject.PhenomenonTime);
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2018-02-05T12:31:53.00Z"), testObject.ResultTime);
            
            // Asserting good data quality
            Assert.IsTrue(testObject.ResultQuality.IsGood);
        }

        [TestMethod]
        public void Obs_12_Read_ComplexFeatureOfInterest()
        {
            string filepath = TestCommon.TestHelper.TestFileFolder + @"\Observation_complexfeature.xml";

            // Processing XML data
            var testObject = new Observation(ReadFile(filepath));

            // Asserting feature name.
            Assert.AreEqual("my-plant/fic-11", testObject.FeatureOfInterest);

            // Asserting a field in the data record
            Assert.IsTrue(testObject.FeatureOfInterestDetails.ItemNames.Contains("SomeField"));

            // Not asserting other contents of data record because Item_DataRecord
            // is tested elsewhere.
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.InvalidMessageException))]
        public void Obs_Neg_11_Read_FeatureOfInterest_title()
        {
            // Testing that the deserialisation produces an error if there are xlink
            // attributes in a complex feature of interest.
            
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\Neg_Obs_feature_title.xml";
            new Observation(ReadFile(filepath));
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.InvalidMessageException))]
        public void Obs_Neg_12_Read_FeatureOfInterest_type()
        {
            // Testing that the deserialisation produces an error if there are xlink
            // attributes in a complex feature of interest.

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\Neg_Obs_feature_type.xml";
            new Observation(ReadFile(filepath));
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.DateTimeException))]
        public void Obs_Neg_21_Create_BadPhenoTimeKindA()
        {
            // Expecting an exception if phenomenon time is not in UTC

            var result = new Item_Category("hello");
            new Observation(result)
            {
                PhenomenonTime = ParseDateTimeInUtc("2020-02-17T14:39:00Z").ToLocalTime()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.DateTimeException))]
        public void Obs_Neg_22_Create_BadPhenoTimeKindB()
        {
            // Expecting an exception if phenomenon time is not in UTC

            var result = new Item_Category("hello");
            new Observation(result)
            {
                PhenomenonTime = DateTime.SpecifyKind(ParseDateTimeInUtc("2020-02-17T14:39:00Z"), DateTimeKind.Unspecified)
            };
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.DateTimeException))]
        public void Obs_Neg_23_Create_BadResultTimeKindA()
        {
            // Expecting an exception if result time is not in UTC

            var result = new Item_Category("hello");
            new Observation(result)
            {
                ResultTime = ParseDateTimeInUtc("2020-02-17T14:39:00Z").ToLocalTime()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(Cocop.MessageSerialiser.Meas.Neutral.DateTimeException))]
        public void Obs_Neg_23_Create_BadResultTimeKindB()
        {
            // Expecting an exception if result time is not in UTC

            var result = new Item_Category("hello");
            new Observation(result)
            {
                ResultTime = DateTime.SpecifyKind(ParseDateTimeInUtc("2020-02-17T14:39:00Z"), DateTimeKind.Unspecified)
            };
        }

        [TestMethod]
        public void Obs_20_Create_DefaultValues()
        {
            var startTime = DateTime.Now;
            Observation originalObj = new Observation(new Item_Measurement(null));

            // Serializing and validating the XML document
            Observation parsedObj = SerialiseAndReadResultObj(originalObj);

            // Checking string values
            Assert.IsNull(parsedObj.Description);
            Assert.IsNull(parsedObj.Name);

            // Checking URI values
            Assert.AreEqual("", parsedObj.FeatureOfInterest.ToString().TrimEnd('/'));
            Assert.AreEqual("", parsedObj.ObservedProperty.ToString().TrimEnd('/'));
            Assert.AreEqual("", parsedObj.Procedure.ToString().TrimEnd('/'));

            // Checking result time by comparing to start time
            var timespan_result = parsedObj.ResultTime - startTime;
            Assert.IsTrue(timespan_result.Milliseconds < 200);

            // Checking phenomenon time by comparing to start time
            var timespan_phenomenonTime = parsedObj.PhenomenonTime - startTime;
            Assert.IsTrue(timespan_phenomenonTime.Milliseconds < 200);

            // Asserting good data quality
            Assert.IsTrue(parsedObj.ResultQuality.IsGood);
        }

        [TestMethod]
        public void Obs_21_Create()
        {
            Observation originalObj = new Observation(new Item_Measurement(null))
            {
                Name = "Some name",
                Description = "Some description",
                FeatureOfInterest = "somefeature",
                ObservedProperty = "someproperty",
                PhenomenonTime = ParseDateTimeInUtc("2018-02-23T10:00:00Z"),
                Procedure = "someprocedure",
                ResultTime = ParseDateTimeInUtc("2018-02-23T10:00:00Z"),
                ResultQuality = DataQuality.CreateGood()
            };

            // Serializing and validating the XML document
            Observation parsedObj = SerialiseAndReadResultObj(originalObj);

            // Checking string values
            Assert.AreEqual("Some description", parsedObj.Description);
            Assert.AreEqual("Some name", parsedObj.Name);

            // Checking reference values
            Assert.AreEqual("somefeature", parsedObj.FeatureOfInterest.TrimEnd('/'));
            Assert.AreEqual("someproperty", parsedObj.ObservedProperty.TrimEnd('/'));
            Assert.AreEqual("someprocedure", parsedObj.Procedure.TrimEnd('/'));

            // Checking result time
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2018-02-23T10:00:00Z"), parsedObj.ResultTime);

            // Comparing phenomenon time
            TestCommon.TestHelper.AssertDateTime(ParseDateTimeInUtc("2018-02-23T10:00:00Z"), parsedObj.PhenomenonTime);

            // Asserting good data quality
            Assert.IsTrue(parsedObj.ResultQuality.IsGood);
        }
        
        [TestMethod]
        public void Obs_22_Create_BadQuality()
        {
            Observation originalObj = new Observation(new Item_Measurement(null))
            {
                // Populating the object
                ResultQuality = DataQuality.CreateBad()
            };

            // Serializing and validating the XML document
            Observation parsedObj = SerialiseAndReadResultObj(originalObj);

            // Asserting bad data quality
            Assert.IsFalse(parsedObj.ResultQuality.IsGood);
        }

        [TestMethod]
        public void Obs_23_Create_ComplexFeatureOfInterest()
        {
            var featureOfInterestDetails = new Item_DataRecord();
            featureOfInterestDetails.Add("testctgfield", new Item_Category("testctgvalue"));

            Observation originalObj = new Observation(new Item_Measurement(null))
            {
                FeatureOfInterest = "testfeaturename",
                FeatureOfInterestDetails = featureOfInterestDetails
            };

            // Serializing and validating the XML document
            Observation parsedObj = SerialiseAndReadResultObj(originalObj);

            Assert.AreEqual("testfeaturename", parsedObj.FeatureOfInterest);
            //Assert.AreEqual("testctgvalue", parsedObj.FeatureOfInterestDetails.GetItem().Value);
            Assert.IsTrue(parsedObj.FeatureOfInterestDetails.ItemNames.Contains("testctgfield"));

            // Not Item_DataRecord in detail, because there is another test for that
        }

        private byte[] ReadFile(String filepath)
        {
            return System.IO.File.ReadAllBytes(filepath);
        }

        private Observation SerialiseAndReadResultObj(Observation testObject)
        {
            byte[] xmlBytes = testObject.ToXmlBytes();

            // Use this string value when debugging
            var stringValue = System.Text.Encoding.UTF8.GetString(xmlBytes);
            
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Custom1_GmlOmSweTsml);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.OM_ObservationType));
            }

            // Parsing the XML document
            return new Observation(xmlBytes);
        }

        private DateTime ParseDateTimeInUtc(string s)
        {
            return TestCommon.TestHelper.ParseDateTimeInUtc(s);
        }
    }
}
