using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab2_Threads
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool restart = false;
            while (true)
            {
                Car car1 = new Car("Kitt", 120);
                Car car2 = new Car("\"Mach 5\"", 120);

                // Allow the user to check the race status or exit
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                restart = await Task.Run(() => CheckForKeyPress(cancellationTokenSource.Token, car1, car2, cancellationTokenSource, restart));
                int distance = 10;

                Console.WriteLine($"The race between {car1.Name} and {car2.Name} is about to begin. Press enter to start");
                Console.ReadLine();
                // countdown to make both cars start at the same time
                CountdownEvent countdownEvent = new CountdownEvent(2);

                Thread car1Thread = new Thread(() => car1.PedalToTheMetal(distance, countdownEvent, cancellationTokenSource.Token));
                Thread car2Thread = new Thread(() => car2.PedalToTheMetal(distance, countdownEvent, cancellationTokenSource.Token));

                car1Thread.Start();
                car2Thread.Start();

                // Waits for both cars to pass countdownEvent signal before continuing
                countdownEvent.Wait();

               
                // Start CheckForKeyPress method asynchronously and keeps running alongside the race
                Task<bool> checkForKeyPressTask = CheckForKeyPress(cancellationTokenSource.Token, car1, car2, cancellationTokenSource, restart);

                while (!car1.RaceComplete || !car2.RaceComplete)
                {
                    if (car1.RaceComplete && car2.RaceComplete)
                    {
                        Console.WriteLine("It's a tie! Both cars completed the race at the same time.");
                        break;
                    }
                    else if (car1.RaceComplete && !car2.RaceComplete)
                    {
                        Console.WriteLine($"{car1.Name} completed the race first!");
                        break;
                    }
                    else if (!car1.RaceComplete && car2.RaceComplete)
                    {
                        Console.WriteLine($"{car2.Name} completed the race first!");
                        break;
                    }
                }

                // Stops monitoring key presses
                cancellationTokenSource.Cancel();
                car1Thread.Join();
                car2Thread.Join();

                //// Wait for the race completion or cancellation. Waits for user interaction or cancellation task (which automatically runs when race finishes or user terminates)
                //await Task.WhenAny(checkForKeyPressTask, Task.Delay(Timeout.Infinite, cancellationTokenSource.Token));

                //// Check if the user wants to restart
                //restart = checkForKeyPressTask.Result;

                if (!RestartRacePrompt(restart))
                {
                    break;
                }
            }
        }

        static async Task<bool> CheckForKeyPress(CancellationToken cancellationToken, Car car1, Car car2, CancellationTokenSource cancellationTokenSource, bool restart)
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
                    ConsoleKeyInfo confirmKey = Console.ReadKey(true);

                    if (confirmKey.Key == ConsoleKey.Y)
                    {
                        Console.Clear();
                        Console.WriteLine("Race has been stopped");
                        return await RestartRacePromptAsync();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Option not recognized");
                }
            }

            return restart;
        }

        static async Task<bool> RestartRacePromptAsync()
        {
            Console.WriteLine("Would you like to race again? (y/n)");
            string input = Console.ReadLine().ToLower();

            while (true)
            {
                if (input == "n")
                {
                    Console.WriteLine("Thanks for racing!");
                    Environment.Exit(0);
                }
                else if (input == "y")
                {
                    Console.Clear();
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Only (y/n) accepted as input");
                }
            }
        }

        static bool RestartRacePrompt(bool restart)
        {
            if (restart)
            {
                return RestartRacePromptAsync().Result;
            }

            Console.WriteLine("Would you like to race again? (y/n)");
            string input = Console.ReadLine().ToLower();

            while (true)
            {
                if (input == "n")
                {
                    Console.WriteLine("Thanks for racing!");
                    return false;
                }
                else if (input == "y")
                {
                    Console.Clear();
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Only (y/n) accepted as input");
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
        public void PedalToTheMetal(int distance, CountdownEvent countdownEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"There {Name} goes!");
            countdownEvent.Signal();

            for (int i = 0; i <= distance; i++) 
            {
                // Check for cancellation before each iteration
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"{Name} received cancellation signal. Stopping race.");
                    return;
                }
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

