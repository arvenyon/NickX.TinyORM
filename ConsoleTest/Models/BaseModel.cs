using NickX.TinyORM.Mapping.Attributes;
using System;

namespace ConsoleTest.Models
{
    public abstract class BaseModel
    {
        [PrimaryKey, Column("Guid", false, NickX.TinyORM.Mapping.Enums.DefaultValues.UniqueIdentifier)]
        public Guid Guid { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateModified { get; set; }

        public override string ToString()
        {
            var retVal = string.Empty;
            foreach (var prop in this.GetType().GetProperties())
                if (prop.CanRead)
                    retVal += String.Format(@"[{0}] = {1}{2}", prop.Name, prop.GetValue(this), Environment.NewLine);
            return retVal;
        }
    }
}
