using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;


namespace RaceTrackSim
{
    class InsufficientFundsException : Exception
    {

        public InsufficientFundsException(string value) : base(value)
        {
                 
        }

    }
}

