using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DevelopmentTests;

public class GenerateSensitivityTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GenRsa()
    {
        TestContext.WriteLine(JsonConvert.SerializeObject(RSA.Create().ExportParameters(true)));
    }

    [Test]
    public void GenSecrets()
    {
        var names = new[]
        {
            "MobileDeviceSecrets",
            "MobileApiClientSecrets",
            "WebApiClientSecrets",
            "AdminApiClientSecrets",
            "AdminApiResourceSecrets",
            "MobileApiResourceSecrets",
            "ProfileApiResourceSecrets"
        };

        foreach (var propertyName in names)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public static Secret[] {propertyName} => new[]");

            sb.AppendLine("{");
            for (var i = 0; i < 5; i++)
            {
                sb.Append('\t');
                sb.AppendLine($"new Secret(\"{Guid.NewGuid().ToString().ToUpper()}\".Sha256()),");
            }

            sb.AppendLine("};");
            sb.AppendLine();

            TestContext.WriteLine(sb.ToString());
        }
    }
}