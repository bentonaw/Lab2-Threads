using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab2_Threads
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool restart = false;
            while (true)
            {
                Car car1 = new Car("Kitt", 120);
                Car car2 = new Car("\"Mach 5\"", 120);

                int distance = 10;

                // countdown to make both cars start at the same time
                CountdownEvent countdownEvent = new CountdownEvent(2);

                Thread car1Thread = new Thread(() => car1.PedalToTheMetal(distance, countdownEvent));
                Thread car2Thread = new Thread(() => car2.PedalToTheMetal(distance, countdownEvent));

                car1Thread.Start();
                car2Thread.Start();

                // Waits for both cars to pass countdownEvent signal before continuing
                countdownEvent.Wait();

                // Allow the user to check the race status or exit
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => CheckForKeyPress(cancellationTokenSource.Token, car1, car2, cancellationTokenSource,restart));


                while (!car1.RaceComplete || !car2.RaceComplete)
                {
                    if (car1.RaceComplete && car2.RaceComplete)
                    {
                        break;
                    }
                }

                // Stops monitoring key presses
                cancellationTokenSource.Cancel();
            }
        }

        static async Task CheckForKeyPress(CancellationToken cancellationToken, Car car1, Car car2, CancellationTokenSource cancellationTokenSource, bool restart)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Press Enter to check status or Esc to exit.");
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.WriteLine($"{car1.Name}: {car1.DistanceTraveled} km, Speed: {car1.Speed} km/h");
                    Console.WriteLine($"{car2.Name}: {car2.DistanceTraveled} km, Speed: {car2.Speed} km/h");
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Do you really want to stop the race? (Y/N)");
                    ConsoleKeyInfo confirmKey = await Task.Run(() => Console.ReadKey(true), cancellationToken);

                    if (confirmKey.Key == ConsoleKey.Y)
                    {
                        Console.Clear();
                        Console.WriteLine("Race has been stopped");
                        cancellationTokenSource.Cancel();
                        await Console.Out.WriteLineAsync("Would you like to race again? (y/n)");
                        while (true)
                        {
                            string input = Console.ReadLine().ToLower();
                            if (input == "n")
                            {
                                await Console.Out.WriteLineAsync("thanks for racing");
                                Environment.Exit(0);
                            }
                            else if (input == "y")
                            {
                                Console.Clear();
                                restart = true;
                                break;
                            }
                            else 
                            {
                                Console.WriteLine("invalid input. try again");
                            }
                        }
                        
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Option not recognized");
                }
            }
        }
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
                Thread.Sleep(3000);
                DistanceTraveled++;
                
                //lets there be an event every 30 seconds
                if (i % 300 == 0)
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

