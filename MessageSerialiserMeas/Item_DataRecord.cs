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
using SXml = System.Xml;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents a complex measurement result with multiple fields.
    /// <para>In this module, the code has been derived from OGC(r) SWE Common Data Model Encoding Standard
    /// (OGC 08-094r1; please see the file "ref_and_license_ogc_swecommon.txt").</para>
    /// <seealso cref="Item_Array"/>
    /// </summary>
    public sealed class Item_DataRecord : Item, IEnumerable<Item_DataRecord.DataRecordItem>
    {
        /// <summary>
        /// An item within a data record. This class implements to enable the <c>IEnumerable</c> interface,
        /// which enables the collection initialiser.
        /// </summary>
        public sealed class DataRecordItem
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="n">Name.</param>
            /// <param name="i">Item.</param>
            internal DataRecordItem(string n, Item i)
            {
                Name = n;
                ItemObj = i;
                DataQualityObj = DataQuality.CreateGood();
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="n">Name.</param>
            /// <param name="i">Item.</param>
            /// <param name="dq">Data quality value.</param>
            internal DataRecordItem(string n, Item i, DataQuality dq)
            {
                Name = n;
                ItemObj = i;
                DataQualityObj = dq;
            }
            
            /// <summary>
            /// Name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Item.
            /// </summary>
            public Item ItemObj { get; private set; }

            /// <summary>
            /// Data quality.
            /// </summary>
            public DataQuality DataQualityObj { get; private set; }
        }


        private const string emptyRecordItemIndicator = "__cocop-empty-value";
        private readonly Dictionary<string, DataRecordItem> m_recItems = new Dictionary<string, DataRecordItem>();


        /// <summary>
        /// Constructor.
        /// </summary>
        public Item_DataRecord()
            : base(XNeut.Helper.TypeUri_Complex)
        {
            // Empty ctor body
        }

        /// <summary>
        /// Constructor. Use this to populate the data record from an XmlNode object.
        /// </summary>
        /// <param name="nodes">XML nodes. If there are two nodes, one should be the xsi:type parameter and
        /// the other the actual data record.</param>
        public Item_DataRecord(SXml.XmlNode[] nodes)
            : this(XmlNodeToProxy(nodes))
        {
            // Empty ctor body
        }

        private static XsdNs.DataRecordType XmlNodeToProxy(SXml.XmlNode[] nodes)
        {
            foreach (var n in nodes)
            {
                if (n.NodeType != SXml.XmlNodeType.Element)
                {
                    // Assuming this is an attribute; either namespace-related or xsi:type
                    continue;
                }

                // Assuming this is the desired data record
                try
                {
                    using (var nodeReader = new SXml.XmlNodeReader(n))
                    {
                        var serialiser = new SXml.Serialization.XmlSerializer(typeof(XsdNs.DataRecordType));
                        return (XsdNs.DataRecordType)serialiser.Deserialize(nodeReader);
                    }
                }
                catch (Exception e)
                {
                    throw new XNeut.InvalidMessageException("Failed to read data record from XML data", e);
                }
            }

            throw new XNeut.InvalidMessageException("Cannot read data record because no element was found in node array");
        }

        /// <summary>
        /// Constructor. Use to instantiate an item from XML data (observation result).
        /// </summary>
        /// <param name="el">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_DataRecord(XsdNs.DataRecordPropertyType el)
            : this(el.DataRecord)
        {
            // Empty ctor body
        }

        /// <summary>
        /// Constructor. Use to instantiate an item from XML data (data record field).
        /// </summary>
        /// <param name="el">XML data.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if an error is encountered.</exception>
        internal Item_DataRecord(XsdNs.DataRecordType el)
            : base(XNeut.Helper.TypeUri_Complex)
        {
            ReadDataRecord(el);
        }

        private void ReadDataRecord(XsdNs.DataRecordType el)
        {
            // Getting identifier
            Identifier = el.identifier;
            
            // Any fields?
            if (el.field != null)
            {
                foreach (var field in el.field)
                {
                    string fieldName = field.name;

                    if (fieldName == emptyRecordItemIndicator)
                    {
                        // Empty item -> skip it.
                        // Empty items can exist, because there are use cases for
                        // empty data records but the XML schema does not allow
                        // empty records.
                        continue;
                    }

                    try
                    {
                        ProcessOneField(field);
                    }
                    catch (NullReferenceException e)
                    {
                        throw new XNeut.InvalidMessageException("Data record field is invalid: " + fieldName, e);
                    }
                }
            }
        }

        private void ProcessOneField(XsdNs.DataRecordTypeField field)
        {
            string fieldName = field.name;
            var fieldObj = field.dataComponent;

            Item itemRead = null;
            DataQuality dataQuality = null;
            var simpleComponent = true;

            try
            {
                if (fieldObj is XsdNs.AbstractSimpleComponentType)
                {
                    // Simple component (as of SWE schemata)
                    simpleComponent = true;
                    itemRead = ReadSimpleData(field);
                }
                else
                {
                    // Complex component
                    simpleComponent = false;

                    switch (field.dataComponentTypeInfo)
                    {
                        case XsdNs.DataRecordFieldTypeType.DataRecord:
                            var dataRec = (XsdNs.DataRecordType)fieldObj;
                            itemRead = new Item_DataRecord(dataRec);
                            break;

                        case XsdNs.DataRecordFieldTypeType.AbstractGmlAsSweDataComponent:
                            itemRead = ProcessTimeSeriesField((XsdNs.AbstractGmlAsSweDataComponentType)fieldObj);
                            break;

                        case XsdNs.DataRecordFieldTypeType.DataArray:
                            itemRead = new Item_Array((XsdNs.DataArrayType)fieldObj);
                            break;

                        default:
                            // For robustness, just skip unknown field types
                            itemRead = null;
                            break;
                    }
                }
            }
            // Datetime values
            catch (FormatException e)
            {
                throw new XNeut.InvalidMessageException("Data record field is invalid: " + fieldName, e);
            }
            // Other errors
            catch (XNeut.InvalidMessageException e)
            {
                throw new XNeut.InvalidMessageException("Data record field is invalid: " + fieldName, e);
            }

            if (itemRead == null)
            {
                // Unknown field type -> skip for robustness
                return;
            }

            if (simpleComponent)
            {
                // Getting data quality
                var simpleComp = (XsdNs.AbstractSimpleComponentType)fieldObj;

                if (simpleComp.quality != null && simpleComp.quality.Length > 0)
                {
                    var qualityProp = simpleComp.quality[0];
                    dataQuality = new DataQuality(qualityProp.Title);
                }
            }

            if (dataQuality == null)
            {
                Add(fieldName, itemRead);
            }
            else
            {
                Add(fieldName, itemRead, dataQuality);
            }
        }

        private Item ReadSimpleData(XsdNs.DataRecordTypeField field)
        {
            var fieldObj = field.dataComponent;

            switch (field.dataComponentTypeInfo)
            {
                case XsdNs.DataRecordFieldTypeType.Boolean:
                    return new Item_Boolean((XsdNs.BooleanType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.Category:
                    return new Item_Category((XsdNs.CategoryType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.CategoryRange:
                    return new Item_CategoryRange((XsdNs.CategoryRangeType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.Count:
                    return new Item_Count((XsdNs.CountType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.CountRange:
                    return new Item_CountRange((XsdNs.CountRangeType)fieldObj);
                    
                case XsdNs.DataRecordFieldTypeType.Quantity:
                    return new Item_Measurement((XsdNs.QuantityType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.QuantityRange:
                    return new Item_MeasurementRange((XsdNs.QuantityRangeType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.Text:
                    return new Item_Text((XsdNs.TextType)fieldObj);

                case XsdNs.DataRecordFieldTypeType.Time:
                    return new Item_TimeInstant((XsdNs.TimeType1)fieldObj);

                case XsdNs.DataRecordFieldTypeType.TimeRange:
                    return new Item_TimeRange((XsdNs.TimeRangeType)fieldObj);
                    
                default:
                    // For robustness, just skip unknown field types
                    return null;
            }
        }

        private Item ProcessTimeSeriesField(XsdNs.AbstractGmlAsSweDataComponentType wrapperElement)
        {
            var proxy = (XsdNs.TimeseriesDomainRangeType)wrapperElement.AbstractGML;

            // If the time series object has a time position list, assuming
            // the type is a flexible time series (not constant interval)

            if (proxy.domainSet.TimePositionList != null)
            {
                return new Item_TimeSeriesFlexible(proxy);
            }
            else
            {
                return new Item_TimeSeriesConstant(proxy);
            }
        }

        internal override bool SupportsDataQualityInDataRecord
        {
            get { return false; }
        }

        internal override object GetObjectForXml_Result(string idPrefix)
        {
            return ToXmlProxyWithWrapper(idPrefix);
        }

        internal override XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            return ToXmlProxy(idPrefix);
        }

        /// <summary>
        /// Returns a string value for display.
        /// </summary>
        /// <returns>String for display.</returns>
        public override string ToDisplayString()
        {
            return string.Format("Data record ({0} fields)", m_recItems.Count);
        }

        /// <summary>
        /// Builds a proxy for XML serialisation.
        /// </summary>
        /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
        /// require an ID. For certain XML elements, the schema requires an ID that is unique within
        /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
        /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
        /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
        /// <returns>Proxy.</returns>
        internal XsdNs.DataRecordPropertyType ToXmlProxyWithWrapper(string idPrefix)
        {
            return new XsdNs.DataRecordPropertyType
            {
                DataRecord = ToXmlProxy(idPrefix)
            };
        }

        /// <summary>
        /// Builds a proxy for XML serialisation.
        /// </summary>
        /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
        /// require an ID. For certain XML elements, the schema requires an ID that is unique within
        /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
        /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
        /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
        /// <returns>Proxy.</returns>
        internal XsdNs.DataRecordType ToXmlProxy(string idPrefix)
        {
            Dictionary<string, DataRecordItem> itemsForAdd = null;

            // If there are no fields, adding an empty value
            if (m_recItems.Count < 1)
            {
                // Add the "empty item" indicator
                itemsForAdd = new Dictionary<string, DataRecordItem>()
                {
                    {emptyRecordItemIndicator, new DataRecordItem(emptyRecordItemIndicator, new Item_Text(""))}
                };
            }
            else
            {
                // Just add the items
                itemsForAdd = m_recItems;
            }

            var retval = new XsdNs.DataRecordType
            {
                // Assigning identifier
                identifier = Identifier,

                // Creating array for fields
                field = new XsdNs.DataRecordTypeField[itemsForAdd.Count]
            };
            
            // Assigning field values
            int counter = 0;
            
            foreach (string fieldName in itemsForAdd.Keys)
            {
                var idPrefixForField = idPrefix + "_" + (counter + 1) + "_";

                var recItem = itemsForAdd[fieldName];
                
                // Add the field element to the return value
                retval.field[counter] = CreateOneProxyField(fieldName, recItem, idPrefixForField);
                ++counter;
            }

            return retval;
        }

        /// <summary>
        /// Generates an XML proxy from the object. Use this if you want to include the
        /// object to another XML document. *Please note*: the enclosing document must
        /// explicitly bind the XLink namespace (http://www.w3.org/1999/xlink) to the
        /// prefix 'xlink'. This requirement is a bad thing, but no nice
        /// workaround was found. This is due to
        /// XLink-related issues in proxies that stem from the inability of 'xsd.exe'
        /// to process nested attribute groups in an XML schema.
        /// </summary>
        /// <returns>XML proxy.</returns>
        public XsdNs.DataRecordPropertyType ToDataRecordPropertyProxy()
        {
            // Using a GUID as the ID prefix, a specific value is not supposedly important.
            var idPrefix = Guid.NewGuid().ToString();
            return ToXmlProxyWithWrapper(idPrefix);
        }
        
        private XsdNs.DataRecordTypeField CreateOneProxyField(string fieldName, DataRecordItem recItem, string idPrefixForField)
        {
            // Create field element for the data record
            var currentField = new XsdNs.DataRecordTypeField
            {
                name = fieldName,

                // Setting type information
                dataComponentTypeInfo = GetDataRecordFieldType(recItem.ItemObj)
            };

            // Set the item enclosed by the field
            var objectForMarshal = recItem.ItemObj.GetObjectForXml_DataRecordField(idPrefixForField);
            currentField.dataComponent = objectForMarshal;

            // Adding data quality information if not good
            if (!recItem.DataQualityObj.IsGood)
            {
                string qualityValue = recItem.DataQualityObj.Value;

                // Only simple components can have quality information
                var simpleItem = (XsdNs.AbstractSimpleComponentType)objectForMarshal;

                var qualityInfo = new XsdNs.QualityPropertyType
                {
                    Title = qualityValue
                };

                simpleItem.quality = new XsdNs.QualityPropertyType[1];
                simpleItem.quality[0] = qualityInfo;
            }

            return currentField;
        }
        
        private XsdNs.DataRecordFieldTypeType GetDataRecordFieldType(Item item)
        {
            switch (item.ObservationTypeUri)
            {
                case XNeut.Helper.TypeUri_Category:
                    return XsdNs.DataRecordFieldTypeType.Category;

                case XNeut.Helper.TypeUri_CategoryRange:
                    return XsdNs.DataRecordFieldTypeType.CategoryRange;

                case XNeut.Helper.TypeUri_Complex:
                    return GetDataRecordFieldTypeComplex(item);

                case XNeut.Helper.TypeUri_Count:
                    return XsdNs.DataRecordFieldTypeType.Count;

                case XNeut.Helper.TypeUri_CountRange:
                    return XsdNs.DataRecordFieldTypeType.CountRange;

                case XNeut.Helper.TypeUri_Measurement:
                    return XsdNs.DataRecordFieldTypeType.Quantity;

                case XNeut.Helper.TypeUri_MeasurementRange:
                    return XsdNs.DataRecordFieldTypeType.QuantityRange;

                case XNeut.Helper.TypeUri_Temporal:
                    return GetDataRecordFieldTypeTemporal(item);

                case XNeut.Helper.TypeUri_Text:
                    return XsdNs.DataRecordFieldTypeType.Text;

                case XNeut.Helper.TypeUri_Truth:
                    return XsdNs.DataRecordFieldTypeType.Boolean;

                case XNeut.Helper.TypeUri_TimeSeriesConstant:
                case XNeut.Helper.TypeUri_TimeSeriesFlexible:
                    return XsdNs.DataRecordFieldTypeType.AbstractGmlAsSweDataComponent;
                    
                default:
                    throw new XNeut.InvalidMessageException("Unexpected type URI \"" + item.ObservationTypeUri + "\"");
            }
        }

        private XsdNs.DataRecordFieldTypeType GetDataRecordFieldTypeComplex(Item item)
        {
            // Type check required for the object
            var type = item.GetType();

            if (type.Equals(typeof(Item_DataRecord)))
            {
                return XsdNs.DataRecordFieldTypeType.DataRecord;
            }
            else if (type.Equals(typeof(Item_Array)))
            {
                return XsdNs.DataRecordFieldTypeType.DataArray;
            }

            throw new XNeut.InvalidMessageException("Unexpected complex type " + type.ToString());
        }

        private XsdNs.DataRecordFieldTypeType GetDataRecordFieldTypeTemporal(Item item)
        {
            // Type check required for the object
            var type = item.GetType();

            if (type.Equals(typeof(Item_TimeInstant)))
            {
                return XsdNs.DataRecordFieldTypeType.Time;
            }
            else if (type.Equals(typeof(Item_TimeRange)))
            {
                return XsdNs.DataRecordFieldTypeType.TimeRange;
            }

            throw new XNeut.InvalidMessageException("Unexpected temporal type " + type.ToString());
        }

        /// <summary>
        /// The name of each item in the record.
        /// </summary>
        public HashSet<string> ItemNames
        {
            get { return new HashSet<string>(m_recItems.Keys); }
        }

        /// <summary>
        /// The identifier of the data record. This is currently used only internally.
        /// </summary>
        internal string Identifier
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets an item by its name.
        /// </summary>
        /// <param name="n">Name.</param>
        /// <returns>Item.</returns>
        /// <exception cref="KeyNotFoundException">If the given field does not exist.</exception>
        public Item this[string n]
        {
            get
            {
                if (!m_recItems.ContainsKey(n))
                {
                    throw new KeyNotFoundException("The data record has no field called \"" + n + "\"");
                }

                return m_recItems[n].ItemObj;
            }
        }
        
        /// <summary>
        /// Gets the data quality of an item.
        /// </summary>
        /// <param name="n">Item name.</param>
        /// <returns>Quality information.</returns>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the item does not support data quality information in a data record.</exception>
        public DataQuality GetQualityOfItem(string n)
        {
            if (!m_recItems.ContainsKey(n))
            {
                throw new KeyNotFoundException("The data record has no field called \"" + n + "\"");
            }

            var recItem = m_recItems[n];
            
            // Data quality supported?
            if (!recItem.ItemObj.SupportsDataQualityInDataRecord)
            {
                throw new XNeut.InvalidMessageException("\"" + n + "\": the item type does not support data quality in data records");
            }
            
            return recItem.DataQualityObj;
        }


        #region IEnumerable and Add for collection initialiser

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<DataRecordItem> GetEnumerator()
        {
            return m_recItems.Values.GetEnumerator();
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
        /// Sets a field.
        /// </summary>
        /// <param name="n">Name.</param>
        /// <param name="i">Item.</param>
        /// <param name="dq">Data quality.</param>
        /// <exception cref="ArgumentException">Thrown if the item already exists.</exception>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the item does not support data quality information in a data record.</exception>
        public void Add(string n, Item i, DataQuality dq)
        {
            if (m_recItems.ContainsKey(n))
            {
                throw new ArgumentException("Duplicate item name \"" + n + "\"", "n");
            }

            // Data quality supported?
            if (!i.SupportsDataQualityInDataRecord)
            {
                throw new XNeut.InvalidMessageException("\"" + n + "\": the item type does not support data quality in data records");
            }

            // Add item and set its quality
            m_recItems.Add(n, new DataRecordItem(n, i, dq));
        }

        /// <summary>
        /// Sets a field.
        /// </summary>
        /// <param name="n">Name.</param>
        /// <param name="i">Item.</param>
        /// <exception cref="ArgumentException">Thrown if the item already exists.</exception>
        public void Add(string n, Item i)
        {
            if (m_recItems.ContainsKey(n))
            {
                throw new ArgumentException("Duplicate item name \"" + n + "\"", "n");
            }
            
            m_recItems.Add(n, new DataRecordItem(n, i));
        }

        #endregion
    }
}
