using Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Common.Services
{
    public class BaseService<T>
         where T : class
    {
        private DbContext Context { get; set; }
        private DbSet<T> Items { get; set; }

        public BaseService()
        {
            Context = new BankDbContext();
            Items = Context.Set<T>();
        }

        public List<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            string orderBy = null,
            bool sortAsc = false,
            int page = 1,
            int pageSize = int.MaxValue)
        {
            var query = Items.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(orderBy))
            {
                if (sortAsc)
                    query = query.OrderBy(e => EF.Property<object>(e, orderBy));
                else
                    query = query.OrderByDescending(e => EF.Property<object>(e, orderBy));
            }

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return query.ToList();
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            var query = Items.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public T GetById(int id)
        {
            return Items.Find(id);
        }

        public void Save(T item)
        {
            var id = GetIntPrimaryKeyValue(item);

            if (id > 0)
                Items.Update(item);
            else
                Items.Add(item);

            Context.SaveChanges();
        }

        public void Delete(T item)
        {
            Items.Remove(item);
            Context.SaveChanges();
        }

        private int GetIntPrimaryKeyValue(T item)
        {
            var entityType = Context.Model.FindEntityType(typeof(T));
            var pk = entityType?.FindPrimaryKey();

            if (pk == null || pk.Properties.Count != 1)
                throw new InvalidOperationException($"Entity '{typeof(T).Name}' must have a single int primary key to use this BaseService 1:1.");

            var pkPropName = pk.Properties[0].Name;

            var value = Context.Entry(item).Property(pkPropName).CurrentValue;

            if (value == null)
                return 0;

            return Convert.ToInt32(value);
        }
    }
}
