using System.Threading.Tasks;

namespace ContainerTagRemover.Interfaces
{
    public interface IAuthenticationClient
    {
        Task AuthenticateAsync();
    }
}
