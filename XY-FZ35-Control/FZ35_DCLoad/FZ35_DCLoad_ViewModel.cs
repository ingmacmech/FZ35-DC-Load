using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XY_FZ35_Control
{
    class FZ35_DCLoad_ViewModel : INotifyPropertyChanged
    {
        private FZ35_DCLoad _Model;

       // Constructor 
       public FZ35_DCLoad_ViewModel(FZ35_DCLoad model)
        {
            _Model = model;
            _OverVoltageProtection = _Model.OverVoltageProtection;
            
        }
        
        // Interface event
        public event PropertyChangedEventHandler PropertyChanged;

        protected internal void OnPropertyChanged (string propertyname)
        {
            if(PropertyChanged != null){
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        // 

        private double _OverVoltageProtection
        {
            get { return _OverVoltageProtection; }
            set
            {
                if (_OverVoltageProtection == value) return;

                _OverVoltageProtection = value;
                OnPropertyChanged("OverVoltageProtection");
            }
        }

    }
}
