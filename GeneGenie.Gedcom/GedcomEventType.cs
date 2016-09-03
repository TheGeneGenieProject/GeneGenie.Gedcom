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

namespace GeneGenie.Gedcom
{
    /// <summary>
    /// 
    /// </summary>
    public enum GedcomEventType
    {
        GenericEvent = 0,

        // Family Events
        ANUL,
        CENS_FAM,
        DIV,
        DIVF,
        ENGA,
        MARB,
        MARC,
        MARR,
        MARL,
        MARS,
        RESI,

        // Individual Events
        BIRT,
        CHR,
        DEAT,
        BURI,
        CREM,
        ADOP,
        BAPM,
        BARM,
        BASM,
        BLES,
        CHRA,
        CONF,
        FCOM,
        ORDN,
        NATU,
        EMIG,
        IMMI,
        CENS,
        PROB,
        WILL,
        GRAD,
        RETI,

        // Facts
        GenericFact,
        CASTFact,
        DSCRFact,
        EDUCFact,
        IDNOFact,
        NATIFact,
        NCHIFact,
        NMRFact,
        OCCUFact,
        PROPFact,
        RELIFact,
        RESIFact,
        SSNFact,
        TITLFact,

        // GEDCOM allows custom records, beginging with _
        Custom
    }
}
