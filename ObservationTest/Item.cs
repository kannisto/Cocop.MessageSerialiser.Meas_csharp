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

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Stub class
    /// </summary>
    public abstract class Item
    {
        public Item(string observationUriString)
        {
            ObservationTypeUri = observationUriString;
        }
        
        virtual internal object GetObjectForXml_Result(string idPrefix)
        {
            throw new NotImplementedException("The type does not support serialisation as an observation result");
        }

        virtual internal XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            throw new NotImplementedException("The type does not support serialisation as a data record field");
        }

        public abstract string ToDisplayString();

        internal string ObservationTypeUri
        {
            get;
            private set;
        }
    }
}