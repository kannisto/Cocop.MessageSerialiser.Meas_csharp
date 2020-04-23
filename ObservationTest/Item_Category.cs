//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 2/2020

using System;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Stub class
    /// </summary>
    public class Item_Category : Item
    {
        public Item_Category(string value) : base(XNeut.Helper.TypeUri_Complex)
        {
            Value = value;
        }

        internal Item_Category(XsdNs.CategoryType el) : base(XNeut.Helper.TypeUri_Category)
        {
            Value = el.value;
        }

        public string Value
        {
            get;
            private set;
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return new XsdNs.CategoryType
            {
                value = Value
            };
        }

        public override string ToDisplayString()
        {
            // Not implemented in this test
            throw new NotImplementedException();
        }
    }
}
