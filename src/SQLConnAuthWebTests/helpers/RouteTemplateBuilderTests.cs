using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.helpers;
using System;

namespace SQLConnAuthWebTests.helpers;

[TestClass]
public class RouteTemplateBuilderTests
{
   [TestMethod]
   public void BasePath_Should_Build_Correctly_With_DBNameRouting() {
      // With leading slash
      SqlAuthAppPaths paths1 = new("/db", "srv", true);
      RouteTemplateBuilder builder1 = new(paths1);
      string expected = "db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/srv/";
      Assert.AreEqual(expected, builder1.BaseTemplatePath);

      // Without leading slash
      SqlAuthAppPaths paths2 = new("db", "srv", true);
      RouteTemplateBuilder builder2 = new(paths2);
      Assert.AreEqual(expected, builder2.BaseTemplatePath);

      // Deeper root path with leading slash
      SqlAuthAppPaths paths3 = new("/db/level2", "srv", true);
      RouteTemplateBuilder builder3 = new(paths3);
      string expecteddeep = "db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/srv/";
      Assert.AreEqual(expecteddeep, builder3.BaseTemplatePath);

      // Deeper root path without leading slash
      SqlAuthAppPaths paths4 = new("db/level2", "srv", true);
      RouteTemplateBuilder builder4 = new(paths4);
      Assert.AreEqual(expecteddeep, builder4.BaseTemplatePath);
   }

   [TestMethod]
   public void BasePath_Should_Build_Correctly_Without_DBNameRouting() {
      // With leading slash
      SqlAuthAppPaths paths1 = new("/db", "srv", false);
      RouteTemplateBuilder builder1 = new(paths1);
      string expected = "db/{sqlauthparamsrv}/{sqlauthparamusr}/srv/";
      Assert.AreEqual(expected, builder1.BaseTemplatePath);

      // Without leading slash
      SqlAuthAppPaths paths2 = new("db", "srv", false);
      RouteTemplateBuilder builder2 = new(paths2);
      Assert.AreEqual(expected, builder2.BaseTemplatePath);

      // Deeper root path with leading slash
      SqlAuthAppPaths paths3 = new("/db/level2", "srv", false);
      RouteTemplateBuilder builder3 = new(paths3);
      string expecteddeep = "db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/srv/";
      Assert.AreEqual(expecteddeep, builder3.BaseTemplatePath);

      // Deeper root path without leading slash
      SqlAuthAppPaths paths4 = new("db/level2", "srv", false);
      RouteTemplateBuilder builder4 = new(paths4);
      Assert.AreEqual(expecteddeep, builder4.BaseTemplatePath);
   }

   [TestMethod]
   public void BuildTemplate_Should_Append_Template_Correctly() {
      // With leading slash
      SqlAuthAppPaths paths1 = new("/db", "srv", true);
      RouteTemplateBuilder builder1 = new(paths1);
      string template = "/db/extra/path";
      string expected = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/srv/extra/path";
      Assert.AreEqual(expected, builder1.BuildTemplate(template));

      // Without leading slash
      SqlAuthAppPaths paths2 = new("db", "srv", true);
      RouteTemplateBuilder builder2 = new(paths2);
      Assert.AreEqual(expected, builder2.BuildTemplate(template));

      // Deeper root path with leading slash
      SqlAuthAppPaths paths3 = new("/db/level2", "srv", true);
      RouteTemplateBuilder builder3 = new(paths3);
      string templatedeep = "/db/level2/extra/path";
      string expecteddeep = "/db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/srv/extra/path";
      Assert.AreEqual(expecteddeep, builder3.BuildTemplate(templatedeep));

      // Deeper root path without leading slash
      SqlAuthAppPaths paths4 = new("db/level2", "srv", true);
      RouteTemplateBuilder builder4 = new(paths4);
      Assert.AreEqual(expecteddeep, builder4.BuildTemplate(templatedeep));
   }

   [TestMethod]
   public void BuildTemplate_Should_Handle_Empty_Root() {
      // With leading slash
      SqlAuthAppPaths paths1 = new("", "tail", true);
      RouteTemplateBuilder builder1 = new(paths1);
      string template = "/extra/path";
      string expected = "/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/tail/extra/path";
      Assert.AreEqual(expected, builder1.BuildTemplate(template));

      // Without leading slash (should behave the same)
      SqlAuthAppPaths paths2 = new("", "tail", true);
      RouteTemplateBuilder builder2 = new(paths2);
      Assert.AreEqual(expected, builder2.BuildTemplate(template));
   }

   [TestMethod]
   public void Constructor_Should_Throw_On_Null_Paths() =>
      _ = Assert.ThrowsExactly<ArgumentNullException>(() => new RouteTemplateBuilder(null!));

   [TestMethod]
   public void BasePath_Should_Build_Correctly_With_DBNameRouting_EmptyTail() {
      // With leading slash
      SqlAuthAppPaths paths1 = new("/db", "", true);
      RouteTemplateBuilder builder1 = new(paths1);
      string expected = "db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/";
      Assert.AreEqual(expected, builder1.BaseTemplatePath);
      string template = "/db/extra/path";
      string expectedtemplate = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/extra/path";
      Assert.AreEqual(expectedtemplate, builder1.BuildTemplate(template));

      // Without leading slash
      SqlAuthAppPaths paths2 = new("db", "", true);
      RouteTemplateBuilder builder2 = new(paths2);
      Assert.AreEqual(expected, builder2.BaseTemplatePath);
      Assert.AreEqual(expectedtemplate, builder2.BuildTemplate(template));

      // Deeper root path with leading slash
      SqlAuthAppPaths paths3 = new("/db/level2", "", true);
      RouteTemplateBuilder builder3 = new(paths3);
      string expecteddeep = "db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/";
      Assert.AreEqual(expecteddeep, builder3.BaseTemplatePath);
      string templatedeep = "/db/level2/extra/path";
      string expectedtemplatedeep = "/db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/extra/path";
      Assert.AreEqual(expectedtemplatedeep, builder3.BuildTemplate(templatedeep));

      // Deeper root path without leading slash
      SqlAuthAppPaths paths4 = new("db/level2", "", true);
      RouteTemplateBuilder builder4 = new(paths4);
      Assert.AreEqual(expecteddeep, builder4.BaseTemplatePath);
      Assert.AreEqual(expectedtemplatedeep, builder4.BuildTemplate(templatedeep));
   }

   [TestMethod]
   public void BasePath_Should_Build_Correctly_Without_DBNameRouting_EmptyTail() {
      // With leading slash
      SqlAuthAppPaths paths1 = new("/db", "", false);
      RouteTemplateBuilder builder1 = new(paths1);
      string expected = "db/{sqlauthparamsrv}/{sqlauthparamusr}/";
      Assert.AreEqual(expected, builder1.BaseTemplatePath);
      string template = "/db/extra/path";
      string expectedtemplate = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/extra/path";
      Assert.AreEqual(expectedtemplate, builder1.BuildTemplate(template));

      // Without leading slash
      SqlAuthAppPaths paths2 = new("db", "", false);
      RouteTemplateBuilder builder2 = new(paths2);
      Assert.AreEqual(expected, builder2.BaseTemplatePath);
      Assert.AreEqual(expectedtemplate, builder2.BuildTemplate(template));

      // Deeper root path with leading slash
      SqlAuthAppPaths paths3 = new("/db/level2", "", false);
      RouteTemplateBuilder builder3 = new(paths3);
      string expecteddeep = "db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/";
      Assert.AreEqual(expecteddeep, builder3.BaseTemplatePath);
      string templatedeep = "/db/level2/extra/path";
      string expectedtemplatedeep = "/db/level2/{sqlauthparamsrv}/{sqlauthparamusr}/extra/path";
      Assert.AreEqual(expectedtemplatedeep, builder3.BuildTemplate(templatedeep));

      // Deeper root path without leading slash
      SqlAuthAppPaths paths4 = new("db/level2", "", false);
      RouteTemplateBuilder builder4 = new(paths4);
      Assert.AreEqual(expecteddeep, builder4.BaseTemplatePath);
      Assert.AreEqual(expectedtemplatedeep, builder4.BuildTemplate(templatedeep));
   }
}
