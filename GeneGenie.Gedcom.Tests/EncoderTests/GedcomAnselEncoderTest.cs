// <copyright file="GedcomAnselEncoderTest.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Tests.EncoderTests
{
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using GeneGenie.Gedcom.Parser.Enums;
    using Xunit;

    /// <summary>
    /// Rigorous tests to ensure that Ansel files can be can be properly loaded and converted to Unicode.
    /// </summary>
    public class GedcomAnselEncoderTest
    {
        /// <summary>
        /// Checks that the Ansel encoding is recognised.
        /// </summary>
        [Fact]
        public void Ansel_charset_is_recognised()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Ansel.ged");

            Assert.Equal(GedcomCharset.Ansel, reader.Parser.Charset);
        }

        /// <summary>
        /// Test to catch the addition of child records so that we can add tests for them.
        /// If this fails, we need to test the characters in the new entries.
        /// </summary>
        [Fact]
        public void Record_count_is_33_children_plus_two_parents()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Ansel.ged");

            Assert.Equal(35, reader.Database.Individuals.Count);
        }

        /// <summary>
        /// Uses the Ansel test file supplied by Heiner Eichmann to test every character in Ansel can be decoded into Unicode.
        /// The expected test data has been rigorously checked to ensure the Unicode characters match exactly. If your editor
        /// is not Unicode aware updates them, they'll probably fail.
        /// </summary>
        /// <param name="childName">The name of the child test record in the Ansel.ged file.</param>
        /// <param name="birthPlace">The expected text to find in the birth place field, after translation to Unicode.</param>
        /// <param name="deathPlace">The expected text to find in the death place field, after translation to Unicode.</param>
        [Theory]
        [InlineData("/Special Characters 0/", "slash l - uppercase (Ł), slash o - uppercase (Ø), slash d - uppercase (Ð), thorn - uppercase (Þ)", "ligature ae - uppercase (Æ), ligature oe - uppercase (Œ), miagkii znak (ʹ), middle dot (·), musical flat (♭)")]
        [InlineData("/Special Characters 1/", "patent mark (®), plus-or-minus (±), hook o - uppercase (Ơ), hook u - uppercase (Ư)", "alif (ʾ), ayn (ʿ), slash l - lowercase (ł), slash o - lowercase (ø), slash d - lowercase (đ)")]
        [InlineData("/Special Characters 2/", "thorn - lowercase (þ), ligature ae - lowercase (æ), ligature oe - lowercase (œ), tverdyi znak (ʺ)", "dotless i - lowercase (ı), british pound (£), eth (ð), hook o - lowercase (ơ), hook u - lowercase (ư)")]
        [InlineData("/Special Characters 3/", "degree sign (°), script l (ℓ), phonograph copyright mark (℗), copyright symbol (©)", "musical sharp (♯), inverted question mark (¿), inverted exclamation mark (¡), es zet (ß)")]
        [InlineData("code: E0 (Unicode: hook above, 0309) /low rising tone mark/", "ẢB̉C̉D̉ẺF̉G̉H̉ỈJ̉K̉L̉M̉N̉ỎP̉Q̉R̉S̉T̉ỦV̉W̉X̉ỶZ̉", "ảb̉c̉d̉ẻf̉g̉h̉ỉj̉k̉l̉m̉n̉ỏp̉q̉r̉s̉t̉ủv̉w̉x̉ỷz̉")]
        [InlineData("code: E1 (Unicode: grave, 0300) /grave accent/", "ÀB̀C̀D̀ÈF̀G̀H̀ÌJ̀K̀L̀M̀ǸÒP̀Q̀R̀S̀T̀ÙV̀ẀX̀ỲZ̀", "àb̀c̀d̀èf̀g̀h̀ìj̀k̀l̀m̀ǹòp̀q̀r̀s̀t̀ùv̀ẁx̀ỳz̀")]
        [InlineData("code: E2 (Unicode: acute, 0301) /acute accent/", "ÁB́ĆD́ÉF́ǴH́ÍJ́ḰĹḾŃÓṔQ́ŔŚT́ÚV́ẂX́ÝŹ", "áb́ćd́éf́ǵh́íj́ḱĺḿńóṕq́ŕśt́úv́ẃx́ýź")]
        [InlineData("code: E3 (Unicode: circumflex, 0302) /circumflex accent/", "ÂB̂ĈD̂ÊF̂ĜĤÎĴK̂L̂M̂N̂ÔP̂Q̂R̂ŜT̂ÛV̂ŴX̂ŶẐ", "âb̂ĉd̂êf̂ĝĥîĵk̂l̂m̂n̂ôp̂q̂r̂ŝt̂ûv̂ŵx̂ŷẑ")]
        [InlineData("code: E4 (Unicode: tilde, 0303) /tilde/", "ÃB̃C̃D̃ẼF̃G̃H̃ĨJ̃K̃L̃M̃ÑÕP̃Q̃R̃S̃T̃ŨṼW̃X̃ỸZ̃", "ãb̃c̃d̃ẽf̃g̃h̃ĩj̃k̃l̃m̃ñõp̃q̃r̃s̃t̃ũṽw̃x̃ỹz̃")]
        [InlineData("code: E5 (Unicode: macron, 0304) /macron/", "ĀB̄C̄D̄ĒF̄ḠH̄ĪJ̄K̄L̄M̄N̄ŌP̄Q̄R̄S̄T̄ŪV̄W̄X̄ȲZ̄", "āb̄c̄d̄ēf̄ḡh̄īj̄k̄l̄m̄n̄ōp̄q̄r̄s̄t̄ūv̄w̄x̄ȳz̄")]
        [InlineData("code: E6 (Unicode: breve, 0306) /breve/", "ĂB̆C̆D̆ĔF̆ĞH̆ĬJ̆K̆L̆M̆N̆ŎP̆Q̆R̆S̆T̆ŬV̆W̆X̆Y̆Z̆", "ăb̆c̆d̆ĕf̆ğh̆ĭj̆k̆l̆m̆n̆ŏp̆q̆r̆s̆t̆ŭv̆w̆x̆y̆z̆")]
        [InlineData("code: E7 (Unicode: dot above, 0307) /dot above/", "ȦḂĊḊĖḞĠḢİJ̇K̇L̇ṀṄȮṖQ̇ṘṠṪU̇V̇ẆẊẎŻ", "ȧḃċḋėḟġḣi̇j̇k̇l̇ṁṅȯṗq̇ṙṡṫu̇v̇ẇẋẏż")]
        [InlineData("code: E8 (Unicode: diaeresis, 0308) /umlaut (dieresis)/", "ÄB̈C̈D̈ËF̈G̈ḦÏJ̈K̈L̈M̈N̈ÖP̈Q̈R̈S̈T̈ÜV̈ẄẌŸZ̈", "äb̈c̈d̈ëf̈g̈ḧïj̈k̈l̈m̈n̈öp̈q̈r̈s̈ẗüv̈ẅẍÿz̈")]
        [InlineData("code: E9 (Unicode: caron, 030C) /hacek/", "ǍB̌ČĎĚF̌ǦȞǏJ̌ǨĽM̌ŇǑP̌Q̌ŘŠŤǓV̌W̌X̌Y̌Ž", "ǎb̌čďěf̌ǧȟǐǰǩľm̌ňǒp̌q̌řšťǔv̌w̌x̌y̌ž")]
        [InlineData("code: EA (Unicode: ring above, 030A) /circle above (angstrom)/", "ÅB̊C̊D̊E̊F̊G̊H̊I̊J̊K̊L̊M̊N̊O̊P̊Q̊R̊S̊T̊ŮV̊W̊X̊Y̊Z̊", "åb̊c̊d̊e̊f̊g̊h̊i̊j̊k̊l̊m̊n̊o̊p̊q̊r̊s̊t̊ův̊ẘx̊ẙz̊")]
        [InlineData("code: EB (Unicode: ligature left half, FE20) /ligature, left half/", "A︠B︠C︠D︠E︠F︠G︠H︠I︠J︠K︠L︠M︠N︠O︠P︠Q︠R︠S︠T︠U︠V︠W︠X︠Y︠Z︠", "a︠b︠c︠d︠e︠f︠g︠h︠i︠j︠k︠l︠m︠n︠o︠p︠q︠r︠s︠t︠u︠v︠w︠x︠y︠z︠")]
        [InlineData("code: EC (Unicode: ligature right half, FE21) /ligature, right half/", "A︡B︡C︡D︡E︡F︡G︡H︡I︡J︡K︡L︡M︡N︡O︡P︡Q︡R︡S︡T︡U︡V︡W︡X︡Y︡Z︡", "a︡b︡c︡d︡e︡f︡g︡h︡i︡j︡k︡l︡m︡n︡o︡p︡q︡r︡s︡t︡u︡v︡w︡x︡y︡z︡")]
        [InlineData("code: ED (Unicode: comma above right, 0315) /high comma, off center/", "A̕B̕C̕D̕E̕F̕G̕H̕I̕J̕K̕L̕M̕N̕O̕P̕Q̕R̕S̕T̕U̕V̕W̕X̕Y̕Z̕", "a̕b̕c̕d̕e̕f̕g̕h̕i̕j̕k̕l̕m̕n̕o̕p̕q̕r̕s̕t̕u̕v̕w̕x̕y̕z̕")]
        [InlineData("code: EE (Unicode: double acute, 030B) /double acute accent/", "A̋B̋C̋D̋E̋F̋G̋H̋I̋J̋K̋L̋M̋N̋ŐP̋Q̋R̋S̋T̋ŰV̋W̋X̋Y̋Z̋", "a̋b̋c̋d̋e̋f̋g̋h̋i̋j̋k̋l̋m̋n̋őp̋q̋r̋s̋t̋űv̋w̋x̋y̋z̋")]
        [InlineData("code: EF (Unicode: candrabindu, 0310) /candrabindu/", "A̐B̐C̐D̐E̐F̐G̐H̐I̐J̐K̐L̐M̐N̐O̐P̐Q̐R̐S̐T̐U̐V̐W̐X̐Y̐Z̐", "a̐b̐c̐d̐e̐f̐g̐h̐i̐j̐k̐l̐m̐n̐o̐p̐q̐r̐s̐t̐u̐v̐w̐x̐y̐z̐")]
        [InlineData("code: F0 (Unicode: cedilla, 0327) /cedilla/", "A̧B̧ÇḐȨF̧ĢḨI̧J̧ĶĻM̧ŅO̧P̧Q̧ŖŞŢU̧V̧W̧X̧Y̧Z̧", "a̧b̧çḑȩf̧ģḩi̧j̧ķļm̧ņo̧p̧q̧ŗşţu̧v̧w̧x̧y̧z̧")]
        [InlineData("code: F1 (Unicode: ogonek, 0328) /right hook/", "ĄB̨C̨D̨ĘF̨G̨H̨ĮJ̨K̨L̨M̨N̨ǪP̨Q̨R̨S̨T̨ŲV̨W̨X̨Y̨Z̨", "ąb̨c̨d̨ęf̨g̨h̨įj̨k̨l̨m̨n̨ǫp̨q̨r̨s̨t̨ųv̨w̨x̨y̨z̨")]
        [InlineData("code: F2 (Unicode: dot below, 0323) /dot below/", "ẠḄC̣ḌẸF̣G̣ḤỊJ̣ḲḶṂṆỌP̣Q̣ṚṢṬỤṾẈX̣ỴẒ", "ạḅc̣ḍẹf̣g̣ḥịj̣ḳḷṃṇọp̣q̣ṛṣṭụṿẉx̣ỵẓ")]
        [InlineData("code: F3 (Unicode: diaeresis below, 0324) /double dot below/", "A̤B̤C̤D̤E̤F̤G̤H̤I̤J̤K̤L̤M̤N̤O̤P̤Q̤R̤S̤T̤ṲV̤W̤X̤Y̤Z̤", "a̤b̤c̤d̤e̤f̤g̤h̤i̤j̤k̤l̤m̤n̤o̤p̤q̤r̤s̤t̤ṳv̤w̤x̤y̤z̤")]
        [InlineData("code: F4 (Unicode: ring below, 0325) /circle below/", "ḀB̥C̥D̥E̥F̥G̥H̥I̥J̥K̥L̥M̥N̥O̥P̥Q̥R̥S̥T̥U̥V̥W̥X̥Y̥Z̥", "ḁb̥c̥d̥e̥f̥g̥h̥i̥j̥k̥l̥m̥n̥o̥p̥q̥r̥s̥t̥u̥v̥w̥x̥y̥z̥")]
        [InlineData("code: F5 (Unicode: double low line, 0333) /double underscore/", "A̳B̳C̳D̳E̳F̳G̳H̳I̳J̳K̳L̳M̳N̳O̳P̳Q̳R̳S̳T̳U̳V̳W̳X̳Y̳Z̳", "a̳b̳c̳d̳e̳f̳g̳h̳i̳j̳k̳l̳m̳n̳o̳p̳q̳r̳s̳t̳u̳v̳w̳x̳y̳z̳")]
        [InlineData("code: F6 (Unicode: line below, 0332) /underscore/", "A̲B̲C̲D̲E̲F̲G̲H̲I̲J̲K̲L̲M̲N̲O̲P̲Q̲R̲S̲T̲U̲V̲W̲X̲Y̲Z̲", "a̲b̲c̲d̲e̲f̲g̲h̲i̲j̲k̲l̲m̲n̲o̲p̲q̲r̲s̲t̲u̲v̲w̲x̲y̲z̲")]
        [InlineData("code: F7 (Unicode: comma below, 0326) /left hook/", "A̦B̦C̦D̦E̦F̦G̦H̦I̦J̦K̦L̦M̦N̦O̦P̦Q̦R̦ȘȚU̦V̦W̦X̦Y̦Z̦", "a̦b̦c̦d̦e̦f̦g̦h̦i̦j̦k̦l̦m̦n̦o̦p̦q̦r̦șțu̦v̦w̦x̦y̦z̦")]
        [InlineData("code: F8 (Unicode: left half ring below, 031C) /right cedilla/", "A̜B̜C̜D̜E̜F̜G̜H̜I̜J̜K̜L̜M̜N̜O̜P̜Q̜R̜S̜T̜U̜V̜W̜X̜Y̜Z̜", "a̜b̜c̜d̜e̜f̜g̜h̜i̜j̜k̜l̜m̜n̜o̜p̜q̜r̜s̜t̜u̜v̜w̜x̜y̜z̜")]
        [InlineData("code: F9 (Unicode: breve below, 032E) /half circle below/", "A̮B̮C̮D̮E̮F̮G̮ḪI̮J̮K̮L̮M̮N̮O̮P̮Q̮R̮S̮T̮U̮V̮W̮X̮Y̮Z̮", "a̮b̮c̮d̮e̮f̮g̮ḫi̮j̮k̮l̮m̮n̮o̮p̮q̮r̮s̮t̮u̮v̮w̮x̮y̮z̮")]
        [InlineData("code: FA (Unicode: double tilde left half, FE22) /double tilde, left half/", "A︢B︢C︢D︢E︢F︢G︢H︢I︢J︢K︢L︢M︢N︢O︢P︢Q︢R︢S︢T︢U︢V︢W︢X︢Y︢Z︢", "a︢b︢c︢d︢e︢f︢g︢h︢i︢j︢k︢l︢m︢n︢o︢p︢q︢r︢s︢t︢u︢v︢w︢x︢y︢z︢")]
        [InlineData("code: FB (Unicode: double tilde right half, FE23) /double tilde, right half/", "A︣B︣C︣D︣E︣F︣G︣H︣I︣J︣K︣L︣M︣N︣O︣P︣Q︣R︣S︣T︣U︣V︣W︣X︣Y︣Z︣", "a︣b︣c︣d︣e︣f︣g︣h︣i︣j︣k︣l︣m︣n︣o︣p︣q︣r︣s︣t︣u︣v︣w︣x︣y︣z︣")]
        [InlineData("code: FE (Unicode: comma above, 0313) /high comma, centered/", "A̓B̓C̓D̓E̓F̓G̓H̓I̓J̓K̓L̓M̓N̓O̓P̓Q̓R̓S̓T̓U̓V̓W̓X̓Y̓Z̓", "a̓b̓c̓d̓e̓f̓g̓h̓i̓j̓k̓l̓m̓n̓o̓p̓q̓r̓s̓t̓u̓v̓w̓x̓y̓z̓")]
        public void Characters_can_be_translated_to_unicode(string childName, string birthPlace, string deathPlace)
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Ansel.ged");

            var child = reader.Database.Individuals.First(i => i.GetName().Name == childName);

            Assert.Equal(birthPlace, child.Birth.Place.Name);
            Assert.Equal(deathPlace, child.Death.Place.Name);
        }
    }
}
