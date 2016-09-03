/*
 *  $Id: GedcomRecordList.cs 199 2008-11-15 15:20:44Z davek $
 *
 *  Copyright (C) 2008 David A Knight <david@ritter.demon.co.uk>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 *
 */

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;

    public class GedcomRecordList<T> : List<T>
    {
        public GedcomRecordList()
        {
        }

        public GedcomRecordList(int capacity)
            : base(capacity)
        {
        }

        public event EventHandler Changed;

        private void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        public new void Add(T item)
        {
            base.Add(item);
            OnChanged();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            OnChanged();
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnChanged();
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            OnChanged();
        }

        public new bool Remove(T item)
        {
            bool ret = base.Remove(item);

            if (ret)
            {
                OnChanged();
            }

            return ret;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnChanged();
        }

        public new int RemoveAll(Predicate<T> match)
        {
            int ret = base.RemoveAll(match);
            if (ret > 0)
            {
                OnChanged();
            }

            return ret;
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            OnChanged();
        }

        public new void Clear()
        {
            base.Clear();
            OnChanged();
        }
    }
}
