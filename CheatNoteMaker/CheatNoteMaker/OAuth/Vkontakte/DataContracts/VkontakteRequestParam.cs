using System.Runtime.Serialization;

namespace CheatNoteMaker.OAuth.Vkontakte.DataContracts
{
    [DataContract]
    public class VkontakteRequestParam
    {
        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        #region Overridden

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Key, this.Value);
        }

        #endregion
    }
}