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
    /// An item that contains a range of category values (lower bound and upper bound).
    /// This type does not exist in the Observations and Measurements standard, but the
    /// type appears as a field type for a data record in the SWE common standard.
    /// <para>Please note that there must be a sensible means to order the categories.
    /// Otherwise, having a category range does not make sense.</para>
    /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_Category"/>
    /// </summary>
    public sealed class Item_CategoryRange : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lower">Lower bound of the range.</param>
        /// <param name="upper">Upper bound of the range.</param>
        /// <exception cref="ArgumentException">Thrown if one of the categories contains a space or if only either of the categories is an empty value.</exception>
        public Item_CategoryRange(string lower, string upper)
            : base(XNeut.Helper.TypeUri_CategoryRange)
        {
            LowerBound = lower == null ? "" : lower.Trim();
            UpperBound = upper == null ? "" : upper.Trim();

            EvaluateCategories();
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (a result or data record field).
        /// </summary>
        /// <param name="el">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        /// <exception cref="ArgumentException">Thrown if the category value contains a space. Only use spaces in human-readable text, not anything that indicates a reference to a category.</exception>
        internal Item_CategoryRange(XsdNs.CategoryRangeType el)
            : base(XNeut.Helper.TypeUri_CategoryRange)
        {
            if (el.ValuesHelper == null || el.ValuesHelper.Count == 0)
            {
                // No values specified
                LowerBound = "";
                UpperBound = "";
            }
            else if (el.ValuesHelper.Count != 2)
            {
                throw new XNeut.InvalidMessageException("Expected exactly 2 values in range");
            }
            else
            {
                LowerBound = el.ValuesHelper[0];
                UpperBound = el.ValuesHelper[1];

                EvaluateCategories();
            }
        }

        private void EvaluateCategories()
        {
            // One can be empty but not both
            if (string.IsNullOrEmpty(LowerBound) || string.IsNullOrEmpty(UpperBound))
            {
                throw new ArgumentException("Range must have two specified bounds");
            }

            if (XNeut.Helper.StringContainsSpace(LowerBound) || XNeut.Helper.StringContainsSpace(UpperBound))
            {
                throw new ArgumentException("Category string must not contain spaces - never use spaces in a reference but only in human-readable text");
            }
        }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return GetObjectForXml_DataRecordField(idPrefix);
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return new XsdNs.CategoryRangeType()
            {
                ValuesHelper = new System.Collections.Generic.List<string>()
                {
                    LowerBound, UpperBound
                }
            };
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            if (string.IsNullOrEmpty(LowerBound) || string.IsNullOrEmpty(UpperBound))
            {
                return "";
            }

            return LowerBound + ".." + UpperBound;
        }

        /// <summary>
        /// The lower bound of the range.
        /// </summary>
        public string LowerBound
        {
            get;
            private set;
        }

        /// <summary>
        /// The upper bound of the range.
        /// </summary>
        public string UpperBound
        {
            get;
            private set;
        }
    }
}
