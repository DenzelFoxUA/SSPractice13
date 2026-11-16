using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;


namespace Practice13
{
    public struct Color
    {
        public byte R { get; init; } = 0;
        public byte G { get; init; } = 0;
        public byte B { get; init; } = 0;

        public Color() { }

        [JsonConstructor]
        public Color(byte r, byte g, byte b)
        {
            if (r > 255 || g > 255 || b > 255)
                throw new ArgumentOutOfRangeException
                    ("Out of range::color byte values");

            R = r;
            G = g;
            B = b;
        }

        #region [EQUALS]

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is not Color other)
                return false;

            return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B);
        }

        public static bool operator == (Color one, Color other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(Color one, Color other)
        {
            return !(one.Equals(other));
        }

        #endregion

        public void Print()
        {
            Console.Write($"\x1b[38;2;{R};{G};{B}m");
            Console.WriteLine("[ COLOR ]");
            Console.Write("\x1b[0m");
        }
    }

    public static class HelpfullFuncs
    {
        public static List<Fruit> LoadAll(string filePath)
        {
            List<Fruit> fruits = new();
            List<string> fields = new();

            bool insideBlock = false;

            var lines = File.ReadLines(filePath);

            foreach (string currLine in lines)
            {
                string line = currLine.Trim();

                if (line.Length == 0)
                    continue;

                //відкрити блок
                if (line == "{")
                {
                    if (insideBlock)
                        throw new FormatException("Nested block");

                    insideBlock = true;
                    fields.Clear(); //очистити старий блок коли в середині нового
                }
                else if (line == "}") //закрити
                {
                    if (!insideBlock)
                        throw new FormatException("Unexpected }");

                    Fruit fruit = fields.Count switch
                    {
                        2 => new Fruit(),
                        3 => new Citrus(),
                        _ => throw new FormatException("Invalid fruit data")
                    };

                    //створити і записати 
                    fruit.ReadFields(fields.ToArray());
                    fruits.Add(fruit);

                    insideBlock = false;
                }
                else
                {
                    if (!insideBlock)
                        throw new FormatException("Data outside block");

                    fields.Add(line);
                }
            }

            if (insideBlock)
                throw new FormatException("Missing closing }");

            return fruits;
        }

        public static byte GetByteFromInput(string nameOfVariable)

        {
            string uInput = "";
            byte result = 0;

            bool isOk = false;

            do
            {
                Console.WriteLine($"Enter {nameOfVariable}");
                uInput = Console.ReadLine()!;

                if (uInput is not null)
                  isOk = byte.TryParse(uInput, out result);

                if(!isOk)
                {
                    Console.WriteLine($"Wrong entrie! Try again!");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            while (!isOk);

            return result;

        }

        public static uint GetUintFromInput(string nameOfVariable)

        {
            string uInput = "";
            uint result = 0;

            bool isOk = false;

            do
            {
                Console.WriteLine($"Enter {nameOfVariable}");
                uInput = Console.ReadLine()!;

                if (uInput is not null)
                    isOk = uint.TryParse(uInput, out result);

                if (!isOk)
                {
                    Console.WriteLine($"Wrong entrie! Try again!");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            while (!isOk);

            return result;

        }

        public static string GetStringFromInput(string variableName)
        {
            string result = string.Empty;

            do
            {
                Console.WriteLine($"Please enter {variableName}:");
                result = Console.ReadLine()!;
            }
            while (result == string.Empty || result is null);

            return result;
        }
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Citrus), "citrus")]
    public class Fruit : IFruit
    {
        public string Name { get; private set; } = "";
        public Color Color { get; private set; } = new();

        public Fruit() { }

        [JsonConstructor]
        public Fruit(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        #region [FILE]

        public virtual void ReadFields(string[] result)
        {
            Name = result[0];

            string[] color = result[1].Split(',');

            byte r = 0;
            byte g = 0;
            byte b = 0;

            bool isOk = byte.TryParse(color[0], out r);

            if (!isOk)
                throw new InvalidDataException
                    ($"Data read error: {nameof(result)} byte red");

            isOk = byte.TryParse(color[1], out g);

            if (!isOk)
                throw new InvalidDataException
                    ($"Data read error: {nameof(result)} byte green");


            isOk = byte.TryParse(color[2], out b);

            if (!isOk)
                throw new InvalidDataException
                    ($"Data read error: {nameof(result)} byte blue");

            Color = new Color(r, g, b);
        }

        public virtual void Input(string fileName)
        {
            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                fileName
            );

            string [] result = File.ReadAllLines(path);
            ReadFields(result);
        }

        #endregion

        public virtual void Input()
        {
            Name = HelpfullFuncs.GetStringFromInput("Fruit name");

            byte red = HelpfullFuncs.GetByteFromInput("Red byte value");
            byte green = HelpfullFuncs.GetByteFromInput("Green byte value");
            byte blue = HelpfullFuncs.GetByteFromInput("Blue byte value");

            Color = new(red, green, blue);

        }

        public virtual void Print()
        {
            Console.WriteLine($"Fruit: {Name}");
            Color.Print();
            
        }

        public override string ToString()
        {
            return $"{Name}: \n" +
                $"Color: {Color.R}/{Color.G}/{Color.B}";
        }

    }

    public class Citrus : Fruit, IFruit
    {
        /// <summary>
        /// IN MILIGRAMS
        /// </summary>
        public uint VitaminC { get; private set; } = 0;

        public Citrus() { }

        [JsonConstructor]
        public Citrus(string name, Color color, uint vitaminC) 
            : base(name, color)
        {
            VitaminC = vitaminC;
        }


        #region [FILE]

        public override void ReadFields(string[] result)
        {
            base.ReadFields(result);

            uint mlg = 0;

            bool isOk = uint.TryParse(result[2], out mlg);

            if (isOk)
                VitaminC = mlg;
            else
                throw new InvalidDataException($"Data error::{nameof(result)}");

        }

        public override void Input(string filePath)
        {
            string[] result = File.ReadAllLines(filePath);

            ReadFields(result);
        }

        #endregion

        public override void Input()
        {
            base.Input();

            VitaminC = HelpfullFuncs.GetUintFromInput("VitaminC amount in gram");

        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine($"Vitamin C: {VitaminC} miligrams");
        }
    }

}
