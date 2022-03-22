using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeas.Adapters {
    /// <summary>
    /// Interface for managing different slicers
    /// </summary>
    public interface ISlicerCLI {

        public string ExecuteCommand(string _attributes);
    }
}
