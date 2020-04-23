//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 2019
// Last modified: 2/2020


using System;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace Cocop.MessageSerialiser.Meas
{
    class InsertObservationResponseForTest : ExtensibleResponse
    {
        // Stub class for test
        
        public InsertObservationResponseForTest()
            : base()
        {
            // Empty ctor body
        }
        
        public InsertObservationResponseForTest(byte[] xmlBytes)
            : base()
        {
            // Deserialising the document
            var proxy = (XsdNs.InsertObservationResponseType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.InsertObservationResponseType), xmlBytes);
            ReadExtensibleResponseItemsFromProxy(proxy);
        }

        public byte[] ToXmlBytes()
        {
            var proxy = new XsdNs.InsertObservationResponseType();

            // Populating data from the base class
            PopulateExtensibleResponseToProxy(proxy);

            try
            {
                // Serialising
                return XNeut.Helper.ToXmlBytes(proxy);
            }
            catch (InvalidOperationException e)
            {
                throw new XNeut.InvalidMessageException("XML serialisation failed", e);
            }
        }
    }
}
