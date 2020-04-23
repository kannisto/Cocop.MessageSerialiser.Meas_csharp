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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXml = System.Xml;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;
using Cocop.MessageSerialiser.Meas;

namespace ItemTest
{
    [TestClass]
    public class UnitTest1
    {
        /*
	     * This test has its focus on Item_* classes and their XML serialisation.
	     * 
	     * An appropriate XML validation is easiest with the enclosing Observation class.
	     * Due to the serialisation complexity of the observation class, it is included as 
	     * such instead of creating a redundant stub class.
	     * 
	     * Each item type has its dedicated read and serialisation test.
	     */

        // This test focuses on:
        // - The basic (de)serialisation of the various item types enclosed in an observation
        //   - This includes testing the ResultTypeManager class
        // - The basic (de)serialisation of the various item types enclosed in a data record (nested data records included)
        // - Inclusion of data quality values in a data record
        // - Attempt to associate data quality value to a data record field that does not support it
        //
        // This test does *not* pay any particular attention on:
        // - Parsing XML data, including datetimes, booleans, doubles etc. (because the native XML library does that)
        // - (De)serialisation of observation metadata (tested elsewhere)
        // - Attempting to parse an invalid XML document


        #region ToDisplayString

        [TestMethod]
        public void ToDisplayString()
        {
            var timeValue1 = System.Xml.XmlConvert.ToDateTime("2019-04-15T08:47:12Z", System.Xml.XmlDateTimeSerializationMode.Utc);
            var timeValue2 = System.Xml.XmlConvert.ToDateTime("2019-04-15T08:49:44Z", System.Xml.XmlDateTimeSerializationMode.Utc);

            // Asserting items with a display string that shows all the payload of the item.
            // That is:
            // a) has only one value, such as boolean or category
            // b) is a range (lower and upper, other than measurement range)
            Assert.AreEqual("true", new Item_Boolean(true).ToDisplayString());
            Assert.AreEqual("ctg_1", new Item_Category("ctg_1").ToDisplayString());
            Assert.AreEqual("", new Item_Category((string)null).ToDisplayString());
            Assert.AreEqual("ctg_1..ctg_2", new Item_CategoryRange("ctg_1", "ctg_2").ToDisplayString());
            Assert.AreEqual("23", new Item_Count(23).ToDisplayString());
            Assert.AreEqual("2..5", new Item_CountRange(2, 5).ToDisplayString());
            Assert.AreEqual("Hello world", new Item_Text("Hello world").ToDisplayString());
            Assert.AreEqual("", new Item_Text((string)null).ToDisplayString());
            Assert.AreEqual("2019-04-15T08:47:12Z", new Item_TimeInstant(timeValue1).ToDisplayString());
            Assert.AreEqual("2019-04-15T08:47:12Z..2019-04-15T08:49:44Z", new Item_TimeRange(timeValue1, timeValue2).ToDisplayString());

            // Asserting measurement and measurement range. Assuming a precision of three decimals and a period
            // as the decimal separator.
            Assert.AreEqual("4.2 kg", new Item_Measurement("kg", 4.2).ToDisplayString());
            Assert.AreEqual("4.222 kg", new Item_Measurement("kg", 4.222001).ToDisplayString());
            Assert.AreEqual("4.1..4.513 cm", new Item_MeasurementRange("cm", 4.1, 4.51251).ToDisplayString());

            // Asserting array
            var array = new Item_Array(new List<Item_Array.ArrayColumn>
            {
                new Item_Array.ArrayColumn(typeof(string), "col1"),
                new Item_Array.ArrayColumn(typeof(string), "col2")
            })
            {
                { "a", "b" },
                { "c", "d" },
                { "e", "f" }
            };
            Assert.AreEqual("Array 3x2", array.ToDisplayString());

            // Asserting data record
            var dataRecord = new Item_DataRecord()
            {
                {"my_text", new Item_Text("My text") },
                {"my_count", new Item_Count(4) }
            };
            Assert.AreEqual("Data record (2 fields)", dataRecord.ToDisplayString());

            // Asserting time series (constant interval)
            var baseTimeForTimeSeries = System.Xml.XmlConvert.ToDateTime("2019-04-15T08:47:12Z", System.Xml.XmlDateTimeSerializationMode.Utc);
            var timeSeriesConstant = new Item_TimeSeriesConstant("Cel", baseTimeForTimeSeries, TimeSpan.FromMinutes(5))
            {
                44.4, 41.2, 40.3
            };
            Assert.AreEqual("Time series (3 values)", timeSeriesConstant.ToDisplayString());

            // Asserting time series (flexible interval)
            var timeSeriesFlexible = new Item_TimeSeriesFlexible("cm")
            {
                { ParseDateTimeInUtc("2019-04-15T08:47:12Z"), 11.3 },
                { ParseDateTimeInUtc("2019-04-15T08:48:12Z"), 12.1 },
                { ParseDateTimeInUtc("2019-04-15T08:49:12Z"), 12.9 },
                { ParseDateTimeInUtc("2019-04-15T08:50:12Z"), 13.1 }
            };
            Assert.AreEqual("Time series (4 values)", timeSeriesFlexible.ToDisplayString());
        }

        #endregion ToDisplayString


        #region Boolean

        [TestMethod]
        public void Boolean_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Boolean.xml";

            // Getting result element
            var resultEl = (bool)ParseRawResult(filepath);

            var testObject = new Item_Boolean(resultEl);

            // Asserting
            Assert.IsFalse(testObject.Value);
        }

        [TestMethod]
        public void Boolean_Create()
        {
            Item_Boolean testObject1 = new Item_Boolean(true);
            Item_Boolean testObject2 = new Item_Boolean(false);

            // Serialising and deserialising the test object
            Item_Boolean testObjectIn1 = (Item_Boolean)SerialiseAndReadResultObj(testObject1, XNeut.Helper.TypeUri_Truth);
            Item_Boolean testObjectIn2 = (Item_Boolean)SerialiseAndReadResultObj(testObject2, XNeut.Helper.TypeUri_Truth);

            // Asserting
            Assert.IsTrue(testObjectIn1.Value);
            Assert.IsFalse(testObjectIn2.Value);
        }

        #endregion


        #region Category

        [TestMethod]
        public void Category_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Category.xml";

            // Getting result element
            var resultEl = (XsdNs.ReferenceType)ParseRawResult(filepath);

            var testObject = new Item_Category(resultEl);

            // Checking results
            Assert.AreEqual("some<&<<ctg", testObject.Value);
        }

        [TestMethod]
        public void Category_Read_Null()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Category_empty.xml";

            // Getting result element
            var resultEl = (XsdNs.ReferenceType)ParseRawResult(filepath);

            var testObject = new Item_Category(resultEl);

            // Checking results
            Assert.IsTrue(string.IsNullOrEmpty(testObject.Value));
        }

        [TestMethod]
        public void Category_Create()
        {
            var categoryRef = "http://somecategory><sf>";
            Item_Category testObject = new Item_Category(categoryRef);

            // Serialising and deserialising the test object
            Item_Category testObjectIn = (Item_Category)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Category);

            // Checking results
            Assert.AreEqual(categoryRef, testObjectIn.Value);
        }

        [TestMethod]
        public void Category_Create_SpaceInValue()
        {
            var myAction = new Action(() =>
            {
                var categoryRef = "dsf sdf";
                new Item_Category(categoryRef);
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Category string must not contain spaces");
        }

        [TestMethod]
        public void Category_Create_Null()
        {
            var testObject = new Item_Category((string)null);

            // Serialising and deserialising the test object
            var testObjectIn = (Item_Category)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Category);

            // Checking results
            Assert.IsTrue(string.IsNullOrEmpty(testObjectIn.Value));
        }

        #endregion


        #region CategoryRange

        [TestMethod]
        public void CategoryRange_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_CategoryRange.xml";

            // Getting result element
            var resultEl = (XsdNs.CategoryRangeType)ParseRawResult(filepath);

            var testObject = new Item_CategoryRange(resultEl);

            // Checking results
            Assert.AreEqual("process_phase_b", testObject.LowerBound);
            Assert.AreEqual("process_phase_c", testObject.UpperBound);
        }
        
        [TestMethod]
        public void CategoryRange_Read_TooManyValues()
        {
            var myAction = new Action(() =>
            {
                var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_Item_CategoryRange_TooManyValues.xml";

                var resultEl = (XsdNs.CategoryRangeType)ParseRawResult(filepath);
                new Item_CategoryRange(resultEl);
            });

            // Asserting exception
            AssertInvalidMessageException(myAction, "Expected exactly 2 values");
        }

        [TestMethod]
        public void CategoryRange_Create()
        {
            // Creating the object to be serialised
            var testObject = new Item_CategoryRange("F", "L");

            // Serialising and deserialising
            var testObjectIn = (Item_CategoryRange)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_CategoryRange);

            // Asserting
            Assert.AreEqual("F", testObjectIn.LowerBound);
            Assert.AreEqual("L", testObjectIn.UpperBound);
        }

        [TestMethod]
        public void CategoryRange_Create_SpaceInValue()
        {
            var myAction = new Action(() =>
            {
                // Space in category value -> exception expected
                new Item_CategoryRange("F", "L f");
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Category string must not contain spaces");
        }

        [TestMethod]
        public void CategoryRange_Create_Null()
        {
            var myAction = new Action(() => {
                new Item_CategoryRange(null, null);
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Range must have two specified bounds");
        }

        [TestMethod]
        public void CategoryRange_Create_OneIsEmpty()
        {
            var myAction = new Action(() => {
                new Item_CategoryRange("", "sff");
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Range must have two specified bounds");
        }

        #endregion CategoryRange


        #region Count

        [TestMethod]
        public void Count_TestReadXml()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Count.xml";

            // Getting result element
            var resultEl = (string)ParseRawResult(filepath);

            var testObject = new Item_Count(resultEl);

            // Asserting
            Assert.AreEqual(20, testObject.Value);
        }

        [TestMethod]
        public void Count_TestCreateXml()
        {
            var testObject = new Item_Count(313);

            // Serialising and deserialising the test object
            var testObjectIn = (Item_Count)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Count);

            // Asserting
            Assert.AreEqual(313, testObjectIn.Value);
        }

        #endregion


        #region CountRange

        [TestMethod]
        public void CountRange_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_CountRange.xml";
            
            // Getting result element
            var resultEl = (XsdNs.CountRangeType)ParseRawResult(filepath);

            var testObject = new Item_CountRange(resultEl);

            // Checking results
            Assert.AreEqual(3, testObject.LowerBound);
            Assert.AreEqual(5, testObject.UpperBound);
        }

        [TestMethod]
        public void CountRange_Read_TooManyValues()
        {
            var myAction = new Action(() =>
            {
                var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_Item_CountRange_TooManyValues.xml";

                var resultEl = (XsdNs.CountRangeType)ParseRawResult(filepath);
                new Item_CountRange(resultEl);
            });

            // Asserting exception
            AssertInvalidMessageException(myAction, "Expected exactly 2 values");
        }

        [TestMethod]
        public void CountRange_Create()
        {
            // Creating the object to be serialised
            var testObject = new Item_CountRange(-534, 265);

            // Serialising and deserialising
            var testObjectIn = (Item_CountRange)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_CountRange);

            // Asserting
            Assert.AreEqual(-534, testObjectIn.LowerBound);
            Assert.AreEqual(265, testObjectIn.UpperBound);
        }

        #endregion CountRange


        #region DataRecord

        [TestMethod]
        public void DataRecord_ReadEmpty()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_DataRecord_empty.xml";

            // Getting result element
            var resultEl = (XsdNs.DataRecordPropertyType)ParseRawResult(filepath);

            Item_DataRecord testObject = new Item_DataRecord(resultEl);
            Assert.AreEqual(0, testObject.ItemNames.Count);
        }

        [TestMethod]
        public void DataRecord_Read()
        {
            // This case also tests data quality
            
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_DataRecord.xml";

            // Getting result element
            var resultEl = (XsdNs.DataRecordPropertyType)ParseRawResult(filepath);

            Item_DataRecord testObject = new Item_DataRecord(resultEl);

            // Asserting identifier
            Assert.AreEqual("my-identifier", testObject.Identifier);

            // Getting fields
            var fieldBool1 = (Item_Boolean)testObject["SomeBoolean1"];
            var fieldBool2 = (Item_Boolean)testObject["SomeBoolean2"];
            var fieldCategory1 = (Item_Category)testObject["SomeCategory1"];
            var fieldCategoryRange1 = (Item_CategoryRange)testObject["SomeCategoryRange1"];
            var fieldCount1 = (Item_Count)testObject["SomeCount1"];
            var fieldCountRange1 = (Item_CountRange)testObject["SomeCountRange1"];
            var fieldMeas1 = (Item_Measurement)testObject["SomeMeasurement1"];
            var fieldMeasurementRange1 = (Item_MeasurementRange)testObject["SomeMeasurementRange1"];
            var fieldText1 = (Item_Text)testObject["SomeText1"];
            var fieldTimeInstant1 = (Item_TimeInstant)testObject["SomeTimeInstant1"];
            var fieldTimePeriod1 = (Item_TimeRange)testObject["SomeTimePeriod1"];
            
            // Boolean (two values in case of a default value that would match the assertion)
            Assert.IsFalse(fieldBool1.Value);
            Assert.IsTrue(fieldBool2.Value);

            // Category
            Assert.AreEqual("process_step_a", fieldCategory1.Value);

            // CategoryRange
            Assert.AreEqual("step_c", fieldCategoryRange1.LowerBound);
            Assert.AreEqual("step_d", fieldCategoryRange1.UpperBound);

            // Count
            Assert.AreEqual(442, fieldCount1.Value);

            // CountRange
            Assert.AreEqual(45, fieldCountRange1.LowerBound);
            Assert.AreEqual(89, fieldCountRange1.UpperBound);

            // Measurement
            Assert.AreEqual("kg", fieldMeas1.UnitOfMeasure);
            Assert.AreEqual(5.6, fieldMeas1.Value, 0.0001);

            // MeasurementRange
            Assert.AreEqual("cm", fieldMeasurementRange1.UnitOfMeasure);
            Assert.AreEqual(2.6, fieldMeasurementRange1.LowerBound);
            Assert.AreEqual(3.4, fieldMeasurementRange1.UpperBound);

            // Text
            Assert.AreEqual("Some text", fieldText1.Value);

            // TimeInstant
            AssertDateTime(ParseDateTimeInUtc("2018-03-02T10:17:00Z"), fieldTimeInstant1.Value);

            // TimePeriod
            AssertDateTime(ParseDateTimeInUtc("2018-03-09T15:17:00Z"), fieldTimePeriod1.Start);
            AssertDateTime(ParseDateTimeInUtc("2018-03-09T15:18:09Z"), fieldTimePeriod1.End);
            
            // Nested DataRecord
            Item_DataRecord nestedDataRecord = (Item_DataRecord)testObject["NestedDataRecord"];
            Item_Measurement nestedMeasurement = (Item_Measurement)nestedDataRecord["NestedMeasurement"];
            Assert.AreEqual("m", nestedMeasurement.UnitOfMeasure);
            Assert.AreEqual(22.44, nestedMeasurement.Value, 0.0001);

            // Time series (constant interval); not asserting contents because Item_TimeSeries is tested elsewhere
            var timeseriesConstant = (Item_TimeSeriesConstant)testObject["TimeSeriesConstant"];
            Assert.AreEqual(4, timeseriesConstant.ValueCount); // Expecting 4 items in this time series

            // Time series (flexible interval)
            var timeseriesFlexible = (Item_TimeSeriesFlexible)testObject["TimeSeriesFlexible"];
            Assert.AreEqual(3, timeseriesFlexible.ValueCount); // Expecting 3 items in this time series

            // Array
            var array = (Item_Array)testObject["SomeArray"];
            Assert.AreEqual(2, array.Columns.Count);
            Assert.AreEqual(3, array.RowCount);
            Assert.AreEqual("MeasurementCol", array.Columns[0].Name);
            Assert.AreEqual("cm", array.Columns[0].UnitOfMeasure);
            Assert.AreEqual(-0.057, (double)array[2][0], 0.00001);
            Assert.AreEqual("Hello 3", array[2][1]);

            // Asserting data quality (for the types that support it)
            Assert.IsTrue(testObject.GetQualityOfItem("SomeBoolean1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeCategory1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeCategoryRange1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeCount1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeCountRange1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeMeasurement1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeMeasurementRange1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeText1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeTimeInstant1").IsGood);
            Assert.IsTrue(testObject.GetQualityOfItem("SomeTimePeriod1").IsGood);

            Assert.IsFalse(testObject.GetQualityOfItem("SomeBoolean2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeCategory2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeCategoryRange2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeCount2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeCountRange2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeMeasurement2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeMeasurementRange2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeText2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeTimeInstant2").IsGood);
            Assert.IsFalse(testObject.GetQualityOfItem("SomeTimePeriod2").IsGood);
        }

        [TestMethod]
        public void DataRecord_CreateEmpty()
        {
            // According to the SWE XML schema, a data record must not be empty.
            // However, for maximal robustness, this application allows empty
            // data records.

            var testObject = new Item_DataRecord();
            var testObjectIn = (Item_DataRecord)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Complex);
            
            Assert.AreEqual(0, testObjectIn.ItemNames.Count);
        }

        [TestMethod]
        public void DataRecord_Create_SimpleItems()
        {
            // Testing the (de)serialisation of simple items. Complex items are tested elsewhere.
            // Also testing data quality.
            
            // Creating a time range
            DateTime start = ParseDateTimeInUtc("2018-03-08T23:01:44Z");
            DateTime end = ParseDateTimeInUtc("2018-03-09T07:32:30Z");
            Item_TimeRange timeRangeItem = new Item_TimeRange(start, end);

            // Creating the actual test object
            Item_DataRecord testObject = new Item_DataRecord()
            {
                // Boolean (including bad data quality)
                { "FBool1", new Item_Boolean(false) },
                { "FBool2", new Item_Boolean(true), DataQuality.CreateBad() },

                // Category
                { "FCategory", new Item_Category("ctg_xyz") },

                // Category range
                { "FCategoryRange", new Item_CategoryRange("ctg_b", "ctg_c") },

                // Count (with explicitly good data quality)
                { "FCount", new Item_Count(42), DataQuality.CreateGood() },

                // Count range
                { "FCountRange", new Item_CountRange(0, 345) },

                // Measurement
                { "FMeasurement", new Item_Measurement("s", 45.3) },

                // Measurement range
                { "FMeasurementRange", new Item_MeasurementRange("m", 0.5, 0.8) },

                // Text
                { "FText", new Item_Text("This is my text") },

                // Time instant
                { "FTimeInstant", new Item_TimeInstant(ParseDateTimeInUtc("2018-03-02T14:22:05Z")) },

                // Time range
                { "FTimePeriod", timeRangeItem }
            };

            // Assigning an identifier
            testObject.Identifier = "some-identifier";


            // Serialising and deserialising the test object
            Item_DataRecord testObjectIn = (Item_DataRecord)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Complex);


            // Asserting

            Assert.AreEqual("some-identifier", testObjectIn.Identifier);

            Assert.IsTrue(testObjectIn.ItemNames.Contains("FBool1"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FBool2"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FCategory"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FCategoryRange"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FCount"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FCountRange"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FMeasurement"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FMeasurementRange"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FTimeInstant"));

            // Boolean (two values in case of a default value that would match the assertion)
            Item_Boolean fieldBool1 = (Item_Boolean)testObjectIn["FBool1"];
            Item_Boolean fieldBool2 = (Item_Boolean)testObjectIn["FBool2"];
            Assert.IsFalse(fieldBool1.Value);
            Assert.IsTrue(fieldBool2.Value);

            // Category
            Item_Category fieldCategory = (Item_Category)testObjectIn["FCategory"];
            Assert.AreEqual("ctg_xyz", fieldCategory.Value);

            // Category range
            var fieldCtgRange = (Item_CategoryRange)testObjectIn["FCategoryRange"];
            Assert.AreEqual("ctg_b", fieldCtgRange.LowerBound);
            Assert.AreEqual("ctg_c", fieldCtgRange.UpperBound);

            // Count
            Item_Count fieldCount = (Item_Count)testObjectIn["FCount"];
            Assert.AreEqual(42, fieldCount.Value);

            // Count range
            var fieldCountRange = (Item_CountRange)testObjectIn["FCountRange"];
            Assert.AreEqual(0, fieldCountRange.LowerBound);
            Assert.AreEqual(345, fieldCountRange.UpperBound);

            // Measurement
            Item_Measurement fieldMeas = (Item_Measurement)testObjectIn["FMeasurement"];
            Assert.AreEqual("s", fieldMeas.UnitOfMeasure);
            Assert.AreEqual(45.3, fieldMeas.Value, 0.0001);

            // Measurement range
            var fieldMeasRange = (Item_MeasurementRange)testObjectIn["FMeasurementRange"];
            Assert.AreEqual("m", fieldMeasRange.UnitOfMeasure);
            Assert.AreEqual(0.5, fieldMeasRange.LowerBound);
            Assert.AreEqual(0.8, fieldMeasRange.UpperBound);

            // Text
            Item_Text fieldText = (Item_Text)testObjectIn["FText"];
            Assert.AreEqual("This is my text", fieldText.Value);

            // TimeInstant
            Item_TimeInstant fieldTimeInstant = (Item_TimeInstant)testObjectIn["FTimeInstant"];
            AssertDateTime(ParseDateTimeInUtc("2018-03-02T14:22:05Z"), fieldTimeInstant.Value);

            // TimePeriod
            Item_TimeRange fieldTimePeriod = (Item_TimeRange)testObjectIn["FTimePeriod"];
            AssertDateTime(ParseDateTimeInUtc("2018-03-08T23:01:44Z"), fieldTimePeriod.Start);
            AssertDateTime(ParseDateTimeInUtc("2018-03-09T07:32:30Z"), fieldTimePeriod.End);
            
            // Asserting data quality. The value "bad" appears only once as
            // (1) reading quality information from various types is tested in the "read" test and
            // (2) quality information is written in Item_DataRecord class for all item types, so 
            // the implementation is the same for all

            Assert.IsTrue(testObjectIn.GetQualityOfItem("FBool1").IsGood);
            Assert.IsFalse(testObjectIn.GetQualityOfItem("FBool2").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FCategory").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FCount").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FMeasurement").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FMeasurementRange").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FText").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FTimeInstant").IsGood);
            Assert.IsTrue(testObjectIn.GetQualityOfItem("FTimePeriod").IsGood);
        }

        [TestMethod]
        public void DataRecord_Create_ComplexItems()
        {
            // Testing the (de)serialisation of complex items. Simple items are tested elsewhere.
            // Not testing data quality, because a complex item cannot have a data quality
            // in a data record. In some complex types, the enclosed simple items can have data quality.


            // Creating a data record to be nested
            Item_DataRecord nestedRecord = new Item_DataRecord
            {
                { "FNestedMeasurement", new Item_Measurement("m", -0.34) }
            };

            // Creating a time series with a constant interval.
            var timeSeriesConst = new Item_TimeSeriesConstant("mm", DateTime.Now.ToUniversalTime(), TimeSpan.FromSeconds(1))
            {
                4, 4.3, 4.12
            };

            // Creating a time series with a flexible interval.
            var timeSeriesFlex = new Item_TimeSeriesFlexible("mm")
            {
                { DateTime.Now.ToUniversalTime(), 5.12 },
                { DateTime.Now.ToUniversalTime().AddHours(1), 5.31 }
            };

            // Creating an array
            var arrayColumns = new List<Item_Array.ArrayColumn>()
            {
                new Item_Array.ArrayColumn(typeof(long), "meas_index"),
                new Item_Array.ArrayColumn(typeof(double), "thickness", "mm")
            };
            var array = new Item_Array(arrayColumns)
            {
                { (long)15, 3.22 }
            };

            // Creating the actual test object
            Item_DataRecord testObject = new Item_DataRecord()
            {
                // Nested data record
                { "FNestedRecord", nestedRecord },

                // Time series (constant)
                { "FTimeSeriesConst", timeSeriesConst },

                // Time series (flexible)
                { "FTimeSeriesFlex", timeSeriesFlex },

                // Array
                { "FArray", array }
            };

            testObject.Identifier = "some-identifier-2";


            // Serialising and deserialising the test object
            Item_DataRecord testObjectIn = (Item_DataRecord)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Complex);


            // Asserting.

            Assert.AreEqual("some-identifier-2", testObjectIn.Identifier);

            Assert.IsTrue(testObjectIn.ItemNames.Contains("FNestedRecord"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FTimeSeriesConst"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FTimeSeriesFlex"));
            Assert.IsTrue(testObjectIn.ItemNames.Contains("FArray"));

            // Nested DataRecord
            Item_DataRecord nestedDataRecordIn = (Item_DataRecord)testObjectIn["FNestedRecord"];
            Item_Measurement nestedMeasurement = (Item_Measurement)nestedDataRecordIn["FNestedMeasurement"];
            Assert.AreEqual("m", nestedMeasurement.UnitOfMeasure);
            Assert.AreEqual(-0.34, nestedMeasurement.Value, 0.0001);

            // Time series (constant interval); only asserting one value because Item_TimeSeriesConstant is tested elsewhere
            var timeSeriesConstIn = (Item_TimeSeriesConstant)testObjectIn["FTimeSeriesConst"];
            Assert.AreEqual(4.12, timeSeriesConstIn.GetValue(2));

            // Time series (flexible interval); only asserting one value because Item_TimeSeriesFlexible is tested elsewhere
            var timeSeriesFlexIn = (Item_TimeSeriesFlexible)testObjectIn["FTimeSeriesFlex"];
            Assert.AreEqual(5.31, timeSeriesFlexIn.GetValue(1));

            // Array. Only asserting a few fields, because Item_Array is tested thoroughly elsewhere.
            var arrayIn = (Item_Array)testObjectIn["FArray"];
            Assert.AreEqual(2, arrayIn.Columns.Count);
            Assert.AreEqual(1, arrayIn.RowCount);
            Assert.AreEqual("meas_index", arrayIn.Columns[0].Name);
            Assert.AreEqual("mm", arrayIn.Columns[1].UnitOfMeasure);
            Assert.AreEqual(3.22, (double)arrayIn[0][1], 0.00001);
        }

        [TestMethod]
        public void DataRecord_QualityCheck()
        {
            var myAction = new Action(() =>
            {
                // Trying to associate data quality to a data record field that does not support it

                Item_DataRecord record = new Item_DataRecord();
                Item_DataRecord nestedRecord = new Item_DataRecord();

                record.Add("SomeItem", nestedRecord, DataQuality.CreateGood());
            });

            // Asserting exception
            AssertInvalidMessageException(myAction, "\"SomeItem\": the item type does not support data quality");
        }

        [TestMethod]
        public void DataRecord_CollInit()
        {
            var record = new Item_DataRecord()
            {
                { "ItemA", new Item_Count(12) },
                { "ItemB", new Item_Measurement("Cel", 45.2), DataQuality.CreateBad() }
            };

            var countA = (Item_Count)record["ItemA"];
            var measB = (Item_Measurement)record["ItemB"];
            var dqA = record.GetQualityOfItem("ItemA");
            var dqB = record.GetQualityOfItem("ItemB");

            Assert.AreEqual(12, countA.Value);
            Assert.AreEqual(45.2, measB.Value, 0.001);
            Assert.IsTrue(dqA.IsGood);
            Assert.IsFalse(dqB.IsGood);
        }

        [TestMethod]
        public void DataRecord_CollInit_dataQual_negative()
        {
            var myAction = new Action(() =>
            {
                // Trying to associate data quality to a data record field that does not support it

                new Item_DataRecord()
                {
                    { "ItemA", new Item_DataRecord(), DataQuality.CreateBad() },
                };
            });

            // Asserting exception
            AssertInvalidMessageException(myAction, "\"ItemA\": the item type does not support data quality");
        }

        [TestMethod]
        public void DataRecord_ReadFromXmlNodeObj()
        {
            // Testing the structuring of a data record from XmlNodes to proxies.
            // Not testing individual fields and their types thoroughly.

            // Reading an XML document from a file
            var xmlDoc = new SXml.XmlDocument();
            xmlDoc.Load(TestCommon.TestHelper.TestFileFolder + "\\Item_DataRecord.xml");

            // Supplying the data record node to the constructor
            var resultFromXpath = xmlDoc.SelectSingleNode("//*[local-name()='result']/*[local-name()='DataRecord']");
            var testObject = new Item_DataRecord(new SXml.XmlNode[] { resultFromXpath });

            // Asserting a measurement
            var fieldMeas1 = (Item_Measurement)testObject["SomeMeasurement1"];
            Assert.AreEqual("kg", fieldMeas1.UnitOfMeasure);
            Assert.AreEqual(5.6, fieldMeas1.Value, 0.0001);

            // Asserting a TimeInstant
            var fieldTimeInstant1 = (Item_TimeInstant)testObject["SomeTimeInstant1"];
            AssertDateTime(ParseDateTimeInUtc("2018-03-02T10:17:00Z"), fieldTimeInstant1.Value);
        }

        [TestMethod]
        public void DataRecord_WriteToXmlProxy()
        {
            // Test the generation of an XML proxy from a data record.
            // First, write the generated XmlNodes to a XML document
            // and then read them back.

            // 1) Generate XML data
            var xmlBytes = GenerateProxyFromDataRecord();

            // 2) Read to XmlDocument
            var xmlString = System.Text.Encoding.UTF8.GetString(xmlBytes);
            xmlString = xmlString.TrimStart(new char[] { '\uFEFF' }); // Strip UTF-8 byte order mark
            var xmlDoc = new SXml.XmlDocument();
            xmlDoc.LoadXml(xmlString);

            // 3) Read the data record back
            var dataRecordIn = new Item_DataRecord(new SXml.XmlNode[] { xmlDoc.DocumentElement });

            // 4) Assert the content of the data record
            var measItemIn = (Item_Measurement)dataRecordIn["MyMeas"];
            var timeItemIn = (Item_TimeInstant)dataRecordIn["MyTime"];
            Assert.AreEqual(0.44, measItemIn.Value, 0.0001);
            Assert.AreEqual("t", measItemIn.UnitOfMeasure);
            AssertDateTime(ParseDateTimeInUtc("2020-02-13T11:51:00Z"), timeItemIn.Value);
        }

        private byte[] GenerateProxyFromDataRecord()
        {
            // Have a separate function to avoid accidentally asserting the
            // original data record object

            // 1) Create a data record
            var dataRecord = new Item_DataRecord()
            {
                { "MyMeas", new Item_Measurement("t", 0.44) },
                { "MyTime", new Item_TimeInstant(ParseDateTimeInUtc("2020-02-13T11:51:00Z")) }
            };

            // 2) Generate XML proxy
            var recordProxy = dataRecord.ToDataRecordPropertyProxy().DataRecord;

            // 3) Serialise to XML
            using (var xmlStream = new System.IO.MemoryStream())
            {
                using (var xmlWriter = new SXml.XmlTextWriter(xmlStream, System.Text.Encoding.UTF8))
                {
                    // Serialising
                    var serializer = new SXml.Serialization.XmlSerializer(typeof(XsdNs.DataRecordType));
                    serializer.Serialize(xmlWriter, recordProxy);
                    
                    var bytes = xmlStream.ToArray();
                    return bytes;
                }
            }
        }

        #endregion


        #region Measurement

        [TestMethod]
        public void Measurement_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Measurement.xml";

            // Getting result element
            var resultEl = (XsdNs.MeasureType)ParseRawResult(filepath);

            Item_Measurement testObject = new Item_Measurement(resultEl);

            // Checking unit of measure
            Assert.AreEqual("t", testObject.UnitOfMeasure);

            // Checking measurement value
            double measValue = testObject.Value;
            Assert.IsTrue(Math.Abs(measValue - 20.3) < 0.0001);
        }

        [TestMethod]
        public void Measurement_Create()
        {
            Item_Measurement testObject = new Item_Measurement("t", 22.7);

            // Serialising and deserialising the test object
            Item_Measurement testObjectIn = (Item_Measurement)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Measurement);

            // Asserting
            Assert.AreEqual<string>(testObject.UnitOfMeasure, testObjectIn.UnitOfMeasure);
            Assert.IsTrue(Math.Abs(testObject.Value - testObjectIn.Value) < 0.0001);
        }

        #endregion


        #region MeasurementRange

        [TestMethod]
        public void MeasurementRange_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_MeasurementRange.xml";

            // Getting result element
            var resultEl = (XsdNs.QuantityRangeType)ParseRawResult(filepath);
            
            var testObject = new Item_MeasurementRange(resultEl);

            // Checking unit of measure
            Assert.AreEqual("Cel", testObject.UnitOfMeasure);

            // Checking measurement values
            Assert.IsTrue(Math.Abs(testObject.LowerBound - 17.2) < 0.0001);
            Assert.IsTrue(Math.Abs(testObject.UpperBound - 18.1) < 0.0001);
        }

        [TestMethod]
        public void MeasurementRange_Read_TooManyValues()
        {
            var myAction = new Action(() =>
            {
                var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_Item_MeasurementRange_TooManyValues.xml";
                
                var resultEl = (XsdNs.QuantityRangeType)ParseRawResult(filepath);
                new Item_MeasurementRange(resultEl);
            });

            // Asserting exception
            AssertInvalidMessageException(myAction, "Expected exactly 2 values");
        }

        [TestMethod]
        public void MeasurementRange_Create()
        {
            // Creating the object to be serialised
            var testObject = new Item_MeasurementRange("W", 55.4, 65.3);

            // Serialising and deserialising
            var testObjectIn = (Item_MeasurementRange)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_MeasurementRange);

            // Asserting
            Assert.AreEqual("W", testObjectIn.UnitOfMeasure);
            Assert.AreEqual(55.4, testObjectIn.LowerBound);
            Assert.AreEqual(65.3, testObjectIn.UpperBound);
        }

        #endregion


        #region Text

        [TestMethod]
        public void Text_TestReadXml()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Text.xml";

            // Getting result element
            var resultEl = (string)ParseRawResult(filepath);

            var testObject = new Item_Text(resultEl);

            // Asserting
            Assert.AreEqual("This is my string", testObject.Value);
        }

        [TestMethod]
        public void Text_TestCreateXml()
        {
            var testObject = new Item_Text("Hello universum");

            // Serialising and deserialising the test object
            var testObjectIn = (Item_Text)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Text);

            // Asserting
            Assert.AreEqual("Hello universum", testObjectIn.Value);
        }

        #endregion


        #region TimeInstant

        [TestMethod]
        public void TimeInstant_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_TimeInstant.xml";

            // Getting result element
            var resultEl = (XsdNs.TimeInstantPropertyType)ParseRawResult(filepath);

            Item_TimeInstant testObject = new Item_TimeInstant(resultEl.TimeInstant);

            // Asserting
            AssertDateTime(ParseDateTimeInUtc("2020-02-21T12:44:00Z"), testObject.Value);
        }

        [TestMethod]
        public void TimeInstant_Create()
        {
            DateTime dateTime = SXml.XmlConvert.ToDateTime("2020-03-24T10:00:00Z", SXml.XmlDateTimeSerializationMode.Utc);
            Item_TimeInstant testObject = new Item_TimeInstant(dateTime);

            // Serialising and deserialising the test object
            Item_TimeInstant testObjectIn = (Item_TimeInstant)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Temporal);

            // Asserting
            AssertDateTime(dateTime, testObjectIn.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(XNeut.DateTimeException))]
        public void TimeInstant_Create_BadKind()
        {
            // Using an invalid DateTime kind
            var input = ParseDateTimeInUtc("2020-02-17T13:38:00Z");
            new Item_TimeInstant(input.ToLocalTime());
        }

        #endregion


            #region TimeRange

        [TestMethod]
        public void TimeRange_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_TimeRange.xml";

            // Getting result element
            var resultEl = (XsdNs.TimePeriodPropertyType)ParseRawResult(filepath);

            Item_TimeRange testObject = new Item_TimeRange(resultEl);

            // Asserting
            AssertDateTime(ParseDateTimeInUtc("2020-02-21T12:43:00Z"), testObject.Start);
            AssertDateTime(ParseDateTimeInUtc("2020-02-21T12:44:00Z"), testObject.End);
        }

        [TestMethod]
        public void TimeRange_Create()
        {
            DateTime dateTimeStart = ParseDateTimeInUtc("2020-03-24T10:00:00Z");
            DateTime dateTimeEnd = dateTimeStart.AddSeconds(13);

            // Testing end before start (error expected)
            var myAction = new Action(() =>
            {
                new Item_TimeRange(dateTimeEnd, dateTimeStart);
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Time period: end time must not be before start");

            // Creating the actual test object for serialisation
            var testObject = new Item_TimeRange(dateTimeStart, dateTimeEnd);

            // Serialising and deserialising the test object
            Item_TimeRange testObjectIn = (Item_TimeRange)SerialiseAndReadResultObj(testObject, XNeut.Helper.TypeUri_Temporal);

            // Asserting
            AssertDateTime(dateTimeStart, testObjectIn.Start);
            AssertDateTime(dateTimeEnd, testObjectIn.End);
        }

        [TestMethod]
        [ExpectedException(typeof(XNeut.DateTimeException))]
        public void TimeRange_Create_BadStart()
        {
            DateTime dateTimeStart = ParseDateTimeInUtc("2020-03-24T10:00:00Z");
            DateTime dateTimeEnd = dateTimeStart.AddSeconds(13);
            dateTimeStart = dateTimeStart.ToLocalTime(); // This is local -> bad

            new Item_TimeRange(dateTimeStart, dateTimeEnd);
        }

        [TestMethod]
        [ExpectedException(typeof(XNeut.DateTimeException))]
        public void TimeRange_Create_BadEnd()
        {
            DateTime dateTimeStart = DateTime.Parse("2020-03-24T10:00:00");
            DateTime dateTimeEnd = dateTimeStart.AddSeconds(13); // This is local -> bad
            dateTimeStart = dateTimeStart.ToUniversalTime();

            new Item_TimeRange(dateTimeStart, dateTimeEnd);
        }

        #endregion


        #region TimeSeries

        [TestMethod]
        public void TimeSeries_Read()
        {
            // This test case concentrates only on the Item_TimeSeries base class -> not asserting any timestamps of values

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\Item_TimeSeriesFlexible.xml";

            // Getting result element
            var resultEl = (XsdNs.TimeseriesDomainRangeType)ParseRawResult(filepath);

            // Reading the proxy
            var testObject_init = new Item_TimeSeriesFlexible(resultEl);
            var testObject = (Item_TimeSeries)testObject_init;

            // Checking the measurement unit
            Assert.AreEqual("m", testObject.UnitOfMeasure);

            // Asserting description
            Assert.AreEqual("Example", testObject.Description);

            // Assert item count
            Assert.AreEqual(4, testObject.ValueCount);

            // Checking measurement values
            Assert.AreEqual(2.03, testObject.GetValue(0), 0.0001);
            Assert.AreEqual(2.06, testObject.GetValue(1), 0.0001);
            Assert.AreEqual(2.42, testObject.GetValue(2), 0.0001);
            Assert.AreEqual(2.23, testObject.GetValue(3), 0.0001);
            
            // Checking data qualities
            Assert.IsTrue(testObject.GetDataQuality(0).IsGood);
            Assert.IsFalse(testObject.GetDataQuality(1).IsGood);
            Assert.IsTrue(testObject.GetDataQuality(2).IsGood);
            Assert.IsFalse(testObject.GetDataQuality(3).IsGood);
        }

        [TestMethod]
        public void TimeSeries_Create()
        {
            // This test case concentrates only on the Item_TimeSeries base class
            
            // Creating the actual test object for serialisation
            var originalObj = new Item_TimeSeriesFlexible("Cel")
            {
                { ParseDateTimeInUtc("2018-03-16T08:30:00Z"), -9.4, DataQuality.CreateGood() },
                { ParseDateTimeInUtc("2018-03-16T08:40:00Z"), -8.3, DataQuality.CreateGood() },
                { ParseDateTimeInUtc("2018-03-16T08:50:00Z"), -7, DataQuality.CreateBad() },
                { ParseDateTimeInUtc("2018-03-16T09:00:00Z"), -6.9, DataQuality.CreateGood() }
            };
            originalObj.Description = "Hello";

            // Serialising and deserialising the test object
            var parsedObj = (Item_TimeSeries)SerialiseAndReadResultObj(originalObj, XNeut.Helper.TypeUri_TimeSeriesFlexible);

            // Asserting item count and unit of measure
            Assert.AreEqual("Cel", parsedObj.UnitOfMeasure);
            Assert.AreEqual(4, parsedObj.ValueCount);

            // Asserting description
            Assert.AreEqual("Hello", parsedObj.Description);

            // Checking measurement values
            Assert.AreEqual(-9.4, parsedObj.GetValue(0), 0.0001);
            Assert.AreEqual(-8.3, parsedObj.GetValue(1), 0.0001);
            Assert.AreEqual(-7, parsedObj.GetValue(2), 0.0001);
            Assert.AreEqual(-6.9, parsedObj.GetValue(3), 0.0001);

            // Checking data qualities
            Assert.IsTrue(parsedObj.GetDataQuality(0).IsGood);
            Assert.IsTrue(parsedObj.GetDataQuality(1).IsGood);
            Assert.IsFalse(parsedObj.GetDataQuality(2).IsGood);
            Assert.IsTrue(parsedObj.GetDataQuality(3).IsGood);
        }

        #endregion


        #region TimeSeriesFlexible

        [TestMethod]
        public void TsFlex_Read()
        {
            // Only testing the features specific to Item_TimeSeriesFlexible

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\Item_TimeSeriesFlexible.xml";

            // Getting result element
            var resultEl = (XsdNs.TimeseriesDomainRangeType)ParseRawResult(filepath);

            // Reading the proxy
            var testObject = new Item_TimeSeriesFlexible(resultEl);

            var expectedTimeEntries = new List<DateTime>
            {
                ParseDateTimeInUtc("2020-01-01T00:00:00Z"),
                ParseDateTimeInUtc("2020-01-02T00:00:00Z"),
                ParseDateTimeInUtc("2020-01-03T00:00:00Z"),
                ParseDateTimeInUtc("2020-01-04T00:00:00Z")
            };

            // Checking that all datetime entries were parsed correctly
            for (int a = 0; a < expectedTimeEntries.Count; ++a)
            {
                AssertDateTime(expectedTimeEntries[a], testObject.GetTimestamp(a));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(XNeut.DateTimeException))]
        public void TsFlex_Create_BadDateTimeKind()
        {
            new Item_TimeSeriesFlexible("Cel")
            {
                { ParseDateTimeInUtc("2018-03-16T08:30:00Z").ToLocalTime(), -9.4 } // Wrong DateTime kind
            };
        }

        [TestMethod]
        public void TsFlex_Create()
        {
            // Only testing the features specific to Item_TimeSeriesFlexible

            DateTime timestamp1 = ParseDateTimeInUtc("2018-03-16T08:30:00Z");
            DateTime timestamp2 = ParseDateTimeInUtc("2018-03-16T08:40:00Z");
            DateTime timestamp3 = ParseDateTimeInUtc("2018-03-16T08:50:00Z");
            DateTime timestamp4 = ParseDateTimeInUtc("2018-03-16T09:00:00Z");

            // Creating the actual test object for serialisation
            var originalObj = new Item_TimeSeriesFlexible("Cel")
            {
                { timestamp1, -9.4 },
                { timestamp2, -8.3 },
                { timestamp3, -7, DataQuality.CreateBad() },
                { timestamp4, -6.9 }
            };
            originalObj.Description = "Hello";

            // Serialising and deserialising the test object
            var parsedObj = (Item_TimeSeriesFlexible)SerialiseAndReadResultObj(originalObj, XNeut.Helper.TypeUri_TimeSeriesFlexible);

            var expectedTimeEntries = new List<DateTime>
            {
                timestamp1,
                timestamp2,
                timestamp3,
                timestamp4
            };

            // Checking that all datetime entries were parsed correctly
            for (int a = 0; a < expectedTimeEntries.Count; ++a)
            {
                AssertDateTime(expectedTimeEntries[a], parsedObj.GetTimestamp(a));
            }
        }
        
        [TestMethod]
        public void TsFlex_CollInit()
        {
            var time1 = DateTime.Now.ToUniversalTime();
            var time2 = time1.AddSeconds(1);
            var time3 = time1.AddSeconds(2);
            
            var timeseries = new Item_TimeSeriesFlexible("m")
            {
                { time1, 0.1, DataQuality.CreateBad() },
                { time2, 0.2 },
                { time3, 0.3, DataQuality.CreateGood() }
            };

            Assert.AreEqual(time1.ToString(), timeseries.GetTimestamp(0).ToString());
            Assert.AreEqual(time2.ToString(), timeseries.GetTimestamp(1).ToString());
            Assert.AreEqual(time3.ToString(), timeseries.GetTimestamp(2).ToString());

            Assert.AreEqual(0.1, timeseries.GetValue(0), 0.001);
            Assert.AreEqual(0.2, timeseries.GetValue(1), 0.001);
            Assert.AreEqual(0.3, timeseries.GetValue(2), 0.001);

            Assert.IsFalse(timeseries.GetDataQuality(0).IsGood);
            Assert.IsTrue(timeseries.GetDataQuality(1).IsGood);
            Assert.IsTrue(timeseries.GetDataQuality(2).IsGood);
        }
        
        #endregion


        #region TimeSeriesConstant

        [TestMethod]
        public void TsConst_Read()
        {
            // Only testing the features specific to Item_TimeSeriesConstant

            var filepath = TestCommon.TestHelper.TestFileFolder + @"\Item_TimeSeriesConstant.xml";

            // Getting result element
            var resultEl = (XsdNs.TimeseriesDomainRangeType)ParseRawResult(filepath);

            // Reading the proxy
            var testObject = new Item_TimeSeriesConstant(resultEl);

            // Asserting basetime value
            var expectedBaseTime = ParseDateTimeInUtc("2020-02-21T12:00:00Z");
            AssertDateTime(expectedBaseTime, testObject.BaseTime);

            // Asserting spacing value
            var expectedSpacing = TimeSpan.FromHours(1);
            Assert.AreEqual(0, TimeSpan.Compare(expectedSpacing, testObject.Spacing));

            // Asserting value count
            Assert.AreEqual(4, testObject.ValueCount);
        }

        [TestMethod]
        [ExpectedException(typeof(XNeut.DateTimeException))]
        public void TsConst_Create_BadDateTimeKind()
        {
            var baseTime = ParseDateTimeInUtc("2018-06-26T09:13:44Z").ToLocalTime(); // Bad kind
            var spacing = TimeSpan.FromMinutes(30);

            var originalObj = new Item_TimeSeriesConstant("m", baseTime, spacing);
        }

        [TestMethod]
        public void TsConst_Create()
        {
            // Only testing the features specific to Item_TimeSeriesRegular.

            var baseTime = ParseDateTimeInUtc("2018-06-26T09:13:44Z");
            var spacing = TimeSpan.FromMinutes(30);

            var originalObj = new Item_TimeSeriesConstant("m", baseTime, spacing)
            {
                // Not adding many items because the base class already implements this.
                4,
                { 3.2, DataQuality.CreateBad() }
            };

            // Serialising and deserialising the test object
            var parsedObj = (Item_TimeSeriesConstant)SerialiseAndReadResultObj(originalObj, XNeut.Helper.TypeUri_TimeSeriesConstant);

            // Asserting basetime value
            AssertDateTime(baseTime, parsedObj.BaseTime);

            // Asserting spacing value
            Assert.AreEqual(0, TimeSpan.Compare(spacing, parsedObj.Spacing));

            // Asserting items
            Assert.AreEqual(2, parsedObj.ValueCount);
            Assert.AreEqual(4, parsedObj.GetValue(0), 0.001);
            Assert.AreEqual(3.2, parsedObj.GetValue(1), 0.001);
            Assert.IsTrue(parsedObj.GetDataQuality(0).IsGood);
            Assert.IsFalse(parsedObj.GetDataQuality(1).IsGood);
        }
        
        [TestMethod]
        public void TsConst_CollInit()
        {
            // Testing the collection initialiser

            var testItem = new Item_TimeSeriesConstant("Cel", DateTime.Now.ToUniversalTime(), TimeSpan.FromSeconds(1))
            {
                { 45.2 },
                { 45.4, DataQuality.CreateBad() },
                { 45.5, DataQuality.CreateGood() }
            };

            Assert.AreEqual(45.2, testItem.GetValue(0), 0.0001);
            Assert.AreEqual(45.4, testItem.GetValue(1), 0.0001);
            Assert.AreEqual(45.5, testItem.GetValue(2), 0.0001);

            Assert.IsTrue(testItem.GetDataQuality(0).IsGood);
            Assert.IsFalse(testItem.GetDataQuality(1).IsGood);
            Assert.IsTrue(testItem.GetDataQuality(2).IsGood);

            Assert.AreEqual(3, testItem.ValueCount);
        }

        #endregion


        #region Common methods
        
        private void AssertInvalidMessageException(Action testWorker, string expectedMessageStart)
        {
            // Expecting exception
            try
            {
                testWorker.Invoke();
                Assert.Fail("InvalidMessageException was expected");
            }
            catch (XNeut.InvalidMessageException e)
            {
                Assert.IsTrue(e.Message.StartsWith(expectedMessageStart), "Unexpected exception message " + e.Message);
            }
        }

        private Item SerialiseAndReadResultObj(Item testObject, string obsTypeUri)
        {
            // Using Observation class in serialisation.
            // A lot of redundant code would be required here to enable XML validation.
            Observation observation = new Observation(testObject);
            byte[] xmlBytes = observation.ToXmlBytes();

            //var xmlString = System.Text.Encoding.UTF8.GetString(xmlBytes);

            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Custom1_GmlOmSweTsml | TestCommon.Validator.SchemaType.Custom2);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.OM_ObservationType));
            }

            // Parsing the XML document (validation enabled)
            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                using (var reader = System.Xml.XmlReader.Create(xmlStream))
                {
                    // Deserialising the document
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XsdNs.OM_ObservationType));
                    var observationRaw = (XsdNs.OM_ObservationType)serializer.Deserialize(reader);

                    // Testing that result mapping after URI works as expected
                    object resultObj = observationRaw.result;
                    return ResultTypeManager.BuildResultFromXml(obsTypeUri, resultObj);
                }
            }
        }

        private object ParseRawResult(string filepath)
        {
            var xmlBytes = System.IO.File.ReadAllBytes(filepath);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XsdNs.OM_ObservationType));

            // Creating an XmlTextReader from the XML document
            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                using (var reader = System.Xml.XmlReader.Create(xmlStream))
                {
                    // Deserialising the file
                    var observationRaw = (XsdNs.OM_ObservationType)serializer.Deserialize(reader);

                    return observationRaw.result;
                }
            }
        }

        private void AssertDateTime(DateTime exp, DateTime act)
        {
            TestCommon.TestHelper.AssertDateTime(exp, act);
        }

        private DateTime ParseDateTimeInUtc(string s)
        {
            return TestCommon.TestHelper.ParseDateTimeInUtc(s);
        }
        
        #endregion
    }
}
