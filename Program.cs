using EscapeFromTheWoods.Database;
using System;
using System.Diagnostics;
using System.Threading;

namespace EscapeFromTheWoods
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Hello World!");
            string connectionString = "mongodb://localhost:27017";
            MongoDBMonkeyRepo repo = new MongoDBMonkeyRepo(connectionString);

            string path = @"C:\NET\monkeys";

            // Create first woods and place five monkeys
            Map m1 = new Map(0, 500, 0, 500);
            Wood w1 = WoodBuilder.GetWood(2000, m1, path, repo);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());

            // Create first woods and place five monkeys
            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = WoodBuilder.GetWood(2500, m2, path, repo);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            // Create first woods and place five monkeys
            Map m3 = new Map(0, 400, 0, 400);
            Wood w3 = WoodBuilder.GetWood(2000, m3, path, repo);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());

            // Create threads for each Wood's Escape method
            Thread thread1 = new Thread(() => w1.Escape());
            Thread thread2 = new Thread(() => w2.Escape());
            Thread thread3 = new Thread(() => w3.Escape());

            // Start the threads
            thread1.Start();
            thread2.Start();
            thread3.Start();

            // Wait for all threads to finish
            thread1.Join();
            thread2.Join();
            thread3.Join();

            stopwatch.Stop();
            // Write result.
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }
    }
}
