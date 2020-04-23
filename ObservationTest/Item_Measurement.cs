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
    internal class Item_Measurement : Item
    {
        // Stub class

        public Item_Measurement(object o) : base(XNeut.Helper.TypeUri_Measurement)
        { }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            var retval = new XsdNs.MeasureType
            {
                uom = "Cel",
                Value = 45.1
            };

            return retval;
        }

        public override string ToDisplayString()
        {
            // Not implemented in this test
            throw new NotImplementedException();
        }
    }
}