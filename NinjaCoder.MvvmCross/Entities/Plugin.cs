﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the Plugin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Entities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///  Defines the Plugin type.
    /// </summary>
    [DataContract]
    public class Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin" /> class.
        /// </summary>
        public Plugin()
        {
            this.NugetCommands = new List<string>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is community plugin].
        /// </summary>
        [DataMember]
        public bool IsCommunityPlugin { get; set; }

        /// <summary>
        /// Gets or sets the nuget commands.
        /// </summary>
        [IgnoreDataMember]
        public List<string> NugetCommands { get; set; }
    }
}
