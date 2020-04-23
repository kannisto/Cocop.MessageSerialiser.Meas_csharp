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
using XNeut = Cocop.MessageSerialiser.Meas.Neutral;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace TaskStatusReportTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TaskStatusReport_ReadMinimal()
        {
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskStatusReport_minimal.xml";
            var xmlBytes = ReadFile(filepath);
            
            AssertDefault(
                xmlBytes: xmlBytes,
                taskId: "MyTask_1",
                procedureId: "MyProcedure_1",
                reqStatus: TaskingRequestStatusCodeType.Accepted,
                approxCreateTime: ParseDateTimeInUtc("2019-03-20T19:55:03.814Z")
                );
        }

        [TestMethod]
        public void TaskStatusReport_Read()
        {
            // Reading
            var filepath = TestCommon.TestHelper.TestFileFolder + @"\TaskStatusReport.xml";
            var statusReport = new TaskStatusReport(ReadFile(filepath));

            // Asserting

            // #1 in XML schema
            Assert.AreEqual("http://mytask", statusReport.TaskId);

            // #2 in XML schema
            AssertDateTime(ParseDateTimeInUtc("2019-03-19T09:00:00Z"), statusReport.EstimatedTimeOfCompletion.Value);

            // #3 in XML schema: event; not supported

            // #4 in XML schema
            Assert.AreEqual(52.3, statusReport.PercentCompletion.Value, 0.001);
            
            // #5 in XML schema
            Assert.AreEqual("http://myproc", statusReport.ProcedureId);

            // #6 in XML schema
            Assert.AreEqual(TaskingRequestStatusCodeType.Pending, statusReport.RequestStatus);

            // #7 in XML schema
            Assert.AreEqual(2, statusReport.StatusMessages.Count);
            Assert.AreEqual("Step 2 completed", statusReport.StatusMessages[1]);

            // #8 in XML schema
            Assert.AreEqual(TaskStatusCodeType.InExecution, statusReport.TaskStatusCode);

            // #9 in XML schema
            AssertDateTime(ParseDateTimeInUtc("2019-03-19T08:22:19Z"), statusReport.UpdateTime);

            // #10 in XML schema: alternative (not supported)

            // #11 in XML schema
            Assert.AreEqual(2, statusReport.TaskingParameters.ItemNames.Count);
            Assert.IsTrue(statusReport.TaskingParameters.ItemNames.Contains("SomeCategory1"));
        }

        [TestMethod]
        public void TaskStatusReport_CreateDefault()
        {
            var approxCreateTime = DateTime.Now.ToUniversalTime();
            var taskId = "MyTask";
            var procedureId = "MyProcedure";

            // Create an object with default values and serialise it
            var report = new TaskStatusReport(taskId, procedureId)
            {
                RequestStatus = TaskingRequestStatusCodeType.Pending
            };

            // Serialising
            var xmlBytes = report.ToXmlBytes();

            // Validating
            Validate(xmlBytes);

            AssertDefault(
                xmlBytes: xmlBytes,
                taskId: taskId,
                procedureId: procedureId,
                reqStatus: TaskingRequestStatusCodeType.Pending,
                approxCreateTime: approxCreateTime
                );
        }

        [TestMethod]
        public void TaskStatusReport_BadEstimatedTimeOfCompletion()
        {
            var testObject = new TaskStatusReport("sometask", "someproc");

            try
            {
                testObject.EstimatedTimeOfCompletion = DateTime.Now.ToLocalTime(); // This has bad kind
                Assert.Fail("Expected an exception");
            }
            catch (XNeut.DateTimeException)
            { }
        }

        [TestMethod]
        public void TaskStatusReport_BadUpdateTime()
        {
            var testObject = new TaskStatusReport("sometask", "someproc");

            try
            {
                testObject.UpdateTime = DateTime.Now.ToLocalTime(); // This has bad kind
                Assert.Fail("Expected an exception");
            }
            catch (XNeut.DateTimeException)
            { }
        }

        [TestMethod]
        public void TaskStatusReport_Create()
        {
            // Create an object and serialise it
            var taskingParams = new Item_DataRecord
            {
                { "myparam", new Item_Category("myctg") }
            };

            var testObject = new TaskStatusReport("sometask", "someproc")
            {
                EstimatedTimeOfCompletion = ParseDateTimeInUtc("2019-03-21T08:22:19Z"),
                PercentCompletion = 3.2,
                RequestStatus = TaskingRequestStatusCodeType.Rejected,
                StatusMessages = new System.Collections.Generic.List<string>()
                {
                    "msg1", " msg2   "
                },
                TaskStatusCode = TaskStatusCodeType.Completed,
                UpdateTime = ParseDateTimeInUtc("2019-03-21T08:11:19Z"),
                TaskingParameters = taskingParams
            };
            
            var xmlBytes = testObject.ToXmlBytes();

            // Validation
            Validate(xmlBytes);

            // Parse
            var statusReport = new TaskStatusReport(xmlBytes);

            // Using a separate assert function to make sure the correct object is asserted
            TaskStatusReport_Create_Assert(statusReport);
        }

        private void TaskStatusReport_Create_Assert(TaskStatusReport statusReport)
        {
            // #1 in XML schema
            Assert.AreEqual("sometask", statusReport.TaskId);

            // #2 in XML schema
            AssertDateTime(ParseDateTimeInUtc("2019-03-21T08:22:19Z"), statusReport.EstimatedTimeOfCompletion.Value);

            // #3 in XML schema, event, not supported

            // #4 in XML schema
            Assert.AreEqual(3.2, statusReport.PercentCompletion.Value, 0.001);

            // #5 in XML schema
            Assert.AreEqual("someproc", statusReport.ProcedureId);

            // #6 in XML schema
            Assert.AreEqual(TaskingRequestStatusCodeType.Rejected, statusReport.RequestStatus);

            // #7 in XML schema
            Assert.AreEqual(2, statusReport.StatusMessages.Count);
            Assert.AreEqual("msg2", statusReport.StatusMessages[1]);

            // #8 in XML schema
            Assert.AreEqual(TaskStatusCodeType.Completed, statusReport.TaskStatusCode);

            // #9 in XML schema
            AssertDateTime(ParseDateTimeInUtc("2019-03-21T08:11:19Z"), statusReport.UpdateTime);

            // #10 in XML schema, alternative, not supported

            // #11 in XML schema
            Assert.AreEqual(1, statusReport.TaskingParameters.ItemNames.Count);
            Assert.IsTrue(statusReport.TaskingParameters.ItemNames.Contains("myparam"));
        }

        private void AssertDefault(byte[] xmlBytes, string taskId, string procedureId, TaskingRequestStatusCodeType reqStatus, DateTime approxCreateTime)
        {
            var statusReport = new TaskStatusReport(xmlBytes);
            
            // XML schema fields not supported:
            // - #3 event
            // - #10 alternative


            // Asserting required fields

            // #1 in XML schema
            Assert.AreEqual(taskId, statusReport.TaskId);

            // #5 in XML schema
            Assert.AreEqual(procedureId, statusReport.ProcedureId);

            // #6 in XML schema
            Assert.AreEqual(reqStatus, statusReport.RequestStatus);

            // #9 in XML schema
            // By default, the update time is the creation time of the object.
            // Comparing with the assumed approximate creation time.
            AssertApproximateDateTime(approxCreateTime, statusReport.UpdateTime);


            // These are the default values
            
            // #2 in XML schema
            Assert.IsNull(statusReport.EstimatedTimeOfCompletion);

            // #4 in XML schema
            Assert.IsNull(statusReport.PercentCompletion);

            // #7 in XML schema
            Assert.AreEqual(0, statusReport.StatusMessages.Count);

            // #8 in XML schema
            Assert.AreEqual(TaskStatusCodeType.Unknown, statusReport.TaskStatusCode);

            // #11 in XML schema
            Assert.AreEqual(0, statusReport.TaskingParameters.ItemNames.Count);
        }

        private void AssertApproximateDateTime(DateTime expected, DateTime actual)
        {
            if (expected.Kind != actual.Kind)
            {
                throw new InvalidOperationException("DateTime kind mismatch");
            }
            
            var diff_ms = (expected - actual).TotalMilliseconds;
            Assert.IsTrue(Math.Abs(diff_ms) < 200);
        }

        private void AssertDateTime(DateTime expected, DateTime actual)
        {
            TestCommon.TestHelper.AssertDateTime(expected, actual);
        }

        private DateTime ParseDateTimeInUtc(string s)
        {
            return TestCommon.TestHelper.ParseDateTimeInUtc(s);
        }

        private byte[] ReadFile(string filepath)
        {
            return System.IO.File.ReadAllBytes(filepath);
        }

        private void Validate(byte[] xmlBytes)
        {
            // Validating the document
            var validator = new TestCommon.Validator(TestCommon.Validator.SchemaType.Sps_GmlSwe);

            using (var xmlStream = new System.IO.MemoryStream(xmlBytes))
            {
                validator.Validate(xmlStream, typeof(XsdNs.StatusReportType));
            }
        }
    }
}
