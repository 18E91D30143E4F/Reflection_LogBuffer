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
                        if ((memberInfo.MemberType & MemberTypes.Property) != 0)
                        {
                            typeInfo.PublicProperties.Add($"{memberInfo}");
                        }
                        else if ((memberInfo.MemberType & MemberTypes.Method) != 0)
                        {
                            typeInfo.PublicMethods.Add($"{memberInfo}");
                        }
                        else
                        {
                            typeInfo.PublicOther.Add($"{memberInfo.MemberType} {memberInfo}");
                        }
                    }
                }
                typesInfo.Add(typeInfo);
            }
            return typesInfo;
        }

    }
}