using Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Common.Services
{
    public class BaseService<T> where T : class
    {
        private readonly BankDbContext _context;
        private readonly DbSet<T> _items;

        public BaseService(BankDbContext context)
        {
            _context = context;
            _items = context.Set<T>();
        }

        public List<T> GetAll(
            Expression<Func<T, bool>>? filter = null,
            string? orderBy = null,
            bool sortAsc = false,
            int page = 1,
            int pageSize = int.MaxValue)
        {
            var query = _items.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = sortAsc
                    ? query.OrderBy(e => EF.Property<object>(e, orderBy))
                    : query.OrderByDescending(e => EF.Property<object>(e, orderBy));
            }

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int Count(Expression<Func<T, bool>>? filter = null)
        {
            var query = _items.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public T? GetById(int id) => _items.Find(id);

        public void Save(T item)
        {
            var id = GetIntPrimaryKeyValue(item);

            if (id > 0)
                _items.Update(item);
            else
                _items.Add(item);

            _context.SaveChanges();
        }

        public void Delete(T item)
        {
            _items.Remove(item);
            _context.SaveChanges();
        }

        private int GetIntPrimaryKeyValue(T item)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var pk = entityType?.FindPrimaryKey();

            if (pk == null || pk.Properties.Count != 1)
                throw new InvalidOperationException(
                    $"Entity '{typeof(T).Name}' must have a single int primary key.");

            var pkPropName = pk.Properties[0].Name;
            var value = _context.Entry(item).Property(pkPropName).CurrentValue;

            return value == null ? 0 : Convert.ToInt32(value);
        }
    }
}