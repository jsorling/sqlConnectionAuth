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
      SqlAuthAppPaths paths = new("/db", "srv", true);
      RouteTemplateBuilder builder = new(paths);
      string expected = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/srv/";
      Assert.AreEqual(expected, builder.BasePath);
   }

   [TestMethod]
   public void BasePath_Should_Build_Correctly_Without_DBNameRouting() {
      SqlAuthAppPaths paths = new("/db", "srv", false);
      RouteTemplateBuilder builder = new(paths);
      string expected = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/srv/";
      Assert.AreEqual(expected, builder.BasePath);
   }

   [TestMethod]
   public void BuildTemplate_Should_Append_Template_Correctly() {
      SqlAuthAppPaths paths = new("/db", "srv", true);
      RouteTemplateBuilder builder = new(paths);
      string template = "/db/extra/path";
      string expected = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/srv/db/extra/path";
      Assert.AreEqual(expected, builder.BuildTemplate(template));
   }

   [TestMethod]
   public void BuildTemplate_Should_Handle_Empty_Root() {
      SqlAuthAppPaths paths = new("", "tail", true);
      RouteTemplateBuilder builder = new(paths);
      string template = "/extra/path";
      string expected = "/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/tail/extra/path";
      Assert.AreEqual(expected, builder.BuildTemplate(template));
   }

   [TestMethod]
   public void Constructor_Should_Throw_On_Null_Paths() =>
      _ = Assert.ThrowsExactly<ArgumentNullException>(() => new RouteTemplateBuilder(null!));

   [TestMethod]
   public void BasePath_Should_Build_Correctly_With_DBNameRouting_EmptyTail() {
      SqlAuthAppPaths paths = new("/db", "", true);
      RouteTemplateBuilder builder = new(paths);
      string expected = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/";
      Assert.AreEqual(expected, builder.BasePath);
      string template = "/db/extra/path";
      string expectedtemplate = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}/db/extra/path";
      Assert.AreEqual(expectedtemplate, builder.BuildTemplate(template));
   }

   [TestMethod]
   public void BasePath_Should_Build_Correctly_Without_DBNameRouting_EmptyTail() {
      SqlAuthAppPaths paths = new("/db", "", false);
      RouteTemplateBuilder builder = new(paths);
      string expected = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/";
      Assert.AreEqual(expected, builder.BasePath);
      string template = "/db/extra/path";
      string expectedtemplate = "/db/{sqlauthparamsrv}/{sqlauthparamusr}/db/extra/path";
      Assert.AreEqual(expectedtemplate, builder.BuildTemplate(template));
   }
}
