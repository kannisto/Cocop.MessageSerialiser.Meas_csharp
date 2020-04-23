//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 8/2018
// Last modified: 2/2020

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cocop.MessageSerialiser.Meas;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;

namespace TaskingParameterProcessorTest
{
    [TestClass]
    public class UnitTest1
    {
        // This test focuses on:
        // - Reading an empty parameter set (101)
        // - Reading a parameter set (102)
        // - Creating an empty parameter set (201)
        // - Creating a parameter set (202)
        //
        // This test does *not* focus on:
        // - Item_DataRecord class (tested elsewhere)

        [TestMethod]
        public void TaskingPmProc_101_ReadEmpty()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskSubmitRequest_empty_params.xml";
            var xmlBytes = ReadFile(filepath);
            var statusReportProxy = GetParamsFromXml(xmlBytes);

            var testObject = new TaskingParameterProcessor(statusReportProxy);

            Assert.AreEqual(0, testObject.Parameters.ItemNames.Count);
        }

        [TestMethod]
        public void TaskingPmProc_102_Read()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskSubmitRequest.xml";
            var xmlBytes = ReadFile(filepath);
            var statusReportProxy = GetParamsFromXml(xmlBytes);

            var testObject = new TaskingParameterProcessor(statusReportProxy);

            // Not asserting the contents of the data record (tested elsewhere)
            Assert.AreEqual(2, testObject.Parameters.ItemNames.Count);
        }

        [TestMethod]
        public void TaskingPmProc_201_CreateEmpty()
        {
            var dataRecord = new Item_DataRecord();
            var testObject = new TaskingParameterProcessor(dataRecord);

            // Serialise, validate and deserialise
            byte[] xmlBytes = Serialise(testObject.ToXmlProxy("test-"));
            Validate(xmlBytes);
            var statusReportProxy = GetParamsFromXml(xmlBytes);
            var testObjectIn = new TaskingParameterProcessor(statusReportProxy);

            Assert.AreEqual(0, testObjectIn.Parameters.ItemNames.Count);
        }

        [TestMethod]
        public void TaskingPmProc_201_Create()
        {
            var dataRecord = new Item_DataRecord
            {
                { "mycategory", new Item_Category("testctg") },
                { "mycount", new Item_Count(7) }
            };
            var testObject = new TaskingParameterProcessor(dataRecord);

            // Serialise, validate and deserialise
            byte[] xmlBytes = Serialise(testObject.ToXmlProxy("test-"));
            Validate(xmlBytes);
            var statusReportProxy = GetParamsFromXml(xmlBytes);
            var testObjectIn = new TaskingParameterProcessor(statusReportProxy);

            Assert.AreEqual(2, testObjectIn.Parameters.ItemNames.Count);
            Assert.IsTrue(testObjectIn.Parameters.ItemNames.Contains("mycount"));
        }

        private XsdNs.ParameterDataType GetParamsFromXml(byte[] xmlBytes)
        {
            // Assuming Submit (TaskingRequest)
            var xmlDocProxy = (XsdNs.SubmitType)XNeut.Helper.DeserialiseFromXml(typeof(XsdNs.SubmitType), xmlBytes);
            return xmlDocProxy.taskingParameters.ParameterData;
        }

        private byte[] ReadFile(string filepath)
        {
            return System.IO.File.ReadAllBytes(filepath);
        }

        private byte[] Serialise(XsdNs.ParameterDataType proxy)
        {
            // Using "submit" request as the wrapper
            var xmlDocProxy = new XsdNs.SubmitType()
            {
                service = "SPS",
                version = "2.0.0",
                procedure = "my-procedure",
                taskingParameters = new XsdNs.TaskingRequestTypeTaskingParameters()
                {
                    ParameterData = proxy
                }
            };

            return XNeut.Helper.ToXmlBytes(xmlDocProxy);
        }

        private void Validate(byte[] xmlBytes)
        {
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Sps_GmlSwe);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.SubmitType));
            }
        }
    }
}
