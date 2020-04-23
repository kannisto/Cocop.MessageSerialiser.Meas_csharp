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
    class ObservationExample
    {
        // This example shows
        // 1) how to create an observation object and serialise it to XML
        // 2) how to read an observation object from XML

        public byte[] CreateXml()
        {
            // Starting by creating the actual measurement item.
            // The type of this could as well be another class called "Item_*".
            var myMassMeasurement = new MsgMeas.Item_Measurement("Cel", 22.4);

            // Creating an observation to contain the measurement.
            // Also, specifying metadata.
            var timestamp = DateTime.Parse("2018-02-23T10:00:00Z").ToUniversalTime();

            var myObservation = new MsgMeas.Observation(myMassMeasurement)
            {
                // All of this metadata is optional. If the XML schema requires
                // a value, a default is assigned. Please see the Observations and
                // Measurements standard for a description of these fields.
                Description = "Temperature measurement from the TI-22",
                FeatureOfInterest = "factory_x/department_a/ti-22",
                ObservedProperty = "temperature",
                Procedure = "sensors/pt100",
                PhenomenonTime = timestamp, // UTC time required
                ResultTime = timestamp, // UTC time required
                ResultQuality = MsgMeas.DataQuality.CreateGood()
            };

            // Serializing the observation to XML
            return myObservation.ToXmlBytes();
        }

        public void ReadXml(byte[] xmlBytes)
        {
            MsgMeas.Observation myObservation;

            // Try to process the message
            try
            {
                myObservation = new MsgMeas.Observation(xmlBytes);
            }
            catch (MsgMeas.Neutral.InvalidMessageException e)
            {
                throw new InvalidOperationException("Failed to read observation: " + e.Message, e);
            }

            // Check if the metadata is as expected
            const string ExpectedFeature = "factory_x/department_a/ti-22";
            if (!myObservation.FeatureOfInterest.Equals(ExpectedFeature))
            {
                throw new Exception("Expected " + ExpectedFeature + " but got " + myObservation.FeatureOfInterest);
            }

            // Accessing the actual measurement value. The type could as well be any of the Item_* classes.
            var measurement = (MsgMeas.Item_Measurement)myObservation.Result;
            var msgToConsole = string.Format("The value is {0} {1}", measurement.Value, measurement.UnitOfMeasure);
            Console.WriteLine(msgToConsole);
        }
    }
}
