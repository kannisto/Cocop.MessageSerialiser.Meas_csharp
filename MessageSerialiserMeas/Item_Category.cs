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
    /// Represents a category observation. It may be a process state or
    /// any other enumeration value. This class should *not* be used
    /// for free-form textual data! For that, use the class <see cref="Item_Text"/>.
    /// <para>In this module, the code has been derived from
    /// OpenGIS(r) Geography Markup Language (GML) Encoding Standard
    /// (OGC 07-036; please see the file "ref_and_license_ogc_gml.txt") and
    /// OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_Boolean"/>
    /// <seealso cref="Item_CategoryRange"/>
    /// <seealso cref="Item_Text"/>
    /// </summary>
    public sealed class Item_Category : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="categ">Category.</param>
        /// <exception cref="ArgumentException">Thrown if the category value contains a space. Only use spaces in human-readable text, not anything that indicates a reference to a category.</exception>
        public Item_Category(string categ) : base(XNeut.Helper.TypeUri_Category)
        {
            if (categ != null && XNeut.Helper.StringContainsSpace(categ))
            {
                throw new ArgumentException("Category string must not contain spaces - never use spaces in a reference but only in human-readable text");
            }

            Value = categ;
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (observation result).
        /// </summary>
        /// <param name="el">XML data.</param>
        /// <exception cref="ArgumentException">Thrown if the category value contains a space. Only use spaces in human-readable text, not anything that indicates a reference to a category.</exception>
        internal Item_Category(XsdNs.ReferenceType el) : this(el.Title)
        {
            // Empty ctor body (another local ctor called)
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (data record field).
        /// </summary>
        /// <param name="el">XML data.</param>
        /// <exception cref="ArgumentException">Thrown if the category value contains a space. Only use spaces in human-readable text, not anything that indicates a reference to a category.</exception>
        internal Item_Category(XsdNs.CategoryType el) : this(el.value)
        {
            // Empty ctor body (another local ctor called)
        }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return new XsdNs.ReferenceType
            {
                Title = Value
            };
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return new XsdNs.CategoryType
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
        /// The category value.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
    }
}