using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects.NET;


namespace ConsoleDataObjects.Entities
{
    public abstract class Person : DataObject
    {
        public abstract string Name { get; set; }
        public abstract string SecondName { get; set; }
        public abstract string Surname { get; set; }
        public abstract int Age { get; set; }
        public abstract string Info { get; set; }
    }
}
