using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeas.Cloud.Server.SlicerEngine {
    /// <summary>
    /// Interface for managing different slicers
    /// </summary>
    public interface ISlicerCLI {

        public string ExecuteCommand(string _attributes);
    }
}
