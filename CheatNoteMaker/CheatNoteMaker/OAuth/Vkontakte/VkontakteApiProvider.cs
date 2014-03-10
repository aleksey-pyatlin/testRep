using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using CheatNoteMaker.OAuth.Vkontakte.DataContracts;

namespace CheatNoteMaker.OAuth.Vkontakte
{
    public class VkontakteApiProvider
    {
        #region Private constants

        private const string ApiVersion = "5.14";

        private const string ApiBaseUri = "https://api.vk.com/method/";

        #endregion

        public static VkontakteResponse<T> Call<T>(string accessToken, string methodName, IEnumerable<KeyValuePair<string, object>> parameters = null)
            where T : class 
        {
            var request = WebRequest.Create(FormatUri(accessToken, methodName, parameters));

            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    return VkontakteResponse<T>.Deserialize(responseStream);
                }
            }
        }

        #region Private helpers

        private static string FormatUri(string accessToken, string methodName, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var uri = new StringBuilder(string.Format("{0}{1}?v={2}&access_token={3}", ApiBaseUri, methodName, ApiVersion, Uri.EscapeDataString(accessToken)));

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    uri.AppendFormat("&{0}={1}", p.Key, p.Value);
                }
            }

            return uri.ToString();
        }

        #endregion


    }
}