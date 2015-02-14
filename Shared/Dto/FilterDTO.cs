namespace SoftwareMind.Shared.Dto
{
    public class FilterDTO
    {
        public enum FilterComparison
        {
            None, Lt, Gt, Eq
        }

        public FilterComparison Comparison { get; set; }
        public string Field { get; set; }
    }

    public class FilterDTO<T> : FilterDTO
    {
        public T Value { get; set; }
    }
}