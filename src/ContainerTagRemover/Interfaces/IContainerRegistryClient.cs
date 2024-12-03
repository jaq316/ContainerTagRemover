using System.Threading.Tasks;
using System.Collections.Generic;

namespace ContainerTagRemover.Interfaces
{
    public interface IContainerRegistryClient
    {
        Task<IEnumerable<string>> ListTagsAsync(string repository);
        Task DeleteTagAsync(string repository, string tag);
    }
}
