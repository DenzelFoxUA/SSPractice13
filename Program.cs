using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace Practice13
{

    internal class Program
    {
        static List<Fruit> MakeByInput()
        {
            List<Fruit> fruits = new();

            uint numOfFruits = 2;

            for (uint i = 0; i < numOfFruits; i++)
            {
                var fruit = new Fruit();

                fruit.Input();

                fruits.Add(fruit);
            }

            for (uint i = 0; i < numOfFruits; i++)
            {
                var fruit = new Citrus();

                fruit.Input();

                fruits.Add(fruit);
            }

            return fruits;
        }

        static void Main(string[] args)
        {
            //List<Fruit> fruits = MakeByInput();

            string jsonFileName = "fruits.json";
            string txtFileName = "Fruits.txt";

            string currPath = Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..", "..", "..",
                    "Files/"
                )
            );

            List<Fruit> fruits = new();

            try
            {
                 fruits = HelpfullFuncs.LoadAll(currPath + txtFileName);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
           

            fruits.Sort((fruitA, fruitB) => 
            string.Compare(fruitA.Name, fruitB.Name, StringComparison.Ordinal));

            Color yellow = new Color(255, 255, 0);

            foreach(var fruit in fruits)
            {
                if (fruit.Color == yellow)
                    fruit.Print();
            }


            //serialization

            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                IncludeFields = true
            };

            string jsonString = string.Empty;

            try
            {
                jsonString = JsonSerializer.Serialize(fruits, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            File.WriteAllText(currPath + jsonFileName, jsonString);

            //deserialize

            Console.WriteLine("========= [ FRUITS AFTER DESERIALIZING ] ==========");

            string json = string.Empty;

            try
            {
                json = File.ReadAllText(currPath + jsonFileName);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            List<Fruit> fruitsCopy = new();

            try
            {
                fruitsCopy = JsonSerializer.Deserialize<List<Fruit>>(json)
                    ?? new();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
           
            foreach (var fruit in fruitsCopy)
            {
                fruit.Print();
            }

        }
    }
}
