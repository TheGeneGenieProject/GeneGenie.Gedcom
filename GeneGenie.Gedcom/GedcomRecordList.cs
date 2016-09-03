// <copyright file="GedcomRecordList.cs" company="GeneGenie.com">
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see http:www.gnu.org/licenses/ .
//
// </copyright>
// <author> Copyright (C) 2008 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <typeparam name="T">TODO: Not sure what uses this yet.</typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    public class GedcomRecordList<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordList{T}"/> class.
        /// </summary>
        public GedcomRecordList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public GedcomRecordList(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Occurs when [changed].
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Adds an object to the end of the list.
        /// </summary>
        /// <param name="item">The object to be added to the end of the list. The value can be null for reference types.</param>
        public new void Add(T item)
        {
            base.Add(item);
            OnChanged();
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the list. The collection itself cannot be null, but it can contain elements that are null, if type <paramref name="collection" /> is a reference type.</param>
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            OnChanged();
        }

        /// <summary>
        /// Inserts an element into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnChanged();
        }

        /// <summary>
        /// Inserts the elements of a collection into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the list. The collection itself cannot be null, but it can contain elements that are null, if type <paramref name="collection" /> is a reference type.</param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            OnChanged();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="item">The object to remove from the list. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item" /> is successfully removed; otherwise, false.  This method also returns false if <paramref name="item" /> was not found in the list.
        /// </returns>
        public new bool Remove(T item)
        {
            bool ret = base.Remove(item);

            if (ret)
            {
                OnChanged();
            }

            return ret;
        }

        /// <summary>
        /// Removes the element at the specified index of the list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnChanged();
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the list .
        /// </returns>
        public new int RemoveAll(Predicate<T> match)
        {
            int ret = base.RemoveAll(match);
            if (ret > 0)
            {
                OnChanged();
            }

            return ret;
        }

        /// <summary>
        /// Removes a range of elements from the list.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            OnChanged();
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            OnChanged();
        }

        private void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }
    }
}
