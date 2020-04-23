//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2018
// Last modified: 2/2020

using System;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// A class to manage result typing. It was implemented to facilitate the testing of 
    /// Item_* classes, as it also needs this functionality.
    /// 
    /// This class is tested in Item_* tests.
    /// 
    /// This functionality could be located in the Item class, but that would make
    /// the Item class dependent on its subclasses, which would be bad design
    /// (bi-directional dependencies).
    /// </summary>
    internal class ResultTypeManager
    {
        /// <summary>
        /// Builds a result object according to the given result type.
        /// </summary>
        /// <param name="obsType">Result type.</param>
        /// <param name="result">Raw result object from XML.</param>
        /// <returns>Result object.</returns>
        /// <exception cref="InvalidCastException">Typing caused an error.</exception>
        /// <exception cref="NotSupportedException">The observation type is not supported.</exception>
        public static Item BuildResultFromXml(string obsType, object result)
	    {
            switch (obsType)
            {
                case XNeut.Helper.TypeUri_Category:
                    return new Item_Category((XsdNs.ReferenceType)result);

                case XNeut.Helper.TypeUri_CategoryRange:
                    return new Item_CategoryRange((XsdNs.CategoryRangeType)result);

                case XNeut.Helper.TypeUri_Complex:
                    return BuildComplex(result);

                case XNeut.Helper.TypeUri_Count:
                    return new Item_Count((string)result);

                case XNeut.Helper.TypeUri_CountRange:
                    return new Item_CountRange((XsdNs.CountRangeType)result);

                case XNeut.Helper.TypeUri_Measurement:
                    return new Item_Measurement((XsdNs.MeasureType)result);

                case XNeut.Helper.TypeUri_MeasurementRange:
                    return new Item_MeasurementRange((XsdNs.QuantityRangeType)result);

                case XNeut.Helper.TypeUri_Text:
                    return new Item_Text((string)result);

                case XNeut.Helper.TypeUri_TimeSeriesConstant:
                    return new Item_TimeSeriesConstant((XsdNs.TimeseriesDomainRangeType)result);

                case XNeut.Helper.TypeUri_TimeSeriesFlexible:
                    return new Item_TimeSeriesFlexible((XsdNs.TimeseriesDomainRangeType)result);

                case XNeut.Helper.TypeUri_Truth:
                    return new Item_Boolean((bool)result);
                    
                case XNeut.Helper.TypeUri_Temporal:
                    return BuildTemporal(result);

                default:
                    throw new NotSupportedException("No support implemented for type \"" + obsType + "\"");
            }
	    }

        private static Item BuildComplex(object result)
        {
            var resultType = result.GetType();

            if (resultType.Equals(typeof(XsdNs.DataRecordPropertyType)))
            {
                return new Item_DataRecord((XsdNs.DataRecordPropertyType)result);
            }
            else if (resultType.Equals(typeof(XsdNs.DataArrayType)))
            {
                return new Item_Array((XsdNs.DataArrayType)result);
            }
            else
            {
                throw new NotSupportedException("Unexpected result type in complex observation");
            }
        }

        private static Item BuildTemporal(object result)
        {
            var resultType = result.GetType();

            if (resultType.Equals(typeof(XsdNs.TimeInstantPropertyType)))
            {
                return new Item_TimeInstant((XsdNs.TimeInstantPropertyType)result);
            }
            else if (resultType.Equals(typeof(XsdNs.TimePeriodPropertyType)))
            {
                return new Item_TimeRange((XsdNs.TimePeriodPropertyType)result);
            }
            else
            {
                throw new NotSupportedException("Unexpected result type in temporal observation");
            }
        }
    }
}
