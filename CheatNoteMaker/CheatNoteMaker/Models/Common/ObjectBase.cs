using System;
using System.ComponentModel.DataAnnotations.Schema;

using CheatNoteMaker.Models.Users;

namespace CheatNoteMaker.Models.Common
{
    public class ObjectBase
    {
        public ObjectBase()
        {
            Id = 0;
            DateCreated = DateModified = DateTime.Now;
        }

        public int Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }

        public int? CreatedById { get; set; }
        public int? ModifiedById { get; set; }
        public int? DeletedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual UserInfo CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual UserInfo ModifiedBy { get; set; }

        [ForeignKey("DeletedById")]
        public virtual UserInfo DeletedBy { get; set; }
    }
}