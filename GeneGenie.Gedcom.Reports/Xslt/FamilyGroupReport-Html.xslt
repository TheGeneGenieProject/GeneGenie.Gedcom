<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:output method="html" encoding="utf-8" version="1.0" 
		doctype-public="-//W3C//DTD HTML 4.01//EN"
		doctype-system="http://www.w3.org/TR/html4/strict.dtd"
		indent="yes"
		media-type="text/html" />

	<xsl:param name="xrefId" />

	<xsl:key name="births" match="EventRec[@Type='Birth']" use="Participant/Link[@Target='IndividualRec']/@Ref" />
	<xsl:key name="marriages" match="EventRec[@Type='Marriage']" use="Participant/Link[@Target='IndividualRec']/@Ref" />
	<xsl:key name="deaths" match="EventRec[@Type='Death']" use="Participant/Link[@Target='IndividualRec']/@Ref" />
	<xsl:key name="family" match="FamilyRec" use="Child/Link/@Ref" />
	<xsl:key name="parentIn" match="FamilyRec" use="HusbFath/Link/@Ref|WifeMoth/Link/@Ref" />
	<xsl:key name="individual" match="IndividualRec" use="@Id" />
	<xsl:key name="events" match="EventRec" use="Participant/Link[@Target='IndividualRec']/@Ref" />
	<xsl:key name="sources" match="SourceRec" use="@Id" />

	<xsl:template match="/">
		<xsl:apply-templates select="GEDCOM"/>
	</xsl:template>

	<xsl:template match="GEDCOM">
		<html>
			<head>
				<title>Family Group Sheet - <xsl:apply-templates select="IndividualRec[position() = 1]/IndivName/NamePart[@Type='surname']" mode="title" /></title>
				<meta name="generator" content="{HeaderRec/FileCreation/Product/Name} {HeaderRec/FileCreation/Product/Version}" />
				<style type="text/css">
					th { text-align: left; }
					th, td { vertical-align: top; }
					tr.childrow { border-top: solid 1px black; }
				</style>
			</head>
			<body>
				<xsl:apply-templates select="FamilyRec[@Id = $xrefId]"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="HeaderRec">
	
	</xsl:template>

	<xsl:template match="FamilyRec">
		<xsl:variable name="husband" select="HusbFath/Link[@Target='IndividualRec']/@Ref" />
		<xsl:variable name="wife" select="WifeMoth/Link[@Target='IndividualRec']/@Ref" />
				
		<h1>Family Group Sheet - <xsl:apply-templates select="../IndividualRec[position() = 1]/IndivName/NamePart[@Type='surname']" mode="title" /></h1>
				
		<table>
			<tr>
				<th>Husband:</th>
				<th colspan="3"><xsl:apply-templates select="key('individual', $husband)" mode="title" /></th>
			</tr>
			<tr>
				<th>Born:</th>
				<td><xsl:value-of select="key('births',$husband)/Date"/></td>
				<th>In:</th>
				<td><xsl:value-of select="key('births',$husband)/Place"/></td>
			</tr>
			<tr>
				<th>Died:</th>
				<td><xsl:value-of select="key('deaths',$husband)/Date"/></td>
				<th>In:</th>
				<td><xsl:value-of select="key('deaths',$husband)/Place"/></td>
			</tr>
			
			<tr>
				<th>Wife:</th>
				<th colspan="3"><xsl:apply-templates select="key('individual', $wife)" mode="title" /></th>
			</tr>
			<tr>
				<th>Born:</th>
				<td><xsl:value-of select="key('births',$wife)/Date"/></td>
				<th>In:</th>
				<td><xsl:value-of select="key('births',$wife)/Place"/></td>
			</tr>
			<tr>
				<th>Died:</th>
				<td><xsl:value-of select="key('deaths',$wife)/Date"/></td>
				<th>In:</th>
				<td><xsl:value-of select="key('deaths',$wife)/Place"/></td>
			</tr>
		</table>
		
		<table>
			<tr>
				<th>&#160;</th>
				<th coslpan="4">Children</th>
			</tr>
			<xsl:for-each select="Child/Link/@Ref">
				<xsl:call-template name="ChildList">
					<xsl:with-param name="no" select="position()"/>
				</xsl:call-template>
			</xsl:for-each>
		</table>
		
		<xsl:apply-templates select="key('individual', $husband)" />
		<xsl:apply-templates select="key('individual', $wife)" />
		<xsl:for-each select="Child/Link/@Ref">
			<xsl:apply-templates select="key('individual', .)" />
		</xsl:for-each>
		
	</xsl:template>

	<xsl:template name="ChildList">
		<xsl:param name="no" />
		<xsl:param name="indi" select="key('individual', .)" />
		<xsl:variable name="indiId" select="." />
		<tr class="childrow">
			<td><xsl:value-of select="$no"/></td>
			<th>Name</th>
			<td colspan="3"><xsl:apply-templates select="$indi" mode="title"/></td>
		</tr>
		<xsl:for-each select="key('events', .)">
			<xsl:sort select="@Type" />
			<xsl:if test="@Type = 'Birth' or @Type = 'Death' or @Type = 'Marriage'">
				<tr>
					<td>
						<xsl:choose>
							<xsl:when test="position() = 1">
								<xsl:value-of select="substring($indi/Gender,1,1)"/>
							</xsl:when>
							<xsl:otherwise>&#160;</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<xsl:choose>
							<xsl:when test="@Type = 'Birth'">
								Born:
							</xsl:when>
							<xsl:when test="@Type = 'Death'">
								Died:
							</xsl:when>
							<xsl:otherwise>
								Married:
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<xsl:value-of select="Date"/>
					</td>
					<th>In:</th>
					<td>
						<xsl:value-of select="Place"/>
					</td>
				</tr>
							
				<xsl:if test="@Type = 'Marriage'">
					<tr>
						<td>&#160;</td>
						<th>Spouse:</th>
						<td colspan="3">
							<xsl:variable name="spouseId" select="Participant/Link[@Ref != $indiId]/@Ref"/>
							<xsl:apply-templates select="key('individual', $spouseId)" mode="title" />
						</td>
					</tr>
				</xsl:if>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="IndividualRec" mode="title">
		<xsl:value-of select="IndivName/NamePart[@Type='given name']" />
		<xsl:text>&#160;</xsl:text>
		<xsl:value-of select="IndivName/NamePart[@Type='surname']" />
	</xsl:template>

	<xsl:template match="IndividualRec">
		<xsl:variable name="indiId" select="@Id"/>
		<xsl:variable name="family" select="../FamilyRec[@Id = $xrefId]"/>
		<xsl:variable name="fatherId" select="$family/HusbFath/Link[@Target='IndividualRec']/@Ref" />
		<xsl:variable name="motherId" select="$family/WifeMoth/Link[@Target='IndividualRec']/@Ref" />
				
		<xsl:variable name="childIn" select="key('family', @Id)"/>
				
		<xsl:variable name="indi" select="."/>
				
		<h1>Family Group Sheet - <xsl:apply-templates select="../IndividualRec[position() = 1]/IndivName/NamePart[@Type='surname']" mode="title" /></h1>
				
		<table>
			<tr>
				<th>
					<xsl:choose>
						<xsl:when test="$indiId = $fatherId">
							Husband
						</xsl:when>
						<xsl:when test="$indiId = $motherId">
							Wife
						</xsl:when>
						<xsl:otherwise>
							Child
						</xsl:otherwise>
					</xsl:choose>
				</th>
				<td>
					<xsl:apply-templates select="." mode="title"/>
				</td>
			</tr>
			
			<xsl:for-each select="key('births', $indiId)">
				<xsl:apply-templates select="." />
			</xsl:for-each>
		
			<xsl:for-each select="key('deaths', $indiId)">
				<xsl:sort select="@Type" />
				<xsl:apply-templates select="." />
			</xsl:for-each>
		
			<tr>
				<th>Reference Number:</th>
				<td><xsl:value-of select="UserReferenceNumber"/></td>
			</tr>
		
			<tr>
				<th>Relationship with Father</th>
				<td>
					<xsl:variable name="indiFather" select="$childIn/HusbFath/Link[@Target='IndividualRec']/@Ref"/>
					<xsl:apply-templates select="key('individual', $indiFather)" mode="title" />
					<xsl:text>&#160;-&#160;</xsl:text>
					<xsl:value-of select="$childIn/Child/Link[@Ref = $indiId]/RelToFath"/>
				</td>
			</tr>
			<tr>
				<th>Relationship with Mother</th>
				<td>
					<xsl:variable name="indiMother" select="$childIn/WifeMoth/Link[@Target='IndividualRec']/@Ref"/>
					<xsl:apply-templates select="key('individual', $indiMother)" mode="title" />
					<xsl:text>&#160;-&#160;</xsl:text>
					<xsl:value-of select="$childIn/Child/Link[@Ref = $indiId]/RelToMoth"/>
				</td>
			</tr>

			<xsl:for-each select="key('events', $indiId)">
				<xsl:sort select="@Type" />
				<xsl:if test="@Type != 'Birth' and @Type != 'Death' and @Type != 'Marriage'">
					<xsl:apply-templates select="." />
				</xsl:if>
			</xsl:for-each>

			<tr>
				<th>Address and Phone(s)</th>
				<td>
					<!-- FIXME insert addresses / phones -->
				</td>
			</tr>
		
			<tr>
				<th>Medical</th>
				<td>
					<!-- FIXME insert medical -->
				</td>
			</tr>
		
		
			<tr>
				<th colspan="2">Notes</th>
			</tr>
			<xsl:for-each select="Note">
				<tr>
					<td colspan="2">
						<xsl:value-of select="."/>
					</td>
				</tr>
			</xsl:for-each>
			
			<xsl:for-each select="key('marriages', $indiId)">
				<xsl:variable name="spouseId" select="Participant/Link[@Ref != $indiId]/@Ref"/>
				<tr>
					<th colspan="2">
						Marriage Information
					</th>
				</tr>
				<tr>
					<th>Spouse:</th>
					<td><xsl:apply-templates select="key('individual', $spouseId)" mode="title" /></td>
				</tr>
				<tr>
					<th>Beginning status:</th>
					<td>?</td>
				</tr>
				<tr>
					<th>Reference Number:</th>
					<td><xsl:value-of select="$indi/UserReferenceNumber"/></td>
				</tr>
				<tr>
					<th colspan="2">
						Marriage Notes
					</th>
				</tr>
				<xsl:for-each select="Note">
					<tr>
						<td colspan="2">
							<xsl:value-of select="."/>
						</td>
					</tr>
				</xsl:for-each>	
			</xsl:for-each>
			
		</table>
		


		
	</xsl:template>
	
	<xsl:template match="EventRec">
		<tr>
			<th><xsl:value-of select="@Type"/></th>
			<td><xsl:value-of select="Date"/></td>
		</tr>
		<tr>
			<th>In:</th>
			<td><xsl:value-of select="Place"/></td>
		</tr>
		<tr>
			<th>Source:</th>
			<td>
				<xsl:variable name="sourceId" select="Evidence/Citation/Source/Link[@Target='SourceRec']/@Ref"/>
				<xsl:apply-templates select="key('sources', $sourceId)" />
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="SourceRec">
		<xsl:value-of select="Title" />
	</xsl:template>
	
</xsl:stylesheet>

