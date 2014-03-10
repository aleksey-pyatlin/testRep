using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using CheatNoteMaker.OAuth.Vkontakte.DataContracts;

using DotNetOpenAuth.OAuth2;

namespace CheatNoteMaker.OAuth.Vkontakte
{
    public class Vkontakte
    {
        #region Fields

        private readonly VkontakteClient _client;

        private VkontakteUserInfo _userInfo;

        #endregion

        #region C-tor

        public Vkontakte()
        {
            _client = InitializeClient();
        }

        #endregion

        #region Public properties

        public VkontakteUserInfo UserInfo
        {
            get
            {
                return this._userInfo ?? (this._userInfo = this.GetUserInfo());
            }
        }

        #endregion

        #region Public methods
        
        public IAuthorizationState Authorize(Uri returnUrl = null, IEnumerable<string> scope = null)
        {
            var authorization = _client.ProcessUserAuthorization();

            if (authorization == null)
            {
                _client.RequestUserAuthorization(scope, returnUrl);
            }

            return authorization;
        }

        #endregion

        #region Private helpers

        private VkontakteUserInfo GetUserInfo()
        {
            var authorization = Authorize();

            if (authorization == null)
            {
                return null;
            }

            var result = VkontakteApiProvider.Call<IEnumerable<VkontakteUserInfo>>(authorization.AccessToken, "users.get");

            return result.Data.FirstOrDefault();
        }

        private static VkontakteClient InitializeClient()
        {
            return new VkontakteClient
            {
                ClientIdentifier = ConfigurationManager.AppSettings["VK_AUTH_CONSUMER_KEY"],
                ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["VK_AUTH_CONSUMER_SECRET"]),
            };
        }

        #endregion
    }
}