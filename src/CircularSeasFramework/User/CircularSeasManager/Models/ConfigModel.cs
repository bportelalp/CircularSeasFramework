using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CircularSeasManager.Resources;
using Xamarin.Forms;

namespace CircularSeasManager.Models
{
    public class ConfigModel : BaseModel
    {
        //Coordinating Trying connection
        protected System.Timers.Timer timerCloud { get; set; }
        protected System.Timers.Timer timerOctoprint { get; set; }

        public ObservableCollection<CircularSeas.Models.Node> Nodes { get; set; }

        private CircularSeas.Models.Node _nodeSelected;
        public CircularSeas.Models.Node NodeSelected
        {
            get { return _nodeSelected; }
            set
            {
                if(_nodeSelected != value)
                {
                    _nodeSelected = value;
                    NodeId = _nodeSelected?.Id ?? Guid.Empty;
                    NodeName = _nodeSelected?.Name;
                    OnPropertyChanged();
                }
            }
        }

        private string _tryCloudMessage;
        public string TryCloudMessage
        {
            get { return _tryCloudMessage; }
            set
            {
                if (_tryCloudMessage != value)
                {
                    _tryCloudMessage = value;
                    OnPropertyChanged();
                    if(TryCloudMessage == StringResources.IpCloudSucess)
                        TryCloudMessageColor = "Green";
                    else
                        TryCloudMessageColor = "Red";
                    OnPropertyChanged(nameof(TryCloudMessageColor));
                }
            }
        }
        public string TryCloudMessageColor { get; set; } = "Red";

        private string _tryOprintMessage;
        public string TryOprintMessage
        {
            get { return _tryOprintMessage; }
            set
            {
                if (_tryOprintMessage != value)
                {
                    _tryOprintMessage = value;
                    OnPropertyChanged();
                    if (TryOprintMessage == StringResources.IpOctoprintSucess)
                        TryOprintMessageColor = "Green";
                    else
                        TryOprintMessageColor = "Red";
                    OnPropertyChanged(nameof(TryOprintMessageColor));
                }
            }
        }
        public string TryOprintMessageColor { get; set; } = "Red";
    }
}
