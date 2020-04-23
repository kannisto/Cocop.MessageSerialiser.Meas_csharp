//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2/2019
// Last modified: 2/2020

using System;
using System.Collections.Generic;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace Cocop.MessageSerialiser.Meas
{
    // Stub classes

    public class Item_DataRecord : Item
    {
        public Item_DataRecord()
            : base("")
        { }

        public Item_DataRecord(object a)
            : base("")
        { }

        public HashSet<string> ItemNames
        {
            get { return new HashSet<string>(); }
        }

        public string Identifier
        {
            get;
            set;
        }

        public XsdNs.DataRecordType ToXmlProxy(string idPrefix)
        {
            return new XsdNs.DataRecordType();
        }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }
    
    public class Item_Boolean : Item
    {
        public Item_Boolean(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_Category : Item
    {
        public Item_Category(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_CategoryRange : Item
    {
        public Item_CategoryRange(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_Count : Item
    {
        public Item_Count(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_CountRange : Item
    {
        public Item_CountRange(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_Measurement : Item
    {
        public Item_Measurement(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_MeasurementRange : Item
    {
        public Item_MeasurementRange(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_Text : Item
    {
        public Item_Text(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_TimeRange : Item
    {
        public Item_TimeRange(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_TimeSeriesConstant : Item
    {
        public Item_TimeSeriesConstant(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }

    public class Item_TimeSeriesFlexible : Item
    {
        public Item_TimeSeriesFlexible(object a) : base("")
        { }

        public override string ToDisplayString()
        {
            throw new NotImplementedException(); // Not implemented in unit test
        }
    }
}
