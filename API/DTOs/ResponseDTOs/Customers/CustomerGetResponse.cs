using API.DTOs.RequestDTOs.Customers;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.Customers
{
    public class CustomerGetResponse : BaseGetResponse<Customer>
    {
        public CustomerGetFilterRequest Filter { get; set; }
    }
}
