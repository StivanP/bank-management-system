using Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Common.Services
{
    public class BaseJunctionService<TLink> : IDisposable
        where TLink : class
    {
        protected DbContext Context { get; }
        protected DbSet<TLink> Items { get; }

        private readonly IReadOnlyList<IProperty> _pkProps;

        public BaseJunctionService()
        {
            Context = new BankDbContext();
            Items = Context.Set<TLink>();
            _pkProps = ResolveTwoIntPrimaryKeyOrThrow();
        }

        public List<TLink> GetAll(
            Expression<Func<TLink, bool>>? filter = null,
            string? orderBy = null,
            bool sortAsc = false,
            int page = 1,
            int pageSize = int.MaxValue)
        {
            var query = Items.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = sortAsc
                    ? query.OrderBy(e => EF.Property<object>(e, orderBy))
                    : query.OrderByDescending(e => EF.Property<object>(e, orderBy));
            }

            page = page > 0 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : int.MaxValue;

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int Count(Expression<Func<TLink, bool>>? filter = null)
        {
            var query = Items.AsQueryable();
            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public TLink? GetByIds(int key1, int key2)
            => Items.Find(key1, key2);

        public bool Exists(int key1, int key2)
            => Items.Find(key1, key2) != null;
        public void Create(TLink entity)
        {
            var (key1, key2) = ReadKeyValues(entity);

            if (Items.Find(key1, key2) != null)
                throw new InvalidOperationException(
                    $"{typeof(TLink).Name} with key ({key1}, {key2}) already exists.");

            Items.Add(entity);
            Context.SaveChanges();
        }

        public void Update(TLink entity)
        {
            var (key1, key2) = ReadKeyValues(entity);
            var existing = Items.Find(key1, key2);

            if (existing == null)
                throw new InvalidOperationException(
                    $"{typeof(TLink).Name} with key ({key1}, {key2}) was not found.");

            Context.Entry(existing).CurrentValues.SetValues(entity);

            foreach (var pk in _pkProps)
                Context.Entry(existing).Property(pk.Name).IsModified = false;

            Context.SaveChanges();
        }

        public void DeleteByIds(int key1, int key2)
        {
            var entity = Items.Find(key1, key2);

            if (entity == null)
                throw new InvalidOperationException(
                    $"{typeof(TLink).Name} with key ({key1}, {key2}) was not found.");

            Items.Remove(entity);
            Context.SaveChanges();
        } 

        public void Delete(TLink entity)
        {
            Items.Remove(entity);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        private IReadOnlyList<IProperty> ResolveTwoIntPrimaryKeyOrThrow()
        {
            var entityType = Context.Model.FindEntityType(typeof(TLink));
            var pk = entityType?.FindPrimaryKey();

            if (pk == null)
                throw new InvalidOperationException(
                    $"Entity '{typeof(TLink).Name}' has no primary key configured in EF model.");

            if (pk.Properties.Count != 2)
                throw new InvalidOperationException(
                    $"Entity '{typeof(TLink).Name}' must have exactly 2 primary key properties to use BaseJunctionService.");

            if (pk.Properties.Any(p => p.ClrType != typeof(int)))
                throw new InvalidOperationException(
                    $"Entity '{typeof(TLink).Name}' primary key properties must be of type int.");

            return pk.Properties;
        }

        private (int key1, int key2) ReadKeyValues(TLink entity)
        {
            var p1 = typeof(TLink).GetProperty(_pkProps[0].Name);
            var p2 = typeof(TLink).GetProperty(_pkProps[1].Name);

            if (p1 == null || p2 == null)
                throw new InvalidOperationException(
                    $"Cannot read PK properties on '{typeof(TLink).Name}'. Check property names and EF mapping.");

            var v1 = p1.GetValue(entity);
            var v2 = p2.GetValue(entity);

            if (v1 == null || v2 == null)
                throw new InvalidOperationException(
                    $"PK values for '{typeof(TLink).Name}' cannot be null.");

            return (Convert.ToInt32(v1), Convert.ToInt32(v2));
        }
    }
}
