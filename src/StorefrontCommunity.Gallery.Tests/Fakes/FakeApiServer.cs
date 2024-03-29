using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StorefrontCommunity.Gallery.API.Authorization;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;

namespace StorefrontCommunity.Gallery.Tests.Fakes
{
    public sealed class FakeApiServer : TestServer
    {
        public FakeApiServer() : base(new Program().CreateWebHostBuilder()) { }

        public IHostingEnvironment Environment => Host.Services.GetService<IHostingEnvironment>();
        public FakeMessageBroker EventBus => Host.Services.GetService<IMessageBroker>() as FakeMessageBroker;
        public FakeFileStorage FileStorage => Host.Services.GetService<IFileStorage>() as FakeFileStorage;
        public JwtOptions JwtOptions => Host.Services.GetService<IOptions<JwtOptions>>().Value;
    }
}
