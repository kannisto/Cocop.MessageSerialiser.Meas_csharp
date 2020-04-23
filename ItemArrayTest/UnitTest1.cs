//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2/2019
// Last modified: 2/2020

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;
using Cocop.MessageSerialiser.Meas;

namespace ItemArrayTest
{
    [TestClass]
    public class UnitTest1
    {
        /*
	     * This test has its focus on the Item_Array class and its XML serialisation.
	     * 
	     * An appropriate XML validation is easiest with the enclosing Observation class.
	     * Due to the serialisation complexity of the observation class, it is included as 
	     * such instead of creating a redundant stub class.
         * 
         * Some test cases assert error messages, not only exception type. This is because
         * the same exception type could occur from multiple types of errors. There is
         * a danger that the expected exception stems from not the error that was tested
         * but another error.
	     */


        #region ArrayColumn only
        
        [TestMethod]
        public void ArrayColumn_Create_RecogniseSupportedTypes()
        {
            // Testing if the supported data types are recognised for columns
            
            // 1) Expecting success
            new Item_Array.ArrayColumn(typeof(double), "Foo", "m");
            new Item_Array.ArrayColumn(typeof(double), "Foo");
            new Item_Array.ArrayColumn(typeof(bool), "Foo");
            new Item_Array.ArrayColumn(typeof(DateTime), "Foo");
            new Item_Array.ArrayColumn(typeof(long), "Foo");
            new Item_Array.ArrayColumn(typeof(string), "Foo");

            // 2) Expecting failure
            ExpectExceptionFromType(typeof(byte));
            ExpectExceptionFromType(typeof(char));
            ExpectExceptionFromType(typeof(float));
            ExpectExceptionFromType(typeof(int));
            ExpectExceptionFromType(typeof(object));
            ExpectExceptionFromType(typeof(short));
            ExpectExceptionFromType(typeof(uint));
            ExpectExceptionFromType(typeof(ulong));
        }

        private void ExpectExceptionFromType(Type columnType)
        {
            var myAction = new Action(() =>
            {
                new Item_Array.ArrayColumn(columnType, "Foo");
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Unsupported column type");
        }

        [TestMethod]
        public void ArrayColumn_Create_UnitConflict()
        {
            // Only a "measure" (or double) column can take a unit of measure.

            // 1) Expecting success with the double type (no exception expected)
            new Item_Array.ArrayColumn(typeof(double), "Foo", "m");

            // 2) Checking that the others fail
            ExpectExceptionFromUnit(typeof(bool));
            ExpectExceptionFromUnit(typeof(DateTime));
            ExpectExceptionFromUnit(typeof(long));
            ExpectExceptionFromUnit(typeof(string));
        }

        private void ExpectExceptionFromUnit(Type columnType)
        {
            var myAction = new Action(() =>
            {
                // Only a "measure" (or double) column can take a unit of measure
                new Item_Array.ArrayColumn(columnType, "Foo", "m");
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Only measurements (doubles) can have a unit");
        }

        [TestMethod]
        public void ArrayColumn_Create_InvalidName()
        {
            // 1) Name is empty
            ExpectExceptionFromName(null);
            ExpectExceptionFromName("");

            // 2) Name contains spaces
            ExpectExceptionFromName("f f");
            ExpectExceptionFromName("f\nf");

            // 3) Name contains a colon
            ExpectExceptionFromName(":");
            ExpectExceptionFromName(":sf");
            ExpectExceptionFromName("sf:");
            ExpectExceptionFromName("sf:sf");
        }

        private void ExpectExceptionFromName(string name)
        {
            var myAction = new Action(() =>
            {
                // Only a "measure" (or double) column can take a unit of measure
                new Item_Array.ArrayColumn(typeof(double), name);
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Column name is mandatory and must be valid NCName");
	    }

        #endregion ArrayColumn only


        #region Array and ArrayColumn
        
        [TestMethod]
        public void Array_Read_NoRows()
        {
            // Testing if reading succeeds when the document has no rows

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Array_NoRows.xml";

            // Getting result element
            var resultEl = (XsdNs.DataArrayType)ParseRawResult(filepath);

            var testObject = new Item_Array(resultEl);

            // Asserting counts
            Assert.AreEqual(2, testObject.Columns.Count);
            Assert.AreEqual(0, testObject.RowCount);
        }

        [TestMethod]
        public void Array_Read_NoColumns()
        {
            // Testing if reading succeeds when the document has no columns

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Array_NoColumns.xml";

            // Getting result element
            var resultEl = (XsdNs.DataArrayType)ParseRawResult(filepath);

            var testObject = new Item_Array(resultEl);

            // Asserting counts
            Assert.AreEqual(0, testObject.Columns.Count);
            Assert.AreEqual(2, testObject.RowCount);
        }

        [TestMethod]
        public void Array_Read()
        {
            // Testing a typical read with all data types included

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Array.xml";

            // Getting result element
            var resultEl = (XsdNs.DataArrayType)ParseRawResult(filepath);

            var testObject = new Item_Array(resultEl);

            // Asserting counts
            Assert.AreEqual(6, testObject.Columns.Count);
            Assert.AreEqual(3, testObject.RowCount);

            var col1 = testObject.Columns[0];
            var col2 = testObject.Columns[1];
            var col3 = testObject.Columns[2];
            var col4 = testObject.Columns[3];
            var col5 = testObject.Columns[4];
            var col6 = testObject.Columns[5];

            // Asserting columns types
            Assert.AreEqual(typeof(bool), col1.DataType);
            Assert.AreEqual(typeof(DateTime), col2.DataType);
            Assert.AreEqual(typeof(long), col3.DataType);
            Assert.AreEqual(typeof(double), col4.DataType);
            Assert.AreEqual(typeof(string), col5.DataType);
            Assert.AreEqual(typeof(string), col6.DataType); // Unsupported column type maps to string

            // Asserting if datatype is supported
            Assert.IsTrue(col1.DataTypeSupported);
            Assert.IsTrue(col2.DataTypeSupported);
            Assert.IsTrue(col3.DataTypeSupported);
            Assert.IsTrue(col4.DataTypeSupported);
            Assert.IsTrue(col5.DataTypeSupported);
            Assert.IsFalse(col6.DataTypeSupported);

            // Asserting column names
            Assert.AreEqual("BooleanCol", col1.Name);
            Assert.AreEqual("TimeCol", col2.Name);
            Assert.AreEqual("CountCol", col3.Name);
            Assert.AreEqual("MeasurementCol", col4.Name);
            Assert.AreEqual("TextCol", col5.Name);
            Assert.AreEqual("CategoryRangeCol", col6.Name);

            // Asserting boolean values
            Assert.IsTrue((bool)testObject[0][0]);
            Assert.IsFalse((bool)testObject[1][0]);

            // Asserting DateTime values
            AssertDateTime(ParseDateTimeInUtc("2018-02-06T09:58:44.00Z"), (DateTime)testObject[0][1]);
            AssertDateTime(ParseDateTimeInUtc("2018-02-06T09:58:45.00Z"), (DateTime)testObject[1][1]);

            // Asserting long values
            Assert.AreEqual(3, (long)testObject[0][2]);
            Assert.AreEqual(-5, (long)testObject[1][2]);

            // Asserting double values
            Assert.AreEqual(-0.12, (double)testObject[0][3], 0.00001);
            Assert.AreEqual(14, (double)testObject[1][3], 0.00001);

            // Asserting unit of measure
            Assert.AreEqual("cm", testObject.Columns[3].UnitOfMeasure);

            // Asserting string values
            Assert.AreEqual("abc", testObject[0][4]);

            // Asserting category range values (unsupported data types are presented as string)
            Assert.AreEqual("a o", testObject[0][5]);

            // Asserting empty (null) values
            Assert.IsNull(testObject[1][4]);
            Assert.IsNull(testObject[2][0]);
            Assert.IsNull(testObject[2][1]);
            Assert.IsNull(testObject[2][2]);
            Assert.IsNull(testObject[2][3]);
            Assert.IsNull(testObject[2][4]);
            Assert.IsNull(testObject[2][5]);
        }

        [TestMethod]
        public void Array_Read_LabelAndDescription()
        {
            // Testing the reading of label and description of columns

            var filepath = TestCommon.TestHelper.TestFileFolder + "\\Item_Array_LabelAndDesc.xml";

            var resultEl = (XsdNs.DataArrayType)ParseRawResult(filepath);
            var testObject = new Item_Array(resultEl);

            Assert.AreEqual(5, testObject.Columns.Count);

            // Testing description and label for each supported column type
            Assert.AreEqual("Boolean col", testObject.Columns[0].Label);
            Assert.AreEqual("Time col", testObject.Columns[1].Label);
            Assert.AreEqual("Count col", testObject.Columns[2].Label);
            Assert.AreEqual("Measurement col", testObject.Columns[3].Label);
            Assert.AreEqual("Text col", testObject.Columns[4].Label);
            Assert.AreEqual("Boolean desc", testObject.Columns[0].Description);
            Assert.AreEqual("Time desc", testObject.Columns[1].Description);
            Assert.AreEqual("Count desc", testObject.Columns[2].Description);
            Assert.AreEqual("Measurement desc", testObject.Columns[3].Description);
            Assert.AreEqual("Text desc", testObject.Columns[4].Description);
        }

        [TestMethod]
        public void Array_Read_CellParsingFails()
        {
            // Testing the failure of parsing a cell value

            var myAction = new Action(() =>
            {
                var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_Item_Array_CellParsingFails.xml";

                // Getting result element
                var resultEl = (XsdNs.DataArrayType)ParseRawResult(filepath);
                new Item_Array(resultEl);
            });

            AssertInvalidMessageException(myAction, "Failed to parse Double");
        }

        [TestMethod]
        public void Array_Read_CellCountConflict()
        {
            // Testing a conflict in the cell count of a row

            var myAction = new Action(() =>
            {
                var filepath = TestCommon.TestHelper.TestFileFolder + "\\Neg_Item_Array_CellCountConflict.xml";

                // Getting result element
                var resultEl = (XsdNs.DataArrayType)ParseRawResult(filepath);
                new Item_Array(resultEl);
            });

            AssertInvalidMessageException(myAction, "Inconsistent cell count in rows");
        }
        
        [TestMethod]
        public void Array_Create_NoColumns()
        {
            // Testing the creation of an array without columns
            
            var arrayItem = new Item_Array(new List<Item_Array.ArrayColumn>());

            // Adding 3 empty rows
            arrayItem.Add();
            arrayItem.Add();
            arrayItem.Add();

            var arrayItemIn = (Item_Array)SerialiseAndReadResultObj(arrayItem, XNeut.Helper.TypeUri_Complex);

            Assert.AreEqual(0, arrayItemIn.Columns.Count);
            Assert.AreEqual(3, arrayItemIn.RowCount);
        }

        [TestMethod]
        public void Array_Create_NoRows()
        {
            // Testing the creation of an array without rows

            var columns = new List<Item_Array.ArrayColumn>()
            {
                new Item_Array.ArrayColumn(typeof(bool), "BoolCol"),
                new Item_Array.ArrayColumn(typeof(long), "CountCol")
            };
            var arrayItem = new Item_Array(columns);
            
            var arrayItemIn = (Item_Array)SerialiseAndReadResultObj(arrayItem, XNeut.Helper.TypeUri_Complex);

            Assert.AreEqual(2, arrayItemIn.Columns.Count);
            Assert.AreEqual(0, arrayItemIn.RowCount);
        }

        [TestMethod]
        public void Array_Create_DataTypeConflictInCell()
        {
            // Testing an attempt to set a conflicting data type to a cell

            var columns = new List<Item_Array.ArrayColumn>()
            {
                new Item_Array.ArrayColumn(typeof(double), "MeasCol1"),
                new Item_Array.ArrayColumn(typeof(double), "MeasCol2")
            };

            var myAction = new Action(() =>
            {
                new Item_Array(columns)
                {
                    { 4.5, (long)2 }
                };
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Type mismatch");
        }
        
        [TestMethod]
        public void Array_Create_CellCountConflict()
        {
            // Testing an attempt to add a conflicting count of cells in a row

            var myAction = new Action(() =>
            {
                var columns = new List<Item_Array.ArrayColumn>()
                {
                    new Item_Array.ArrayColumn(typeof(bool), "BoolCol"),
                    new Item_Array.ArrayColumn(typeof(long), "CountCol")
                };

                new Item_Array(columns)
                {
                    { true, (long)4, (long)2 }
                };
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, "Received cell count does not match");
        }

        [TestMethod]
        public void Array_Create_BadDateTimeKind()
        {
            // Testing the detection of bad DateTime kind as an array is being created.

            var columns = new List<Item_Array.ArrayColumn>()
            {
                new Item_Array.ArrayColumn(typeof(DateTime), "DateTimeCol")
                {
                    Label = "Label for DateTime",
                    Description = "Desc for DateTime"
                },
                new Item_Array.ArrayColumn(typeof(double), "DoubleCol")
                {
                    Label = "Label for double",
                    Description = "Desc for double"
                }
            };

            var arrayItem = new Item_Array(columns);

            // This does not have UTC as the kind
            var dateTime1 = ParseDateTimeInUtc("2019-01-11T00:11:19Z").ToLocalTime();

            try
            {
                arrayItem.Add(dateTime1, -4.2);
                Assert.Fail("Expected an exception");
            }
            catch (XNeut.DateTimeException) { }
        }

        [TestMethod]
        public void Array_Create()
        {
            // Testing the basic creation of an array with all data types included.
            // - Testing label and description too
            // - Testing both boolean values
            // - Testing empty values (null, empty string)


            var arrayItemIn = CreateArrayForTest();
            var dateTime1 = ParseDateTimeInUtc("2019-01-11T00:11:19Z");
            var dateTime2 = ParseDateTimeInUtc("2019-01-12T00:11:19Z");

            // Asserting counts
            Assert.AreEqual(6, arrayItemIn.Columns.Count);
            Assert.AreEqual(4, arrayItemIn.RowCount);

            // Asserting row 1
            Assert.IsTrue((bool)arrayItemIn[0][0]);
            AssertDateTime(dateTime1, (DateTime)arrayItemIn[0][1]);
            Assert.AreEqual(-4.2, arrayItemIn[0][2]);
            Assert.AreEqual((long)68, arrayItemIn[0][3]);
            Assert.AreEqual("string 1", arrayItemIn[0][4]);
            Assert.AreEqual("emptyname 1", arrayItemIn[0][5]);

            // Asserting row 2
            Assert.IsFalse((bool)arrayItemIn[1][0]);
            AssertDateTime(dateTime2, (DateTime)arrayItemIn[1][1]);
            Assert.AreEqual(0.0, (double)arrayItemIn[1][2], 0.00001);
            Assert.AreEqual((long)0, arrayItemIn[1][3]);
            Assert.AreEqual("string 2", arrayItemIn[1][4]);
            Assert.AreEqual("emptyname 2", arrayItemIn[1][5]);

            // Asserting row 3 (nulls in input)
            Assert.IsNull(arrayItemIn[2][0]);
            Assert.IsNull(arrayItemIn[2][1]);
            Assert.IsNull(arrayItemIn[2][2]);
            Assert.IsNull(arrayItemIn[2][3]);
            Assert.IsNull(arrayItemIn[2][4]);
            Assert.IsNull(arrayItemIn[2][5]);

            // Asserting row 4 (empty strings in input)
            Assert.IsNull(arrayItemIn[3][0]);
            Assert.IsNull(arrayItemIn[3][1]);
            Assert.IsNull(arrayItemIn[3][2]);
            Assert.IsNull(arrayItemIn[3][3]);
            Assert.IsNull(arrayItemIn[3][4]);
            Assert.IsNull(arrayItemIn[3][5]);

            var col1 = arrayItemIn.Columns[0];
            var col2 = arrayItemIn.Columns[1];
            var col3 = arrayItemIn.Columns[2];
            var col4 = arrayItemIn.Columns[3];
            var col5 = arrayItemIn.Columns[4];
            var col6 = arrayItemIn.Columns[5];

            // Asserting column names
            Assert.AreEqual(col1.Name, "BoolCol");
            Assert.AreEqual(col2.Name, "DateTimeCol");
            Assert.AreEqual(col3.Name, "DoubleCol");
            Assert.AreEqual(col4.Name, "LongCol");
            Assert.AreEqual(col5.Name, "StringCol");
            Assert.AreEqual(col6.Name, "NoLabelOrDescCol");

            // Asserting units of measure
            Assert.IsTrue(string.IsNullOrEmpty(col1.UnitOfMeasure));
            Assert.AreEqual("Cel", col3.UnitOfMeasure);

            // Asserting labels and descriptions
            Assert.AreEqual("Label for bool", col1.Label);
            Assert.AreEqual("Label for DateTime", col2.Label);
            Assert.AreEqual("Label for double", col3.Label);
            Assert.AreEqual("Label for long", col4.Label);
            Assert.AreEqual("Label for string", col5.Label);
            Assert.IsNull(col6.Label);
            Assert.AreEqual("Desc for bool", col1.Description);
            Assert.AreEqual("Desc for DateTime", col2.Description);
            Assert.AreEqual("Desc for double", col3.Description);
            Assert.AreEqual("Desc for long", col4.Description);
            Assert.AreEqual("Desc for string", col5.Description);
            Assert.IsNull(col6.Description);
        }

        private Item_Array CreateArrayForTest()
        {
            // Creating the test array in a separate method to prevent asserting the
            // wrong object
            
            var columns = new List<Item_Array.ArrayColumn>()
            {
                new Item_Array.ArrayColumn(typeof(bool), "BoolCol")
                {
                    Label = "Label for bool",
                    Description = "Desc for bool"
                },
                new Item_Array.ArrayColumn(typeof(DateTime), "DateTimeCol")
                {
                    Label = "Label for DateTime",
                    Description = "Desc for DateTime"
                },
                new Item_Array.ArrayColumn(typeof(double), "DoubleCol", "Cel")
                {
                    Label = "Label for double",
                    Description = "Desc for double"
                },
                new Item_Array.ArrayColumn(typeof(long), "LongCol")
                {
                    Label = "Label for long",
                    Description = "Desc for long"
                },
                new Item_Array.ArrayColumn(typeof(string), "StringCol")
                {
                    Label = "Label for string",
                    Description = "Desc for string"
                },
                new Item_Array.ArrayColumn(typeof(string), "NoLabelOrDescCol")
            };

            var dateTime1 = ParseDateTimeInUtc("2019-01-11T00:11:19Z");
            var dateTime2 = ParseDateTimeInUtc("2019-01-12T00:11:19Z");

            var arrayItem = new Item_Array(columns)
            {
                { true, dateTime1, -4.2, (long)68, "string 1", "emptyname 1" },
                { false, dateTime2, 0.0, (long)0, "string 2", "emptyname 2" },
                { null, null, null, null, null, null },
                { "  ", " ", "   ", " ", " ", "    " }
            };

            // Serialising and deserialising
            return (Item_Array)SerialiseAndReadResultObj(arrayItem, XNeut.Helper.TypeUri_Complex);
        }

        #endregion Array


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
            
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Om_Gml | TestCommon.Validator.SchemaType.Swe | TestCommon.Validator.SchemaType.Custom2);

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

        private void AssertDateTime(DateTime expected, DateTime actual)
        {
            TestCommon.TestHelper.AssertDateTime(expected, actual);
        }

        private DateTime ParseDateTimeInUtc(string s)
        {
            return TestCommon.TestHelper.ParseDateTimeInUtc(s);
        }

        #endregion Array and ArrayColumn
    }
}
