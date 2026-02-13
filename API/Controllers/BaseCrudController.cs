using API.DTOs.RequestDTOs.Shared;
using API.DTOs.ResponseDTOs.Shared;
using API.Services;
using Common;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace API.Controllers
{
    public abstract class BaseCrudController<TEntity, TRequest, TGetRequest, TGetResponse, TService> : ControllerBase
        where TEntity : class, new()
        where TService : BaseService<TEntity>, new()
        where TGetRequest : BaseGetRequest, new()
        where TGetResponse : BaseGetResponse<TEntity>, new()
    {
        protected readonly TService Service = new();

        protected abstract void MapToEntity(TRequest model, TEntity entity);

        protected virtual Expression<Func<TEntity, bool>>? BuildFilter(TGetRequest request) => null;

        protected virtual string EntityName => typeof(TEntity).Name;

        [HttpPost("get")]
        public virtual IActionResult Get([FromBody] TGetRequest? model)
        {
            model ??= new TGetRequest();

            var filter = BuildFilter(model);

            var orderBy = string.IsNullOrWhiteSpace(model.OrderBy) ? null : model.OrderBy;

            var page = model.Pager?.Page > 0 ? model.Pager.Page : 1;
            var pageSize = model.Pager?.PageSize > 0 ? model.Pager.PageSize : int.MaxValue;

            var items = Service.GetAll(
                filter: filter,
                orderBy: orderBy,
                sortAsc: model.SortAsc,
                page: page,
                pageSize: pageSize
            );

            var count = Service.Count(filter);

            var response = new TGetResponse
            {
                Items = items,
                OrderBy = orderBy ?? string.Empty,
                SortAsc = model.SortAsc,
                Pager = new PagerResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Count = count
                }
            };

            TryCopyFilter(model, response);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public virtual IActionResult GetById(int id)
        {
            var entity = Service.GetById(id);

            if (entity == null)
                return NotFoundWithError($"{EntityName} with id={id} was not found.");

            return Ok(entity);
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] TRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var entity = new TEntity();
            MapToEntity(model, entity);

            Service.Save(entity);

            return Ok(entity);
        }

        [HttpPut("{id:int}")]
        public virtual IActionResult Update(int id, [FromBody] TRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var entity = Service.GetById(id);

            if (entity == null)
                return NotFoundWithError($"{EntityName} with id={id} was not found.");

            MapToEntity(model, entity);
            Service.Save(entity);

            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public virtual IActionResult Delete(int id)
        {
            var entity = Service.GetById(id);

            if (entity == null)
                return NotFoundWithError($"{EntityName} with id={id} was not found.");

            Service.Delete(entity);
            return Ok();
        }

        protected IActionResult NotFoundWithError(string message)
        {
            ModelState.AddModelError("Global", message);
            return NotFound(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }

        private static void TryCopyFilter(TGetRequest request, TGetResponse response)
        {
            var reqFilterProp = request.GetType().GetProperty("Filter");
            var respFilterProp = response.GetType().GetProperty("Filter");

            if (reqFilterProp == null || respFilterProp == null || !respFilterProp.CanWrite)
                return;

            var filterValue = reqFilterProp.GetValue(request);
            respFilterProp.SetValue(response, filterValue);
        }
    }
}
