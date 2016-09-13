// <copyright file="GedcomGraph.cs" company="GeneGenie.com">
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
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    public class GedcomGraph
    {
        private GedcomRecord record;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomGraph"/> class.
        /// </summary>
        public GedcomGraph()
        {
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public GedcomDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        public GedcomRecord Record
        {
            get
            {
                return record;
            }

            set
            {
                record = value;
                Create();
            }
        }

        /// <summary>
        /// Gets the root.
        /// </summary>
        internal GraphNode<GedcomIndividualRecord> Root { get; private set; }

        /// <summary>
        /// Determines whether the passed person is related to this instance.
        /// </summary>
        /// <param name="relation">The relation.</param>
        /// <returns>
        ///   <c>true</c> if the specified relation is related; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRelated(GedcomIndividualRecord relation)
        {
            bool ret = false;

            List<GraphNode<GedcomIndividualRecord>> visited = new List<GraphNode<GedcomIndividualRecord>>();

            if (Root != null)
            {
                ret = IsRelated(Root, visited, relation);
            }

            return ret;
        }

        private GraphNode<GedcomIndividualRecord> CreateNode(GedcomIndividualRecord indi, GraphType type)
        {
            GraphNode<GedcomIndividualRecord> node = new GraphNode<GedcomIndividualRecord>();

            node.Data = indi;

            // System.Console.WriteLine("Create node for: " + indi.XRefID);
            if (type == GraphType.Ancestors || type == GraphType.Siblings)
            {
                foreach (GedcomFamilyLink famLink in indi.ChildIn)
                {
                    GedcomFamilyRecord family = Database[famLink.Family] as GedcomFamilyRecord;

                    if (family != null)
                    {
                        switch (type)
                        {
                            case GraphType.Ancestors:
                                CreateAncestorEdges(node, family);
                                break;
                            case GraphType.Siblings:
                                CreateSiblingEdges(node, family);
                                break;
                            default:
                                System.Diagnostics.Debug.WriteLine("Invalid graph type");
                                break;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Family link points to non family record");
                    }
                }
            }
            else
            {
                foreach (GedcomFamilyLink famLink in indi.SpouseIn)
                {
                    GedcomFamilyRecord family = Database[famLink.Family] as GedcomFamilyRecord;

                    if (family != null)
                    {
                        switch (type)
                        {
                            case GraphType.Decendants:
                                CreateDecendantEdges(node, family);
                                break;
                            default:
                                System.Diagnostics.Debug.WriteLine("Invalid graph type");
                                break;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Family link points to non family record");
                    }
                }
            }

            return node;
        }

        private void CreateAncestorEdges(GraphNode<GedcomIndividualRecord> node, GedcomFamilyRecord family)
        {
            GedcomIndividualRecord husb = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(family.Husband))
            {
                husb = Database[family.Husband] as GedcomIndividualRecord;
                if (husb == null)
                {
                    System.Diagnostics.Debug.WriteLine("Husband points to non individual record");
                }
            }

            if (!string.IsNullOrEmpty(family.Wife))
            {
                wife = Database[family.Wife] as GedcomIndividualRecord;
                if (wife == null)
                {
                    System.Diagnostics.Debug.WriteLine("Wife points to non individual record");
                }
            }

            if (husb != null)
            {
                GraphNode<GedcomIndividualRecord> father = CreateNode(husb, GraphType.Ancestors);
                node.Edges.Add(father);
            }

            if (wife != null)
            {
                GraphNode<GedcomIndividualRecord> mother = CreateNode(wife, GraphType.Ancestors);
                node.Edges.Add(mother);
            }
        }

        private void CreateDecendantEdges(GraphNode<GedcomIndividualRecord> node, GedcomFamilyRecord family)
        {
            GedcomIndividualRecord indi = node.Data;

            foreach (string childID in family.Children)
            {
                // should never happen, best to check anyway
                if (childID != indi.XRefID)
                {
                    GedcomIndividualRecord child = Database[childID] as GedcomIndividualRecord;
                    if (child != null)
                    {
                        GraphNode<GedcomIndividualRecord> decendant = CreateNode(child, GraphType.Decendants);
                        node.Edges.Add(decendant);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("child in family points to non individual record");
                    }
                }
            }
        }

        private void CreateSiblingEdges(GraphNode<GedcomIndividualRecord> node, GedcomFamilyRecord family)
        {
            GedcomIndividualRecord indi = node.Data;

            foreach (string childID in family.Children)
            {
                if (childID != indi.XRefID)
                {
                    GedcomIndividualRecord child = Database[childID] as GedcomIndividualRecord;
                    if (child != null)
                    {
                        GraphNode<GedcomIndividualRecord> sibling = new GraphNode<GedcomIndividualRecord>();
                        sibling.Data = child;
                        node.Edges.Add(sibling);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("child in family points to non individual record");
                    }
                }
            }

            foreach (GraphNode<GedcomIndividualRecord> sibling in node.Edges)
            {
                sibling.Edges.Add(node);
                foreach (GraphNode<GedcomIndividualRecord> sibling2 in node.Edges)
                {
                    if (sibling2 != sibling)
                    {
                        sibling.Edges.Add(sibling2);
                    }
                }
            }
        }

        private void Create()
        {
            var startIndi = record as GedcomIndividualRecord;

            Root = null;

            if (startIndi != null)
            {
                GraphNode<GedcomIndividualRecord> node = CreateNode(startIndi, GraphType.Ancestors);

                Root = node;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Can only create a graph from an individual record");
            }
        }

        private bool IsRelated(GraphNode<GedcomIndividualRecord> node, List<GraphNode<GedcomIndividualRecord>> visited, GedcomIndividualRecord relation)
        {
            bool ret = false;

            visited.Add(node);

            if (node.Data == relation)
            {
                ret = true;
            }
            else
            {
                foreach (GraphNode<GedcomIndividualRecord> edge in node.Edges)
                {
                    if (!visited.Contains(edge))
                    {
                        ret = IsRelated(edge, visited, relation);
                        if (ret)
                        {
                            break;
                        }
                    }
                }
            }

            return ret;
        }
   }
}
