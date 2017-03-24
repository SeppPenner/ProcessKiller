using System;
using System.Collections.Generic;

namespace ProcessKiller
{
    [Serializable]
    public class Config
    {
        private List<Process> _processes;

        public List<Process> Processes
        {
            get { return _processes; }
            set { _processes = value; }
        }
    }
}