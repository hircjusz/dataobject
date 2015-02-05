using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects.NET;
using DataObjects.NET.Attributes;

namespace ConsoleDataObjects.Entities
{

    [Flat]
    [Index("IX_STATEMENT_LINE_DEF", new string[] { "Alias", "Level", "LevelOrder" })]
    public abstract class StatementLineDef : DataObject
    {
        #region Properties

        [Nullable(false)]
        public abstract string Alias { get; set; }

        [Nullable(false)]
        public abstract string DisplayName_PL { get; set; }

        [Nullable(true)]
        public abstract string DisplayName_EN { get; set; }

        [Nullable(false)]
        public abstract bool IsOptional { get; set; }

        [Nullable(false)]
        public abstract bool IsCalculated { get; set; }

        [Nullable(true)]
        public abstract string LevelName { get; set; }

        [Nullable(false)]
        public abstract int Level { get; set; }

        [Nullable(false)]
        public abstract int LevelOrder { get; set; }

        [Nullable(true)]
        public abstract StatementLineDef ParentLineDef { get; set; }

        [Nullable(true)]
        public abstract bool RZiSFlag { get; set; }

        [Nullable(true)]
        public abstract string Segment { get; set; }

        [Nullable(false)]
        public abstract string SumOfLines { get; set; }

        [Nullable(false)]
        public abstract string DifferenceOfLines { get; set; }

        #endregion Properties

        #region Methods


        public static StatementLineDef GetForAlias(Session session, string code)
        {
            Query qVariant = session.CreateQuery("select StatementLineDef instances where {Alias} = @code");
            qVariant.Parameters.Add("@code", code);
            DataObject[] variants = qVariant.ExecuteArray();
            if (variants.Length > 0)
            {
                return variants[0] as StatementLineDef;
            }
            return null;
        }

        #endregion
    }

}
