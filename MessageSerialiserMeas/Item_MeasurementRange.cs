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
    /// An item that contains a range of measurement values (lower bound and upper bound).
    /// This type does not exist in the Observations and Measurements standard, but the
    /// type appears as a field type for a data record in the SWE common standard.
    /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_CountRange"/>
    /// <seealso cref="Item_Measurement"/>
    /// </summary>
    public sealed class Item_MeasurementRange : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uom">Unit of measure.</param>
        /// <param name="lower">Lower bound of the range.</param>
        /// <param name="upper">Upper bound of the range.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if lower bound is higher than upper bound.</exception>
        public Item_MeasurementRange(string uom, double lower, double upper)
            : base(XNeut.Helper.TypeUri_MeasurementRange)
        {
            if (lower > upper)
            {
                throw new XNeut.InvalidMessageException("Lower bound of range must be lower than or equal to the upper bound");
            }

            UnitOfMeasure = uom;
            LowerBound = lower;
            UpperBound = upper;
        }
        
        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (a result or data record field).
        /// </summary>
        /// <param name="el">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_MeasurementRange(XsdNs.QuantityRangeType el)
            : base(XNeut.Helper.TypeUri_MeasurementRange)
        {
            // If "uom" is null, assign null; otherwise, assign "code"
            UnitOfMeasure = el.uom?.code;
            
            if (el.ValuesHelper == null || el.ValuesHelper.Count != 2)
            {
                throw new XNeut.InvalidMessageException("Expected exactly 2 values in range");
            }

            LowerBound = el.ValuesHelper[0];
            UpperBound = el.ValuesHelper[1];

            if (LowerBound > UpperBound)
            {
                throw new XNeut.InvalidMessageException("Lower bound of range must be lower than or equal to the upper bound");
            }
        }
        
        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return GetObjectForXml_DataRecordField(idPrefix);
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            var proxy = new XsdNs.QuantityRangeType
            {
                uom = new XsdNs.UnitReference()
                {
                    code = UnitOfMeasure
                },
                ValuesHelper = new System.Collections.Generic.List<double> { LowerBound, UpperBound }
            };
            
            return proxy;
        }
        
        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            // This culture uses period as the decimal separators
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            return string.Format("{0}..{1} {2}",
                LowerBound.ToString("0.###", culture),
                UpperBound.ToString("0.###", culture),
                UnitOfMeasure
                );
        }

        /// <summary>
        /// Unit of measure.
        /// </summary>
        public string UnitOfMeasure
        {
            get;
            private set;
        }

        /// <summary>
        /// The lower bound of the range.
        /// </summary>
        public double LowerBound
        {
            get;
            private set;
        }

        /// <summary>
        /// The upper bound of the range.
        /// </summary>
        public double UpperBound
        {
            get;
            private set;
        }
    }
}
