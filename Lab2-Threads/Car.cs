using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        // Callback delegate for notifying when the race is complete
        public delegate void RaceCompleteHandler(Car winner);
        public event RaceCompleteHandler RaceCompleted;

        // The car starts their engine and hits the gas pedal. Scale is adjusted to make the car finish the 10km distance in roughly 30sec instead of 5min (1/10 of original runtime)
        public void PedalToTheMetal(int distance, CountdownEvent countdownEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"There {Name} goes!");
            countdownEvent.Signal();
            StartTime = DateTime.Now;

            // Adjusted sleep duration for each iteration to achieve a race duration of around 30 seconds
            int totalRaceTime = 30000; // 30 seconds in milliseconds

            for (int i = 0; i <= distance; i++)
            {
                // stops the car once race has been cancelled
                if (cancellationToken.IsCancellationRequested)
                {
                    //Console.WriteLine("Driver has been signalled to stop. Car is stopping");
                    return;
                }
                // Calculate sleep duration based on current speed
                int sleepDuration = totalRaceTime / (distance * Speed / 120);

                // Logs the time the car enters the distance
                TimeLog.Add(DateTime.Now);

                Thread.Sleep(sleepDuration);
                DistanceTraveled++;

                //lets there be an event every 30 seconds
                if (i % (distance / 3) == 0)
                {
                    RaceEvent(cancellationToken);
                }
            }
            RaceComplete = true;
            Console.WriteLine($"{Name} passes the finishline!");
        }
       
        public void RaceEvent(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int timeScaleModifier = 100;
                int refuelDuration = 30000 / timeScaleModifier;
                int tireChangeDuration = 20000 * timeScaleModifier;
                int cleanWindshieldDuration = 10000 / timeScaleModifier;

                Random random = new Random();
                int randomEvent = random.Next(1, 51);

                if (randomEvent == 1)
                {
                    Console.WriteLine($"{Name} ran out of gas and needs to refuel. It will stop for 30 seconds.");
                    Thread.Sleep(refuelDuration);
                }
                else if (randomEvent == 2)
                {
                    Console.WriteLine($"{Name} got a flat tire and needs to change it. It will stop for 20 seconds.");
                    Thread.Sleep(tireChangeDuration);
                }
                else if (randomEvent <= 6)
                {
                    Console.WriteLine($"{Name} has a bird on its windshield and needs to clean it. It will stop for 10 seconds.");
                    Thread.Sleep(cleanWindshieldDuration);
                }
                else if (randomEvent <= 16)
                {
                    Console.WriteLine($"{Name} has a minor engine problem and its speed is reduced by 1 km/h.");
                    Speed--;
                }
            }
        }
    }
}
