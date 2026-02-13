using API.DTOs.RequestDTOs.Branches;
using API.DTOs.ResponseDTOs.Branches;
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
    public class BranchesController
            : BaseCrudController<Branch, BranchRequest, BranchGetRequest, BranchGetResponse, BranchService>
    {
        protected override void MapToEntity(BranchRequest model, Branch entity)
        {
            entity.BranchName = (model.BranchName ?? string.Empty).Trim();
            entity.Location = (model.Location ?? string.Empty).Trim();
        }

        protected override Expression<Func<Branch, bool>>? BuildFilter(BranchGetRequest request)
        {
            var f = request.Filter;
            if (f == null) return null;

            return x =>
                (!f.BranchId.HasValue || x.BranchId == f.BranchId.Value) &&
                (string.IsNullOrWhiteSpace(f.BranchName) || x.BranchName.Contains(f.BranchName)) &&
                (string.IsNullOrWhiteSpace(f.Location) || x.Location.Contains(f.Location));
        }
    }
}
