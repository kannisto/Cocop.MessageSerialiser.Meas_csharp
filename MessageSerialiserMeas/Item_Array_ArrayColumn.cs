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
using System.Collections.Generic;
using System.Linq;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace Cocop.MessageSerialiser.Meas
{
    public sealed partial class Item_Array
    {
        // This class is partial to enable having the nested ArrayColumn class in this separate file.

        /// <summary>
        /// Represents a column in an array.
        /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
        /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
        /// </summary>
        public sealed class ArrayColumn
        {
            private readonly HashSet<Type> m_supportedTypes = new HashSet<Type>()
            {
                typeof(bool), typeof(DateTime), typeof(double), typeof(long), typeof(string)
            };
            private readonly string m_supportedTypesString = "bool, DateTime, double, long, string";


            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">Column name.</param>
            /// <param name="type">Column type. The following data types are supported:
            /// <c>boolean</c>, <c>double</c> (measurement), <c>DateTime</c>, <c>long</c> and <c>string</c>.</param>
            public ArrayColumn(Type type, string name)
                : this(type, name, "")
            {
                // Empty ctor body
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">Column name.</param>
            /// <param name="type">Column type. The following data types are supported:
            /// <c>boolean</c>, <c>double</c> (measurement), <c>DateTime</c>, <c>long</c> and <c>string</c>.</param>
            /// <param name="uom">Unit of measure. Only applicable if the type is measurement (i.e., <c>double</c>).</param>
            public ArrayColumn(Type type, string name, string uom)
            {
                // Check that the type is supported
                if (!m_supportedTypes.Contains(type))
                {
                    throw new ArgumentException("Unsupported column type " + type.Name + "; the supported are " + m_supportedTypesString);
                }

                // Type check: only measurements can have a unit
                if (!string.IsNullOrEmpty(uom) && type != typeof(double))
                {
                    throw new ArgumentException("Only measurements (doubles) can have a unit");
                }
                
                CheckIfValidColumnName(name);

                Name = name;
                DataType = type;
                DataTypeSupported = true;
                UnitOfMeasure = uom;
            }

            private void CheckIfValidColumnName(string s)
            {
                // Checking if invalid
                bool invalid = 
					string.IsNullOrEmpty(s) ||
					s.Contains(":") ||
					s.Any(x => char.IsWhiteSpace(x));
					
			    if (invalid)
			    {
				    string errMsg = "Column name is mandatory and must be valid NCName";
				
				    if (!string.IsNullOrEmpty(s))
				    {
					    errMsg = errMsg + ": \"" + s + "\"";
				    }
				
				    throw new ArgumentException(errMsg);
                }
            }
            
            /// <summary>
            /// Constructor. Use this to populate the object from XML.
            /// </summary>
            /// <param name="proxy">XML proxy.</param>
            internal ArrayColumn(XsdNs.DataRecordTypeField proxy)
            {
                switch (proxy.dataComponentTypeInfo)
                {
                    case XsdNs.DataRecordFieldTypeType.Boolean:
                        DataType = typeof(bool);
                        DataTypeSupported = true;
                        break;

                    case XsdNs.DataRecordFieldTypeType.Count:
                        DataType = typeof(long);
                        DataTypeSupported = true;
                        break;

                    case XsdNs.DataRecordFieldTypeType.Quantity:
                        DataType = typeof(double);
                        DataTypeSupported = true;

                        // Assigning unit of measure
                        var measProxy = (XsdNs.QuantityType)proxy.dataComponent;
                        UnitOfMeasure = measProxy.uom?.code; // If uom is null, assign null

                        break;

                    case XsdNs.DataRecordFieldTypeType.Text:
                        DataType = typeof(string);
                        DataTypeSupported = true;
                        break;

                    case XsdNs.DataRecordFieldTypeType.Time:
                        DataType = typeof(DateTime);
                        DataTypeSupported = true;
                        break;

                    default:
                        // Unexpected simple type. The values will be treated as strings.
                        DataType = typeof(string);
                        DataTypeSupported = false;
                        break;
                }

                // Assigning the fields common to all types
                Name = proxy.name;
                var simpleProxy = (XsdNs.AbstractSimpleComponentType)proxy.dataComponent;
                Description = simpleProxy.description;
                Label = simpleProxy.label;
            }

            /// <summary>
            /// Generates an XML proxy from the object.
            /// </summary>
            /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
            /// require an ID. For certain XML elements, the schema requires an ID that is unique within
            /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
            /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
            /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
            /// <returns>Proxy.</returns>
            internal XsdNs.DataRecordTypeField ToXmlProxy(string idPrefix)
            {
                return new XsdNs.DataRecordTypeField()
                {
                    name = Name,
                    dataComponent = GetValueProxy(),
                    dataComponentTypeInfo = GetFieldTypeEnumValue()
                };
            }
            
            /// <summary>
            /// The name of the column.
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// Data type.
            /// </summary>
            public Type DataType
            {
                get;
                private set;
            }

            /// <summary>
            /// Whether the data type is supported. When deserialising XML, this will be
            /// false for unsupported types that map to string.
            /// </summary>
            public bool DataTypeSupported
            {
                get;
                private set;
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
            /// The human-readable label of the column. Leave blank if not needed.
            /// </summary>
            public string Label
            {
                get;
                set;
            }

            /// <summary>
            /// The human-readable description of the column. Leave blank if not needed.
            /// </summary>
            public string Description
            {
                get;
                set;
            }


            #region Private methods for create XML proxy

            private XsdNs.DataRecordFieldTypeType GetFieldTypeEnumValue()
            {
                if (DataType.Equals(typeof(string)))
                {
                    return XsdNs.DataRecordFieldTypeType.Text;
                }
                else if (DataType.Equals(typeof(bool)))
                {
                    return XsdNs.DataRecordFieldTypeType.Boolean;
                }
                else if (DataType.Equals(typeof(long)))
                {
                    return XsdNs.DataRecordFieldTypeType.Count;
                }
                else if (DataType.Equals(typeof(double)))
                {
                    return XsdNs.DataRecordFieldTypeType.Quantity;
                }
                else if (DataType.Equals(typeof(DateTime)))
                {
                    return XsdNs.DataRecordFieldTypeType.Time;
                }
                else
                {
                    throw new ArgumentException("Unexpected column type " + DataType.ToString());
                }
            }

            private XsdNs.AbstractSimpleComponentType GetValueProxy()
            {
                XsdNs.AbstractSimpleComponentType retval = null;

                if (DataType.Equals(typeof(string)))
                {
                    retval = new XsdNs.TextType();
                }
                else if (DataType.Equals(typeof(bool)))
                {
                    retval = new XsdNs.BooleanType();
                }
                else if (DataType.Equals(typeof(long)))
                {
                    retval = new XsdNs.CountType();
                }
                else if (DataType.Equals(typeof(double)))
                {
                    retval = new XsdNs.QuantityType()
                    {
                        uom = new XsdNs.UnitReference()
                        {
                            code = UnitOfMeasure
                        }
                    };
                }
                else if (DataType.Equals(typeof(DateTime)))
                {
                    retval = new XsdNs.TimeType1()
                    {
                        // The XML schema requires this "uom" element here
                        uom = new XsdNs.UnitReference()
                    };
                }
                else
                {
                    throw new ArgumentException("Unexpected column type " + DataType.ToString());
                }

                // Assign descrition and label
                retval.label = Label;
                retval.description = Description;
                return retval;
            }

            #endregion Private methods for create XML proxy
        }
    }
}
