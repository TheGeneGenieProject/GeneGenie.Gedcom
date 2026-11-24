// <copyright file="GedcomWebLink.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// A web link on a person, such as their Find a Grave page, read from the _WLNK tag Ancestry writes
/// (issue #166). See <see cref="GedcomIndividualRecord.WebLinks"/>.
/// </summary>
/// <param name="Title">The link's title, from the TITL line under _WLNK, or null if it has none.</param>
/// <param name="Url">The web address, from the NOTE under _WLNK, or null if it has none.</param>
public record GedcomWebLink(string Title, string Url);
