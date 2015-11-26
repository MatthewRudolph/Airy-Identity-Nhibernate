using System;

namespace Dematt.Airy.Core.Extensions
{
    /// <summary>
    /// String extensions method used when defining Fluent NHibernate auto mapping conventions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Remove first instance of the specified string.
        /// </summary>
        /// <param name="source">The source <see cref="string"/>.</param>
        /// <param name="remove">The <see cref="string"/> to remove.</param>
        /// <returns>The <see cref="string"/> with the source removed</returns>
        public static string RemoveFirstInstance(this string source, string remove)
        {
            int index = source.IndexOf(remove, StringComparison.Ordinal);
            return index < 0 ? source : source.Substring(0, index) + source.Substring(index + remove.Length);
        }

        /// <summary>
        /// Remove first instance of the specified string.
        /// </summary>
        /// <param name="source">The source <see cref="string"/>.</param>
        /// <param name="find">The <see cref="string"/> to find.</param>
        /// <param name="replace">The <see cref="string"/> to put in place of the found string.</param>
        /// <returns>The <see cref="string"/> with the find value replaced by the replace value.</returns>
        public static string ReplaceFirstInstance(this string source, string find, string replace)
        {
            int index = source.IndexOf(find, StringComparison.Ordinal);
            return index < 0 ? source : source.Substring(0, index) + replace + source.Substring(index + find.Length);
        }
    }
}
