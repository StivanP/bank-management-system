namespace API.DTOs.RequestDTOs.Shared
{
    public class BaseGetRequest
    {
        public PagerRequest Pager { get; set; }
        public string OrderBy { get; set; }
        public bool SortAsc { get; set; }
    }
}
