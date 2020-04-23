//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 3/2019
// Last modified: 3/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Meas.Neutral;

namespace HelperTest
{
    [TestClass]
    public class UnitTest1
    {
        // Not testing all of the helper, just parts of it.
        
        [TestMethod]
        public void StringContainsSpace_1()
        {
            // True expected
            Assert.IsTrue(Helper.StringContainsSpace(" "));
            Assert.IsTrue(Helper.StringContainsSpace("a "));
            Assert.IsTrue(Helper.StringContainsSpace(" g"));
            Assert.IsTrue(Helper.StringContainsSpace("f f"));
            Assert.IsTrue(Helper.StringContainsSpace("f f f"));
            Assert.IsTrue(Helper.StringContainsSpace('\r'.ToString())); // carriage return
            Assert.IsTrue(Helper.StringContainsSpace('\n'.ToString())); // newline
            Assert.IsTrue(Helper.StringContainsSpace("f" + '\r' + "f"));

            // False expected
            Assert.IsFalse(Helper.StringContainsSpace("a"));
            Assert.IsFalse(Helper.StringContainsSpace(""));
            Assert.IsFalse(Helper.StringContainsSpace("sfsdf"));
        }

        [TestMethod]
        public void GetRootElNameFromXmlDoc()
        {
            var xmlBytes = ReadTestFile("TaskCancelRequest.xml");
            Assert.AreEqual("Cancel", Helper.GetRootElNameFromXmlDoc(xmlBytes));
        }

        [TestMethod]
        public void DateTimeToString()
        {
            // 1) Positive test case
            // Using UTC as the time zone; this is converted to local time, and a conversion back to UTC is expected
            var inputPositive = new DateTime(
                year: 2019, month: 3, day: 20,
                hour: 19, minute: 55, second: 3,
                millisecond: 814, kind: DateTimeKind.Utc
                );

            // Expecting UTC as the time zone
            Assert.AreEqual("2019-03-20T19:55:03.814Z", Helper.DateTimeToString(inputPositive));


            // 2a) Negative test case: kind must be UTC
            try
            {
                Helper.DateTimeToString(inputPositive.ToLocalTime());
                Assert.Fail();
            }
            catch (DateTimeException) { }

            // 2b) Negative test case: kind must be UTC
            try
            {
                Helper.DateTimeToString(DateTime.SpecifyKind(inputPositive, DateTimeKind.Unspecified));
                Assert.Fail();
            }
            catch (DateTimeException) { }
        }
        
        [TestMethod]
        public void DateTimeFromString()
        {
            // 1a) Positive test case, local time (winter)
            ParseAndAssertDateTime(expKind: DateTimeKind.Utc, expMo: 3, expDay: 1, expHour: 19, timestampXml: "2019-03-01T21:55:03.814+02:00");

            // 1b) Positive test case, local time (summer)
            ParseAndAssertDateTime(expKind: DateTimeKind.Utc, expMo: 5, expDay: 20, expHour: 18, timestampXml: "2019-05-20T21:55:03.814+03:00");

            // 1c) Positive, exotic time zone
            ParseAndAssertDateTime(expKind: DateTimeKind.Utc, expMo: 3, expDay: 20, expHour: 23, timestampXml: "2019-03-20T19:55:03.814-04:00");

            // 1d) Positive, no time zone specified; expect current local time
            ParseAndAssertDateTime(expKind: DateTimeKind.Unspecified, expMo: 3, expDay: 20, expHour: 21, timestampXml: "2019-03-20T21:55:03.814");
            
            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DateTimeFromString("2019-03-20fT21:55:03.814+02:00");
            },
            "Failed to parse DateTime");
        }

        private void ParseAndAssertDateTime(DateTimeKind expKind, int expMo, int expDay, int expHour, string timestampXml)
        {
            // This is a local datetime value
            var parsed = Helper.DateTimeFromString(timestampXml);

            // Expecting UTC although the original value is local
            Assert.AreEqual(expKind, parsed.Kind);

            // Checking that hours were converted to UTC as expected
            Assert.AreEqual(expHour, parsed.Hour);

            // Asserting the datetime value
            var expected = new DateTime(
                year: 2019, month: expMo, day: expDay,
                hour: expHour, minute: 55, second: 3,
                millisecond: 814, kind: expKind
                );
            TestCommon.TestHelper.AssertDateTime(expected, parsed);
        }

        [TestMethod]
        public void DateTimeToUtcIfPossible()
        {
            // This is supposed to transform all local DateTimes to UTC.
            // Unspecified ones remain intact, because this means the time
            // zone is unknown.
            
            var dtUtc = TestCommon.TestHelper.ParseDateTimeInUtc("2020-02-17T16:07:00Z");
            var dtLocal = dtUtc.ToLocalTime();
            var dtUnspecified = DateTime.SpecifyKind(dtUtc, DateTimeKind.Unspecified);
            var dtExotic = DateTime.Parse("2020-02-17T16:07:00-05:00");
            var dtExoticUtc = dtExotic.ToUniversalTime();

            TestCommon.TestHelper.AssertDateTime(dtUtc, Helper.DateTimeToUtcIfPossible(dtUtc));
            TestCommon.TestHelper.AssertDateTime(dtUtc, Helper.DateTimeToUtcIfPossible(dtLocal));
            TestCommon.TestHelper.AssertDateTime(dtUnspecified, Helper.DateTimeToUtcIfPossible(dtUnspecified));
            TestCommon.TestHelper.AssertDateTime(dtExoticUtc, Helper.DateTimeToUtcIfPossible(dtExotic));
        }


        [TestMethod]
        public void DoubleToString()
        {
            // To test "double to string", looked up possible outcomes at:
            // http://books.xmlschemata.org/relaxng/ch19-77065.html
            //
            // Eric van der Vlist: RELAX NG
            // Released December 2003
            // Publisher(s): O'Reilly Media, Inc.
            // ISBN: 0596004214
            Assert.AreEqual("0", Helper.DoubleToString(0));
            Assert.AreEqual("10.1", Helper.DoubleToString(10.1));
            Assert.AreEqual("-3.8", Helper.DoubleToString(-3.8));
            Assert.AreEqual("2E+15", Helper.DoubleToString(2e15));
            Assert.AreEqual("NaN", Helper.DoubleToString(double.NaN));
        }

        [TestMethod]
        public void DoubleFromString()
        {
            // 1) Positive testing
            Assert.AreEqual(0, Helper.DoubleFromString("0 "), 0.001);
            Assert.AreEqual(10.1, Helper.DoubleFromString(" 10.1"), 0.001);
            Assert.AreEqual(-3.8, Helper.DoubleFromString("-3.8  "), 0.001);
            Assert.AreEqual(9e15, Helper.DoubleFromString("9e15"), 0.001);
            Assert.IsTrue(double.IsNaN(Helper.DoubleFromString("  NaN")));

            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("0,4");
            },
            "Failed to parse double");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("");
            },
            "Failed to parse double");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("  ");
            },
            "Failed to parse double");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.DoubleFromString("a");
            },
            "Failed to parse double");
        }

        [TestMethod]
        public void LongToString()
        {
            Assert.AreEqual("0", Helper.LongToString(0));
            Assert.AreEqual("39", Helper.LongToString(39));
            Assert.AreEqual("-8", Helper.LongToString(-8));
            Assert.AreEqual("-9223372036854775808", Helper.LongToString(-9223372036854775808));
            Assert.AreEqual("9223372036854775807", Helper.LongToString(9223372036854775807));
        }

        [TestMethod]
        public void LongFromString()
        {
            // 1) Positivite testing
            Assert.AreEqual(0, Helper.LongFromString("0  "));
            Assert.AreEqual(39, Helper.LongFromString("  39"));
            Assert.AreEqual(-8, Helper.LongFromString("-8 "));
            Assert.AreEqual(-9223372036854775808, Helper.LongFromString(" -9223372036854775808")); // Min value
            Assert.AreEqual(9223372036854775807, Helper.LongFromString("9223372036854775807 ")); // Max value

            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString(" "); // Empty value
            },
            "Failed to parse long");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString("rtt"); // Invalid
            },
            "Failed to parse long");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString("5.3"); // Invalid
            },
            "Failed to parse long");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.LongFromString("9223372036854775808"); // Too big
            },
            "Failed to parse long");
        }

        [TestMethod]
        public void IntToString()
        {
            Assert.AreEqual("0", Helper.IntToString(0));
            Assert.AreEqual("39", Helper.IntToString(39));
            Assert.AreEqual("-8", Helper.IntToString(-8));
            Assert.AreEqual("-2147483648", Helper.IntToString(-2147483648));
            Assert.AreEqual("2147483647", Helper.IntToString(2147483647));
        }

        [TestMethod]
        public void IntFromString()
        {
            // 1) Positivite testing
            Assert.AreEqual(0, Helper.IntFromString("0  "));
            Assert.AreEqual(39, Helper.IntFromString("  39"));
            Assert.AreEqual(-8, Helper.IntFromString("-8 "));
            Assert.AreEqual(-2147483648, Helper.IntFromString(" -2147483648")); // Min value
            Assert.AreEqual(2147483647, Helper.IntFromString("2147483647 ")); // Max value

            // 2) Negative testing
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString(" "); // Empty value
            },
            "Failed to parse Int32");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString("rtt"); // Invalid
            },
            "Failed to parse Int32");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString("5.3"); // Invalid
            },
            "Failed to parse Int32");
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.IntFromString("2147483648"); // Too big
            },
            "Failed to parse Int32");
        }

        [TestMethod]
        public void BoolToString()
        {
            Assert.AreEqual("true", Helper.BoolToString(true));
            Assert.AreEqual("false", Helper.BoolToString(false));
        }

        [TestMethod]
        public void BoolFromString()
        {
            Assert.IsTrue(Helper.BoolFromString("  1 "));
            Assert.IsTrue(Helper.BoolFromString("true  "));
            Assert.IsFalse(Helper.BoolFromString("  0"));
            Assert.IsFalse(Helper.BoolFromString(" false"));

            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.BoolFromString("fafse");
            },
		    "Failed to parse boolean");
        }

        [TestMethod]
        public void TimeSpanToString()
        {
            var p1 = TimeSpan.FromHours(15);
            var p2 = TimeSpan.FromMinutes(1);
            var p3 = TimeSpan.FromSeconds(2);

            Assert.AreEqual("PT15H", Helper.TimeSpanToString(p1));
            Assert.AreEqual("PT1M", Helper.TimeSpanToString(p2));
            Assert.AreEqual("PT2S", Helper.TimeSpanToString(p3));
        }

        [TestMethod]
        public void TimeSpanFromString()
        {
            // 1) Positive testing

            var p1 = TimeSpan.FromHours(15);
            var p2 = TimeSpan.FromMinutes(1);
            var p3 = TimeSpan.FromSeconds(2);

            // Not testing years or months because they vary in length.
            // Expecting that short periods (hours, minutes or seconds) are most important.
            Assert.AreEqual(0, TimeSpan.Compare(p1, Helper.TimeSpanFromString("PT15H")));
            Assert.AreEqual(0, TimeSpan.Compare(p2, Helper.TimeSpanFromString("PT1M")));
            Assert.AreEqual(0, TimeSpan.Compare(p3, Helper.TimeSpanFromString("PT2S")));
            
            // 2) Negative testing
            
            TestCommon.TestHelper.AssertArgumentException(() =>
            {
                Helper.TimeSpanFromString("xyz");
            }
		    , "Failed to parse TimeSpan");
        }

        private byte[] ReadTestFile(string filename)
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + "\\" + filename;
            return System.IO.File.ReadAllBytes(filepath);
        }
    }
}
