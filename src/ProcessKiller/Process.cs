// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Process.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The process model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller
{
    /// <summary>
    /// The process model class.
    /// </summary>
    public abstract class Process
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string FullName { get; set; }
    }
}