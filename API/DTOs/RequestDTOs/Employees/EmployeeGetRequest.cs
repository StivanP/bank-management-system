using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.RequestDTOs.Employees
{
    public class EmployeeGetRequest : BaseGetRequest
    {
        public EmployeeGetFilterRequest Filter { get; set; }
    }
}
