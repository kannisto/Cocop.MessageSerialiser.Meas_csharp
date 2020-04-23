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
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Represents a temporal filter.
    /// <para>In this module, the code has been derived from OGC Filter Encoding 2.0 Encoding Standard – With Corrigendum
    /// (OGC 09-026r2; please see the file "ref_and_license_ogc_filter.txt")
    /// and OGC(r) Sensor Observation Service Interface Standard
    /// (OGC 12-006; please see the file "ref_and_license_ogc_sos.txt").</para>
    /// </summary>
    public sealed class TemporalFilter
    {
        private const string ValueRefPhenoTime = "phenomenonTime";
        private const string ValueRefResultTime = "resultTime";


        /// <summary>
        /// Reference to the value that this filter refers to.
        /// </summary>
        public enum ValueReferenceType
        {
            /// <summary>
            /// The phenomenon time of an observation.
            /// </summary>
            PhenomenonTime = 0,
            /// <summary>
            /// The result time of an observation.
            /// </summary>
            ResultTime = 1
        }

        /// <summary>
        /// Operators.
        /// </summary>
        public enum OperatorType
        {
            /// <summary>
            /// Indicates something after a certain time.
            /// </summary>
            After = 0,
            /// <summary>
            /// Indicates something before a certain time.
            /// </summary>
            Before = 1,
            /// <summary>
            /// Indicates something during a certain time period.
            /// </summary>
            During = 2
        }


        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="valRef">Reference to the value that this filter refers to.</param>
        /// <param name="op">The operator to be applied.</param>
        /// <param name="time">The definition of time. Depending on the operator, 
        /// this must be either a time instant or a time range.</param>
        /// <exception cref="ArgumentException">Thrown if the operator is incompatible with the time definition.</exception>
        public TemporalFilter(ValueReferenceType valRef, OperatorType op, Item time)
        {
            CheckConsistency(op, time);

            ValueReference = valRef;
            Operator = op;
            Time = time;
        }

        /// <summary>
        /// Constructor. Use this to populate the object from an XML proxy.
        /// </summary>
        /// <param name="proxy">Proxy.</param>
        /// <exception cref="XNeut.InvalidMessageException">Thrown if the message is invalid.</exception>
        internal TemporalFilter(XsdNs.GetObservationTypeTemporalFilter proxy)
        {
            try
            {
                // Mapping value reference
                switch (proxy.TemporalOps.ValueReference)
                {
                    case ValueRefPhenoTime:
                        ValueReference = ValueReferenceType.PhenomenonTime;
                        break;

                    case ValueRefResultTime:
                        ValueReference = ValueReferenceType.ResultTime;
                        break;

                    default:
                        throw new XNeut.InvalidMessageException("Unsupported value reference " + proxy.TemporalOps.ValueReference);
                }
                
                // Mapping the operator
                switch (proxy.TemporalOpsTypeInfo)
                {
                    case XsdNs.TemporalObsTypeType.After:
                        Operator = OperatorType.After;
                        break;

                    case XsdNs.TemporalObsTypeType.Before:
                        Operator = OperatorType.Before;
                        
                        break;

                    case XsdNs.TemporalObsTypeType.During:
                        Operator = OperatorType.During;
                        break;

                    default:
                        throw new XNeut.InvalidMessageException("Unsupported temporal operator in message: " + proxy.TemporalOpsTypeInfo.ToString());
                }

                // Getting time item
                Time = GetTimeItemFromProxy(proxy);

                // Checking consistency
                CheckConsistency(Operator, Time);
            }
            catch (ArgumentException e)
            {
                throw new XNeut.InvalidMessageException("Failed to process temporal filter (inconsistent values?)", e);
            }
            catch (NullReferenceException e)
            {
                throw new XNeut.InvalidMessageException("Failed to process temporal filter (something missing?)", e);
            }
        }

        #endregion


        #region Proxy generation

        /// <summary>
        /// Generates an XML proxy from the object.
        /// </summary>
        /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
        /// require an ID. For certain XML elements, the schema requires an ID that is unique within
        /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
        /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
        /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
        /// <returns>Proxy.</returns>
        internal XsdNs.GetObservationTypeTemporalFilter ToXmlProxy(string idPrefix)
        {
            var proxy = new XsdNs.GetObservationTypeTemporalFilter();
            var idPrefixAll = idPrefix + "TempF_";
            
            // Creating condition element
            switch (Operator)
            {
                case OperatorType.After:

                    proxy.TemporalOpsTypeInfo = XsdNs.TemporalObsTypeType.After;
                    proxy.TemporalOps = CreateTimeInstantForProxy(idPrefixAll);
                    break;

                case OperatorType.Before:

                    proxy.TemporalOpsTypeInfo = XsdNs.TemporalObsTypeType.Before;
                    proxy.TemporalOps = CreateTimeInstantForProxy(idPrefixAll);
                    break;

                case OperatorType.During:

                    proxy.TemporalOpsTypeInfo = XsdNs.TemporalObsTypeType.During;
                    proxy.TemporalOps = CreateTimePeriodForProxy(idPrefixAll);
                    break;

                default:
                    throw new NotImplementedException("Unsupported operator " + Operator.ToString());
            }

            // Setting value reference
            switch (ValueReference)
            {
                case ValueReferenceType.PhenomenonTime:
                    proxy.TemporalOps.ValueReference = ValueRefPhenoTime;
                    break;

                case ValueReferenceType.ResultTime:
                    proxy.TemporalOps.ValueReference = ValueRefResultTime;
                    break;

                default:
                    throw new NotImplementedException("Unsupported value reference " + ValueReference.ToString());
            }

            return proxy;
        }

        private XsdNs.BinaryTemporalOpType_TimeInstant CreateTimeInstantForProxy(string idPrefix)
        {
            var myTimeInstant = (Item_TimeInstant)Time;

            // This will return a time instant property
            var timeInstantProp = (XsdNs.TimeInstantPropertyType)myTimeInstant.GetObjectForXml_Result(idPrefix);

            return new XsdNs.BinaryTemporalOpType_TimeInstant
            {
                TimeInstant = timeInstantProp.TimeInstant
            };
        }

        private XsdNs.BinaryTemporalOpType_TimePeriod CreateTimePeriodForProxy(string idPrefix)
        {
            var myTimeRange = (Item_TimeRange)Time;

            // This will return a time period property
            var timePeriodProperty = (XsdNs.TimePeriodPropertyType)myTimeRange.GetObjectForXml_Result(idPrefix);

            return new XsdNs.BinaryTemporalOpType_TimePeriod
            {
                TimePeriod = timePeriodProperty.TimePeriod
            };
        }

        #endregion


        #region Properties

        /// <summary>
        /// The value reference.
        /// </summary>
        public ValueReferenceType ValueReference
        {
            get;
            private set;
        }

        /// <summary>
        /// The operator.
        /// </summary>
        public OperatorType Operator
        {
            get;
            private set;
        }

        /// <summary>
        /// The associated time object.
        /// </summary>
        public Item Time
        {
            get;
            private set;
        }

        #endregion


        #region Other private methods

        private void CheckConsistency(OperatorType op, Item time)
        {
            // Checking the type of the time item
            switch (op)
            {
                case OperatorType.After:
                case OperatorType.Before:

                    if (time.GetType() != typeof(Item_TimeInstant))
                    {
                        throw new ArgumentException("Only time instants are compatible with operator " + op.ToString(), "time");
                    }

                    break;

                case OperatorType.During:

                    if (time.GetType() != typeof(Item_TimeRange))
                    {
                        throw new ArgumentException("Only time ranges are compatible with operator " + op.ToString(), "time");
                    }

                    break;

                default:
                    throw new NotImplementedException("There is no support for operator " + op.ToString());
            }
        }

        private Item GetTimeItemFromProxy(XsdNs.GetObservationTypeTemporalFilter proxy)
        {
            switch (proxy.TemporalOpsTypeInfo)
            {
                case XsdNs.TemporalObsTypeType.After:
                case XsdNs.TemporalObsTypeType.Before:

                    // It is necessary to build a PropertyType object
                    var timeInstantObj = ((XsdNs.BinaryTemporalOpType_TimeInstant)proxy.TemporalOps).TimeInstant;
                    var timeInstantProp = new XsdNs.TimeInstantPropertyType
                    {
                        TimeInstant = timeInstantObj
                    };

                    return new Item_TimeInstant(timeInstantProp);

                case XsdNs.TemporalObsTypeType.During:
                    
                    // It is necessary to build a PropertyType object
                    var timePeriodObj = ((XsdNs.BinaryTemporalOpType_TimePeriod)proxy.TemporalOps).TimePeriod;
                    var timePeriodProp = new XsdNs.TimePeriodPropertyType
                    {
                        TimePeriod = timePeriodObj
                    };

                    return new Item_TimeRange(timePeriodProp);

                default:
                    throw new XNeut.InvalidMessageException("Unsupported temporal operator in message: " + proxy.TemporalOpsTypeInfo.ToString());
            }
        }

        #endregion
    }
}
