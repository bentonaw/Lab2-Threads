using System;
using System.Threading;
using System.Xml.Linq;
namespace Lab2_Threads
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Car car1 = new Car("Kitt", 120);
            Car car2 = new Car("Mach 5", 120);

            int distance = 10;

            // countdown to make both cars start at the same time
            CountdownEvent countdownEvent = new CountdownEvent(2);

            Thread car1Thread = new Thread(() => car1.PedalToTheMetal(distance, countdownEvent));
            Thread car2Thread = new Thread(() => car2.PedalToTheMetal(distance, countdownEvent));

            car1Thread.Start();
            car2Thread.Start();

            // Waits for both cars to pass countdownEvent signal
            countdownEvent.Wait();

            Console.WriteLine($"Car1 traveled {car1.DistanceTraveled} km.");
            Console.WriteLine($"Car2 traveled {car2.DistanceTraveled} km.");


        }
    }
    internal class Car
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public int DistanceTraveled { get; set; }
        public bool RaceComplete { get; set; }

        public Car(string name, int speed)
        {
            Name = name;
            Speed = speed;
            DistanceTraveled = 0;
            RaceComplete = false;
        }

        // The car starts their engine and hits the gas pedal
        public void PedalToTheMetal(int distance, CountdownEvent countdownEvent)
        {
            Console.WriteLine($"There {Name} goes!");
            countdownEvent.Signal();

            for (int i = 0; i <= distance; i++) 
            {
                Thread.Sleep(1000);
                DistanceTraveled++;
                
                //lets there be an event every 30 seconds
                if (i % 3 == 0)
                {
                    RaceEvent();
                }
            
            }

            RaceComplete = true;
            Console.WriteLine($"{Name} passes the finishline!");
        }

        public void StartRace()
        {
            

        }

        public void RaceEvent()
        {
            Random random = new Random();
            int randomEvent = random.Next(1, 51);
        }

        public void RaceStatus()
        {
            
        }
    }
}
