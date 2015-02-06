using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ConsoleDataObjects.Entities;
using ConsoleDataObjects.Serialized;
using DataObjects.NET;
//using ExcelLib;

namespace ConsoleDataObjects.Importers
{
    public class StatementLineDefsImporter
    {

        public void Import(Session session, string excelFilePath)
        {
            string xml=string.Empty;
            //using (var engine = new ExportExcelEngine())
            //{
            //    engine.SetSpreadsheet(excelFilePath);
            //    engine.Process();

            //    xml = engine.StringResult.First();
            //}

            var xmlSerializer = new XmlSerializer(typeof(StatementLineDefImporter));

            var statementCollection = (StatementLineDefImporter)xmlSerializer.Deserialize(new StringReader(xml));

            foreach (var item in statementCollection.StatementLineDefElement)
            {
                var statementExisting = StatementLineDef.GetForAlias(session, item.Alias);
                if (statementExisting == null)
                {

                }
                else
                {

                }

            }

        }
    }
}
