using System.Collections.Generic;
using System.Reflection;

namespace LoadAssembly
{
    public class AssemblyLoader
    {
        public Assembly Assembly { get; set; }

        public AssemblyLoader(string path)
        {
            LoadAssemblyInMemory(path);
        }

        public void LoadAssemblyInMemory(string path)
        {
            Assembly = Assembly.LoadFrom(path);
        }

        public IEnumerable<ClassInfo> GetPublicTypes()
        {
            var types = Assembly.GetTypes();
            var typesInfo = new List<ClassInfo>();

            foreach (var type in types)
            {
                var typeInfo = new ClassInfo
                {
                    ClassName = type.Name,
                    Namespace = type.Namespace ?? "Not identified"
                };

                foreach (var memberInfo in type.GetMembers())
                {
                    if (memberInfo.DeclaringType.IsPublic)
                    {
                        switch (memberInfo.MemberType)
                        {
                            case MemberTypes.Method:
                                typeInfo.PublicMethods.Add($"{memberInfo}");
                                break;
                            case MemberTypes.Property:
                                typeInfo.PublicProperties.Add($"{memberInfo}");
                                break;
                            default:
                                typeInfo.PublicOther.Add($"{memberInfo.MemberType} {memberInfo}");
                                break;
                        }
                    }
                }
                typesInfo.Add(typeInfo);
            }
            return typesInfo;
        }

    }
}