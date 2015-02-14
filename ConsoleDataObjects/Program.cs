using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ConsoleDataObjects.Entities;
using ConsoleDataObjects.Importers;
using ConsoleDataObjects.Serialized;
using DataObjects.NET;


namespace ConsoleDataObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            var domain = BuildDomain();
            Session session = new Session(domain);
            var heading = BuildStatementHeading(session);
            Utility.UtilityActions.DoStateMachineTransition(session, heading, "Status");


            // var path = @"D:\SOURCES\dataobjects\ConsoleDataObjects\XML\StatementLineDef.xlsx";
            //var statementLineDefImporter = new StatementLineDefsImporter();
            //statementLineDefImporter.Import(session,path);

            //BuildPath(domain);

            //UpdatePerson(domain);

            // GetPerson(domain);
            // PersonBuilder(domain);
        }


        private static StatementHeading BuildStatementHeading(Session session)
        {
            StatementHeading p = (StatementHeading)session.CreateObject(typeof(StatementHeading));
            //session.Commit();
            return p;

        }


        private static void BuildPath(Domain domain)
        {
            using (Session session = new Session(domain))
            {
                session.BeginTransaction();

                List<string> paths = new List<string>();


                StatementLineDef p = (StatementLineDef)session.CreateObject(typeof(StatementLineDef));
                p.Alias = "BILANS_A1";
                p.DisplayName_PL = "Name";
                p.IsOptional = true;
                p.IsCalculated = true;
                p.LevelName = "A";
                p.Level = 1;
                p.LevelOrder = 1;
                p.RZiSFlag = true;
                p.Segment = "SME,CTB";
                p.SumOfLines = "";
                p.DifferenceOfLines = "";


                session.Commit();
            }

        }

        private static void GetPerson(Domain domain)
        {

        }

        private static void UpdatePerson(Domain domain)
        {

            using (Session session = new Session(domain))
            {
                //session.ExecTransactionally(TransactionMode.TransactionRequired, delegate()
                //    { });
                session.BeginTransaction();
                var person = (Person)session[122];
                person.Name = "Darek";
                person.Persist();
                session.Commit();
            }

        }

        private static void PersonBuilder(Domain domain)
        {
            using (Session session = new Session(domain))
            {
                // We can access the storage only during transaction, so...
                session.BeginTransaction();
                Console.WriteLine("\nFirst session");
                // That's how we create our first persistent instance of Person
                Console.WriteLine("Creating an instance of Person ");
                Person p = (Person)session.CreateObject(typeof(Person));
                // Instance is already persisted to the storage now
                // Let's set persistent properties
                p.Name = "John";
                p.Surname = "Smith";
                p.SecondName = "V.";
                p.Age = 32;
                p.Info = "John V. Smith, 32 years old";
                Console.WriteLine("Person's properties:");
                Console.WriteLine("\tPerson's name: {0}", p.Name);
                Console.WriteLine("\tPerson's surname: {0}", p.Surname);
                Console.WriteLine("\tPerson's second name: {0}", p.SecondName);
                Console.WriteLine("\tPerson's info: {0}", p.Info);

                session.Commit();
            }

        }

        private static Domain BuildDomain()
        {
            string connectionUrl = "mssql://darek:gespardmonge@localhost/DataObjects";
            string productKeyFile = @"D:\SOURCES\dataobjects\ConsoleDataObjects\productKey.txt";
            string productKey = "Kj0fhmjalYVIv3FlEm7gHZPANdyS3HH#kflIpXBRvsyyLemkrfM3vOGzOsW#e0Joyqs3x9W26YXLaagsZA#ek32sYGHv6H24uVK8fOpcnRr8vd6KIId#E1";
            //if (File.Exists(productKeyFile))
            //    using (StreamReader sr = new StreamReader(productKeyFile))
            //    {
            //        productKey = sr.ReadToEnd().Trim();
            //    }

            //Domain domain = new Domain(connectionUrl, productKey);

            Domain domain = DataObjects.NET.Configuration.DefaultDomain;
            // Domain setup process starts here
            //domain.RegisterCulture(
            //new Culture("En", "U.S. English", new CultureInfo("en-us", false)));
            ////// It's always necessary to specify default culture
            //domain.Cultures["En"].Default = true;
            //// Here we register all types from "SampleA.Model" namespace
            domain.RegisterTypes("ConsoleDataObjects.Entities");
            // Now we can build the domain. An exclusive access
            // to domain database should be provided for this period.
            domain.Build();
            // Domain setup process is finished here



            return domain;
        }
    }
}
