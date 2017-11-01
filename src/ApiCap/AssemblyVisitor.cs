using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiCap
{
    public class AssemblyVisitor
    {
        public virtual void Visit(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Visit(assembly);
            }
        }

        public virtual void Visit(Assembly assembly)
        {
            foreach (var types in assembly.GetExportedTypes().GroupBy(t => t.Namespace).OrderBy(g => g.Key))
            {
                VisitNamespace(types.Key, types);
            }
        }

        public virtual void VisitNamespace(string namespaceName, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Visit(type);
            }
        }

        public virtual void Visit(Type type)
        {
            if (type.IsEnum)
            {
                VisitEnum(type);
            }
            else if (type.IsClass)
            {
                VisitClass(type);
            }
            else if (type.IsInterface)
            {
                VisitInterface(type);
            }
        }

        public virtual void VisitEnum(Type type)
        {
        }

        public virtual void VisitClass(Type type)
        {
            foreach (var propertyInfo in type.GetProperties().OrderBy(pi => pi.Name))
            {
                VisitProperty(propertyInfo);
            }
            foreach (var methodInfo in type.GetMethods().OrderBy(mi => mi.Name))
            {
                if (!methodInfo.IsSpecialName)
                {
                    VisitMethod(methodInfo);
                }
            }
        }

        public virtual void VisitInterface(Type type)
        {
        }

        public virtual void VisitMethod(MethodInfo methodInfo)
        {
        }

        public virtual void VisitProperty(PropertyInfo propertyInfo)
        {
        }
    }

}
