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
using System.Collections.Generic;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Stub class
    /// </summary>
    public class Item_DataRecord : Item
    {
        // Not using a collection of fields because the test does not require it
        private string m_fieldName;
        private Item_Category m_fieldValue;


        public Item_DataRecord() : base(XNeut.Helper.TypeUri_Complex)
        { }

        public Item_DataRecord(XsdNs.DataRecordType el) : this()
        {
            Identifier = el.identifier;

            // Reading field names.
            // The test does not test Item_DataRecord, but it is important to test
            // that the Observation calls a constructor that parses XML.
            foreach (var field in el.field)
            {
                m_fieldName = field.name;
            }
        }

        public string Identifier
        {
            get;
            set;
        }

        public HashSet<string> ItemNames
        {
            get { return new HashSet<string>() { m_fieldName }; }
        }

        public void Add(string field, Item_Category value)
        {
            m_fieldName = field;
            m_fieldValue = value;
        }
        
        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return new XsdNs.DataRecordPropertyType
            {
                DataRecord = ToXmlProxy(idPrefix)
            };
        }

        public override string ToDisplayString()
        {
            // Not implemented in this test
            throw new NotImplementedException();
        }

        internal XsdNs.DataRecordType ToXmlProxy(string idPrefix)
        {
            return new XsdNs.DataRecordType
            {
                // Assigning identifier
                identifier = Identifier,

                // Creating array for fields
                field = new XsdNs.DataRecordTypeField[]
                {
                    new XsdNs.DataRecordTypeField
                    {
                        name = m_fieldName,
                        dataComponentTypeInfo = XsdNs.DataRecordFieldTypeType.Category,
                        dataComponent = new XsdNs.CategoryType
                        {
                            value = m_fieldValue.Value
                        }
                    }
                }
            };
        }
    }
}
