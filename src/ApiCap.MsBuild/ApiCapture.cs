using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApiCap.MsBuild
{
    public class ApiCapture : Task
    {
        public override bool Execute()
        {
            var assemblies = Assemblies.Select(item => Assembly.LoadFrom(item.ItemSpec)).ToArray();
            var outputPath = OutputFileName == null ? "output.txt" : OutputFileName.ItemSpec;
            var visitor = new FileAssemblyVisitor(outputPath);

            visitor.Visit(assemblies);
            return true;
        }

        public ITaskItem[] Assemblies { get; set; }
        public ITaskItem OutputFileName { get; set; }

    }
}
