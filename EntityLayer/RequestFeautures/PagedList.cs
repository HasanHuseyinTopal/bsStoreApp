
namespace EntityLayer.RequestFeautures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new()
            {
                TotalCount = count,
                PageSize = pageSize,
                TotalPage = (int)Math.Ceiling(count / (double)pageSize),
                CurrentPage = pageNumber
            };
            AddRange(items);
        }
        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageSize, int pageNumber)
        {
            var count = source.Count();
            var items =  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        
    }
}
