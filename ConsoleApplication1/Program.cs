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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    class Program
    {
        static TaskItem[] GetProjectItems(Project project, string itemType, string evaluatedIncludeName = "")
        {
            ICollection<ProjectItem> allItems = null;
            if (string.IsNullOrEmpty(evaluatedIncludeName)) {
                allItems = project.GetItems(itemType);
            } else {
                allItems = project.GetItemsByEvaluatedInclude(evaluatedIncludeName);
            }
            var taskItems = allItems
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

        class SchemaFile
        {
            public string Name { get; set; }
            public List<SchemaFile> Dependencies { get; set; } = new List<SchemaFile>();
            public override string ToString()
            {
                return Name;
            }
        }

        static List<SchemaFile> SortFiles(List<SchemaFile> files)
        {
            var sortedFiles = new List<SchemaFile>(files.Count);

            while (files.Any()) {
                var nextFile = files.FirstOrDefault(f => !f.Dependencies.Except(sortedFiles).Any());
                if (nextFile == null)
                    throw new Exception("Невозможно отсортировать файлы из-за циклических зависимостей");

                sortedFiles.Add(nextFile);
                files.Remove(nextFile);
            }
            return sortedFiles;
        }

        static void Main(string[] args)
        {
            SchemaFile f1 = new SchemaFile
            {
                Name = "f1"
            };
            SchemaFile f2 = new SchemaFile
            {
                Name = "f2"
            };
            SchemaFile f3 = new SchemaFile
            {
                Name = "f3"
            };

            f1.Dependencies.Add(f2);
            f2.Dependencies.Add(f3);

            var currentEnv = Environment.CurrentDirectory;
            Environment.CurrentDirectory = @"D:\Repos\BizTalkArtifactsSeparateCompilation\BizTalk Server Project1\";
            //var result = SortFiles(new List<SchemaFile> { f1, f2, f3 });
            //Console.WriteLine(result[0]);
            //Console.WriteLine(result[1]);
            //Console.WriteLine(result[2]);
            //return;

            var project = new Project(@"D:\Repos\BizTalkArtifactsSeparateCompilation\BizTalk Server Project1\BizTalk Server Project1.btproj");
            
            var refItems = GetProjectItems(project, "Reference");


            var schema1Item = GetProjectItems(project, "Schema", "Schema1.xsd");
            var schema2Item = GetProjectItems(project, "Schema", "Schema2.xsd");
            var mapItems = GetProjectItems(project, "Map", "Map1.btm");

            var rootNamespace = project.GetPropertyValue("RootNamespace");
            var schema2Compiler = new SchemaCompilerTask()
            {
                RootNamespace = rootNamespace,
                SchemaItems = schema1Item.Union(schema2Item).ToArray(),
                ProjectReferences = refItems,
                WarningLevel = warningLevel,
                EnableUnitTesting = enableUnitTesting,
                TreatWarningAsError = treatWarningAsError
            };

            Assembly schema2Assembly = null;


            if (schema2Compiler.Execute()) {

            Environment.CurrentDirectory = currentEnv;
                CSharpCompiler csCompiler = new CSharpCompiler();
                
                schema2Assembly = csCompiler.Compile(schema2Compiler.LastGeneratedCodeFiles.Select(fi => fi.FullName).ToArray());
            }


            //var schema1Compiler = new SchemaCompilerTask()
            //{
            //    RootNamespace = rootNamespace,
            //    SchemaItems = schema1Item,
            //    ProjectReferences = refItems.Union(new[]
            //    {
            //        new TaskItem( 
            //            schema2Assembly.GetName().Name,
            //            new Dictionary<string,string>()
            //            {
            //                { "FullPath",schema2Assembly.Location  },
            //                { "FullName",schema2Assembly.Location  },
            //            }
            //            )})
            //            .ToArray(),
            //    WarningLevel = warningLevel,
            //    EnableUnitTesting = enableUnitTesting,
            //    TreatWarningAsError = treatWarningAsError
            //};

            //if (schema1Compiler.Execute()) {

            //    CSharpCompiler csCompiler = new CSharpCompiler();
            //    schema1Assembly = csCompiler.Compile(schema1Compiler.LastGeneratedCodeFiles.Select(fi => fi.FullName).ToArray());
            //}

            //var mapCompiler = new MapperCompilerTask()
            //{
            //    RootNamespace = rootNamespace,
            //    EnableUnitTesting = enableUnitTesting,
            //    WarningLevel = warningLevel,
            //    ProjectReferences = refItems,
            //    TreatWarningAsError = treatWarningAsError,
            //    SchemaItems = schema1Item.Union(schema2Item).ToArray(),
            //    MapItems = mapItems
            //};


            //if (mapCompiler.Execute()) {
            //    var filesToCompile = mapCompiler.LastGeneratedCodeFiles
            //        .Select(fi => fi.FullName)
            //        .ToArray();
            //    CSharpCompiler csCompiler = new CSharpCompiler()
            //    {
            //        AdditionalReferences =new[]{ schemaAssembly.Location }
            //    };

            //    csCompiler.Compile(filesToCompile);

            //} else {
            //    Console.WriteLine("some errors");
            //}
        }
    }
}
