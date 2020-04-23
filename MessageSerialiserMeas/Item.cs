//
// Please make sure to read and understand README.md and LICENSE.txt.
// 
// This file was prepared in the research project COCOP (Coordinating Optimisation of
// Complex Industrial Processes).
// https://cocop-spire.eu/
//
// Author: Petri Kannisto, Tampere University, Finland
// File created: 4/2018
// Last modified: 2/2020

using System;
using XsdNs = Cocop.MessageSerialiser.Meas.XsdGen;

namespace Cocop.MessageSerialiser.Meas
{
    /// <summary>
    /// Abstract base class for items.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="typeUri">The URI that specifies the observation type.</param>
        protected Item(string typeUri)
        {
            ObservationTypeUri = typeUri;
        }

        /// <summary>
        /// The URI of the associated observation type.
        /// </summary>
        internal string ObservationTypeUri
        {
            get;
            private set;
        }

        
        #region Overridable functionality of the base class

        /// <summary>
        /// Whether the item type supports data quality in a data record.
        /// The default is true; override this function if the type does
        /// not support data quality in a data record.
        /// The property exists to enable the discovery of conflicts as
        /// early as possible. Otherwise, the error of associating data
        /// quality would appear only just before serialisation.
        /// </summary>
        virtual internal bool SupportsDataQualityInDataRecord
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the object for result marshalling. The base class has no proper implementation,
        /// but this method shall be overridden in sub-classes as needed.
        /// </summary>
        /// <param name="idPrefix">A prefix to be appended to the IDs of any child XML elements that
        /// require an ID. For certain XML elements, the schema requires an ID that is unique within
        /// the XML document. Instead of generating random IDs, these are systematic and hierarchical
        /// in this software. To ensure uniqueness, each ID prefix can occur only once. The ID is of
        /// type xsd:id. This derives from xsd:NCName, so not all characters are allowed.</param>
        /// <returns>Object.</returns>
        virtual internal object GetObjectForXml_Result(string idPrefix)
        {
            throw new NotSupportedException("The type does not support serialisation as an observation result");
        }

        /// <summary>
        /// Returns the object for data record marshalling. The base class has no proper implementation,
        /// but this method shall be overridden in sub-classes as needed.
        /// </summary>
        /// <returns>Object.</returns>
        virtual internal XsdNs.AbstractDataComponentType GetObjectForXml_DataRecordField(string idPrefix)
        {
            throw new NotSupportedException("The type does not support serialisation as a data record field");
        }

        #endregion Overridable functionality of the base class


        #region Abstract members

        /// <summary>
        /// Creates a string presentation of the value of the item. This method is intended for
        /// scenarios where it is desirable to give a hint about the value without casting
        /// to the actual type.
        /// </summary>
        /// <returns>Value as string.</returns>
        public abstract string ToDisplayString();

        #endregion Abstract members
    }
}