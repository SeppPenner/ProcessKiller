// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The configuration class.
    /// </summary>
    [Serializable]
    public class Config
    {
        /// <summary>
        /// Gets or sets the processes.
        /// </summary>
        public List<Process> Processes { get; set; } = new();
    }
}