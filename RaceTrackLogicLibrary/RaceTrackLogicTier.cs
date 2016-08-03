using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTrackLogicLibrary
{
    public delegate void OnRacerHoundAtStartPositionCallBack(Greyhound racer, double position);

    public delegate void OnRaceHoundAdvanceCallback(Greyhound raceHound, double location);

}
