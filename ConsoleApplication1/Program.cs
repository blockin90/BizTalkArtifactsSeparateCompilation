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
        static TaskItem[] GetProjectItems(Project project, string itemType)
        {
            var taskItems = project.GetItems(itemType)
               .Select(projectItem =>
               {
                   var metadata = projectItem.Metadata.ToDictionary(projectMetadata => projectMetadata.Name, projectMetadata => projectMetadata.EvaluatedValue);
                   var taskItem = new TaskItem(projectItem.EvaluatedInclude, metadata);
                   return taskItem;
               })
               .ToArray();
            return taskItems;
        }

        const int warningLevel = 4;
        const bool enableUnitTesting = false;
        const bool treatWarningAsError = false;

        static void Main(string[] args)
        {
            var project = new Project(@"c:\users\blokin\documents\visual studio 2015\Projects\BizTalk Server Project1\BizTalk Server Project1\BizTalk Server Project1.btproj");

            var refItems = GetProjectItems(project, "Reference");
            var schemaItems = GetProjectItems(project, "Schema");
            var mapItems = GetProjectItems(project, "Map");

            var rootNamespace = project.GetPropertyValue("RootNamespace");
            var schemaCompiler = new SchemaCompilerTask()
            {
                RootNamespace = rootNamespace,
                SchemaItems = schemaItems, 
                ProjectReferences = refItems,
                WarningLevel = warningLevel,
                EnableUnitTesting = enableUnitTesting, 
                TreatWarningAsError = treatWarningAsError
            };
            var mapCompiler = new MapperCompilerTask()
            {
                RootNamespace = rootNamespace,
                EnableUnitTesting = enableUnitTesting,
                WarningLevel = warningLevel,
                ProjectReferences = refItems,
                TreatWarningAsError = treatWarningAsError,
                SchemaItems = schemaItems,
                MapItems = mapItems
            };
            if (schemaCompiler.Execute() && mapCompiler.Execute()) {
                var filesToCompile = schemaCompiler.LastGeneratedCodeFiles
                    .Union(mapCompiler.LastGeneratedCodeFiles)
                    .Select( fi => fi.FullName )
                    .ToArray();
                CSharpCompiler csCompiler = new CSharpCompiler();
                csCompiler.Compile(filesToCompile);

            } else {
                Console.WriteLine("some errors");
            }
        }
    }
}
