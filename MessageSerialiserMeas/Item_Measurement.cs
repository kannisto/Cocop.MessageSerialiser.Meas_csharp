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
    /// A measurement result that has a numeric value and unit of measure.
    /// <para>In this module, the code has been derived from
    /// OpenGIS(r) Geography Markup Language (GML) Encoding Standard
    /// (OGC 07-036; please see the file "ref_and_license_ogc_gml.txt") and
    /// OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_Count"/>
    /// <seealso cref="Item_MeasurementRange"/>
    /// </summary>
    public sealed class Item_Measurement : Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="u">Unit of measure.</param>
        /// <param name="v">Value.</param>
        public Item_Measurement(string u, double v)
            : this(new XsdNs.MeasureType { uom = u, Value = v })
        {
            // Empty ctor body
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (observation result).
        /// </summary>
        /// <param name="el">XML data.</param>
        internal Item_Measurement(XsdNs.MeasureType el) : base(XNeut.Helper.TypeUri_Measurement)
        {
            try
            {
                UnitOfMeasure = el.uom;
                Value = el.Value;
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read measurement (something missing?)", e);
            }
        }

        /// <summary>
        /// Constructor. Use this to instantiate an item from XML (data record field).
        /// </summary>
        /// <param name="el">XML data.</param>
        internal Item_Measurement(XsdNs.QuantityType el) : base(XNeut.Helper.TypeUri_Measurement)
        {
            try
            {
                if (!el.valueSpecified)
                {
                    throw new XNeut.InvalidMessageException("Measurement: value not specified");
                }

                UnitOfMeasure = el.uom.code;
                Value = el.value;
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read measurement (something missing?)", e);
            }
        }

        override internal object GetObjectForXml_Result(string idPrefix)
        {
            return new XsdNs.MeasureType
            {
                uom = UnitOfMeasure,
                Value = Value
            };
        }

        override internal XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            var quantity = new XsdNs.QuantityType
            {
                uom = new XsdNs.UnitReference(),
                value = Value,
                valueSpecified = true
            };

            quantity.uom.code = UnitOfMeasure;
            
            return quantity;
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            // Using the culture to specify decimal separator
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            return Value.ToString("0.###", culture) + " " + UnitOfMeasure;
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
        /// Measurement value.
        /// </summary>
        public double Value
        {
            get;
            private set;
        }
    }
}