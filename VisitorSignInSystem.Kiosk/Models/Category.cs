using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Models
{
    public class Category : INotifyPropertyChanged
    {
        public short Id { get; set; }
        public string _description { get; set; }
        public sbyte _department { get; set; }
        public bool? _active { get; set; }
        private string _icon { get; set; }
        public sbyte _location { get; set; }
        public DateTime _created { get; set; }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged("_description");
            }
        }

        public sbyte Department
        {
            get { return _department; }
            set
            {
                _department = value;
                NotifyPropertyChanged("_department");
            }
        }
        
        public bool? Active { 
            get { return _active; }
            set { 
                _active = value;
                NotifyPropertyChanged("_active");
            }
        }

        public string Icon { 
            get { return _icon; }
            set
            {
                _icon = value;
                NotifyPropertyChanged("_icon");
            }
        }
        
        public sbyte Location
        {
            get { return _location; }
            set
            {
                _location = value;
                NotifyPropertyChanged("_location");
            }
        }
        
        public DateTime Created
        {
            get { return _created; }
            set
            {
                _created = value;
                NotifyPropertyChanged("_created");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
