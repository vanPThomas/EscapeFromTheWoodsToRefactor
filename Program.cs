using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Managers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
            Wood w1 = WoodBuilder.GetWood(2000, m1, path);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());

            // Create first woods and place five monkeys
            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = WoodBuilder.GetWood(2500, m2, path);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            // Create first woods and place five monkeys
            Map m3 = new Map(0, 400, 0, 400);
            Wood w3 = WoodBuilder.GetWood(2000, m3, path);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());

            // Create tasks for each WriteWoodToDB
            Task<ObjectId> task1 = Task.Run(() => WritingManager.WriteWoodToDB(w1, repo));
            Task<ObjectId> task2 = Task.Run(() => WritingManager.WriteWoodToDB(w2, repo));
            Task<ObjectId> task3 = Task.Run(() => WritingManager.WriteWoodToDB(w3, repo));
            // Create tasks for each Wood's Escape method
            Task<Dictionary<Monkey, List<Tree>>> task4 = Task.Run(() => w1.Escape());
            Task<Dictionary<Monkey, List<Tree>>> task5 = Task.Run(() => w2.Escape());
            Task<Dictionary<Monkey, List<Tree>>> task6 = Task.Run(() => w3.Escape());

            // Wait for all tasks to finish
            Task.WaitAll(task1, task2, task3, task4, task5, task6);

            // Retrieve the results
            ObjectId w1ID = task1.Result;
            ObjectId w2ID = task2.Result;
            ObjectId w3ID = task3.Result;
            Dictionary<Monkey, List<Tree>> routew1 = task4.Result;
            Dictionary<Monkey, List<Tree>> routew2 = task5.Result;
            Dictionary<Monkey, List<Tree>> routew3 = task6.Result;

            Task task7 = Task.Run(() => WritingManager.WriteFullRoute(w1ID, routew1, repo));
            Task task8 = Task.Run(() => WritingManager.WriteFullRoute(w2ID, routew2, repo));
            Task task9 = Task.Run(() => WritingManager.WriteFullRoute(w3ID, routew3, repo));

            Task.WaitAll(task7, task8, task9);

            // Wait for all tasks to finish

            //// Create threads for each Wood's Escape method
            //Thread thread1 = new Thread(() => w1.Escape());
            //Thread thread2 = new Thread(() => w2.Escape());
            //Thread thread3 = new Thread(() => w3.Escape());

            //// Start the threads
            //thread1.Start();
            //thread2.Start();
            //thread3.Start();

            //// Wait for all threads to finish
            //thread1.Join();
            //thread2.Join();
            //thread3.Join();

            stopwatch.Stop();
            // Write result.
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }
    }
}


//woodid naar objectid x
//cell processing to manager x
// no writing in escape x
