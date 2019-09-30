using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;

namespace StorefrontCommunity.Gallery.Tests.Fakes
{
    public sealed class FakeFileStorage : IFileStorage
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FakeFileStorage(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            SavedFiles = new List<StoredFile>();
            DeletedFiles = new List<string>();
        }

        public ICollection<StoredFile> SavedFiles { get; }
        public ICollection<string> DeletedFiles { get; }

        public Task Save(StoredFile file)
        {
            SavedFiles.Add(file);
            return Task.CompletedTask;
        }

        public Task Delete(string fileName)
        {
            DeletedFiles.Add(fileName);
            return Task.CompletedTask;
        }

        public async Task<StoredFile> Read(string fileName)
        {
            var filePath = $"{_hostingEnvironment.ContentRootPath}/Fixtures/{fileName}";

            if (!File.Exists(filePath))
            {
                return null;
            }

            var bytes = await File.ReadAllBytesAsync(filePath);

            return new StoredFile(new MemoryStream(bytes), "image/jpeg", fileName);
        }
    }
}
