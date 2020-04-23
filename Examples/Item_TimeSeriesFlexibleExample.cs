//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2/2020
// Last modified: 3/2020

using System;
using MsgMeas = Cocop.MessageSerialiser.Meas;

namespace Examples
{
    class Item_TimeSeriesFlexibleExample
    {
        // This example shows how to create and read an Item_TimeSeriesFlexible.

        public void Create()
        {
            // Creating the timestamps of values
            var timestamp1 = DateTime.Parse("2018-02-16T09:00:00+02:00").ToUniversalTime();
            var timestamp2 = DateTime.Parse("2018-02-16T10:00:00+02:00").ToUniversalTime();
            var timestamp3 = DateTime.Parse("2018-02-16T11:00:00+02:00").ToUniversalTime();
            var timestamp4 = DateTime.Parse("2018-02-16T12:00:00+02:00").ToUniversalTime();

            // Creating a time series with the unit of measure "Cel" (Celsius)
            var series = new MsgMeas.Item_TimeSeriesFlexible("Cel")
            {
                { timestamp1, 22.3, MsgMeas.DataQuality.CreateBad() },
                { timestamp2, 24.5 }, // No data quality specified -> "good" assumed
                { timestamp3, 24.8, MsgMeas.DataQuality.CreateGood() }
            };

            // The "Add" method can also be used
            series.Add(timestamp4, 24.7);

            // Enclosing the series into an Observation
            var myObservation = new MsgMeas.Observation(series)
            {
                FeatureOfInterest = "ti300",
                ObservedProperty = "temperature",
                Description = "Bottom temperature in T300"
            };

            byte[] xmlBytes = myObservation.ToXmlBytes();
            // Do something with the XML data, e.g., send to AMQP topic...
        }

        public void Read(byte[] xmlBytes)
        {
            // Assuming the time series comes enclosed in an Observation
            MsgMeas.Observation myObservation;
            try
            {
                myObservation = new MsgMeas.Observation(xmlBytes);
            }
            catch (MsgMeas.Neutral.InvalidMessageException e)
            {
                throw new InvalidOperationException("Failed to read observation: " + e.Message);
            }

            var series = (MsgMeas.Item_TimeSeriesFlexible)myObservation.Result;

            Console.WriteLine("Hourly surface level values: ");
            Console.WriteLine("(" + series.ValueCount + " values in total)");

            // Printing the values in the time series
            foreach (MsgMeas.Item_TimeSeriesFlexible.TimeSeriesFlexItem item in series)
            {
                // Print timestamp, measured value, measurement unit and data quality information
                var line = string.Format("{0} {1} {2} {3}", item.Timestamp.ToString("yyyy-MM-dd HH:mm"),
                    item.Value, series.UnitOfMeasure, item.DataQualityObj.Value);
                Console.WriteLine(line);
            }
        }
    }
}
