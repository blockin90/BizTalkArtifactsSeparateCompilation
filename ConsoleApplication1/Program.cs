using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using Microsoft.VisualStudio.BizTalkProject.Compiler;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
            ProjectCollection collection = new ProjectCollection();
            //collection.LoadProject()

            var project = new Project(@"c:\users\blokin\documents\visual studio 2015\Projects\BizTalk Server Project1\BizTalk Server Project1\BizTalk Server Project1.btproj");



            var refsItems = project.GetItems("Reference")
                .Select(projectItem => {
                    var metadata = projectItem.Metadata.ToDictionary(projectMetadata => projectMetadata.Name, projectMetadata => projectMetadata.EvaluatedValue);
                    var taskItem = new TaskItem(projectItem.EvaluatedInclude, metadata);
                    return taskItem;
                    })
                .ToArray();

                /*
                    new TaskItem("Microsoft.BizTalk.DefaultPipelines",new Dictionary<string,string>
                    {
                        { "FullPath", @"C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\Microsoft.BizTalk.DefaultPipelines\v4.0_3.0.1.0__31bf3856ad364e35\Microsoft.BizTalk.DefaultPipelines.dll" }
                    }),*/
            foreach (var item in project.GetItems("Reference")) {
                Console.WriteLine($"Name: {item.EvaluatedInclude}, Version: {item.GetMetadata("Version")}");
            }
            foreach (var item in project.GetItems("Schema")) {
                Console.WriteLine($"Name: {item.EvaluatedInclude}, Version: {item.GetMetadata("Version")}");
            }
            return;

            var schemaCompiler = new SchemaCompiler()
            {
                
            };

            var mapCompiler = new MyMapperCompiler()
            {
                RootNamespace = $"ConsoleApplication1Namespace",
                EnableUnitTesting = false,
                WarningLevel = 4,
                TreatWarningAsError = false,
                MapItems = new[] {
                    new TaskItem(
                        "Map1.btm",
                        new Dictionary<string, string>
                        {
                            {"TypeName","Map1" },
                            {"Namespace", "BizTalk_Server_Project1" },
                            {"FileName", "Map1.btm" },
                            {"Directory", new FileInfo(@"c:\users\blokin\documents\visual studio 2015\Projects\BizTalk Server Project1\BizTalk Server Project1\Map1.btm" ).DirectoryName },
                        })
                },
                SchemaItems = new[]
                {
                    new TaskItem
                    (
                        "Schema1.xsd",
                        new Dictionary<string,string>()
                        {
                            { "TypeName","Schema1" },
                            { "Namespace", "BizTalk_Server_Project1"  },
                            {"FullPath", @"c:\users\blokin\documents\visual studio 2015\Projects\BizTalk Server Project1\BizTalk Server Project1\Schema1.xsd" }
                        }
                     )
                },
                ProjectReferences = new[]
                {
                    new TaskItem("Microsoft.BizTalk.DefaultPipelines",new Dictionary<string,string>
                    {
                        { "FullPath", @"C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\Microsoft.BizTalk.DefaultPipelines\v4.0_3.0.1.0__31bf3856ad364e35\Microsoft.BizTalk.DefaultPipelines.dll" }
                    }),
                    new TaskItem("Microsoft.BizTalk.GlobalPropertySchemas",new Dictionary<string,string>
                    {
                        { "FullPath", @"C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\Microsoft.BizTalk.GlobalPropertySchemas\v4.0_3.0.1.0__31bf3856ad364e35\Microsoft.BizTalk.GlobalPropertySchemas.dll" }
                    }),
                    new TaskItem("Microsoft.BizTalk.Pipeline",new Dictionary<string,string>
                    {
                        { "FullPath", @"D:\Develop\Microsoft Visual Studio 14.0\Common7\IDE\PublicAssemblies\Microsoft.BizTalk.Pipeline.dll" }
                    }),
                    new TaskItem("Microsoft.BizTalk.TestTools",new Dictionary<string,string>
                    {
                        { "FullPath", @"D:\Develop\Microsoft Visual Studio 14.0\Common7\IDE\PublicAssemblies\Microsoft.BizTalk.TestTools.dll" }
                    }),
                    new TaskItem("Microsoft.XLANGs.BaseTypes",new Dictionary<string,string>
                    {
                        { "FullPath", @"D:\Develop\Microsoft Visual Studio 14.0\Common7\IDE\PublicAssemblies\Microsoft.XLANGs.BaseTypes.dll" }
                    }),
                    new TaskItem("System",new Dictionary<string,string>
                    {
                        { "FullPath", @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.dll" }
                    }),
                }
            };


            var result = mapCompiler.Execute();
            CSharpCompiler csCompiler = new CSharpCompiler();
            csCompiler.Compile("Map1.btm.cs");

        }
    }
}
