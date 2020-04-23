//
// Please make sure to read and understand README.md and LICENSE.txt.
//
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
// Author: Petri Kannisto, Tampere University, Finland
// Last modified: 4/2020
//
// This API has been derived from standards and XML schemata provided by the
// Open Geospatial Consortium (OGC(r)). Please make sure to read and understand
// the following legal conditions:
// (1) Copyright Notice and Disclaimers at https://www.ogc.org/ogc/legal
// (2) OGC(r) Document Notice; the most recent version is at
//     https://www.ogc.org/ogc/document and another enclosed in file
//     "ogc_document_notice.txt"
// (3) OGC(r) Software Notice; the most recent version is at
//     https://www.ogc.org/ogc/software and another enclosed in file
//     "ogc_software_notice.txt"
// (4) The license of each related standard referred to in this file.

using System;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents a value from free textual input. This class should *not*
    /// be used for enumeration values (such as process states or similar)!
    /// For those, use <c>Item_Category</c> instead.
    /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_Category"/>
    /// </summary>
    public sealed class Item_Text : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="v">Value.</param>
        public Item_Text(string v)
             : base(XNeut.Helper.TypeUri_Text)
        {
            Value = v;
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (a data record field).
        /// </summary>
        /// <param name="t">Proxy.</param>
        internal Item_Text(XsdNs.TextType t)
             : base(XNeut.Helper.TypeUri_Text)
        {
            Value = t.value;
        }


        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return Value;
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return new XsdNs.TextType
            {
                value = Value
            };
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            if (Value == null) return "";
            else return Value;
        }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
    }
}
