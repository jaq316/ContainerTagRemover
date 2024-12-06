using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace ContainerTagRemover.Interfaces
{
    public interface IContainerRegistryClient
    {
        Task AuthenticateAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> ListTagsAsync(string image);
        Task DeleteTagAsync(string image, string tag);
    }
}
