using System.Collections.Generic;
using System.Linq;

namespace LoadAssembly
{
    public class ClassInfo
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public List<string> PublicProperties { get; set; }
        public List<string> PublicMethods { get; set; }
        public List<string> PublicOther { get; set; }

        public ClassInfo()
        {
            PublicProperties = new List<string>();
            PublicMethods = new List<string>();
            PublicOther = new List<string>();
        }

        public override string ToString()
        {
            var result = $"{nameof(Namespace)}: {Namespace}\n{nameof(ClassName)}: {ClassName}\n";

            result = PublicProperties
                .Aggregate(result, (current, strProperty) => current + $"\tpublic property {strProperty}\n");
            result = PublicMethods
                .Aggregate(result, (current, strMethod) => current + $"\tpublic method {strMethod}\n");

            return result;
        }

        public string ToString(bool sortByName)
        {
            var result = $"{nameof(Namespace)}: {Namespace}\n{nameof(ClassName)}: {ClassName}\n";

            result = PublicProperties
               .OrderBy(property => property.Split(" ")[1])
               .Aggregate(result, (current, strProperty) => current + $"\tpublic property {strProperty}\n"); ;
            result = PublicMethods
               .OrderBy(method => method.Split(" ")[1])
               .Aggregate(result, (current, strMethod) => current + $"\tpublic method {strMethod}\n");

            return result;
        }
    }
}