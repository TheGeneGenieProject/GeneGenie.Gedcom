// <copyright file="GedcomReaderWarningCode.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Enums;

/// <summary>
/// The kind of problem a <see cref="Parser.GedcomReaderWarning"/> reports, so that code reading
/// <see cref="Parser.GedcomRecordReader.Warnings"/> can act on some kinds and ignore others without
/// matching the message text.
/// </summary>
public enum GedcomReaderWarningCode
{
    /// <summary>
    /// A record used an id that another record already had, so the reader gave it a new id. Both records
    /// are kept and saved. Links whose tag names the record's kind still find it; others find the first
    /// record with the id.
    /// </summary>
    DuplicateXref,

    /// <summary>
    /// A top-level record had no id, as in 0 _PLAC rather than 0 @P1@ _PLAC, so nothing could point to it
    /// and it wasn't read.
    /// </summary>
    RecordWithoutXref,

    /// <summary>
    /// A tag the reader doesn't read where it is, so it and the lines under it were dropped. It may be a
    /// misspelling or a tag the specification doesn't have, or a valid tag the library doesn't read yet,
    /// such as the LDS ordinances on a person. Custom tags, which start with an underscore, are kept, so
    /// they aren't reported.
    /// </summary>
    UnknownTag,

    /// <summary>
    /// A tag's value wasn't one the GEDCOM specification allows, so the reader ignored it or used a
    /// default, as the message says.
    /// </summary>
    InvalidValue,

    /// <summary>
    /// A link pointed to an id that no record in the file has.
    /// </summary>
    MissingRecord,

    /// <summary>
    /// A link pointed to a record of the wrong kind, such as a family's HUSB pointing to a source, so the
    /// reader didn't follow it.
    /// </summary>
    WrongKindOfRecord,

    /// <summary>
    /// A note record had no text, so it was dropped, along with the links to it.
    /// </summary>
    EmptyNoteDropped,

    /// <summary>
    /// An individual had an ADDR line directly under it, as Family Tree Maker writes, so the address was
    /// read as a residence (RESI) event.
    /// </summary>
    AddressReadAsResidence,

    /// <summary>
    /// A line couldn't be parsed, such as one with an id longer than GEDCOM allows. The file fails to load.
    /// </summary>
    ParseError,
}
