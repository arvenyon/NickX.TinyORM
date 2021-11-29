using System;

namespace ConsoleTest.Models
{
    public class PersonModel : BaseModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public bool IsContactPerson { get; set; }
        public Guid CompanyGuid { get; set; }
    }
}
