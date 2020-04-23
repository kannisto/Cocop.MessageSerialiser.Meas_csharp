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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace XsdHelperTest
{
    [TestClass]
    public class CollectionHelperTest
    {
        // This test focuses on:
        // - The functionality of the various "array to string" and "string to array" methods
        //   - strings (1**)
        //   - doubles (2**)
        //   - datetimes (3**)
        //   - longs (4**)
        //   - for each:
        //     - empty input (including nulls and spaces)
        //     - single-item input
        //     - multi-item input
        //     - extra spaces
        //
        // This test does *not* focus on:
        // - the detailed parsing of datetimes or doubles (because the native XML library does that)

        [TestMethod]
        public void CollHelper_110_StringArrayToString()
        {
            // Empty array
            var emptyArray = new List<string> { };
            Assert.AreEqual("", XNeut.CollectionHelper.StringCollectionToString(emptyArray));

            // One item
            var oneItemArray = new List<string> { "xyz" };
            Assert.AreEqual("xyz", XNeut.CollectionHelper.StringCollectionToString(oneItemArray));

            // Multiple items
            var multiItemArray = new List<string> { "a", "b", "cc", "ddd" };
            Assert.AreEqual("a b cc ddd", XNeut.CollectionHelper.StringCollectionToString(multiItemArray));
        }

        [TestMethod]
        public void CollHelper_111_StringArrayToString_Invalid()
        {
            // Spaces in values
            var multiItemArray = new List<string> { "a b", "c" };

            try
            {
                XNeut.CollectionHelper.StringCollectionToString(multiItemArray);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("A string in XML list must not contain a whitespace", e.Message);
            }
        }

        [TestMethod]
        public void CollHelper_120_StringToStringArray()
        {
            // Null input
            Assert.AreEqual(0, XNeut.CollectionHelper.StringToStringCollection(null).Count);

            // Empty string
            var emptyString = "  ";
            Assert.AreEqual(0, XNeut.CollectionHelper.StringToStringCollection(emptyString).Count);

            // One item
            var oneItemString = " sff ";
            var oneItemOutput = XNeut.CollectionHelper.StringToStringCollection(oneItemString);
            Assert.AreEqual(1, oneItemOutput.Count);
            Assert.AreEqual("sff", oneItemOutput[0]);

            // Multiple items
            var multiItemString = " dff   err ll";
            var multiItemOutput = XNeut.CollectionHelper.StringToStringCollection(multiItemString);
            Assert.AreEqual(3, multiItemOutput.Count);
            Assert.AreEqual(multiItemOutput[0], "dff");
            Assert.AreEqual(multiItemOutput[1], "err");
            Assert.AreEqual(multiItemOutput[2], "ll");
        }

        [TestMethod]
        public void CollHelper_210_DoubleArrayToString()
        {
            // Empty array
            var emptyArray = new List<double>();
            Assert.AreEqual("", XNeut.CollectionHelper.DoubleCollectionToString(emptyArray));

            // One item
            var oneItemArray = new List<double> { -34.33 };
            Assert.AreEqual("-34.33", XNeut.CollectionHelper.DoubleCollectionToString(oneItemArray));

            // Multiple items
            var multiItemArray = new List<double> { -34.33, 35, 3.339, 0 };
            Assert.AreEqual("-34.33 35 3.339 0", XNeut.CollectionHelper.DoubleCollectionToString(multiItemArray));
        }

        [TestMethod]
        public void CollHelper_220_StringTooDoubleArray() // double "oo" so that a search for "to do" will not find this
        {
            // Null input
            Assert.AreEqual(0, XNeut.CollectionHelper.StringTooDoubleCollection(null).Count);

            // Empty string
            var emptyString = "  ";
            Assert.AreEqual(0, XNeut.CollectionHelper.StringTooDoubleCollection(emptyString).Count);

            // One item
            var oneItemString = " 45.324 ";
            var oneItemOutput = XNeut.CollectionHelper.StringTooDoubleCollection(oneItemString);
            Assert.AreEqual(1, oneItemOutput.Count);
            Assert.AreEqual(45.324, oneItemOutput[0], 0.0001);

            // Multiple items
            var multiItemString = " 45.324   -3.3 5";
            var multiItemOutput = XNeut.CollectionHelper.StringTooDoubleCollection(multiItemString);
            Assert.AreEqual(3, multiItemOutput.Count);
            Assert.AreEqual(multiItemOutput[0], 45.324, 0.0001);
            Assert.AreEqual(multiItemOutput[1], -3.3, 0.0001);
            Assert.AreEqual(multiItemOutput[2], 5, 0.0001);
        }

        [TestMethod]
        public void CollHelper_310_DateTimeArrayToString()
        {
            // Empty array
            var emptyArray = new List<DateTime>();
            Assert.AreEqual("", XNeut.CollectionHelper.DateTimeCollectionToString(emptyArray));

            var dateTime1 = DateTime.Parse("2018-05-15T00:00:00Z").ToUniversalTime();
            var dateTime2 = DateTime.Parse("2018-05-16T00:00:00Z").ToUniversalTime();
            var dateTime3 = DateTime.Parse("2018-05-17T00:00:00Z").ToUniversalTime();
            var dateTime4 = DateTime.Parse("2018-05-18T00:00:00Z").ToUniversalTime();

            // One item
            var oneItemArray = new List<DateTime> { dateTime1 };
            Assert.AreEqual("2018-05-15T00:00:00Z", XNeut.CollectionHelper.DateTimeCollectionToString(oneItemArray));

            // Multiple items
            var multiItemArray = new List<DateTime> { dateTime1, dateTime2, dateTime3, dateTime4 };
            Assert.AreEqual("2018-05-15T00:00:00Z 2018-05-16T00:00:00Z 2018-05-17T00:00:00Z 2018-05-18T00:00:00Z", XNeut.CollectionHelper.DateTimeCollectionToString(multiItemArray));
        }

        [TestMethod]
        public void CollHelper_320_StringToDateTimeArray()
        {
            // Null input
            Assert.AreEqual(0, XNeut.CollectionHelper.StringToDateTimeCollection(null).Count);

            // Empty string
            var emptyString = "  ";
            Assert.AreEqual(0, XNeut.CollectionHelper.StringToDateTimeCollection(emptyString).Count);

            // One item
            var oneItemString = " 2018-05-15T00:00:00Z ";
            var oneItemOutput = XNeut.CollectionHelper.StringToDateTimeCollection(oneItemString);
            Assert.AreEqual(1, oneItemOutput.Count);
            Assert.AreEqual("2018-05-15T00:00:00Z", DateTimeToString(oneItemOutput[0]));

            // Multiple items
            var multiItemString = " 2018-05-15T00:00:00Z   2018-05-16T00:00:00Z 2018-05-17T00:00:00Z";
            var multiItemOutput = XNeut.CollectionHelper.StringToDateTimeCollection(multiItemString);
            Assert.AreEqual(3, multiItemOutput.Count);
            Assert.AreEqual("2018-05-15T00:00:00Z", DateTimeToString(multiItemOutput[0]));
            Assert.AreEqual("2018-05-16T00:00:00Z", DateTimeToString(multiItemOutput[1]));
            Assert.AreEqual("2018-05-17T00:00:00Z", DateTimeToString(multiItemOutput[2]));
        }

        [TestMethod]
        public void CollHelper_410_LongArrayToString()
        {
            // Empty array
            var emptyArray = new List<long>();
            Assert.AreEqual("", XNeut.CollectionHelper.LongCollectionToString(emptyArray));
            
            var long1 = -3354;
            var long2 = 0;
            var long3 = 38;
            var long4 = 10043545;

            // One item
            var oneItemArray = new List<long> { long1 };
            Assert.AreEqual("-3354", XNeut.CollectionHelper.LongCollectionToString(oneItemArray));

            // Multiple items
            var multiItemArray = new List<long> { long1, long2, long3, long4 };
            Assert.AreEqual("-3354 0 38 10043545", XNeut.CollectionHelper.LongCollectionToString(multiItemArray));
        }

        [TestMethod]
        public void CollHelper_420_StringToLongArray()
        {
            // Null input
            Assert.AreEqual(0, XNeut.CollectionHelper.StringToLongCollection(null).Count);

            // Empty string
            var emptyString = "  ";
            Assert.AreEqual(0, XNeut.CollectionHelper.StringToLongCollection(emptyString).Count);

            // One item
            var oneItemString = " 5018 ";
            var oneItemOutput = XNeut.CollectionHelper.StringToLongCollection(oneItemString);
            Assert.AreEqual(1, oneItemOutput.Count);
            Assert.AreEqual(5018, oneItemOutput[0]);

            // Multiple items
            var multiItemString = " 324234   -35 -78";
            var multiItemOutput = XNeut.CollectionHelper.StringToLongCollection(multiItemString);
            Assert.AreEqual(3, multiItemOutput.Count);
            Assert.AreEqual(324234, multiItemOutput[0]);
            Assert.AreEqual(-35, multiItemOutput[1]);
            Assert.AreEqual(-78, multiItemOutput[2]);
        }

        private string DateTimeToString(DateTime dt)
        {
            return System.Xml.XmlConvert.ToString(dt, System.Xml.XmlDateTimeSerializationMode.Utc);
        }
    }
}
