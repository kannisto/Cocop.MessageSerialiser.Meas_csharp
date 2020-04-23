//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 1/2019
// Last modified: 2/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace XsdHelperTest
{
    /// <summary>
    /// Summary description for XLinkHelperTest
    /// </summary>
    [TestClass]
    public class XLinkHelperTest
    {
        [TestMethod]
        public void XLinkHelper_1_Type()
        {
            var testObject = new XsdNs.XLinkHelper();

            // 1) Assert default type
            Assert.AreEqual(XsdNs.typeType.simple, testObject.Type);

            // 2) Try set the default type
            testObject.Type = XsdNs.typeType.simple;
            Assert.AreEqual(XsdNs.typeType.simple, testObject.Type);

            // 3) Try set a type other than default
            var exThrown = false;

            try
            {
                testObject.Type = XsdNs.typeType.locator;
            }
            catch { exThrown = true; }

            Assert.IsTrue(exThrown);
        }

        [TestMethod]
        public void XLinkHelper_2_XLinkDefined()
        {
            // Testing if the object can recognise if an XLink is defined or not.
            // If defined, there should be the 'type="simple"' attribute to indicate
            // XLink presence.

            // 1) Default; no XLinks defined
            var testObjectHref = new XsdNs.XLinkHelper();
            var testObjectTitle = new XsdNs.XLinkHelper();
            Assert.IsFalse(testObjectHref.XLinkSpecified);
            Assert.IsFalse(testObjectTitle.XLinkSpecified);
            
            // 2) Specifying a value (space).
            // Expecting XLink defined.
            testObjectHref.Href = "";
            testObjectTitle.Title = "";
            Assert.IsTrue(testObjectHref.XLinkSpecified);
            Assert.IsTrue(testObjectTitle.XLinkSpecified);

            // 3) Specifying a value.
            // Expecting XLink defined.
            testObjectHref.Href = "http://foo";
            testObjectTitle.Title = "sometitle";
            Assert.IsTrue(testObjectHref.XLinkSpecified);
            Assert.IsTrue(testObjectTitle.XLinkSpecified);
        }
    }
}
