using System;
using MJBLogger;

namespace Sample
{
    public class Fruit
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MJBLog Log = new MJBLog();
            Log.Clear();
            Fruit Orange = new Fruit()
            {
                Name = nameof(Orange),
                Type = @"Citris",
                Quantity = 20
            };

            Log.PropertyReport(Orange);
        }
    }
}
