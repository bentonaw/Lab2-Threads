using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2_Threads
{
    internal class RaceInfo
    {
        public static string GetLeader(Car car1, Car car2)
        {
            if (car1.DistanceTraveled == car2.DistanceTraveled)
            {
                // Compare the DateTime values for the current distance
                DateTime car1Time = GetTimeAtDistance(car1, car1.DistanceTraveled);
                DateTime car2Time = GetTimeAtDistance(car2, car2.DistanceTraveled);

                if (car1Time == car2Time)
                {
                    return "The cars are neck and neck!";
                }

                return car1Time < car2Time ? car1.Name : car2.Name;
            }

            return car1.DistanceTraveled > car2.DistanceTraveled ? car1.Name : car2.Name;
        }

        private static DateTime GetTimeAtDistance(Car car, int distance)
        {
            // Adjusting the index for the 0-based list
            int adjustedIndex = distance - 1;

            return (adjustedIndex >= 0 && adjustedIndex < car.TimeLog.Count)
                ? car.TimeLog[adjustedIndex]
                : DateTime.MaxValue;
        }

    }
}
