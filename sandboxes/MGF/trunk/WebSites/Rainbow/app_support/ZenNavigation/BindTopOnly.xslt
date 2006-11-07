<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rainbow="urn:rainbow">
	<xsl:output method="html" version="4.0" indent="no"/>
	<xsl:param name="ClientScriptLocation"/>
	<xsl:param name="ActivePageId"/>
	<xsl:param name="Orientation"/>
	<xsl:param name="ContainerCssClass"/>
	<xsl:param name="UsePathTraceInUrl"/>
	<xsl:param name="UsePageNameInUrl"/>
	<xsl:template match="/">
		<xsl:element name="div">
			<xsl:attribute name="class"><xsl:value-of select="$ContainerCssClass"/></xsl:attribute>
			<ul>
				<xsl:apply-templates select="MenuData/MenuGroup"/>
			</ul>
		</xsl:element>
	</xsl:template>
	<xsl:template match="MenuItem[@ParentPageId='0']">
		<xsl:choose>
			<xsl:when test="rainbow:CheckRoles(string(@AuthRoles))">
				<xsl:element name="li">
						<xsl:choose>
							<xsl:when test="descendant-or-self::MenuItem[@ID=$ActivePageId]">
								<xsl:attribute name="class"><xsl:text>MenuItemSelected</xsl:text></xsl:attribute>
							</xsl:when>
						</xsl:choose>
					<xsl:element name="a">
						<xsl:choose>
							<xsl:when test="$UsePageNameInUrl = 'true'">
								<xsl:attribute name="href"><xsl:value-of select="rainbow:BuildUrl(string(@UrlPageName),number(@ID))"/></xsl:attribute>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name="href"><xsl:value-of select="rainbow:BuildUrl(number(@ID))"/></xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:choose>
							<xsl:when test="descendant-or-self::MenuItem[@ID=$ActivePageId]">
								<xsl:attribute name="class"><xsl:text>MenuItemSelected</xsl:text></xsl:attribute>
							</xsl:when>
						</xsl:choose>
						<xsl:value-of select="@PageName"/>
					</xsl:element>
				</xsl:element>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
