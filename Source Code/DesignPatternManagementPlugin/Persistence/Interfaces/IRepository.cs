using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Persistence.Interfaces
{
    /// <summary>
    /// An interface for an object repository.
    /// </summary>
    /// <typeparam name="T">The type of the entity served by a derived class.</typeparam>
    /// <see>
    /// http://geekswithblogs.net/seanfao/archive/2009/12/03/136680.aspx
    /// </see>
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> Fetch();
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T Single(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Attach(T entity);
        void SaveChanges();
        void SaveChanges(SaveOptions options);
    }
}
