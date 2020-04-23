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
using SysColl = System.Collections.Generic;
using MsgMeas = Cocop.MessageSerialiser.Meas;

namespace Examples
{
    class Item_DataRecordExample
    {
        // This example shows
        // 1) How to create an Item_DataRecord object and enclose it in an Observation
        // 2) How to read an Item_DataRecord enclosed in an Observation

        public void Create()
        {
            // This example creates a data record with the following structure:
            // - dataRecord
            //   - MyMeas: Item_Measurement, 45.3 s, good quality (implicit)
            //   - MyTime: Item_TimeInstant, 2018-03-02T14:22:05Z, bad quality (explicit)
            //   - NestedRecord: Item_DataRecord
            //     - NestedMeas: Item_Measurement, -0.34 m, good quality (implicit)

            // The easiest way to add fields is to use the collection initialiser as below.
            // In each row, the items are as follows:
            // 1) the name of the data record field,
            // 2) the related Item_* object,
            // 3) data quality information (optional)
            var dataRecord = new MsgMeas.Item_DataRecord()
            {
                // Adding a measurement value
                { "MyMeas", new MsgMeas.Item_Measurement("s", 45.3) },
     
                // Adding a time instant
                { "MyTime", new MsgMeas.Item_TimeInstant(DateTime.Parse("2018-03-02T14:22:05Z").ToUniversalTime()),
                    MsgMeas.DataQuality.CreateBad() }
            };

            // You can also use the "Add" method. The code below shows this. In addition,
            // it shows that a data record can nest another data record.
            var nestedDataRecord = new MsgMeas.Item_DataRecord()
            {
                { "NestedMeas", new MsgMeas.Item_Measurement("m", -0.34) }
            };
            dataRecord.Add("NestedRecord", nestedDataRecord);

            // Now, you can include the data record in an Observation, for instance.
            var observation = new MsgMeas.Observation(dataRecord);
            var xmlBytes = observation.ToXmlBytes();
            // Send XML bytes to network...
        }

        public void Read(MsgMeas.Observation observation)
        {
            const string InstantField = "MyTime";
            const string NestedRecordField = "NestedRecord";
            const string NestedMeasurementField = "Meas";

            // If the data record resides in an observation, you can read it as follows.
            var dataRecord = (MsgMeas.Item_DataRecord)observation.Result;

            // Getting the names of the fields in the data record
            SysColl.HashSet<string> itemNames = dataRecord.ItemNames;

            if (!itemNames.Contains(InstantField) || !itemNames.Contains(NestedRecordField) ||
                !itemNames.Contains(NestedMeasurementField))
            {
                throw new InvalidOperationException("Expected field is missing from data record");
            }

            // Getting a time instant value in the data record
            var timeInstantItem = (MsgMeas.Item_TimeInstant)dataRecord[InstantField];

            // Getting a measurement value in the nested data record
            var nestedDataRecord = (MsgMeas.Item_DataRecord)dataRecord[NestedRecordField];
            var nestedMeasurement = (MsgMeas.Item_Measurement)nestedDataRecord[NestedMeasurementField];

            // Do what you want with the values...
        }
    }
}
