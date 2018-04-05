using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{

    class Program
    {
        public static void Main(string[] args)
        {
            
            Graph gr = new Graph();
            gr.GetGraph();
            gr.MakeSectors();
            gr.ParseTrack();


            Console.WriteLine("------------");

            var timebeg = DateTime.Now;
            var obls = gr.GetOblId();
            foreach (var t in obls)
            {
                foreach (var k in t)
                {
                    Console.WriteLine(k);
                }
            }
            var timeend = DateTime.Now;
            Console.WriteLine(timeend - timebeg);
            Console.ReadLine();
            gr.ParseTrack();
        }
    }
}
