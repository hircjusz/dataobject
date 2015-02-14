using System;

namespace SoftwareMind.Shared.Dto
{
    public abstract class ColumnFilterDTO
    {
        internal const string ColumnFilterParameterName = ":pColumnFilter_{0}";
        internal const int LongParameterLength = 31;
        internal static int ParameterCounter = 0;

        public enum FilterComparison
        {
            None, Lt, Gt, Eq
        }

        public FilterComparison Comparison { get; set; }
        public string Field { get; set; }
        public string Render { get; set; }
        public object Value { get; set; }

        public string FilterField
        {
            get
            {
                return string.IsNullOrEmpty(this.Render) ? this.Field : this.Render;
            }
        }

        private string _filterParameter;
        public string FilterParameter
        {
            get
            {
                if (this._filterParameter == null)
                {
                    this._filterParameter = string.Format(ColumnFilterParameterName, FilterField.Replace(':', '_'));
                    if (this._filterParameter.Length >= LongParameterLength)
                    {
                        this._filterParameter = this._filterParameter.Substring(0, LongParameterLength - 3) + (++ParameterCounter % 100).ToString("00");
                    }
                }
                return this._filterParameter;
            }
        }

        public abstract string ToSqlString(string key);
    }

    public class BooleanColumnFilterDTO : ColumnFilterDTO
    {
        public BooleanColumnFilterDTO()
        {
            this.Comparison = ColumnFilterDTO.FilterComparison.Eq;
        }

        new public bool Value
        {
            get { return (bool)base.Value; }
            set { base.Value = value; }
        }

        public override string ToSqlString(string key)
        {
            return string.Format("\"{0}\" = {1}", string.IsNullOrEmpty(this.Render) ? this.Field : this.Render, key);
        }

        public override string ToString()
        {
            return string.Format("(bool){1}{0}", string.IsNullOrEmpty(this.Render) ? this.Field : this.Render, this.Value ? "" : "!");
        }
    }

    public class DateColumnFilterDTO : ColumnFilterDTO
    {
        new public DateTime Value
        {
            get { return (DateTime)base.Value; }
            set { base.Value = value; }
        }

        public override string ToSqlString(string key)
        {
            string comparision;
            switch (this.Comparison)
            {
                case FilterComparison.Lt:
                    comparision = ">";
                    break;
                case FilterComparison.Eq:
                    comparision = "=";
                    break;
                case FilterComparison.Gt:
                    comparision = ">";
                    break;
                default:
                    throw new ArgumentException();
            }

            return string.Format("\"{0}\" {1} {2}", string.IsNullOrEmpty(this.Render) ? this.Field : this.Render, comparision, key);
        }

        public override string ToString()
        {
            return string.Format(
                "(date){0} {2} {1}",
                string.IsNullOrEmpty(this.Render) ? this.Field : this.Render,
                this.Value,
                this.Comparison == FilterComparison.Eq ? "==" : (this.Comparison == FilterComparison.Lt ? ">" : "<")
            );
        }
    }

    public class NumericColumnFilterDTO : ColumnFilterDTO
    {
        new public decimal Value
        {
            get { return (decimal)base.Value; }
            set { base.Value = value; }
        }

        public override string ToSqlString(string key)
        {
            string comparision;
            switch (this.Comparison)
            {
                case FilterComparison.Lt:
                    comparision = ">";
                    break;
                case FilterComparison.Eq:
                    comparision = "=";
                    break;
                case FilterComparison.Gt:
                    comparision = ">";
                    break;
                default:
                    throw new ArgumentException();
            }

            return string.Format("\"{0}\" {1} {2}", string.IsNullOrEmpty(this.Render) ? this.Field : this.Render, comparision, key);
        }

        public override string ToString()
        {
            return string.Format(
                "(number){0} {2} {1}",
                string.IsNullOrEmpty(this.Render) ? this.Field : this.Render,
                this.Value,
                this.Comparison == FilterComparison.Eq ? "==" : (this.Comparison == FilterComparison.Lt ? ">" : "<")
            );
        }
    }

    public class StringColumnFilterDTO : ColumnFilterDTO
    {
        new public string Value
        {
            get { return (string)base.Value; }
            set { base.Value = value; }
        }

        public override string ToSqlString(string key)
        {
            return string.Format("UPPER(\"{0}\") LIKE '%' || UPPER({1}) || '%'", string.IsNullOrEmpty(this.Render) ? this.Field : this.Render, key);
        }

        public override string ToString()
        {
            return string.Format(
                "(string){0} == {1}",
                string.IsNullOrEmpty(this.Render) ? this.Field : this.Render,
                this.Value
            );
        }
    }
}