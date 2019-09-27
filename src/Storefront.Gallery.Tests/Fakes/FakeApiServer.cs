using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Storefront.Gallery.API.Authorization;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.Tests.Fakes
{
    public sealed class FakeApiServer : TestServer
    {
        public FakeApiServer() : base(new Program().CreateWebHostBuilder()) { }

        public IHostingEnvironment Environment => Host.Services.GetService<IHostingEnvironment>();
        public FakeEventBroker EventBus => Host.Services.GetService<IEventBus>() as FakeEventBroker;
        public FakeFileStorage FileStorage => Host.Services.GetService<IFileStorage>() as FakeFileStorage;
        public JwtOptions JwtOptions => Host.Services.GetService<IOptions<JwtOptions>>().Value;
    }
}
