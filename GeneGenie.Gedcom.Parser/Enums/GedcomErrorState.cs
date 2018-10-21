// <copyright file="GedcomErrorState.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser.Enums
{
    /// <summary>
    /// Defines the current error status the parser is in
    /// </summary>
    public enum GedcomErrorState
    {
        /// <summary>
        /// No error has occured
        /// </summary>
        NoError = 0,

        /// <summary>
        /// A level value was expected but not found
        /// </summary>
        LevelExpected,

        /// <summary>
        /// Delimeter after level not found
        /// </summary>
        LevelMissingDelim,

        /// <summary>
        /// The level value is invalid
        /// </summary>
        LevelInvalid,

        /// <summary>
        /// Delimeter after XrefID not found
        /// </summary>
        XrefIDMissingDelim,

        /// <summary>
        /// The ID is too long, can be at most 22 characters
        /// </summary>
        XrefIDTooLong,

        /// <summary>
        /// A GEDCOM tag name (or custom tag name) was expected but not found
        /// </summary>
        TagExpected,

        /// <summary>
        /// Delimeter, or newline after the tag was not found
        /// </summary>
        TagMissingDelimOrTerm,

        /// <summary>
        /// Value expected but not found
        /// </summary>
        LineValueExpected,

        /// <summary>
        /// newline after line value not found
        /// </summary>
        LineValueMissingTerm,

        /// <summary>
        /// The line value is invalid
        /// </summary>
        LineValueInvalid,

        /// <summary>
        /// Deliminator in GEDCOM is a single space, this error will occur
        /// when a multi space delimiter is detected
        /// </summary>
        InvalidDelim,

        /// <summary>
        /// An unknown error has occured while parsing
        /// </summary>
        UnknownError,
    }
}
