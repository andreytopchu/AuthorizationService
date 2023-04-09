using IdentityServer4.Models;

namespace IdentityDbSeeder.SeedData
{
    public static class SecretConfig
    {
        public static Secret[] MobileDeviceSecrets => new[]
        {
            new Secret("33FCAD9A-B2E0-4962-A1F2-E0BFBD7D75B7".Sha256()),
            new Secret("2915F282-6677-4219-AEAA-F4B92B60B3F1".Sha256()),
            new Secret("CA1734BB-9530-42D8-8BE9-CCD4AC4FE790".Sha256()),
            new Secret("99B2164A-F9A7-4B75-9927-7719C7A0FFCC".Sha256()),
            new Secret("2015B36F-FF0E-467D-9965-BF899801FC7F".Sha256()),
        };


        public static Secret[] MobileApiClientSecrets => new[]
        {
            new Secret("D7927BE0-A841-414C-880E-206D08235B6D".Sha256()),
            new Secret("41CFF615-135E-4986-863C-3B7203CE44A3".Sha256()),
            new Secret("77542A25-479E-4CD8-AC85-398C47548F27".Sha256()),
            new Secret("4CFB764E-DC72-414A-9393-BF86626218E3".Sha256()),
            new Secret("C243DDA1-5D3A-4C0A-81FC-1AA3B62494B2".Sha256()),
        };


        public static Secret[] WebApiClientSecrets => new[]
        {
            new Secret("2C78048F-ED10-4DAD-AA1E-54C1C66AA897".Sha256()),
            new Secret("D77FC7BD-FD0E-471E-ACE3-3B9DD45A1803".Sha256()),
            new Secret("2F5A0C54-185A-4742-8B4D-26F058906715".Sha256()),
            new Secret("814F5116-5619-458F-8C58-727B2625C14C".Sha256()),
            new Secret("4D504F40-97B7-41FF-9772-942887906808".Sha256()),
        };


        public static Secret[] AdminApiClientSecrets => new[]
        {
            new Secret("9F45EA47-9BD6-48D8-B218-273A256DB093".Sha256()),
            new Secret("7DD691E1-DD52-403A-8341-47EB02A9B39F".Sha256()),
            new Secret("E682B669-6855-4FD4-8653-04B437E3B368".Sha256()),
            new Secret("F1E8D6D4-9875-4445-84AB-70AD5E5CBC11".Sha256()),
            new Secret("9DE7A19D-5463-464E-BD54-8339FF889432".Sha256()),
        };


        public static Secret[] AdminApiResourceSecrets => new[]
        {
            new Secret("AA6F7FEC-8793-4B03-8BAA-BF8EDCF9F787".Sha256()),
            new Secret("96903788-4094-44BF-95E3-05D0CD1B2264".Sha256()),
            new Secret("1BCB0244-AAAE-4F9C-BB28-B01DE82CAEFD".Sha256()),
            new Secret("F70A655E-C5A6-48F8-870E-0807F6F2A229".Sha256()),
            new Secret("9E67D98C-9B5D-4B07-ACB2-7DC6E9F21A89".Sha256()),
        };

        public static Secret[] MobileApiResourceSecrets => new[]
        {
            new Secret("1703C13B-16F4-437F-8106-34CAD7CDB5A3".Sha256()),
            new Secret("11EBAF1E-4355-4740-8566-58F4AF80C381".Sha256()),
            new Secret("A00C1787-1C82-409D-A98B-12630B54A6CC".Sha256()),
            new Secret("1207FBBC-82CA-4E0F-BA5E-87522B224871".Sha256()),
            new Secret("5B51F48D-2BA2-4BF3-935D-FBD59250631A".Sha256()),
        };

        public static Secret[] ProfileApiResourceSecrets => new[]
        {
            new Secret("45C9D7F8-1F62-49A9-8E4A-FB1F5068F6E1".Sha256()),
            new Secret("63D79235-7508-4108-BB2C-4915B43C10F0".Sha256()),
            new Secret("D6BC7FC8-3CDB-42B8-B8B0-7DEDB51BA79E".Sha256()),
            new Secret("3D6CFA13-0978-4271-B3C3-4905AA0D7427".Sha256()),
            new Secret("09792ECD-2409-4E69-A50E-1C9C01CA7D1A".Sha256()),
        };
    }
}