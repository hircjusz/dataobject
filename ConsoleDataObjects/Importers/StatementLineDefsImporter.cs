using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ConsoleDataObjects.Serialized;
using ExcelLib;

namespace ConsoleDataObjects.Importers
{
    public class StatementLineDefsImporter
    {

        public void Import(string excelFilePath)
        {
            string xml;
            using (var engine = new ExportExcelEngine())
            {
                engine.SetSpreadsheet(excelFilePath);
                engine.Process();

                xml = engine.StringResult.First();
            }

            var xmlSerializer = new XmlSerializer(typeof(StatementLineDefs));

            var bookInfo = (StatementLineDefs)xmlSerializer.Deserialize(new StringReader(xml));

        }
    }
}
