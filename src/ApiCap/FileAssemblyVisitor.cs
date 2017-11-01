using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiCap
{
    public class FileAssemblyVisitor : AssemblyVisitor
    {
        private static readonly Dictionary<Type, string> _typeToFriendlyTypeName = new Dictionary<Type, string>
        {
            { typeof(string), "string" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(short), "short" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(sbyte), "sbyte" },
            { typeof(float), "float" },
            { typeof(ushort), "ushort" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(void), "void" }
        };

        readonly string _filename;
        StreamWriter _writer;

        public FileAssemblyVisitor(string filename)
        {

            _filename = filename;
        }

        public override void Visit(Assembly[] assemblies)
        {
            using (var fileStream = new FileStream(_filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (_writer = new StreamWriter(fileStream))
                {
                    base.Visit(assemblies);
                }
            }
        }

        public override void Visit(Assembly assembly)
        {
            base.Visit(assembly);
        }

        public override void VisitNamespace(string namespaceName, IEnumerable<Type> types)
        {
            _writer.WriteLine($"namespace {namespaceName}");
            _writer.WriteLine("{");
            base.VisitNamespace(namespaceName, types);
            _writer.WriteLine("}");
            _writer.WriteLine("");
        }

        public override void VisitClass(Type type)
        {
            _writer.WriteLine($"\tclass {GetTypeName(type)}");
            _writer.WriteLine("\t{");
            base.VisitClass(type);
            _writer.WriteLine("\t}");
            _writer.WriteLine("\t");
        }

        public override void VisitMethod(MethodInfo methodInfo)
        {
            base.VisitMethod(methodInfo);
            _writer.WriteLine($"\t\t{GetTypeName(methodInfo.ReturnType)} {methodInfo.Name}({GetParameters(methodInfo)});");
        }

        public override void VisitProperty(PropertyInfo propertyInfo)
        {
            _writer.WriteLine($"\t\t{GetTypeName(propertyInfo.PropertyType)} {propertyInfo.Name} {{{GetGetSet(propertyInfo)}}}");
        }

        private static string GetParameters(MethodInfo methodInfo)
        {
            return string.Join(", ", methodInfo.GetParameters().Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));
        }

        private static string GetGetSet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.CanRead)
            {
                if (propertyInfo.CanWrite)
                {
                    return " get; set; ";
                }
                return " get; ";
            }
            else
            {
                if (propertyInfo.CanWrite)
                {
                    return " set; ";
                }
            }

            return "";
        }

        public override void VisitEnum(Type type)
        {
            _writer.WriteLine($"\tenum {type.Name}");
            _writer.WriteLine("\t{");
            List<string> nameValues = new List<string>();
            foreach (var field in type.GetFields())
            {
                if (field.Name == "value__")
                {
                    continue;
                }
                nameValues.Add($"\t\t{field.Name} = {field.GetRawConstantValue()}");
            }
            _writer.WriteLine(string.Join(",\n", nameValues));
            _writer.WriteLine("\t}");
            _writer.WriteLine();
        }

        private static string GetTypeName(Type type)
        {
            string friendlyName;
            if (_typeToFriendlyTypeName.TryGetValue(type, out friendlyName))
            {
                return friendlyName;
            }

            friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }
                friendlyName += $"<{string.Join(", ", type.GetGenericArguments().Select(typeParameter => GetTypeName(typeParameter)))}>";
                return friendlyName;
            }

            if (type.IsArray)
            {
                return GetTypeName(type.GetElementType()) + "[]";
            }

            return friendlyName;
        }
    }
}
