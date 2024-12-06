using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace ContainerTagRemover.Interfaces
{
    public interface IContainerRegistryClient
    {
        Task AuthenticateAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Tag>> ListTagsAsync(string image);
        Task DeleteTagAsync(string image, string tag);
    }

    public class Tag(string name, string digest)
    {
        public string Name { get; } = name;
        public string Digest { get; } = digest;
    }
}
