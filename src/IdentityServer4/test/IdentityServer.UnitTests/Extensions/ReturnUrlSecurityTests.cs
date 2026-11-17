using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class ReturnUrlSecurityTests
    {
        [Theory]
        [InlineData("/connect/authorize/callback", true)]
        [InlineData("~/connect/authorize/callback", true)]
        [InlineData("/connect/authorize/callback?client_id=client", true)]
        [InlineData("//attacker.invalid/connect/authorize/callback", false)]
        [InlineData("/\\attacker.invalid/connect/authorize/callback", false)]
        [InlineData("/\t/attacker.invalid/connect/authorize/callback", false)]
        [InlineData("~/\t/attacker.invalid/connect/authorize/callback", false)]
        [InlineData("/\r/attacker.invalid/connect/authorize/callback", false)]
        [InlineData("/\n/attacker.invalid/connect/authorize/callback", false)]
        [InlineData("/connect/authorize/callback?state=\0", false)]
        public void Return_url_parser_rejects_untrusted_redirects(string url, bool expected)
        {
            var parser = new OidcReturnUrlParser(null, null, NullLogger<OidcReturnUrlParser>.Instance);
            Assert.Equal(expected, url.IsLocalUrl());
            Assert.Equal(expected, parser.IsValidReturnUrl(url));
        }

        [Fact]
        public void All_control_characters_are_rejected()
        {
            for (var value = 0; value <= char.MaxValue; value++)
            {
                if (char.IsControl((char)value))
                {
                    Assert.False(("/" + (char)value + "/host/connect/authorize/callback").IsLocalUrl());
                }
            }
        }
    }
}
