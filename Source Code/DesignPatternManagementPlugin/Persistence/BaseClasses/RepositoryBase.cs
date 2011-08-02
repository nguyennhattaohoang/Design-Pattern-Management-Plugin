using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using log4net;
using Persistence.Interfaces;

namespace Persistence.BaseClasses
{
    /// <summary>
    /// A base class for an object set repository.
    /// </summary>
    /// <typeparam name="T">The type served by concrete implementations of this 
    /// class.</typeparam>
    /// <remarks>This repository manages an implementation Microsoft Entity Framework 
    /// 4.</remarks>
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        #region Fields

        // Private member variables
        private ObjectContext m_ObjectContext;
        private IObjectSet<T> m_ObjectSet;
        private bool m_UsingSharedObjectContext;
        private static readonly ILog m_Logger = LogManager.GetLogger("RepositoryBase");

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Repository class with its own object context.
        /// </summary>
        /// <param name="filePath">The path to the data file.</param>
        /// <param name="contextType">The type of the EF4 object contex tcreated by 
        /// this repository.</param>
        /// <param name="edmName">The name of the Entity Data Model served by this 
        /// repository.</param>
        /// <remarks>
        /// The object context for an EDM is typed to the EDM. The type can be found
        /// in the Designer code for the EDM; it is the class that derives from
        /// ObjectContext. The type is passed to this constructor by a base() call
        /// from the constructor of a derived class.
        /// </remarks>
        protected RepositoryBase(string filePath, Type contextType, string edmName)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.RepositoryBase(filePath, contextType, edmName) invoked.");

            // Create object context and object set
            m_ObjectContext = this.CreateObjectContext(filePath, contextType, edmName);
            m_ObjectSet = m_ObjectContext.CreateObjectSet<T>();
            m_UsingSharedObjectContext = false;

            // Log completion
            m_Logger.Info("RepositoryBase.RepositoryBase() completed.");
        }

        /// <summary>
        /// Initializes a new instance of the Repository class with a shared object context.
        /// </summary>
        /// <param name="objectContext">An Entity Framework 4 object context.</param>
        protected RepositoryBase(ObjectContext objectContext)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.RepositoryBase(objectContext) invoked.");

            // Create object set
            m_ObjectContext = objectContext;
            m_ObjectSet = m_ObjectContext.CreateObjectSet<T>();
            m_UsingSharedObjectContext = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is 
        /// null</exception>
        public void Add(T entity)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Add() invoked.");

            if (entity == null)
            {
                m_Logger.Error("ArgumentNullException: entity");
                throw new ArgumentNullException("entity");
            }
            m_ObjectSet.AddObject(entity);

            // Log completion
            m_Logger.Info("RepositoryBase.Add() completed.");
        }

        /// <summary>
        /// Attaches the specified entity
        /// </summary>
        /// <param name="entity">Entity to attach</param>
        public void Attach(T entity)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Attach() invoked.");

            if (entity == null)
            {
                m_Logger.Error("ArgumentNullException: entity");
                throw new ArgumentNullException("entity");
            }
            m_ObjectSet.Attach(entity);

            // Log completion
            m_Logger.Info("RepositoryBase.Attach() completed.");
        }

        /// <summary>
        /// Deletes the specified entitiy
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is 
        /// null</exception>
        public void Delete(T entity)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Delete() invoked.");

            if (entity == null)
            {
                m_Logger.Error("ArgumentNullException: entity");
                throw new ArgumentNullException("entity");
            }
            m_ObjectSet.DeleteObject(entity);

            // Log completion
            m_Logger.Info("RepositoryBase.Delete() completed.");

        }

        /// <summary>
        /// Deletes records matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        public void Delete(Expression<Func<T, bool>> predicate)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Delete() invoked.");

            var records = from x in m_ObjectSet.Where(predicate) select x;
            foreach (T record in records)
            {
                m_ObjectSet.DeleteObject(record);
            }

            // Log completion
            m_Logger.Info("RepositoryBase.Delete() completed.");
        }

        /// <summary>
        /// Releases all resources used by the Repository.
        /// </summary>
        public void Dispose()
        {
            /* See http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx */

            // Call the protected method in this class
            var disposeOfObjectContext = (m_UsingSharedObjectContext == false);
            Dispose(disposeOfObjectContext);

            // Dispose() will finalize this object, so take it out of the queue
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets all records as an IQueryable
        /// </summary>
        /// <returns>An IQueryable object containing the results of the query</returns>
        public IQueryable<T> Fetch()
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Fetch() invoked.");

            // Get object set
            var objectSet = m_ObjectSet;

            // Log completion
            m_Logger.Info("RepositoryBase.Fetch() completed.");

            return objectSet;
        }

        /// <summary>
        /// Finds a record with the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A collection containing the results of the query</returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Find() invoked.");

            // Get object set
            var objectSet = m_ObjectSet.Where(predicate);

            // Log completion
            m_Logger.Info("RepositoryBase.Find() completed.");

            return objectSet;
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified 
        /// criteria</returns>
        public T First(Expression<Func<T, bool>> predicate)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.First() invoked.");

            // Get object set
            var objectSet = m_ObjectSet.First(predicate);

            // Log completion
            m_Logger.Info("RepositoryBase.First() completed.");

            // Set return value
            return objectSet;
        }

        /// <summary>
        /// Gets all records as an IEnumberable
        /// </summary>
        /// <returns>An IEnumberable object containing the results of the query</returns>
        public IEnumerable<T> GetAll()
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.GetAll() invoked.");

            // Get object set
            var objectSet = Fetch().AsEnumerable();

            // Log completion
            m_Logger.Info("RepositoryBase.GetAll() completed.");

            // Set return value
            return objectSet;
        }

        /// <summary>
        /// Saves all context changes
        /// </summary>
        public void SaveChanges()
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.SaveChanges() invoked.");

            // Save changes
            m_ObjectContext.SaveChanges();

            // Log completion
            m_Logger.Info("RepositoryBase.SaveChanges() completed.");
        }

        /// <summary>
        /// Saves all context changes with the specified SaveOptions
        /// </summary>
        /// <param name="options">Options for saving the context</param>
        public void SaveChanges(SaveOptions options)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.SaveChanges(options) invoked.");

            // Save changes
            m_ObjectContext.SaveChanges(options);

            // Log completion
            m_Logger.Info("RepositoryBase.SaveChanges(options) completed.");
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T Single(Expression<Func<T, bool>> predicate)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.Single() invoked.");

            // Get object set
            var objectSet = m_ObjectSet.Single(predicate);

            // Log completion
            m_Logger.Info("RepositoryBase.Single() completed.");

            // Set return value
            return objectSet;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases all resources used by the Repository.
        /// </summary>
        /// <param name="disposing">A boolean value indicating whether or not to dispose managed 
        /// resources</param>
        protected virtual void Dispose(bool disposing)
        {
            /* See http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx */

            if (!disposing) return;
            if (m_ObjectContext == null) return;
            m_ObjectContext.Dispose();
            m_ObjectContext = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Factory method to create an Entity Framework 4 ObjectContext.
        /// </summary>
        /// <param name="filePath">The path to the target data file.</param>
        /// <param name="contextType">The type of the EF4 object context created by this
        /// repository.</param>
        /// <param name="edmName">The name of the Entity Data Model served by this 
        /// repository.</param>
        /// <returns>A new ObjectContext.</returns>
        private ObjectContext CreateObjectContext(string filePath, Type contextType, string edmName)
        {
            // Log invocation
            m_Logger.Info("RepositoryBase.CreateObjectContext() invoked.");

            // Validate EDM Name
            if (edmName == null)
            {
                // Log exception
                m_Logger.Error("ArgumentNullException: edmName");

                // Throw exception
                throw new ArgumentNullException("edmName");
            }

            // Check file path
            if (filePath == null)
            {
                // Log exception
                m_Logger.Error("ArgumentNullException: filePath");

                // Throw exception
                throw new ArgumentNullException("filePath");
            }

            // Configure a SQL CE connection string  
            var sqlCompactConnectionString = string.Format("Data Source={0}", filePath);

            // Create an Entity Connection String Builder
            var builder = new EntityConnectionStringBuilder();

            /* The builder creates an EDM connection string. It expects to receive the
             * EDM model name; e.g., "Model.Books", as opposed to "BooksContainer", which
             * will be the name of the ObjectContext generated by this method. */

            /* The easiest way to verify the EDM model name is to generate a database from
             * the EDM. The Create Database Wizard has an option to write an EDM connection
             * string to the App.config file. Accept the option, and compare the resulting
             * connection string to the metadata string below. */

            /* Note that the value of the m_EdmName variable is set in the constructor of
             * a derived class. */

            // Configure Builder
            builder.Metadata = string.Format("res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl",
                edmName);
            builder.Provider = "System.Data.SqlServerCe.4.0";
            builder.ProviderConnectionString = sqlCompactConnectionString;
            var edmConnectionString = builder.ToString();

            // Create an EDM connection
            EntityConnection edmConnection;
            try
            {
                edmConnection = new EntityConnection(edmConnectionString);

            }
            catch (Exception e)
            {
                // Log exception
                m_Logger.Error(e.Message);

                // Rethrow exception
                throw;
            }

            /* EF ObjectContext objects are typed to the specific EDM that they
             * represent. The context type is injected into the constructor of
             * this class, and we use the System.Activator.CreateInstance() 
             * method to instantiate an object of that type. */

            // Get the object context
            object objectContext;
            try
            {
                objectContext = Activator.CreateInstance(contextType, edmConnection);
            }
            catch (Exception e)
            {
                // Log exception
                m_Logger.Error(e.Message);

                // Rethrow exception
                throw;
            }

            // Log completion
            m_Logger.Info("RepositoryBase.CreateObjectContext() completed.");

            // Set return value
            return (ObjectContext)objectContext;
        }

        #endregion
    }
}
