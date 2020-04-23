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
    /// Represents a count value, that is, an integer without any unit of measure.
    /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_Boolean"/>
    /// <seealso cref="Item_Category"/>
    /// <seealso cref="Item_CountRange"/>
    /// <seealso cref="Item_Measurement"/>
    /// <seealso cref="Item_Text"/>
    /// </summary>
    public sealed class Item_Count : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="v">Value.</param>
        public Item_Count(long v) : base(XNeut.Helper.TypeUri_Count)
        {
            Value = v;
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (observation result).
        /// </summary>
        /// <param name="el">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_Count(string el) : base(XNeut.Helper.TypeUri_Count)
        {
            try
            {
                Value = XNeut.Helper.LongFromString(el);
            }
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException("Failed to parse count value", e);
            }
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (data record field).
        /// </summary>
        /// <param name="el">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_Count(XsdNs.CountType el) : this(el.value)
        {
            // Empty ctor body
        }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return XNeut.Helper.LongToString(Value);
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return new XsdNs.CountType
            {
                value = (string)GetObjectForXml_Result(null)
            };
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// The value.
        /// </summary>
        public long Value
        {
            get;
            private set;
        }
    }
}