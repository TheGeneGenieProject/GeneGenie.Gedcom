<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:output method="html" encoding="utf-8" version="1.0" 
		doctype-public="-//W3C//DTD HTML 4.01//EN"
		doctype-system="http://www.w3.org/TR/html4/strict.dtd"
		indent="yes"
		media-type="text/html" />

	<xsl:key name="births" match="EventRec[@Type='Birth']" use="Participant/Link[@Target='IndividualRec']/@Ref" />
	<xsl:key name="deaths" match="EventRec[@Type='Death']" use="Participant/Link[@Target='IndividualRec']/@Ref" />
	<xsl:key name="family" match="FamilyRec" use="Child/Link/@Ref" />
	<xsl:key name="parentIn" match="FamilyRec" use="HusbFath/Link/@Ref|WifeMoth/Link/@Ref" />
	<xsl:key name="individual" match="IndividualRec" use="@Id" />

	<xsl:template match="/">
		<xsl:apply-templates select="GEDCOM"/>
	</xsl:template>

	<xsl:template match="GEDCOM">
		<html>
			<head>
				<title>Descendants of <xsl:apply-templates select="IndividualRec[position() = 1]" mode="title" /></title>
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

	<xsl:template match="IndividualRec" mode="birthdeath">
		<xsl:variable name="indiId" select="@Id" />
		<xsl:variable name="birth" select="key('births',$indiId)" />
		<xsl:variable name="death" select="key('deaths',$indiId)" />
		
		<xsl:value-of select="$birth/Date" /> - <xsl:value-of select="$death/Date" /> 
	</xsl:template>

	<xsl:template match="IndividualRec">
		<xsl:variable name="indiId" select="@Id"/>
		<xsl:variable name="family" select="key('family',$indiId)"/>
		<xsl:variable name="fatherId" select="$family/HusbFath/Link[@Target='IndividualRec']/@Ref" />
		<xsl:variable name="motherId" select="$family/WifeMoth/Link[@Target='IndividualRec']/@Ref" />
				
		<h1>Descendants of <xsl:apply-templates select="." mode="title"/></h1>
		
		<xsl:call-template name="decendants">
			<xsl:with-param name="indi" select="." />
			<xsl:with-param name="generation" select='1' />
		</xsl:call-template>
		
	</xsl:template>
	
	<xsl:template name="decendants">
		<xsl:param name="indi" />
		<xsl:param name="generation" />
		
		<ul>
			<li>
				<xsl:value-of select="$generation"/>
				<xsl:text>&#160;</xsl:text>
				<xsl:apply-templates select="$indi" mode="title" />
				<xsl:text>&#160;</xsl:text>
				<xsl:apply-templates select="$indi" mode="birthdeath" />
				<xsl:variable name="indiId" select="$indi/@Id" />
				
				<xsl:for-each select="key('parentIn', $indiId)">
					<br />
					+
					<xsl:choose>
						<xsl:when test="HusbFath/Link[@Target='IndividualRec']/@Ref = $indiId">
							<xsl:variable name="wifeId" select="WifeMoth/Link[@Target='IndividualRec']/@Ref" />
							<xsl:variable name="wife" select="key('individual', $wifeId)"/>
						
							<xsl:apply-templates select="$wife" mode="title" />
							<xsl:text>&#160;</xsl:text>
							<xsl:apply-templates select="$wife" mode="birthdeath" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="husbId" select="HusbFath/Link[@Target='IndividualRec']/@Ref" />
							<xsl:variable name="husb" select="key('individual',$husbId)"/>
						
							<xsl:apply-templates select="$husb" mode="title" />
							<xsl:text>&#160;</xsl:text>
							<xsl:apply-templates select="$husb" mode="birthdeath" />
						</xsl:otherwise>
					</xsl:choose>
									
					<xsl:for-each select="Child/Link[@Target='IndividualRec']">
						<xsl:variable name="childId" select="@Ref" />
						<xsl:call-template name="decendants">
							<xsl:with-param name="indi" select="key('individual', $childId)" />
							<xsl:with-param name="generation" select="$generation + 1" />
						</xsl:call-template>
					</xsl:for-each>
					
				</xsl:for-each>				 
			</li>
		</ul>
		
	</xsl:template>
	
</xsl:stylesheet>

