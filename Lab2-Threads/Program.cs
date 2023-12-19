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
            Console.WriteLine("Press enter to start race");
            Console.ReadLine();

            int maxSpeed = 120;

            Car car1 = new Car("Kitt", maxSpeed);
            Car car2 = new Car("\"Mach 5\"", maxSpeed);

            int distance = 10;

            // countdown to make both cars start at the same time
            CountdownEvent countdownEvent = new CountdownEvent(2);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Thread car1Thread = new Thread(() => car1.PedalToTheMetal(distance, countdownEvent, cancellationTokenSource.Token));
            Thread car2Thread = new Thread(() => car2.PedalToTheMetal(distance, countdownEvent, cancellationTokenSource.Token));

            car1Thread.Start();
            car2Thread.Start();

            // Waits for both cars to pass countdownEvent signal before continuing
            countdownEvent.Wait();

            // Allow the user to check the race status or exit
            Task.Run(() => Utility.CheckForKeyPress(cancellationTokenSource.Token, car1, car2, cancellationTokenSource));

            car1Thread.Join();
            car2Thread.Join();
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine($"The winner is... {RaceInfo.GetWinner(car1, car2, cancellationTokenSource.Token)}!!");
            }
            
            else
            {
                Console.WriteLine("No winners this time");
            }

            // Stops monitoring key presses
            cancellationTokenSource.Cancel();
            Environment.Exit(0);
        }
    }
}
