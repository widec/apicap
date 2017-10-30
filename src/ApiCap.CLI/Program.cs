using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiCap
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new CommandLineApplication();
            CommandArgument argument = application.Argument("assemblypattern", "The pattern to include all assemblies");
            application.HelpOption("-? | -h | --help");
            var outputOption = application.Option("-out | --output", "The path of the output file, default output.txt in the current folder.", CommandOptionType.SingleValue);
            application.OnExecute(() =>
                {
                    Console.WriteLine("APICapv 0.0.1.0");
                    Console.WriteLine("");

                    var files = Directory.GetFiles(Path.GetDirectoryName(argument.Value), Path.GetFileName(argument.Value), SearchOption.TopDirectoryOnly);
                    var assemblies = files.Select(path => Assembly.LoadFrom(path)).ToArray();
                    var outputPath = string.IsNullOrEmpty(outputOption.Value()) ? "output.txt" : outputOption.Value();

                    var visitor = new FileAssemblyVisitor(outputPath);

                    visitor.Visit(assemblies);

                    Console.WriteLine("File is written");
                    return 0;
                });
            application.Execute(args);
        }
    }
}
