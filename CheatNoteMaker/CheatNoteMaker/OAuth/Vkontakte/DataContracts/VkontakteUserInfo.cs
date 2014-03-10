using System.Runtime.Serialization;

namespace CheatNoteMaker.OAuth.Vkontakte.DataContracts
{
    [DataContract]
    public class VkontakteUserInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}