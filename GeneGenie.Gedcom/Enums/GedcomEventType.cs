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
    /// GEDCOM Event Types
    /// </summary>
    public enum GedcomEventType
    {
        /// <summary>
        /// Generic Event
        /// </summary>
        GenericEvent = 0,

        // Family Events

        /// <summary>
        /// Annulment
        /// </summary>
        /// <remarks>
        /// Declaring a marriage void from the beginning (never existed).
        /// </remarks>
        ANUL,

        /// <summary>
        /// Census
        /// </summary>
        /// <remarks>
        /// The event of the periodic count of the population for a designated locality,
        /// such as a national or state Census.
        /// </remarks>
        CENS_FAM,

        /// <summary>
        /// Divorce
        /// </summary>
        /// <remarks>
        /// An event of dissolving a marriage through civil action.
        /// </remarks>
        DIV,

        /// <summary>
        /// Divorce Filed
        /// </summary>
        /// <remarks>
        /// An event of filing for a divorce by a spouse.
        /// </remarks>
        DIVF,

        /// <summary>
        /// Engagement
        /// </summary>
        /// <remarks>
        /// An event of recording or announcing an agreement between two people to become married.
        /// </remarks>
        ENGA,

        /// <summary>
        /// Marriage Bann
        /// </summary>
        /// <remarks>
        /// An event of an official public notice given that two people intend to marry.
        /// </remarks>
        MARB,

        /// <summary>
        /// Marriage Contract
        /// </summary>
        /// <remarks>
        /// An event of recording a formal agreement of marriage, including the prenuptial
        /// agreement in which marriage partners reach agreement about the property rights
        /// of one or both, securing property to their children.
        /// </remarks>
        MARC,

        /// <summary>
        /// Marriage
        /// </summary>
        /// <remarks>
        /// A legal, common-law, or customary event of creating a family unit of a man and a woman as husband and wife.
        /// </remarks>
        MARR,

        /// <summary>
        /// Marriage License
        /// </summary>
        /// <remarks>
        /// An event of obtaining a legal license to marry.
        /// </remarks>
        MARL,

        /// <summary>
        /// Marriage Settlement
        /// </summary>
        /// <remarks>
        /// An event of creating an agreement between two people contemplating marriage, at which time
        /// they agree to release or modify property rights that would otherwise arise from the marriage.
        /// </remarks>
        MARS,

        /// <summary>
        /// Residence
        /// </summary>
        /// <remarks>
        /// An address or place of residence that a family or individual resided.
        /// </remarks>
        RESI,

        // Individual Events

        /// <summary>
        /// Birth
        /// </summary>
        Birth,

        /// <summary>
        /// Christening
        /// </summary>
        CHR,

        /// <summary>
        /// Death
        /// </summary>
        DEAT,

        /// <summary>
        /// Burial
        /// </summary>
        BURI,

        /// <summary>
        /// Cremation
        /// </summary>
        CREM,

        /// <summary>
        /// Adoption
        /// </summary>
        ADOP,

        /// <summary>
        ///Baptism
        /// </summary>
        BAPM,

        /// <summary>
        /// Bar Mitzvah
        /// </summary>
        BARM,

        /// <summary>
        /// Bat Mitzvah
        /// </summary>
        BASM,

        /// <summary>
        /// Blessing
        /// </summary>
        BLES,

        /// <summary>
        /// Adult Christening
        /// </summary>
        CHRA,

        /// <summary>
        /// Confirmation
        /// </summary>
        CONF,

        /// <summary>
        /// First Communion
        /// </summary>
        FCOM,

        /// <summary>
        /// Ordination
        /// </summary>
        ORDN,

        /// <summary>
        /// Naturalization
        /// </summary>
        NATU,

        /// <summary>
        /// Emigration
        /// </summary>
        EMIG,

        /// <summary>
        /// Immigration
        /// </summary>
        IMMI,

        /// <summary>
        /// Census
        /// </summary>
        CENS,

        /// <summary>
        /// Probate
        /// </summary>
        PROB,

        /// <summary>
        /// Will Creation
        /// </summary>
        WILL,

        /// <summary>
        /// Graduation
        /// </summary>
        GRAD,

        /// <summary>
        /// Retirement
        /// </summary>
        RETI,

        // Facts

        /// <summary>
        /// Generic Fact
        /// </summary>
        GenericFact,

        /// <summary>
        /// Caste
        /// </summary>
        CASTFact,

        /// <summary>
        /// Physical Description
        /// </summary>
        DSCRFact,

        /// <summary>
        /// Education
        /// </summary>
        EDUCFact,

        /// <summary>
        /// National ID Number
        /// </summary>
        IDNOFact,

        /// <summary>
        /// National Or Tribal Origin
        /// </summary>
        NATIFact,

        /// <summary>
        /// Number of Children
        /// </summary>
        NCHIFact,

        /// <summary>
        /// Number of Marriages
        /// </summary>
        NMRFact,

        /// <summary>
        /// Occupation
        /// </summary>
        OCCUFact,

        /// <summary>
        /// Possessions
        /// </summary>
        PROPFact,

        /// <summary>
        /// Religion
        /// </summary>
        RELIFact,

        /// <summary>
        /// Residence
        /// </summary>
        RESIFact,

        /// <summary>
        /// Social Security Number
        /// </summary>
        SSNFact,

        /// <summary>
        /// Nobility Type Title
        /// </summary>
        TITLFact,

        // GEDCOM allows custom records, beginning with _

        /// <summary>
        /// Custom
        /// </summary>
        Custom
    }
}
