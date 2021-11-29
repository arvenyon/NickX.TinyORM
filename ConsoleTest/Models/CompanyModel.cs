namespace ConsoleTest.Models
{
    public class CompanyModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CompanyTypes CompanyType { get; set; }
    }

    public enum CompanyTypes
    {
        None,
        Partner,
        Customer,
        Service
    }
}
