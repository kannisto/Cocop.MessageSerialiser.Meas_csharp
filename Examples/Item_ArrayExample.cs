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
    // This example shows how to create and read an Item_Array.

    class Item_ArrayExample
    {
        public void Create()
        {
            // Create columns (3 in total).
            // Please see the documentation of constructors in Item_Array.ArrayColumn to
            // see which data types are supported.
            var myColumns = new SysColl.List<MsgMeas.Item_Array.ArrayColumn>()
            {
                new MsgMeas.Item_Array.ArrayColumn(typeof(DateTime), "timestamp"),
                new MsgMeas.Item_Array.ArrayColumn(typeof(double), "temperature", "Cel"),
                new MsgMeas.Item_Array.ArrayColumn(typeof(long), "batchcount")
            };

            // Create the array item and its rows (2 rows in total).
            // These must match respective column types.
            var myArray = new MsgMeas.Item_Array(myColumns)
            {
                { DateTime.Now.ToUniversalTime().AddHours(-1), -4.2, (long)29 },
                { DateTime.Now.ToUniversalTime(), -3.8, (long)32 }
            };

            // You can also use the "Add" method instead of the array initialiser shown above.
            // The Add method can take any number of arguments.
            myArray.Add(DateTime.Now.ToUniversalTime(), -2.1, (long)36);

            // Now, you can enclose the array in an Observation or Item_DataRecord. E.g.,
            var myObservation = new MsgMeas.Observation(myArray);
            byte[] xmlBytes = myObservation.ToXmlBytes();
        }

        public void Read(MsgMeas.Observation myObservation)
        {
            // This function assumes that you get the array in an Observation.

            var myArray = (MsgMeas.Item_Array)myObservation.Result;

            // Access one field in the array - [row][column]:
            double temperature1 = (double)myArray[0][1];
            double temperature2 = (double)myArray[1][1];

            // Do what you want with the values
            // ...
        }
    }
}
