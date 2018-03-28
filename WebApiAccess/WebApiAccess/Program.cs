using System;
using System.Net.Http;
using Model;
using System.Reflection;

namespace WebApiAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("enter base address");
            string baseAddress = Console.ReadLine();
            ApiCalls a = new ApiCalls(baseAddress);
            a.ReadInput();
            Console.Read();

        }


    } 
}
