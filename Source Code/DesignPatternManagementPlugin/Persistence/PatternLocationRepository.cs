using System;
using Persistence.BaseClasses;
using BusinessModel;

namespace Persistence
{
    /// <summary>
    /// A repository for PatternLocation entities.
    /// </summary>
    public class PatternLocationRepository : RepositoryBase<PatternLocation>
    {
        #region Fields

        // Member variables
        private static Type m_ContextType = typeof(DesignPatternContainer);
        private static string m_EdmName = "DesignPattern";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath">A path to the target data file.</param>
        /// <remarks>
        /// Note that the constructor hard-codes the name of the Entity Data Model served
        /// by this Repository. We hard code the value, rather than pass it in, to isolate
        /// the caller from any knowledge of Entity Framework 4. The result is looser
        /// coupling between Entity Framework 4 and the rest of the application.
        /// </remarks>
        public PatternLocationRepository(string filePath)
            : base(filePath, m_ContextType, m_EdmName)
        {
        }

        #endregion
    }
}
