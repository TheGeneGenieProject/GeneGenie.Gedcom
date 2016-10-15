namespace GeneGenie.Gedcom
{
    using System;

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    public class GedcomGenericComparer
    {
        /// <summary>
        /// Compares two records to see if they are equal.
        /// Safely handles one or both being null.
        /// The records must implement IComparable.
        /// </summary>
        /// <typeparam name="T">A type that implements IComparable.</typeparam>
        /// <param name="item1">The first record.</param>
        /// <param name="item2">The second record.</param>
        /// <returns>
        /// Returns an integer that indicates their relative position in the sort order.
        /// </returns>
        public static int SafeCompareOrder<T>(T item1, T item2)
            where T : IComparable<T>
        {
            if (item1 == null && item2 == null)
            {
                return 0;
            }
            else if (item2 == null)
            {
                return -1;
            }
            else if (item1 == null)
            {
                return 1;
            }
            else
            {
                return item1.CompareTo(item2);
            }
        }
    }
}
