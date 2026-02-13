using API.DTOs.RequestDTOs.Managers;
using API.DTOs.ResponseDTOs.Managers;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class ManagersController
            : BaseCrudController<Manager, ManagerRequest, ManagerGetRequest, ManagerGetResponse, ManagerService>
    {
        protected override void MapToEntity(ManagerRequest model, Manager entity)
        {
            entity.FirstName = (model.FirstName ?? string.Empty).Trim();
            entity.LastName = (model.LastName ?? string.Empty).Trim();
            entity.Email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
            entity.Password = model.Password ?? string.Empty;
            entity.Address = (model.Address ?? string.Empty).Trim();
        }

        protected override Expression<Func<Manager, bool>>? BuildFilter(ManagerGetRequest request)
        {
            var f = request.Filter;
            if (f == null) return null;

            return x =>
                (!f.ManagerId.HasValue || x.ManagerId == f.ManagerId.Value) &&
                (string.IsNullOrWhiteSpace(f.FirstName) || x.FirstName.Contains(f.FirstName)) &&
                (string.IsNullOrWhiteSpace(f.LastName) || x.LastName.Contains(f.LastName)) &&
                (string.IsNullOrWhiteSpace(f.Email) || x.Email.Contains(f.Email));
        }
    }
}
