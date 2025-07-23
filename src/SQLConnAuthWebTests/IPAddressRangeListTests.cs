using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using System;
using System.Linq;

namespace SQLConnAuthWebTests;

[TestClass]
public class IPAddressRangeListTests
{
   [TestMethod]
   public void Add_ValidSingleIPv4_AddsAsCidr() {
      IPAddressRangeList list = [
           "192.168.1.1"
        ];
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual("192.168.1.1/32", list.First());
   }

   [TestMethod]
   public void Add_ValidSingleIPv6_AddsAsCidr() {
      IPAddressRangeList list = [
           "2001:db8::1"
        ];
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual("2001:db8::1/128", list.First());
   }

   [TestMethod]
   public void Add_ValidCidrIPv4_Adds() {
      IPAddressRangeList list = [
           "10.0.0.0/24"
        ];
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual("10.0.0.0/24", list.First());
   }

   [TestMethod]
   public void Add_ValidCidrIPv6_Adds() {
      IPAddressRangeList list = [
           "2001:db8::/64"
        ];
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual("2001:db8::/64", list.First());
   }

   [TestMethod]
   public void Add_ValidSubnetMask_AddsAsCidr() {
      IPAddressRangeList list = [
           "192.168.1.0/255.255.255.0"
        ];
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual("192.168.1.0/24", list.First());
   }

   [TestMethod]
   public void Add_InvalidCidr_Throws() => _ = Assert.ThrowsExactly<FormatException>(() => {
      IPAddressRangeList list = [
            "192.168.1.1/33"
         ];
   });

   [TestMethod]
   public void Add_InvalidSubnetMask_Throws() => _ = Assert.ThrowsExactly<FormatException>(() => {
      IPAddressRangeList list = [
            "192.168.1.0/255.0.255.0"
         ];
   });

   [TestMethod]
   public void Add_InvalidString_Throws() => _ = Assert.ThrowsExactly<FormatException>(() => {
      IPAddressRangeList list = [
            "notanip"
         ];
   });

   [TestMethod]
   public void Remove_RemovesEntry() {
      IPAddressRangeList list = [
           "10.0.0.1"
        ];
      Assert.IsTrue(list.Remove("10.0.0.1/32"));
      Assert.AreEqual(0, list.Count);
   }
}
