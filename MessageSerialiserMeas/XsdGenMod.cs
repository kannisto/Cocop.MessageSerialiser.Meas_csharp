//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 3/2020

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

// This file contains modifications for the classes generated with "xsd.exe".
namespace Cocop.MessageSerialiser.Meas.XsdGen
{
    /// <summary>
    /// Constants.
    /// </summary>
    static class XsdConstants
    {
        /// <summary>
        /// The namespace of COCOP extensions (version 1.1).
        /// </summary>
        public const string COCOP_EXTENSION_NS_V1_1 = "http://www.cocop-spire.eu/om-custom/1.1";

        /// <summary>
        /// The namespace of COCOP extensions (version 1.2).
        /// </summary>
        public const string COCOP_EXTENSION_NS_V1_2 = "http://www.cocop-spire.eu/om-custom/1.2";
        
        /// <summary>
        /// The SWE namespace.
        /// </summary>
        public const string SWE_NS = "http://www.opengis.net/swe/2.0";

        /// <summary>
        /// The FES namespace.
        /// </summary>
        public const string FES_NS = "http://www.opengis.net/fes/2.0";
        
        /// <summary>
        /// The GML namespace.
        /// </summary>
        public const string GML_NS = "http://www.opengis.net/gml/3.2";

        /// <summary>
        /// The TSML namespace.
        /// </summary>
        public const string TSML_NS = "http://www.opengis.net/tsml/1.0";

        /// <summary>
        /// The XLink namespace.
        /// </summary>
        public const string XLINK_NS = "http://www.w3.org/1999/xlink";
    }


    #region xsd lists

    // The default output of "xsd.exe" serialises the "xs:list" of doubles in a non-functional manner.
    // Please see the article "xsd.exe generated code gets error on deserialize":
    // https://social.msdn.microsoft.com/Forums/en-US/e44f3e27-2c57-4eda-83f8-12864d073e97/xsdexe-generated-code-gets-error-on-deserialize?forum=xmlandnetfx

    public partial class DirectPositionListType
    {
        /// <summary>
        /// The values as string.
        /// </summary>
        [XmlText()]
        public string Text
        {
            get
            {
                return XNeut.CollectionHelper.DoubleCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection(value);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<double> ValuesHelper
        {
            get;
            set;
        }
    }

    public partial class DirectPositionType
    {
        /// <summary>
        /// The values as string.
        /// </summary>
        [XmlText()]
        public string Text
        {
            get
            {
                return XNeut.CollectionHelper.DoubleCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection(value);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<double> ValuesHelper
        {
            get;
            set;
        }
    }

    public partial class AllowedValuesType
    {
        /// <summary>
        /// The values as string.
        /// </summary>
        [XmlText()]
        public string Value
        {
            get
            {
                return XNeut.CollectionHelper.DoubleCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection(value);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<double> ValuesHelper
        {
            get;
            set;
        }
    }

    public partial class MeasureListType
    {
        /// <summary>
        /// The values as string.
        /// </summary>
        [XmlText()]
        public string Text
        {
            get
            {
                return XNeut.CollectionHelper.DoubleCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection(value);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<double> ValuesHelper
        {
            get;
            set;
        }
    }

    public partial class MeasureOrNilReasonListType
    {
        /// <summary>
        /// The values as string.
        /// </summary>
        [XmlText()]
        public string[] Text
        {
            get
            {
                var retval = XNeut.CollectionHelper.DoubleCollectionToString(ValuesHelper);
                return new string[] { retval };
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection("");
                    return;
                }
                if (value.Length > 1)
                {
                    throw new System.InvalidOperationException("Expected only one xsd:list");
                }

                ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection(value[0]);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<double> ValuesHelper
        {
            get;
            set;
        }
    }

    public partial class CodeOrNilReasonListType
    {
        /// <summary>
        /// A helper property that assumes that the "Text" property contains a single string
        /// that is actually an xsd:list.
        /// </summary>
        [XmlIgnore()]
        public List<string> TextAsSingleArrayHelper
        {
            get;
            set;
        }

        /// <summary>
        /// The values as string.
        /// </summary>
        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            // The proxies serialise the collection as an array where
            // the first item is the actual xsd:list as a string

            get
            {
                var temp = XNeut.CollectionHelper.StringCollectionToString(TextAsSingleArrayHelper);

                // Returning an array where the items are as an xsd:list
                return new string[] { temp };
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    // Empty value
                    TextAsSingleArrayHelper = new List<string>();
                }
                else if (value.Length > 1)
                {
                    throw new InvalidOperationException("Expected one xsd:list but got many in CodeOrNilReasonListType");
                }
                else
                {
                    // Converting the first item to a list
                    TextAsSingleArrayHelper = XNeut.CollectionHelper.StringToStringCollection(value[0]);
                }
            }
        }
    }

    public partial class TimeRangeType
    {
        // The original API of the proxy is difficult so this class was added for facilitation.


        /// <summary>
        /// Start time.
        /// </summary>
        /// <returns>Start time.</returns>
        /// <exception cref="FormatException">Thrown if the value is invalid.</exception>
        public DateTime GetStartHelper()
        {
            return GetValuesHelper()[0];
        }

        /// <summary>
        /// End time.
        /// </summary>
        /// <returns>End time.</returns>
        /// <exception cref="FormatException">Thrown if the value is invalid.</exception>
        public DateTime GetEndHelper()
        {
            return GetValuesHelper()[1];
        }

        /// <summary>
        /// Helper property to set the values.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <exception cref="XNeut.DateTimeException">Thrown if DateTime kind is not UTC.</exception>
        public void SetValuesHelper(DateTime start, DateTime end)
        {
            var input = new List<DateTime> { start, end };
            value = XNeut.CollectionHelper.DateTimeCollectionToString(input); // throws DateTimeException
        }

        private List<DateTime> GetValuesHelper()
        {
            var retval = XNeut.CollectionHelper.StringToDateTimeCollection(value);

            if (retval.Count != 2)
            {
                throw new FormatException("Invalid time range value \"" + value + "\"");
            }

            return retval;
        }
    }

    #endregion xsd lists


    #region Ranges

    /// <remarks/>
    public partial class QuantityRangeType
    {
        // Without this, the serialisation of the range does not work as it should
        
        /// <remarks/>
        [XmlElement(ElementName = "value")]
        public string ValueEl
        {
            get
            {
                return XNeut.CollectionHelper.DoubleCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringTooDoubleCollection(value);
            }
        }
        
        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<double> ValuesHelper
        {
            get;
            set;
        }
    }

    /// <remarks/>
    public partial class CountRangeType
    {
        // Without this, the serialisation of the range does not work as it should

        /// <remarks/>
        [XmlElement(ElementName = "value")]
        public string ValueEl
        {
            get
            {
                return XNeut.CollectionHelper.LongCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringToLongCollection(value);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<long> ValuesHelper
        {
            get;
            set;
        }
    }

    /// <remarks/>
    public partial class CategoryRangeType
    {
        // Without this, the serialisation of the range does not work as it should

        /// <remarks/>
        [XmlElement(ElementName = "value")]
        public string ValueEl
        {
            get
            {
                return XNeut.CollectionHelper.StringCollectionToString(ValuesHelper);
            }
            set
            {
                ValuesHelper = XNeut.CollectionHelper.StringToStringCollection(value);
            }
        }

        /// <summary>
        /// A helper property to access the values.
        /// </summary>
        [XmlIgnore]
        public List<string> ValuesHelper
        {
            get;
            set;
        }
    }

    #endregion Ranges


    #region Href and title fields

    // In proxy class generation, "xsd.exe" does not support nested attribute groups
    // (see https://social.msdn.microsoft.com/Forums/en-US/707c8a47-a29f-4262-b052-ac66dc99d604/nested-xml-attribute-groups?forum=asmxandxml)
    // Therefore, the required XLink attributes are added manually in this code region.


    /// <summary>
    /// Class created for consistency checks in XLink references.
    /// </summary>
    internal class XLinkHelper
    {
        // The "href", "title" and "type" attributes come from the XLink spec.
        // To conform to the spec, the "simple" usage pattern is utilised.
        // In that pattern, if the "href" is *not* present, the "type" attribute must be assigned the value "simple".
        // See https://www.w3.org/TR/xlink11/#xlinkattusagepat
        //
        // This class uses the "type" attribute whenever an XLink ref is specified.

        private bool m_typeExplicitlySet = false;


        /// <summary>
        /// Constructor.
        /// </summary>
        public XLinkHelper()
        {
            // Empty ctor body
        }

        /// <summary>
        /// Whether the XLink has been specified. This is important information, because XLink
        /// cannot appear simultaneously with local content. This applies at least to the
        /// "complex feature of interest" feature of observations.
        /// </summary>
        public bool XLinkSpecified
        {
            get
            {
                return Href != null || Title != null || m_typeExplicitlySet;
            }
        }

        /// <remarks/>
        public string Title
        {
            get;
            set;
        }

        /// <remarks/>
        public string Href
        {
            get;
            set;
        }

        /// <remarks/>
        public typeType Type
        {
            // The current API only supports simple xlinks.
            // Not to be set explicitly.
            get { return typeType.simple; }
            set
            {
                if (value != typeType.simple)
                {
                    throw new InvalidOperationException("Unexpected XLink type " + value.ToString());
                }

                m_typeExplicitlySet = true;
            }
        }
    }

    public partial class ReferenceType
    {
        private XLinkHelper m_XLinkHelperObj = new XLinkHelper();
        

        [XmlIgnore()]
        private XLinkHelper XLinkHelperObj
        {
            get { return m_XLinkHelperObj; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "title", Namespace = XsdConstants.XLINK_NS)]
        public string Title
        {
            get { return m_XLinkHelperObj.Title; }
            set { m_XLinkHelperObj.Title = value; }
        }

        /// <summary>
        /// This field is set in other setters. Therefore, do not set it except in this class.
        /// </summary>
        [XmlAttribute(AttributeName = "type", Namespace = XsdConstants.XLINK_NS)]
        public typeType Type
        {
            get { return m_XLinkHelperObj.Type; }
            set { m_XLinkHelperObj.Type = value; }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get { return m_XLinkHelperObj.XLinkSpecified; }
            set {} // Not to be set explicitly
        }

        // This class also includes the "xlink:href" attribute to enable URIs.
        // They are required for Observation typing.
        /// <remarks/>
        [XmlAttribute(AttributeName = "href", Namespace = XsdConstants.XLINK_NS, Form = System.Xml.Schema.XmlSchemaForm.Qualified, DataType = "anyURI")]
        public string Href
        {
            get { return m_XLinkHelperObj.Href; }
            set { m_XLinkHelperObj.Href = value; }
        }
    }


    public partial class OM_ProcessPropertyType
    {
        private XLinkHelper m_xLinkHelperObj = new XLinkHelper();


        [XmlIgnore()]
        private XLinkHelper XLinkHelperObj
        {
            get { return m_xLinkHelperObj; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "title", Namespace = "http://www.w3.org/1999/xlink")]
        public string Title
        {
            get { return XLinkHelperObj.Title; }
            set { XLinkHelperObj.Title = value; }
        }

        /// <summary>
        /// This field is set in other setters. Therefore, do not set it except in this class.
        /// </summary>
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public typeType Type
        {
            get { return XLinkHelperObj.Type; }
            set { XLinkHelperObj.Type = value; }
        }
    }

    public partial class FeaturePropertyType
    {
        private XLinkHelper m_xLinkHelperObj = new XLinkHelper();

        [XmlIgnore()]
        internal XLinkHelper XLinkHelperObj
        {
            get { return m_xLinkHelperObj; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "title", Namespace = "http://www.w3.org/1999/xlink")]
        public string Title
        {
            get { return XLinkHelperObj.Title; }
            set { XLinkHelperObj.Title = value; }
        }

        /// <summary>
        /// This field is set in other setters. Therefore, do not set it except in this class.
        /// </summary>
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public typeType Type
        {
            get { return XLinkHelperObj.Type; }
            set { XLinkHelperObj.Type = value; }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get { return XLinkHelperObj.XLinkSpecified; }
            set { } // Not to be set explicitly
        }
    }

    public partial class DQ_Element_PropertyType
    {
        private XLinkHelper m_xLinkHelperObj = new XLinkHelper();


        [XmlIgnore()]
        private XLinkHelper XLinkHelperObj
        {
            get { return m_xLinkHelperObj; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "title", Namespace = "http://www.w3.org/1999/xlink")]
        public string Title
        {
            get { return XLinkHelperObj.Title; }
            set { XLinkHelperObj.Title = value; }
        }

        /// <summary>
        /// This field is set in other setters. Therefore, do not set it except in this class.
        /// </summary>
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public typeType Type
        {
            get { return XLinkHelperObj.Type; }
            set { XLinkHelperObj.Type = value; }
        }
    }

    public partial class QualityPropertyType
    {
        private XLinkHelper m_xLinkHelperObj = new XLinkHelper();


        [XmlIgnore()]
        private XLinkHelper XLinkHelperObj
        {
            get { return m_xLinkHelperObj; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "title", Namespace = "http://www.w3.org/1999/xlink")]
        public string Title
        {
            get { return XLinkHelperObj.Title; }
            set { XLinkHelperObj.Title = value; }
        }

        /// <summary>
        /// This field is set in other setters. Therefore, do not set it except in this class.
        /// </summary>
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public typeType Type
        {
            get { return XLinkHelperObj.Type; }
            set { XLinkHelperObj.Type = value; }
        }
    }

    #endregion


    #region Generics-related failures of the .NET XmlSerialiser class
    
    public partial class RangeSetType
    {
        // The proxy generation for elements QuantityList and CategoryList has failed so this partial class exists.

        /// <remarks/>
        //[XmlElement("AbstractScalarValueList", typeof(object))]
        [XmlElement("CategoryList", typeof(CodeOrNilReasonListType))] // AbstractScalarValueList (anyType)
        [XmlElement("QuantityList", typeof(MeasureOrNilReasonListType))] // AbstractScalarValueList (anyType)
        [XmlElement("DataBlock", typeof(DataBlockType))]
        [XmlElement("File", typeof(FileType))]
        [XmlElement("ValueArray", typeof(ValueArrayType))]
        public object[] Items
        {
            get;
            set;
        }
    }

    public partial class DomainSetType
    {
        // This partial class exists because the original DomainSetType fails to deserialise TimePositionList
        
        /// <summary>
        /// Time position list.
        /// </summary>
        [XmlElement(ElementName = "TimePositionList", Type = typeof(TimePositionListType), Namespace = XsdConstants.TSML_NS)]
        public TimePositionListType TimePositionList
        {
            get;
            set;
        }
    }

    public partial class TimePositionListType
    {
        // The "Timestamps" functions facilitates the processing of the datetime values of the XML proxy.

        /// <summary>
        /// Gets timestamp values.
        /// </summary>
        /// <returns>Timestamps.</returns>
        /// <exception cref="FormatException">Thrown if the timestamps are in an invalid format.</exception>
        internal List<DateTime> GetTimestampsHelper()
        {
            return XNeut.CollectionHelper.StringToDateTimeCollection(this.timePositionList);
        }

        /// <summary>
        /// Sets timestamp values.
        /// </summary>
        /// <param name="dt">Timestamps.</param>
        /// <exception cref="XNeut.DateTimeException">Thrown if DateTime kind is not UTC.</exception>
        internal void SetTimestampsHelper(List<DateTime> dt)
        {
            timePositionList = XNeut.CollectionHelper.DateTimeCollectionToString(dt); // throws DateTimeException
        }
    }

    public partial class ExtensionType
    {
        // The generate ExtensionType proxy fails to deserialise the TimeseriesMetadataExtension field.
        
        /// <remarks/>
        [XmlElement(Namespace = XsdConstants.TSML_NS, Type = typeof(TimeseriesMetadataExtensionType))]
        public object TimeseriesMetadataExtension
        {
            get;
            set;
        }
    }

    public abstract partial class ExtensibleResponseType
    {
        // Without this class, the "extension" member would be
        // deserialised as XmlNodes
        
        /// <summary>
        /// Extension object. For some reason, this explicit typing is required here.
        /// </summary>
        [XmlElement(ElementName = "extension", Type = typeof(DataRecordPropertyType))]
        public object[] Extension
        {
            get { return extensionField; }
            set { extensionField = value; }
        }
    }
    
    public partial class AbstractDataComponentPropertyType
    {
        // This partial class was created because the original DataArrayTypeElementType
        // fails to deserialise the AbstractDataComponent member.

        /// <remarks/>
        [XmlElement(ElementName = "DataRecord")]
        public DataRecordType AbstractDataComponent
        {
            get;
            set;
        }
    }

    #endregion


    #region Generics: Temporal filter for GetObservation

    public partial class GetObservationTypeTemporalFilter
    {
        // The default proxy fails to deserialise the temporalOps field.
        // In the XSD, there are multiple elements that may substitute
        // the "temporalOps" element but "xsd.exe" fails to recognise it.

        /// <summary>
        /// Temporal operations in the filter.
        /// </summary>
        [XmlElement(ElementName = "After", Namespace = XsdConstants.FES_NS, Type = typeof(BinaryTemporalOpType_TimeInstant))]
        [XmlElement(ElementName = "Before", Namespace = XsdConstants.FES_NS, Type = typeof(BinaryTemporalOpType_TimeInstant))]
        [XmlElement(ElementName = "During", Namespace = XsdConstants.FES_NS, Type = typeof(BinaryTemporalOpType_TimePeriod))]
        [XmlChoiceIdentifier("TemporalOpsTypeInfo")]
        public BinaryTemporalOpType TemporalOps
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate the type of the temporalOps field.
        /// This field is required for the deserialisation to work.
        /// </summary>
        [XmlIgnore()]
        public TemporalObsTypeType TemporalOpsTypeInfo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Required for serialisation.
    /// </summary>
    public enum TemporalObsTypeType
    {
        /// <summary>
        /// No value.
        /// </summary>
        None,
        /// <summary>
        /// After.
        /// </summary>
        [XmlEnum(Name = XsdConstants.FES_NS + ":After")]
        After,
        /// <summary>
        /// Before.
        /// </summary>
        [XmlEnum(Name = XsdConstants.FES_NS + ":Before")]
        Before,
        /// <summary>
        /// During.
        /// </summary>
        [XmlEnum(Name = XsdConstants.FES_NS + ":During")]
        During
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = XsdConstants.FES_NS)]
    public class BinaryTemporalOpType : TemporalOpsType
    {
        /// <summary>
        /// Value reference.
        /// </summary>
        public string ValueReference
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This class exists to enable the serialisation of temporal filters.
    /// </summary>
    [Serializable()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = XsdConstants.FES_NS)]
    public class BinaryTemporalOpType_TimeInstant : BinaryTemporalOpType
    {
        /// <summary>
        /// Time instant.
        /// </summary>
        [XmlElement(Namespace = XsdConstants.GML_NS)]
        public TimeInstantType TimeInstant
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This class exists to enable the serialisation of temporal filters.
    /// </summary>
    [Serializable()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = XsdConstants.FES_NS)]
    public class BinaryTemporalOpType_TimePeriod : BinaryTemporalOpType
    {
        /// <summary>
        /// Time period.
        /// </summary>
        [XmlElement(Namespace = XsdConstants.GML_NS)]
        public TimePeriodType TimePeriod
        {
            get;
            set;
        }
    }

    #endregion Generics: Temporal filter for GetObservation


    #region Generics: Data record

    /// <summary>
    /// Data record type.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = XsdConstants.SWE_NS)]
    public class DataRecordTypeField
    {
        // This class entirely replaces the original DataRecordTypeField class.
        // The reason is that the class XmlSerialiser fails to deserialise the 
        // field objects using the original class.
        // This choice-based approach seems to work decently (please note
        // that the original schema is not based on a choice but on inheritance).
        //
        // See the documentation of XmlChoiceIdentifierAttribute Class:
        // https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlchoiceidentifierattribute(v=vs.110).aspx
        //
        // If there are problems with the choice approach, another possibility
        // is to use XmlAnyElement for the field values and then query the contents
        // with XPath.

        /// <remarks/>
        [XmlAttribute(DataType = "NCName")]
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// Data component.
        /// </summary>
        [XmlElement(ElementName = "DataArray", Namespace = XsdConstants.SWE_NS, Type = typeof(DataArrayType))]
        [XmlElement(ElementName = "Boolean", Namespace = XsdConstants.SWE_NS, Type = typeof(BooleanType))]
        [XmlElement(ElementName = "Category", Namespace = XsdConstants.SWE_NS, Type = typeof(CategoryType))]
        [XmlElement(ElementName = "CategoryRange", Namespace = XsdConstants.SWE_NS, Type = typeof(CategoryRangeType))]
        [XmlElement(ElementName = "Count", Namespace = XsdConstants.SWE_NS, Type = typeof(CountType))]
        [XmlElement(ElementName = "CountRange", Namespace = XsdConstants.SWE_NS, Type = typeof(CountRangeType))]
        [XmlElement(ElementName = "DataRecord", Namespace = XsdConstants.SWE_NS, Type = typeof(DataRecordType))]
        [XmlElement(ElementName = "Quantity", Namespace = XsdConstants.SWE_NS, Type = typeof(QuantityType))]
        [XmlElement(ElementName = "QuantityRange", Namespace = XsdConstants.SWE_NS, Type = typeof(QuantityRangeType))]
        [XmlElement(ElementName = "Text", Namespace = XsdConstants.SWE_NS, Type = typeof(TextType))]
        [XmlElement(ElementName = "Time", Namespace = XsdConstants.SWE_NS, Type = typeof(TimeType1))]
        [XmlElement(ElementName = "TimeRange", Namespace = XsdConstants.SWE_NS, Type = typeof(TimeRangeType))]
        [XmlElement(ElementName = "AbstractGmlAsSweDataComponent", Namespace = XsdConstants.COCOP_EXTENSION_NS_V1_1, Type = typeof(AbstractGmlAsSweDataComponentType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("dataComponentTypeInfo")]
        public object dataComponent
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate the type of the dataComponent field.
        /// This field is required for the deserialisation to work.
        /// </summary>
        [XmlIgnore()]
        public DataRecordFieldTypeType dataComponentTypeInfo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This enum is required for the choice-based serialisation of DataRecordTypeField.
    /// </summary>
    public enum DataRecordFieldTypeType
    {
        /// <summary>
        /// No type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Boolean.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":Boolean")]
        Boolean,
        /// <summary>
        /// Category.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":Category")]
        Category,
        /// <summary>
        /// Category range.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":CategoryRange")]
        CategoryRange,
        /// <summary>
        /// Count.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":Count")]
        Count,
        /// <summary>
        /// Count range.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":CountRange")]
        CountRange,
        /// <summary>
        /// Data record.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":DataRecord")]
        DataRecord,
        /// <summary>
        /// Quantity (i.e., measurement).
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":Quantity")]
        Quantity,
        /// <summary>
        /// Quantity range (i.e., measurement range).
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":QuantityRange")]
        QuantityRange,
        /// <summary>
        /// Textual value.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":Text")]
        Text,
        /// <summary>
        /// Time instant.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":Time")]
        Time,
        /// <summary>
        /// Time range.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":TimeRange")]
        TimeRange,
        /// <summary>
        /// The wrapper element of a time series.
        /// </summary>
        [XmlEnum(Name = XsdConstants.COCOP_EXTENSION_NS_V1_1 + ":AbstractGmlAsSweDataComponent")]
        AbstractGmlAsSweDataComponent,
        /// <summary>
        /// The wrapper element of a time series.
        /// </summary>
        [XmlEnum(Name = XsdConstants.SWE_NS + ":DataArray")]
        DataArray
    }

    #endregion Generics: Data record


    #region Generics: AbstractGmlAsSweDataComponent

    public partial class AbstractGmlAsSweDataComponentType : AbstractDataComponentType
    {
        // Using choice-based typing for the AbstractGML field (XML inheritance does not work here as it should)

        /// <summary>
        /// The "AbstractGML" element which will have a subtype as the actual type.
        /// </summary>
        [XmlElement(ElementName = "TimeseriesDomainRange", Namespace = XsdConstants.TSML_NS, Type = typeof(TimeseriesDomainRangeType))]
        [XmlChoiceIdentifier("AbstractGMLTypeInfo")]
        public AbstractGMLType AbstractGML
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate the type of the AbstractGML field.
        /// This field is required for the serialisation to work.
        /// </summary>
        [XmlIgnore()]
        public AbstractGMLTypeType AbstractGMLTypeInfo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This is required for AbstractGmlAsSweDataComponentType.
    /// </summary>
    public enum AbstractGMLTypeType
    {
        /// <summary>
        /// Default: no type specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// Time series enclosed.
        /// </summary>
        [XmlEnum(Name = XsdConstants.TSML_NS + ":TimeseriesDomainRange")]
        TimeseriesDomainRangeType
    }

    #endregion Generics: AbstractGmlAsSweDataComponent


    #region Generics: SweDataComponentAsFeatureType

    public partial class FeaturePropertyType
    {
        // Using choice-based typing for the AbstractGML field (XML inheritance does not work here as it should)

        /// <summary>
        /// The "AbstractFeature" element which will have a subtype as the actual type.
        /// </summary>
        [XmlElement(ElementName = "SweDataComponentAsFeature", Namespace = XsdConstants.COCOP_EXTENSION_NS_V1_1, Type = typeof(SweDataComponentAsFeatureType))]
        [XmlChoiceIdentifier("AbstractFeatureTypeInfo")]
        public AbstractFeatureType AbstractFeature
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate the type of the AbstractFeature field.
        /// This field is required for the serialisation to work.
        /// </summary>
        [XmlIgnore()]
        public AbstractFeatureTypeType AbstractFeatureTypeInfo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This is required for AbstractFeatureType in FeaturePropertyType.
    /// </summary>
    public enum AbstractFeatureTypeType
    {
        /// <summary>
        /// Default: no type specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// SWE data component enclosed.
        /// </summary>
        [XmlEnum(Name = XsdConstants.COCOP_EXTENSION_NS_V1_1 + ":SweDataComponentAsFeature")]
        SweDataComponentAsFeatureType
    }

    #endregion Generics: SweDataComponentAsFeatureType


    #region DataArray

    /// <remarks/>
    public partial class EncodedValuesPropertyType
    {
        // Without this partial class, there is no means to access the child element
        
        /// <remarks/>
        [XmlElement(Namespace = XsdConstants.COCOP_EXTENSION_NS_V1_2)]
        public ArrayType Array
        {
            // This ArrayType is a COCOP extension
            get;
            set;
        }
    }

    public partial class ArrayType
    {
        // Deserialisation failed with the xsd.exe-generated 2-dimensional array.

        /// <remarks/>
        [XmlElement(ElementName = "Row", Namespace = XsdConstants.COCOP_EXTENSION_NS_V1_2, Type = typeof(ArrayRowType))]
        public ArrayRowType[] Row
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = XsdConstants.COCOP_EXTENSION_NS_V1_2)]
    public class ArrayRowType
    {
        // xsd.exe did not generate this type from the XML schema

        /// <remarks/>
        [XmlElement(ElementName = "I", Namespace = XsdConstants.COCOP_EXTENSION_NS_V1_2, Type = typeof(string))]
        public string[] I
        {
            get;
            set;
        }
    }

    #endregion
}
