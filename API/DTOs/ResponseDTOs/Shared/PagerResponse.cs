using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.ResponseDTOs.Shared
{
    public class PagerResponse : PagerRequest
    {
        public int Count { get; set; }
    }
}
