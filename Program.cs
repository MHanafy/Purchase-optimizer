using Gluh.TechnicalTest.Domain.BruteForce;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Gluh.TechnicalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //PreMute();
            var purchaseRequirements = new TestData().Create();
            var purchaseOptimizer = new PurchaseOptimizer();

            purchaseOptimizer.Optimize(purchaseRequirements);
        }

        static void PreMute()
        {
            var data1 = new string[3][];
            data1[0] = new string[] { "1","2","3","4","5","6"};
            data1[1] = new string[] {"a","b","c","d","e" };
            data1[2] = new string[] {"A","B","C","D","E","F","G","H","I" };


            var premuter = new Premuting<string>(data1);
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            //var mutations = premuter.GetPremutations();
            //stopwatch.Stop();


            //using (var progress = new ProgressBar())
            //{
            //    for (int i = 0; i <= 1000; i++)
            //    {
            //        progress.Report((double)i / 100);
            //        Thread.Sleep(20);
            //    }
            //}
            //Console.WriteLine("Done.");
            var progress = new ProgressBar();


            premuter.Premute((x,y,z) =>
            {
                //Console.WriteLine($"{string.Join('-', x)}");
                progress.Report(z / y);
            });
            progress.Dispose();

            Console.ReadKey();
        }
    }
}
