<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:output method="html" encoding="utf-8" version="1.0" 
		doctype-public="-//W3C//DTD HTML 4.01//EN"
		doctype-system="http://www.w3.org/TR/html4/strict.dtd"
		indent="yes"
		media-type="text/html" />

	<xsl:template match="/">
		<xsl:apply-templates select="GEDCOM"/>
	</xsl:template>

	<xsl:template match="GEDCOM">
		<html>
			<head>
				<title>Summary of <xsl:apply-templates select="IndividualRec[position() = 1]" mode="title" /></title>
				<meta name="generator" content="{HeaderRec/FileCreation/Product/Name} {HeaderRec/FileCreation/Product/Version}" />
				<style type="text/css">
					th { text-align: left; }
					th, td { vertical-align: top; }
				</style>
			</head>
			<body>
				<xsl:apply-templates select="IndividualRec[position() = 1]"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="HeaderRec">
	
	</xsl:template>

	<xsl:template match="IndividualRec" mode="title">
		<xsl:value-of select="IndivName/NamePart[@Type='given name']" />
		<xsl:text>&#160;</xsl:text>
		<xsl:value-of select="IndivName/NamePart[@Type='surname']" />
	</xsl:template>

	<xsl:template match="IndividualRec">
		<xsl:variable name="indiId" select="@Id"/>
		<xsl:variable name="family" select="../FamilyRec[Child/Link[@Ref=$indiId]]"/>
		<xsl:variable name="fatherId" select="$family/HusbFath/Link[@Target='IndividualRec']/@Ref" />
		<xsl:variable name="motherId" select="$family/WifeMoth/Link[@Target='IndividualRec']/@Ref" />
				
		<h1>Summary of <xsl:apply-templates select="." mode="title"/></h1>
		
		<table>
			<tr>
				<th>Name:</th><td><xsl:apply-templates select="." mode="title"/></td>
			</tr>
			<tr>
				<th>Gender:</th><td><xsl:value-of select="Gender"/></td>
			</tr>
			<tr>
				<th>Father:</th><td><xsl:apply-templates select="../IndividualRec[@Id=$fatherId]" mode="title" /></td>
			</tr>
			<tr>
				<th>Mother:</th><td><xsl:apply-templates select="../IndividualRec[@Id=$motherId]" mode="title" /></td>
			</tr>
		</table>
		
		<h2>Individual Facts</h2>
		<xsl:if test="count(../EventRec[Participant/Link[@Target='IndividualRec' and @Ref=$indiId]]) &gt; 0">
			<table>
			<xsl:apply-templates select="../EventRec[Participant/Link[@Target='IndividualRec' and @Ref=$indiId] and @Type != 'Marriage']" mode="facts" />
			</table>
		</xsl:if>
				  
		<h2>Marriages / Children</h2>
		<xsl:if test="count(../EventRec[Participant/Link[@Target='IndividualRec' and @Ref=$indiId] and @Type='Marriage']) &gt; 0">
			<table>
			<xsl:apply-templates select="../EventRec[Participant/Link[@Target='IndividualRec' and @Ref=$indiId] and @Type = 'Marriage']" mode="marriages">
				<xsl:with-param name="indiId" select="$indiId"/>
			</xsl:apply-templates>
			</table>
		</xsl:if>
		
	</xsl:template>
	
	<xsl:template match="EventRec" mode="facts">
		<xsl:variable name="eventType" select="@Type" />

		<xsl:if test="$eventType != 'Marriage'">
			<tr>
				<th><xsl:value-of select="$eventType" /></th>
				<td>
					<xsl:value-of select="Date" />
					in
					<xsl:value-of select="Place" />
				</td>
			</tr>
		</xsl:if>
		
	</xsl:template>

	<xsl:template match="EventRec" mode="marriages">
		<xsl:param name="indiId" />
		
		<xsl:variable name="eventType" select="@Type" />

		<xsl:if test="$eventType = 'Marriage'">
			<xsl:variable name="spouseId" select="Participant/Link[@Target='IndividualRec' and @Ref != $indiId]/@Ref" />
			<tr>
				<th colspan="2">
					<xsl:apply-templates select="../IndividualRec[@Id = $spouseId]" mode="title" />
				</th>
			</tr>
			<tr>
				<th><xsl:value-of select="$eventType" /></th>
				<td>
					<xsl:value-of select="Date" />
					in
					<xsl:value-of select="Place" />.
					Marriage of <xsl:apply-templates select="../IndividualRec[@Id = $indiId]" mode="title" />
					and <xsl:apply-templates select="../IndividualRec[@Id = $spouseId]" mode="title" /> 
				</td>
			</tr>
			<tr>
				<th>Children</th>
				<td>
					<xsl:for-each select="../FamilyRec[HusbFath/Link[@Target='IndividualRec' and @Ref=$indiId] and WifeMoth/Link[@Target='IndividualRec' and @Ref=$spouseId]]/Child/Link[@Target='IndividualRec']">
						<xsl:variable name="childId" select="@Ref" />
						<xsl:apply-templates select="../../../IndividualRec[@Id = $childId]" mode="title" />
						<xsl:if test="position() != last()">
							<br />
						</xsl:if>
					</xsl:for-each>
					<xsl:for-each select="../FamilyRec[HusbFath/Link[@Target='IndividualRec' and @Ref=$spouseId] and WifeMoth/Link[@Target='IndividualRec' and @Ref=$indiId]]/Child/Link[@Target='IndividualRec']">
						<xsl:variable name="childId" select="@Ref" />
						<xsl:apply-templates select="../../../IndividualRec[@Id = $childId]" mode="title" />
						<xsl:if test="position() != last()">
							<br />
						</xsl:if>
					</xsl:for-each>
				</td>
			</tr>
		</xsl:if>
		
	</xsl:template>
	
</xsl:stylesheet>

