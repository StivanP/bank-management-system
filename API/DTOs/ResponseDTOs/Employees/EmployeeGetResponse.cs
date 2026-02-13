using API.DTOs.RequestDTOs.Employees;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.Employees
{
    public class EmployeeGetResponse : BaseGetResponse<Employee>
    {
        public EmployeeGetFilterRequest Filter { get; set; }
    }
}
