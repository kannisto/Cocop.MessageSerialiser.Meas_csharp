﻿//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2019
// Last modified: 3/2020


using System;
using System.Linq;

namespace Cocop.MessageSerialiser.Meas.Neutral
{
    /// <summary>
    /// Stub class
    /// </summary>
    class Helper
    {
        internal static bool StringContainsSpace(string str)
        {
            return str.Any(x => char.IsWhiteSpace(x));
        }
    }
}
