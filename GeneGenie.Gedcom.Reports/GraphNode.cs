// <copyright file="GraphNode.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Reports
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <typeparam name="T">TODO: Doc 2</typeparam>
    internal class GraphNode<T>
    {
        /// <summary>
        /// Gets or sets the data held on this graph node. TODO: Doc
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the edges. Not sure if these are contained within this node or held elsewhere and referenced.
        /// </summary>
        public List<GraphNode<T>> Edges { get; set; } = new List<GraphNode<T>>();
    }
}
