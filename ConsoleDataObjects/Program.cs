using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DataObjects.NET;

namespace ConsoleDataObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildDomain();
        }

        private static Domain BuildDomain()
        {
            string connectionUrl = "mssql://darek:gespardmonge@localhost/DataObjects";
            string productKeyFile = @"D:\SOURCES\dataobjects\ConsoleDataObjects\productKey.txt";
            string productKey = "";
            //if (File.Exists(productKeyFile))
            //    using (StreamReader sr = new StreamReader(productKeyFile))
            //    {
            //        productKey = sr.ReadToEnd().Trim();
            //    }

            Domain domain = DataObjects.NET.Configuration.DefaultDomain;//new Domain(connectionUrl, productKeyFile);
            // Domain setup process starts here
            //domain.RegisterCulture(
            //new Culture("En", "U.S. English", new CultureInfo("en-us", false)));
            //// It's always necessary to specify default culture
            //domain.Cultures["En"].Default = true;
            // Here we register all types from "SampleA.Model" namespace
            domain.RegisterTypes("ConsoleDataObjects.Entities");
            // Now we can build the domain. An exclusive access
            // to domain database should be provided for this period.
            domain.Build();
            // Domain setup process is finished here
            return domain;
        }
    }
}
