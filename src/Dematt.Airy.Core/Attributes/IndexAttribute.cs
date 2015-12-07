using System;

namespace Dematt.Airy.Core.Attributes
{
    /// <summary>
    /// Attribute that informs the persistence layer that it should create an index based on this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        /// <param name="name">The name to use for the index.</param>
        /// <param name="unique">Is the index to be unique. Optional parameter if not passed will be false.</param>
        /// <remarks>
        /// If the same name is used across different properties within a class then the index 
        /// will be based on all the Index attributes that have the same name.
        /// </remarks>
        public IndexAttribute(string name, bool unique = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            Name = name;
            Unique = unique;
        }

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique.
        /// </summary>
        public bool Unique { get; set; }
    }
}
