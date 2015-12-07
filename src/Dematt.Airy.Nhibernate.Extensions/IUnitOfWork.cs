namespace Dematt.Airy.Nhibernate.Extensions
{
    /// <summary>
    /// Interface for creating a class the encapsulates an atomic unit of work.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// A method that signals the unit of work as begun.
        /// </summary>
        void Begin();

        /// <summary>
        /// A method that commits the complete unit of work.
        /// </summary>
        void Commit();

        /// <summary>
        /// A method that cancel the unit of work ensuring that no changes are or have been made.
        /// </summary>
        void Cancel();
    }
}
