using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Models
{
    public class Categories : INotifyPropertyChanged
    {
        private string _icon = "";

        public short Id { get; set; }
        public string _desc { get; set; }

        public string Description
        {
            get { return _desc; }
            set
            {
                _desc = value;
                NotifyPropertyChanged("_desc");
            }
        }


        public bool? Active { get; set; }
        public string Icon { 
            get { return _icon; }
            set
            {
                _icon = value;
                NotifyPropertyChanged("_icon");
            }
        }
        public int Location { get; set; }
        public DateTime Created { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        }
    }
}
