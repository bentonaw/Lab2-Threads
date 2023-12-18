using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Threads
{
    internal class Car
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public DateTime StartTime { get; set; }
        public List<DateTime> TimeLog { get; } = new List<DateTime>(); // Added property to log time at each distance
        public int DistanceTraveled { get; set; }
        public bool RaceComplete { get; set; }

        public Car(string name, int speed)
        {
            Name = name;
            Speed = speed;
            DistanceTraveled = 0;
            RaceComplete = false;
        }

        // The car starts their engine and hits the gas pedal. Scale is adjusted to make the car finish the 10km distance in roughly 30sec instead of 5min (1/10 of original runtime)
        public void PedalToTheMetal(int distance, CountdownEvent countdownEvent)
        {
            Console.WriteLine($"There {Name} goes!");
            countdownEvent.Signal();
            StartTime = DateTime.Now;

            // Adjusted sleep duration for each iteration to achieve a race duration of around 30 seconds
            int totalRaceTime = 30000; // 30 seconds in milliseconds


            for (int i = 0; i <= distance; i++)
            {

                // Calculate sleep duration based on current speed
                int sleepDuration = totalRaceTime / (distance * Speed / 120);

                // Logs the time the car enters the distance
                TimeLog.Add(DateTime.Now);

                Thread.Sleep(sleepDuration);
                DistanceTraveled++;

                //lets there be an event every 30 seconds
                if (i % (distance / 3) == 0)
                {
                    RaceEvent();
                }
            }
            RaceComplete = true;
            Console.WriteLine($"{Name} passes the finishline!");
        }
        public void RaceEvent()
        {
            Random random = new Random();
            int randomEvent = random.Next(1, 51);

            if (randomEvent == 1)
            {
                Console.WriteLine($"{Name} ran out of gas and needs to refuel. It will stop for 30 seconds.");
                Thread.Sleep(300);
            }
            else if (randomEvent == 2)
            {
                Console.WriteLine($"{Name} got a flat tire and needs to change it. It will stop for 20 seconds.");
                Thread.Sleep(200);
            }
            else if (randomEvent <= 6)
            {
                Console.WriteLine($"{Name} has a bird on its windshield and needs to clean it. It will stop for 10 seconds.");
                Thread.Sleep(100);
            }
            else if (randomEvent <= 16)
            {
                Console.WriteLine($"{Name} has a minor engine problem and its speed is reduced by 1 km/h.");
                Speed--;
            }
        }
    }
}
