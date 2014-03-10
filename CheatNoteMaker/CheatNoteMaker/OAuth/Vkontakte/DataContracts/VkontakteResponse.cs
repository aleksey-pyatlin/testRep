using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CheatNoteMaker.OAuth.Vkontakte.DataContracts
{
    [DataContract]
    public class VkontakteResponse<T> where T : class
    {
        [DataMember(Name = "response")]
        public T Data { get; set; }

        [DataMember(Name = "error")]
        public VkontakteError Error { get; set; }

        public bool Success
        {
            get
            {
                return this.Error == null;
            }
        }

        #region Serialization

        private static readonly DataContractJsonSerializer JSONSerializer = new DataContractJsonSerializer(typeof(VkontakteResponse<T>));

        public static VkontakteResponse<T> Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException("json");
            }

            return Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }

        public static VkontakteResponse<T> Deserialize(Stream jsonStream)
        {
            if (jsonStream == null)
            {
                throw new ArgumentNullException("jsonStream");
            }
            
            return (VkontakteResponse<T>)JSONSerializer.ReadObject(jsonStream);
        }

        #endregion
    }
}