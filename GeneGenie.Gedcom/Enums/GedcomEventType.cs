// <copyright file="GedcomEventType.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// TODO: All of these need documenting properly.
    /// </summary>
    public enum GedcomEventType
    {
        /// <summary>
        /// A generic event.
        /// </summary>
        GenericEvent = 0,

        // Family Events

        /// <summary>
        /// The anul
        /// </summary>
        ANUL,

        /// <summary>
        /// The cens fam
        /// </summary>
        CENS_FAM,

        /// <summary>
        /// The div
        /// </summary>
        DIV,

        /// <summary>
        /// The divf
        /// </summary>
        DIVF,

        /// <summary>
        /// The enga
        /// </summary>
        ENGA,

        /// <summary>
        /// The marb
        /// </summary>
        MARB,

        /// <summary>
        /// The marc
        /// </summary>
        MARC,

        /// <summary>
        /// The marr
        /// </summary>
        MARR,

        /// <summary>
        /// The marl
        /// </summary>
        MARL,

        /// <summary>
        /// The mars
        /// </summary>
        MARS,

        /// <summary>
        /// The resi
        /// </summary>
        RESI,

        // Individual Events

        /// <summary>Date of birth.</summary>
        Birth,

        /// <summary>
        /// The character
        /// </summary>
        CHR,

        /// <summary>
        /// The deat
        /// </summary>
        DEAT,

        /// <summary>
        /// The buri
        /// </summary>
        BURI,

        /// <summary>
        /// The crem
        /// </summary>
        CREM,

        /// <summary>
        /// The adop
        /// </summary>
        ADOP,

        /// <summary>
        /// The bapm
        /// </summary>
        BAPM,

        /// <summary>
        /// The barm
        /// </summary>
        BARM,

        /// <summary>
        /// The basm
        /// </summary>
        BASM,

        /// <summary>
        /// The bles
        /// </summary>
        BLES,

        /// <summary>
        /// The chra
        /// </summary>
        CHRA,

        /// <summary>
        /// The conf
        /// </summary>
        CONF,

        /// <summary>
        /// The fcom
        /// </summary>
        FCOM,

        /// <summary>
        /// The ordn
        /// </summary>
        ORDN,

        /// <summary>
        /// The natu
        /// </summary>
        NATU,

        /// <summary>
        /// The emig
        /// </summary>
        EMIG,

        /// <summary>
        /// The immi
        /// </summary>
        IMMI,

        /// <summary>
        /// The cens
        /// </summary>
        CENS,

        /// <summary>
        /// The prob
        /// </summary>
        PROB,

        /// <summary>
        /// The will
        /// </summary>
        WILL,

        /// <summary>
        /// The grad
        /// </summary>
        GRAD,

        /// <summary>
        /// The reti
        /// </summary>
        RETI,

        // Facts

        /// <summary>
        /// The generic fact
        /// </summary>
        GenericFact,

        /// <summary>
        /// The cast fact
        /// </summary>
        CASTFact,

        /// <summary>
        /// The DSCR fact
        /// </summary>
        DSCRFact,

        /// <summary>
        /// The educ fact
        /// </summary>
        EDUCFact,

        /// <summary>
        /// The idno fact
        /// </summary>
        IDNOFact,

        /// <summary>
        /// The nati fact
        /// </summary>
        NATIFact,

        /// <summary>
        /// The nchi fact
        /// </summary>
        NCHIFact,

        /// <summary>
        /// The NMR fact
        /// </summary>
        NMRFact,

        /// <summary>
        /// The occu fact
        /// </summary>
        OCCUFact,

        /// <summary>
        /// The property fact
        /// </summary>
        PROPFact,

        /// <summary>
        /// The reli fact
        /// </summary>
        RELIFact,

        /// <summary>
        /// The resi fact
        /// </summary>
        RESIFact,

        /// <summary>
        /// The SSN fact
        /// </summary>
        SSNFact,

        /// <summary>
        /// The titl fact
        /// </summary>
        TITLFact,

        // GEDCOM allows custom records, beginging with _

        /// <summary>
        /// The custom
        /// </summary>
        Custom
    }
}
