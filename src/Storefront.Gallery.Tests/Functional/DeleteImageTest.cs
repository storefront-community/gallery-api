using System.Linq;
using Storefront.Gallery.API.Models.EventModel.Published;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.Tests.Factories;
using Storefront.Gallery.Tests.Fakes;
using Xunit;

namespace Storefront.Gallery.Tests.Functional
{
    public sealed class DeleteImageTest
    {
        private readonly FakeApiServer _server;
        private readonly FakeApiToken _token;
        private readonly FakeApiClient _client;

        public DeleteImageTest()
        {
            _server = new FakeApiServer();
            _token = new FakeApiToken(_server.JwtOptions);
            _client = new FakeApiClient(_server, _token);
        }

        [Theory]
        [InlineData("item")]
        [InlineData("item-group")]
        public void ShouldDeleteImageAfterListeningToSubscribedEvent(string gallery)
        {
            var subscribedEvent = new Event<dynamic>
            {
                Name = $"menu.{gallery}.deleted",
                Payload = new { TenantId = _token.TenantId, Id = ConstantFactory.Id }
            };

            var prefix = $"{subscribedEvent.Payload.TenantId}-{subscribedEvent.Payload.Id}.{gallery}";

            _server.EventBus.Publish(subscribedEvent);

            Assert.Contains(_server.FileStorage.DeletedFiles, filename => filename == $"{prefix}.cover.jpg");
            Assert.Contains(_server.FileStorage.DeletedFiles, filename => filename == $"{prefix}.standard.jpg");
            Assert.Contains(_server.FileStorage.DeletedFiles, filename => filename == $"{prefix}.thumbnail.jpg");
        }

        [Theory]
        [InlineData("item")]
        [InlineData("item-group")]
        public void ShouldPublishEventAfterDeletingEachImage(string gallery)
        {
            var subscribedEvent = new Event<dynamic>
            {
                Name = $"menu.{gallery}.deleted",
                Payload = new { TenantId = _token.TenantId, Id = ConstantFactory.Id }
            };

            var prefix = $"{subscribedEvent.Payload.TenantId}-{subscribedEvent.Payload.Id}.{gallery}";

            _server.EventBus.Publish(subscribedEvent);

            Assert.Equal(3, _server.EventBus.PublishedEvents.Count(publishedEvent =>
                publishedEvent.Name == "gallery.image.deleted"));

            Assert.Contains(_server.EventBus.PublishedEvents, @event =>
                @event.Name == "gallery.image.deleted" &&
                (@event.Payload as ImageDeletedPayload).Filename == $"{prefix}.cover.jpg");

            Assert.Contains(_server.EventBus.PublishedEvents, @event =>
                @event.Name == "gallery.image.deleted" &&
                (@event.Payload as ImageDeletedPayload).Filename == $"{prefix}.standard.jpg");

            Assert.Contains(_server.EventBus.PublishedEvents, @event =>
                @event.Name == "gallery.image.deleted" &&
                (@event.Payload as ImageDeletedPayload).Filename == $"{prefix}.thumbnail.jpg");
        }
    }
}
