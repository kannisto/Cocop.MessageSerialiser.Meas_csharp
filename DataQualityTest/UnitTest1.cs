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

namespace DataQualityTest
{
    [TestClass]
    public class UnitTest1
    {
        // This test covers:
        // - The instantiation and evaluation of basic quality values (10)
        // - The instantiation of a bad quality value with a custom reason (20)
        // - Parsing quality values from strings (30)
        //
        // This test does not cover:
        // - an extensive number of different inputs for parsing
        
        private const string ERR_INVALID_START = "Cannot interpret data quality value";
        private const string ERR_WHITESPACES_START = "Data quality string must not contain whitespaces";


        [TestMethod]
        public void Dq_10_BasicGoodness()
        {
            // Test basic good and bad qualities

            DataQuality good = DataQuality.CreateGood();
            DataQuality bad = DataQuality.CreateBad();

            Assert.IsTrue(good.IsGood);
            Assert.IsFalse(bad.IsGood);
        }

        [TestMethod]
        public void Dq_20_CustomBad()
        {
            // Test bad qualities with a custom reason

            DataQuality customBad1 = DataQuality.CreateBad("myreason");
            DataQuality customBad2 = DataQuality.CreateBad("myreason/subreason");

            Assert.IsFalse(customBad1.IsGood);
            Assert.IsFalse(customBad2.IsGood);
        }

        [TestMethod]
        public void Dq_25_CustomBad_Errors()
        {
            // Test bad qualities with a custom reason (with errors)

            var myAction = new Action(() =>
            {
                DataQuality.CreateBad("myreaso n");
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, ERR_WHITESPACES_START);
        }

        [TestMethod]
        public void Dq_30_Parse()
        {
            // Test creation for a raw string

            var value1 = "good";
            var value2 = "bad";
            var value3 = "bad/justreason";
            
            DataQuality fromStr1 = new DataQuality(value1);
            DataQuality fromStr2 = new DataQuality(value2);
            DataQuality fromStr3 = new DataQuality(value3);

            Assert.IsTrue(fromStr1.IsGood);
            Assert.IsFalse(fromStr2.IsGood);
            Assert.IsFalse(fromStr3.IsGood);
        }

        [TestMethod]
        public void Dq_35_Parse_Errors()
        {
            // Test creation for a raw string (with errors)
            
            var unexpected1 = "baad/myreason";
            
            // Unexpected URI
            ExpectException(unexpected1, ERR_INVALID_START);

            // Spaces in the value
            ExpectException("bad/fdf f", ERR_WHITESPACES_START);
        }

        private void ExpectException(string input, string expectedMsgStart)
        {
            var myAction = new Action(() =>
            {
                new DataQuality(input);
            });

            TestCommon.TestHelper.AssertArgumentException(myAction, expectedMsgStart);
        }
    }
}
