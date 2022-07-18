using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Model
{
    public class CreatingFlightJson
    {

        public string idExternal { get; set; }

        public Flight flight { get; set; }
        
        public int expectedPaxCount { get; set; }

        public CreatingFlightJson()
        {
            flight = new Flight();
        }
    }
}
