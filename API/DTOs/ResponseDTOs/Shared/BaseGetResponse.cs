namespace API.DTOs.ResponseDTOs.Shared
{
    public class BaseGetResponse<E>
    {
        public List<E> Items { get; set; }
        public PagerResponse Pager { get; set; }
        public string OrderBy { get; set; }
        public bool SortAsc { get; set; }
    }
}
