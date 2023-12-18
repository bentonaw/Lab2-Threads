using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Threads
{
    internal class Utility
    {
        internal static void CheckForKeyPress(CancellationToken cancellationToken, Car car1, Car car2, CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Press Enter to check status or Esc to exit.");
                Thread.Sleep(100);

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.WriteLine($"{car1.Name}: {car1.DistanceTraveled} km, Speed: {car1.Speed} km/h");
                    Console.WriteLine($"{car2.Name}: {car2.DistanceTraveled} km, Speed: {car2.Speed} km/h");
                    string leader = RaceInfo.GetLeader(car1, car2);
                    if (!leader.Contains("neck and neck"))
                    {
                        Console.WriteLine($"{leader} is in the lead!");
                    }
                    else
                    {
                        Console.WriteLine(leader);
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Race has been stopped");
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Thanks for racing!");
                    Environment.Exit(0);
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

