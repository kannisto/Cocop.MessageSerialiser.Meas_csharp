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
    class Item_TimeSeriesConstantExample
    {
        // This example shows how to create and read an Item_TimeSeriesConstant.

        public void Create()
        {
            // This time series represents the hourly accumulation of rainfall in millimeters (mm)
            var series = new MsgMeas.Item_TimeSeriesConstant("m", DateTime.Now.ToUniversalTime(), TimeSpan.FromHours(1))
            {
                { 0.4 }, // No data quality specified -> "good" is assumed
                { 0.5 },
                { 1.1, MsgMeas.DataQuality.CreateBad() },
                { 1.7, MsgMeas.DataQuality.CreateGood() }
            };

            // Can also use the "Add" method
            series.Add(1.5, MsgMeas.DataQuality.CreateGood());

            // Enclosing the series into an Observation
            var myObservation = new MsgMeas.Observation(series)
            {
                FeatureOfInterest = "li300",
                ObservedProperty = "surface-level",
                Description = "Surface level of T300 once every hour"
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

            var series = (MsgMeas.Item_TimeSeriesConstant)myObservation.Result;

            Console.WriteLine("Hourly surface level values: ");
            Console.WriteLine("(" + series.ValueCount + " values in total)");

            // Printing the values in the time series
            int index = 0;

            foreach (MsgMeas.Item_TimeSeriesConstant.TimeSeriesConstItem item in series)
            {
                // Calculate the difference to start
                var timestamp = series.BaseTime.AddMinutes(index * series.Spacing.TotalMinutes);

                // Print timestamp, measured value, measurement unit and data quality information
                var line = string.Format("{0} {1} {2} {3}", timestamp.ToString("yyyy-MM-dd HH:mm"),
                    item.Value, series.UnitOfMeasure, item.DataQualityObj.Value);
                Console.WriteLine(line);

                ++index;
            }
        }
    }
}
