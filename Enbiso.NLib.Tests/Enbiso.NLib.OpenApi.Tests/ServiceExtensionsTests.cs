using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Enbiso.NLib.OpenApi.Tests
{
    public class ServiceExtensionsTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        [Fact]
        public void FetchingConfigurationTest()
        {            
            var services = _fixture.Create<IServiceCollection>();
            services.AddOpenApi(o => o.Id = "Test");
            services.ReceivedWithAnyArgs().Configure<OpenApiOptions>(opt => {});
        }
    }
}
