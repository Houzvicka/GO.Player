using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBO.UWP.Player.Model;

namespace HBO.UWP.Player.Contracts
{
    public interface ICommunicationService
    {
        Task<Registration> SilentRegister(Uri registrationUri);
        Task<LoginResponse> Login(Uri loginUri, string username, string password, int operatorId, CurrentDevice device);
        Task<Categories> GetCategories(Uri categoriesUri);
        Task<ContentsItem> GetShowDetail(Uri showUri);
        Task<Video> GetPlayableLink(Uri playUri, Guid showGuid, string individualization);
    }
}
