// <copyright file="GedcomRecordList.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2008 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// TODO: Doc + i think we might be able to use an ObservableList instead.
    /// </summary>
    /// <typeparam name="T">TODO: Not sure what uses this yet.</typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    public class GedcomRecordList<T> : ObservableCollection<T>
    {
        public override int GetHashCode()
        {
            int hc = 0;
            if (Items != null)
            {
                foreach (var p in Items)
                {
                    hc ^= p.GetHashCode();
                }
            }

            return hc;
        }
    }
}
