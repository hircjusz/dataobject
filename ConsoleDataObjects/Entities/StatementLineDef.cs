using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects.NET.Attributes;

namespace ConsoleDataObjects.Entities
{
    [Flat]
    public abstract class StatementLineDef:DataObject
    {
        public abstract  string SortPath { get; set; }
    }
}
