using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab2_Threads
{
    internal class Race
    {
        // all times are adjusted according to the scale of 1/10.
        public static string GetLeader(Car car1, Car car2)
        {
            if (car1.DistanceTraveled == car2.DistanceTraveled)
            {
                // Compare the times taken to reach the current distance
                TimeSpan car1Time = car1.TimeLog.Count > 0 ? car1.TimeLog[car1.DistanceTraveled - 1] : TimeSpan.MaxValue;
                TimeSpan car2Time = car2.TimeLog.Count > 0 ? car2.TimeLog[car2.DistanceTraveled - 1] : TimeSpan.MaxValue;

                return car1Time < car2Time ? car1.Name : car2.Name;
            }

            return car1.DistanceTraveled > car2.DistanceTraveled ? car1.Name : car2.Name;
        }
    }
}
