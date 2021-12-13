using NickX.TinyORM.Mapping.Attributes;
using System;

namespace ConsoleTest.Models
{
    public class PersonModel : BaseModel
    {
        [Column("FIRSTNAME")]
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public bool IsContactPerson { get; set; }

        [ForeignKey(typeof(CompanyModel), nameof(Guid))]
        public Guid CompanyGuid { get; set; }
    }
}
