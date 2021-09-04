using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Models
{
    public class Visitor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Kiosk { get; set; }
        public int Location { get; set; }
        //public string Mobile { get; set; }
        public bool IsHandicap { get; set; }
        public ulong VisitCategoryId { get; set; }
    }
}
