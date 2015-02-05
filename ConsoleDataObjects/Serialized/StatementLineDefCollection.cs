using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDataObjects.Serialized
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class StatementLineDefCollection
    {

        private StatementLineDefCollectionStatementLineDefElement[] statementLineDefElementField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StatementLineDefElement")]
        public StatementLineDefCollectionStatementLineDefElement[] StatementLineDefElement
        {
            get
            {
                return this.statementLineDefElementField;
            }
            set
            {
                this.statementLineDefElementField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StatementLineDefCollectionStatementLineDefElement
    {

        private string aliasField;

        private string displayName_PLField;

        private string displayName_ENField;

        private string isOptionalField;

        private string isCalculatedField;

        private string levelNameField;

        private string levelField;

        private string levelOrderField;

        private string parentLineDefField;

        private string rZiSFlagField;

        private string segmentField;

        private string sumOfLinesField;

        private string differenceOfLinesField;

        /// <remarks/>
        public string Alias
        {
            get
            {
                return this.aliasField;
            }
            set
            {
                this.aliasField = value;
            }
        }

        /// <remarks/>
        public string DisplayName_PL
        {
            get
            {
                return this.displayName_PLField;
            }
            set
            {
                this.displayName_PLField = value;
            }
        }

        /// <remarks/>
        public string DisplayName_EN
        {
            get
            {
                return this.displayName_ENField;
            }
            set
            {
                this.displayName_ENField = value;
            }
        }

        /// <remarks/>
        public string IsOptional
        {
            get
            {
                return this.isOptionalField;
            }
            set
            {
                this.isOptionalField = value;
            }
        }

        /// <remarks/>
        public string IsCalculated
        {
            get
            {
                return this.isCalculatedField;
            }
            set
            {
                this.isCalculatedField = value;
            }
        }

        /// <remarks/>
        public string LevelName
        {
            get
            {
                return this.levelNameField;
            }
            set
            {
                this.levelNameField = value;
            }
        }

        /// <remarks/>
        public string Level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }

        /// <remarks/>
        public string LevelOrder
        {
            get
            {
                return this.levelOrderField;
            }
            set
            {
                this.levelOrderField = value;
            }
        }

        /// <remarks/>
        public string ParentLineDef
        {
            get
            {
                return this.parentLineDefField;
            }
            set
            {
                this.parentLineDefField = value;
            }
        }

        /// <remarks/>
        public string RZiSFlag
        {
            get
            {
                return this.rZiSFlagField;
            }
            set
            {
                this.rZiSFlagField = value;
            }
        }

        /// <remarks/>
        public string Segment
        {
            get
            {
                return this.segmentField;
            }
            set
            {
                this.segmentField = value;
            }
        }

        /// <remarks/>
        public string SumOfLines
        {
            get
            {
                return this.sumOfLinesField;
            }
            set
            {
                this.sumOfLinesField = value;
            }
        }

        /// <remarks/>
        public string DifferenceOfLines
        {
            get
            {
                return this.differenceOfLinesField;
            }
            set
            {
                this.differenceOfLinesField = value;
            }
        }
    }




}
