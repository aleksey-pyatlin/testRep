using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CheatNoteMaker.OAuth.Vkontakte.DataContracts
{
    [DataContract]
    public class VkontakteError
    {
        [DataMember(Name = "error_code")]
        public string Code { get; set; }

        [DataMember(Name = "error_msg")]
        public string Message { get; set; }

        [DataMember(Name = "request_params")]
        public IEnumerable<VkontakteRequestParam> RequestParams { get; set; }

        #region Overridden

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Code, this.Message);
        }

        #endregion
    }
}