namespace SoftwareMind.Shared.Dto
{
    public class SortDTO
    {
        public string Field { get; set; }
        public string Render { get; set; }
        public string SortField
        {
            get
            {
                return string.IsNullOrEmpty(this.Render) ? this.Field : this.Render;
            }
        }
        public SortDirection Direction { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.SortField, this.Direction == SortDirection.Ascending ? "ASC" : "DESC");
        }

        public string ToSqlString()
        {
            return string.Format("\"{0}\" {1}", this.SortField, this.Direction == SortDirection.Ascending ? "ASC" : "DESC");
        }
    }
}