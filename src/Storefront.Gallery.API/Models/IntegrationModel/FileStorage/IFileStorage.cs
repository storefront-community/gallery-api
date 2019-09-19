using System.Threading.Tasks;

namespace Storefront.Gallery.API.Models.IntegrationModel.FileStorage
{
    public interface IFileStorage
    {
        Task Save(StoredFile file);
        Task Delete(string fileName);
        Task<StoredFile> Read(string fileName);
    }
}
