using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CircularSeas;

namespace CircularSeasManager.Models
{
    public class OrderModel : BaseModel
    {
        private CircularSeas.Models.Order _order;
        public CircularSeas.Models.Order Order
        {
            get { return _order; }
            set
            {
                if (_order != value)
                {
                    _order = value;
                    OnPropertyChanged();
                }
            }
        }


        //Materials Section
        public ObservableCollection<CircularSeas.Models.Material> Materials { get; set; }
        protected Guid _materialCandidate;

        private CircularSeas.Models.Material _materialSelected;
        public CircularSeas.Models.Material MaterialSelected
        {
            get { return _materialSelected; }
            set
            {
                if(_materialSelected != value)
                {
                    _materialSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _amount = 1;
        public int Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged(); }
        }

        //orders Section
        public ObservableCollection<CircularSeas.Models.Order> PendingOrders { get; set; }
        
    }
}
