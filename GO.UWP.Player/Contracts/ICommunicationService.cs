using System;
using System.Threading.Tasks;
using GO.UWP.Player.Model;

namespace GO.UWP.Player.Contracts
{
    public interface ICommunicationService
    {
        Task<Registration> SilentRegister(Uri registrationUri);
        Task<LoginResponse> Login(Uri loginUri, string username, string password, int operatorId, CurrentDevice device);
        Task<CategoriesItem> GetCategory(Uri categoriesUri);
        Task<Categories> GetCategories(Uri categoriesUri);
        Task<ContentsItem> GetShowDetail(Uri showUri);
        Task<Video> GetPlayableLink(Uri playUri, Guid showGuid, string individualization);
    }
}
