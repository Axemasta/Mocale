using System.Text;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Parsers;

namespace Mocale.UnitTests.Parsers;

public class ResxLocalizationParserTests : FixtureBase
{
    #region Setup

    private readonly Mock<ILogger<ResxLocalizationParser>> logger;

    public ResxLocalizationParserTests()
    {
        logger = new Mock<ILogger<ResxLocalizationParser>>();
    }

    public override object CreateSystemUnderTest()
    {
        return new ResxLocalizationParser(logger.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void ParseLocalizationStream_WhenStreamIsNotResx_ShouldReturnNull()
    {
        // Arrange
        var sut = GetSut<ILocalizationParser>();

        var notXml = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas diam sapien, posuere vel arcu sit amet, tempus tincidunt eros. Duis fringilla ligula eu massa cursus facilisis. Donec congue eget eros at porttitor. Maecenas sed nisl augue. Nam vel ex neque. Nam posuere convallis mauris, eu fringilla neque tempor sit amet. Nullam sollicitudin dolor eu justo aliquet cursus. Curabitur et risus vel elit viverra iaculis congue vel ex. Sed mollis pretium interdum.";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(notXml));

        // Act
        var localizations = sut.ParseLocalizationStream(stream);

        // Assert
        Assert.Null(localizations);

        //logger.VerifyLog(
        //    log => log.LogError(
        //        It.IsAny<Exception>(),
        //        It.IsAny<string>()),
        //    Times.Once());
    }

    [Fact]
    public void ParseLocalizationStream_WhenStreamIsXmlButNotResx_ShouldReturnNull()
    {
        // Arrange
        var sut = GetSut<ILocalizationParser>();

        var xml = """
            <?xml version="1.0"?>
            <catalog>
               <book id="bk101">
                  <author>Gambardella, Matthew</author>
                  <title>XML Developer's Guide</title>
                  <genre>Computer</genre>
                  <price>44.95</price>
                  <publish_date>2000-10-01</publish_date>
                  <description>An in-depth look at creating applications 
                  with XML.</description>
               </book>
               <book id="bk102">
                  <author>Ralls, Kim</author>
                  <title>Midnight Rain</title>
                  <genre>Fantasy</genre>
                  <price>5.95</price>
                  <publish_date>2000-12-16</publish_date>
                  <description>A former architect battles corporate zombies, 
                  an evil sorceress, and her own childhood to become queen 
                  of the world.</description>
               </book>
            </catalog>
            """;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var localizations = sut.ParseLocalizationStream(stream);

        // Assert
        Assert.Null(localizations);

        //logger.VerifyLog(
        //    log => log.LogError(
        //        It.IsAny<Exception>(),
        //        It.IsAny<string>()),
        //    Times.Once());
    }

    [Fact]
    public void ParseLocalizationStream_WhenStreamIsResx_ShouldReturnExpectedResponse()
    {
        // Arrange
        var sut = GetSut<ILocalizationParser>();

        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
                <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
                <xsd:element name="root" msdata:IsDataSet="true">
                  <xsd:complexType>
                    <xsd:choice maxOccurs="unbounded">
                      <xsd:element name="metadata">
                        <xsd:complexType>
                          <xsd:sequence>
                            <xsd:element name="value" type="xsd:string" minOccurs="0" />
                          </xsd:sequence>
                          <xsd:attribute name="name" use="required" type="xsd:string" />
                          <xsd:attribute name="type" type="xsd:string" />
                          <xsd:attribute name="mimetype" type="xsd:string" />
                          <xsd:attribute ref="xml:space" />
                        </xsd:complexType>
                      </xsd:element>
                      <xsd:element name="assembly">
                        <xsd:complexType>
                          <xsd:attribute name="alias" type="xsd:string" />
                          <xsd:attribute name="name" type="xsd:string" />
                        </xsd:complexType>
                      </xsd:element>
                      <xsd:element name="data">
                        <xsd:complexType>
                          <xsd:sequence>
                            <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                            <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
                          </xsd:sequence>
                          <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
                          <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
                          <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
                          <xsd:attribute ref="xml:space" />
                        </xsd:complexType>
                      </xsd:element>
                      <xsd:element name="resheader">
                        <xsd:complexType>
                          <xsd:sequence>
                            <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                          </xsd:sequence>
                          <xsd:attribute name="name" type="xsd:string" use="required" />
                        </xsd:complexType>
                      </xsd:element>
                    </xsd:choice>
                  </xsd:complexType>
                </xsd:element>
              </xsd:schema>
              <resheader name="resmimetype">
                <value>text/microsoft-resx</value>
              </resheader>
              <resheader name="version">
                <value>2.0</value>
              </resheader>
              <resheader name="reader">
                <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
              </resheader>
              <resheader name="writer">
                <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
              </resheader>
              <data name="CurrentLocaleName" xml:space="preserve">
                <value>English</value>
              </data>
              <data name="LocalizationCurrentProviderIs" xml:space="preserve">
                <value>The current localization provider is:</value>
              </data>
              <data name="LocalizationProviderName" xml:space="preserve">
                <value>Resx</value>
              </data>
              <data name="MocaleDescription" xml:space="preserve">
                <value>Localization framework for .NET Maui</value>
              </data>
              <data name="MocaleTitle" xml:space="preserve">
                <value>Mocale</value>
              </data>
            </root>
            """;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var localizations = sut.ParseLocalizationStream(stream);

        // Assert
        var expectedLocalizations = new Dictionary<string, string>()
        {
            { "CurrentLocaleName", "English" },
            { "LocalizationCurrentProviderIs", "The current localization provider is:" },
            { "LocalizationProviderName", "Json" },
            { "MocaleDescription", "Localization framework for .NET Maui" },
            { "MocaleTitle", "Mocale" },
        };

        localizations.Should().BeEquivalentTo(expectedLocalizations);
    }

    #endregion Tests
}
