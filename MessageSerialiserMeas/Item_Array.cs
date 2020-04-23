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
using System.Collections;
using System.Collections.Generic;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents an array of data.
    /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_DataRecord"/>
    /// <seealso cref="Item_TimeSeriesConstant"/>
    /// <seealso cref="Item_TimeSeriesFlexible"/>
    /// </summary>
    public sealed partial class Item_Array : Item, IEnumerable<object[]>
    {
        // This class is partial to enable having the nested ArrayColumn class in another file.
        
        private readonly List<object[]> m_rows = new List<object[]>();


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="columns">The columns of the array.</param>
        public Item_Array(List<ArrayColumn> columns)
            : base(XNeut.Helper.TypeUri_Complex)
        {
            Columns = columns;
        }

        /// <summary>
        /// Constructor. Use this when populating the object from XML.
        /// </summary>
        /// <param name="proxy">XML proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_Array(XsdNs.DataArrayType proxy)
            : base(XNeut.Helper.TypeUri_Complex)
        {
            Columns = ReadColumnsFromProxy(proxy);
            m_rows = ReadRowsFromProxy(proxy);
        }
        
        /// <summary>
        /// The columns of the array.
        /// </summary>
        public List<ArrayColumn> Columns
        {
            get;
            private set;
        }

        /// <summary>
        /// The count of rows in the table.
        /// </summary>
        public int RowCount
        {
            get { return m_rows.Count;  }
        }
        
        /// <summary>
        /// Returns the row at given index.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>Row.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of bounds.</exception>
        public object[] this[int index]
        {
            get
            {
                if (index >= m_rows.Count)
                {
                    throw new IndexOutOfRangeException("Index " + index + " exceeds the highest row index");
                }

                return m_rows[index];
            }
        }


        #region IEnumerable and Add for collection initialiser

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<object[]> GetEnumerator()
        {
            return m_rows.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a row to the table. The type of each value must match the respective column type.
        /// </summary>
        /// <param name="items">The items of the row.</param>
        /// <exception cref="ArgumentException">Thrown if (1) the items types do not match the
        /// specified columns or (2) if the cell count does not match the column count.</exception>
        /// <exception cref="XNeut.DateTimeException">Thrown if cell type is DateTime and the value is not in UTC.</exception>
        public void Add(params object[] items)
        {
            // Checking item count
            if (items.Length != Columns.Count)
            {
                throw new ArgumentException("Received cell count does not match with column count");
            }
            
            // Checking typing
            for (int a = 0; a < items.Length; ++a)
            {
                var item = items[a];

                if (ValueIsEmpty(item))
                {
                    // No type check for empty
                    continue;
                }

                var columnType = Columns[a].DataType;
                var cellType = item.GetType();

                if (columnType != cellType)
                {
                    throw new ArgumentException("Type mismatch: expected " + columnType.Name + ", got " + cellType.Name);
                }
                else if (columnType == typeof(DateTime))
                {
                    // Checking that the timestamp is in UTC
                    var dateTime = (DateTime)item;
                    XNeut.Helper.ExpectDateTimeIsUtc(dateTime); // throws DateTimeException
                }
            }

            m_rows.Add(items);
        }

        #endregion IEnumerable and Add for collection initialiser


        #region Overridden Item members

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            // Same as data record field
            return GetObjectForXml_DataRecordField(idPrefix);
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            // Creating the proxy skeleton
            var proxy = new XsdNs.DataArrayType()
            {
                // The schema requires the "elementCount" element but nothing inside it
                elementCount = new XsdNs.CountPropertyType(),

                // The schema requires the "elementType" element
                elementType = CreateColumnsForProxy(idPrefix),
                values = new XsdNs.EncodedValuesPropertyType()
                {
                    Array = CreateRowsForProxy() // throws DateTimeException
                }
            };
            
            return proxy;
        }

        internal override bool SupportsDataQualityInDataRecord
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            return string.Format("Array {0}x{1}", m_rows.Count, Columns.Count);
        }

        #endregion Overridden Item members


        #region Private methods for read

        private List<ArrayColumn> ReadColumnsFromProxy(XsdNs.DataArrayType proxy)
        {
            try
            {
                var retval = new List<ArrayColumn>();
                
                // Are there any fields?
                if (proxy.elementType != null && proxy.elementType.AbstractDataComponent != null &&
                    proxy.elementType.AbstractDataComponent.field != null)
                {
                    var fields = proxy.elementType.AbstractDataComponent.field;

                    // Processing each field
                    foreach (var field in fields)
                    {
                        // Processing column proxy
                        retval.Add(new ArrayColumn(field));
                    }
                }

                return retval;
            }
            catch (InvalidCastException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read columns of array - expected DataRecord nested in elementType", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read columns of array - something missing", e);
            }
        }

        private List<object[]> ReadRowsFromProxy(XsdNs.DataArrayType proxy)
        {
            try
            {
                var retval = new List<object[]>();

                // Are there any rows?
                if (proxy.values == null || proxy.values.Array == null ||
                    proxy.values.Array.Row == null)
                {
                    return retval;
                }
                
                var rows = proxy.values.Array.Row;
                
                foreach (var row in rows)
                {
                    var parsedValuesOfRow = new object[Columns.Count];

                    if (row.I == null)
                    {
                        // Expecting no cells in the row
                        if (Columns.Count != 0)
                        {
                            throw new XNeut.InvalidMessageException("No columns defined for array but a row has cells");
                        }
                    }
                    else
                    {
                        // Checking that the cell count matches the column count
                        if (row.I.Length != Columns.Count)
                        {
                            throw new XNeut.InvalidMessageException("Inconsistent cell count in rows of array");
                        }
                        
                        for (int a = 0; a < row.I.Length; ++a)
                        {
                            // Attempting to parse the value after the type of the column
                            var valueParsed = TryParseAfterType(row.I[a], Columns[a].DataType);

                            parsedValuesOfRow[a] = valueParsed;
                        }
                    }

                    retval.Add(parsedValuesOfRow);
                }

                return retval;
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to read rows of array - something missing", e);
            }
        }

        private object TryParseAfterType(string raw, Type type)
        {
            if (ValueIsEmpty(raw))
            {
                // Empty value
                return null;
            }

            raw = raw.Trim();

            try
            {
                if (type == typeof(long))
                {
                    return XNeut.Helper.LongFromString(raw);
                }
                else if (type == typeof(double))
                {
                    return XNeut.Helper.DoubleFromString(raw);
                }
                else if (type == typeof(DateTime))
                {
                    // Parsing and converting to UTC
                    return XNeut.Helper.DateTimeFromString(raw);
                }
                else if (type == typeof(string))
                {
                    // This will condition will match unsupported simple types too
                    return raw;
                }
                else if (type == typeof(bool))
                {
                    return XNeut.Helper.BoolFromString(raw);
                }
                else
                {
                    throw new ArgumentException("Unexpected column type " + type.ToString());
                }
            }
            catch (ArgumentException e)
            {
                throw new XNeut.InvalidMessageException("Failed to parse " + type.Name + " from string \"" + raw + "\"", e);
            }
        }

        #endregion Private methods for read


        #region Private methods for write

        private XsdNs.DataArrayTypeElementType CreateColumnsForProxy(string idPrefix)
        {
            // Populating columns to proxy

            var retval = new XsdNs.DataArrayTypeElementType
            {
                // Name attribute is required in the XML schema and must have a valid NCName as the value.
                // However, the value has no reasonable use case here.
                name = "columns"
            };

            // Omitting data record if no columns, because an empty
            // data record is not allowed in the XML schema.
            if (Columns.Count > 0)
            {
                retval.AbstractDataComponent = new XsdNs.DataRecordType()
                {
                    field = new XsdNs.DataRecordTypeField[Columns.Count]
                };

                // Adding column proxies
                for (int a = 0; a < Columns.Count; ++a)
                {
                    var colIdPrefix = idPrefix + (a+1) + "-";
                    var fieldProxy = Columns[a].ToXmlProxy(colIdPrefix);
                    retval.AbstractDataComponent.field[a] = fieldProxy;
                }
            }

            return retval;
        }

        private XsdNs.ArrayType CreateRowsForProxy()
        {
            var retval = new XsdNs.ArrayType()
            {
                Row = new XsdNs.ArrayRowType[m_rows.Count]
            };

            // Populating rows to proxy
            for (int rowI = 0; rowI < m_rows.Count; ++rowI)
            {
                var cellValues = new string[Columns.Count];

                // Serialising cell values
                for (int colI = 0; colI < Columns.Count; ++colI)
                {
                    var objectOfCell = m_rows[rowI][colI];
                    var typeOfColumn = Columns[colI].DataType;
                    var serialisedValue = SerialiseAfterType(objectOfCell, typeOfColumn); // throws DateTimeException
                    cellValues[colI] = serialisedValue;
                }

                retval.Row[rowI] = new XsdNs.ArrayRowType()
                {
                    I = cellValues
                };
            }

            return retval;
        }

        private string SerialiseAfterType(object value, Type type)
        {
            // Empty values
            if (ValueIsEmpty(value))
            {
                return "";
            }
            
            if (type == typeof(string))
            {
                return ((string)value).Trim();
            }
            else if (type == typeof(bool))
            {
                return XNeut.Helper.BoolToString((bool)value);
            }
            else if (type == typeof(DateTime))
            {
                return XNeut.Helper.DateTimeToString((DateTime)value); // throws DateTimeException
            }
            else if (type == typeof(long))
            {
                return XNeut.Helper.LongToString((long)value);
            }
            else if (type == typeof(double))
            {
                return XNeut.Helper.DoubleToString((double)value);
            }
            else
            {
                throw new ArgumentException("Unexpected cell type " + type.ToString());
            }
        }

        #endregion Private methods for write


        private bool ValueIsEmpty(object value)
        {
            return
                value == null ||
                value.GetType() == typeof(string) && ((string)value).Trim().Length < 1;
        }
    }
}
