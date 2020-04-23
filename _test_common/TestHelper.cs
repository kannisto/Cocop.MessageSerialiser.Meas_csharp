//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating
// Optimisation of Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 5/2018
// Last modified: 2/2020


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCommon
{
    /// <summary>
    /// Helper class for tests.
    /// </summary>
    static class TestHelper
    {
        private static string appBaseFolder = null;


        /// <summary>
        /// The path of the folder where test files are located.
        /// </summary>
        public static string TestFileFolder
        {
            get
            {
                // This assumes that the executable is located in path "<project>\bin\Debug" (or "\Release")":
                var path = AppBaseFolder + @"\..\..\..\_testfiles_from_spec";

                return System.IO.Path.GetFullPath(path);
            }
        }

        /// <summary>
        /// The path of the folder where XML schema files are located.
        /// </summary>
        public static string SchemaFolderOpenGis
        {
            get
            {
                // This assumes that the executable is located in path "<project>\bin\Debug" (or "\Release")":
                var path = AppBaseFolder + @"\..\..\..\_codegen_xsd_exe";

                return System.IO.Path.GetFullPath(path);
            }
        }
        
        /// <summary>
        /// The application base folder.
        /// </summary>
        private static string AppBaseFolder
        {
            get
            {
                if (appBaseFolder == null)
                {
                    appBaseFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                }

                return appBaseFolder;
            }
        }

        /// <summary>
        /// Parses a DateTime value and uses UTC kind to the result.
        /// </summary>
        /// <param name="s">DateTime string. This must end to 'Z' to indicate UTC.</param>
        /// <returns>DateTime value.</returns>
        public static DateTime ParseDateTimeInUtc(string s)
        {
            if (!s.EndsWith("Z"))
            {
                throw new InvalidOperationException("Test error: timestamp must end with 'Z'");
            }

            return DateTime.Parse(s).ToUniversalTime();
        }

        /// <summary>
        /// Asserts a datetime value.
        /// </summary>
        /// <param name="expected">Expected.</param>
        /// <param name="actual">Actual.</param>
        public static void AssertDateTime(DateTime expected, DateTime actual)
        {
            // Asserting kind
            Assert.AreEqual(expected.Kind, actual.Kind);
            
            // Assert the value
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
        
        /// <summary>
        /// Asserts an ArgumentException from given function.
        /// </summary>
        /// <param name="testWorker">Function to be run.</param>
        /// <param name="expectedMessageStart">The start of the expected error message.</param>
        public static void AssertArgumentException(Action testWorker, string expectedMessageStart)
        {
            // Expecting exception
            try
            {
                testWorker.Invoke();
                Assert.Fail("Exception was expected");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.StartsWith(expectedMessageStart), "Unexpected exception message " + e.Message);
            }
        }
    }
}
