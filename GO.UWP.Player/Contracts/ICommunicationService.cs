using System;
using System.Threading.Tasks;
using GO.UWP.Player.Model;

namespace GO.UWP.Player.Contracts
{
    public interface ICommunicationService
    {
        Task<Registration> SilentRegister(Uri registrationUri);
        Task<Operators> GetOperators(Uri defaultOperatorUri);
        Task<LoginResponse> Login(Uri loginUri, string username, string password, Guid operatorId, CurrentDevice device);
        Task<CategoriesItem> GetCategory(Uri categoriesUri);
        Task<Categories> GetCategories(Uri categoriesUri);
        Task<ContentsItem> GetShowDetail(Uri showUri);
        Task<Video> GetPlayableLink(Uri playUri, Guid showGuid, string individualization, Guid operatorGuid,string languageCode, string apiPlatform);
        Task<Item> GetSearchResults(Uri searchUri, string searchQuery);
    }
}
