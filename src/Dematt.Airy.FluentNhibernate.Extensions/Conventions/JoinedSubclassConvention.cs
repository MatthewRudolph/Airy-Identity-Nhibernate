////namespace Dematt.Cheaha.DataNh.Conventions
////{
////    using FluentNHibernate.Conventions;
////    using FluentNHibernate.Conventions.Instances;

////    /// <summary>
////    /// Specifies the Joined Sub-class Convention type used by NHibernate for automatically mapping domain entities to a database schema.
////    /// </summary>
////    public class JoinedSubclassConvention : IJoinedSubclassConvention
////    {
////        /// <summary>
////        /// Applies the convention.
////        /// </summary>
////        /// <param name="instance">An instance of a class the implements the <see cref="IJoinedSubclassInstance"/> interface.</param>
////        public void Apply(IJoinedSubclassInstance instance)
////        {
////            instance.Table(instance.EntityType.Name);
////            instance.Key.Column(instance.EntityType.Name + "Id");
////        }
////    }
////}