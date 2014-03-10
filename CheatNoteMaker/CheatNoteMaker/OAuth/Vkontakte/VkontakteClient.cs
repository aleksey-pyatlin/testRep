using System;

using DotNetOpenAuth.OAuth2;

namespace CheatNoteMaker.OAuth.Vkontakte
{
    /// <summary>
    /// OAuth 2.0 client for VKontakte
    /// </summary>
    public class VkontakteClient : WebServerClient
    {
        /// <summary>
        /// The vkontakte description
        /// </summary>
        private static readonly AuthorizationServerDescription VkontakteDescription =
            new AuthorizationServerDescription
                {
                    //ProtocolVersion = ProtocolVersion.V20,
                    AuthorizationEndpoint = new Uri("https://oauth.vk.com/authorize"),
                    TokenEndpoint = new Uri("https://oauth.vk.com/access_token")
                };

        /// <summary>
        /// Initializes a new instance of the <see cref="VkontakteClient"/> class.
        /// </summary>
        public VkontakteClient()
            : base(VkontakteDescription)
        {
        }
    }
}