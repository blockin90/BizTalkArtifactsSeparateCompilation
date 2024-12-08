using BtsItemCompiler;
using BtsItemCompiler.BtsCodegenerators;
using BtsItemCompiler.Compilers;
using BtsItemCompiler.ProjectItems;
using BtsItemCompiler.Resolvers;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.IO;
using System.Linq;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             для схем:
             1. по имени типа получаем соответствующие TaskItem'ы проекта
             2. определяем их зависимости (импорты, инклуды, рекурсивно) - или не нужно, если будут прописаны относительные пути (?)
             3. по полученному списку файлов генерим .cs файлы
             4. полученные .cs файл компилируем в сборку
             5. возвращаем сборку
             */

            /*
             для преобразований в текущей реализации схемы подкладываются в виде файлов, которые потом компилируются вместе с .btm файлов
             в теории можно компилить их отдельно, и добавлять как зависимость
             */


            //инфу по именам типов можем получить из конфига
            var schema1TypeNama = "Schema1";
            var schema2TypeNama = "Schema2";
            var assemblyName = "BizTalk Server Project1"; 

            var compiledSchema1 = BtsCompiler.CompileSchema(assemblyName, schema1TypeNama);

            
            var project = ProjectResolver.GetProjectByAssemblyName(assemblyName);

            var projectWrapper = new BtsProject(project);
            var referencesPaths = projectWrapper.GetReferences();
            
            var mapItems = new[]
            {
                projectWrapper.GetTaskItemByTypeName( BtsProjectItemType.Map, "Map1")
            };

            var schemaFiles = SchemaResolver.GetSchemasWithDependencies(projectWrapper, schema1TypeNama)
                .Union(SchemaResolver.GetSchemasWithDependencies(projectWrapper, schema2TypeNama))                
                .Distinct(new SchemaBuildFileInfoEqualityComparer())
                .ToArray();
                //schemaItems.Select(projectItem => SchemaBuildFileInfo.GetSchemaFileInfo(projectItem.ITaskItem, projectWrapper.RootNamespace));

            var filesToMapCompile = mapItems.Select(projectItem => MapBuildFileInfo.GetMapperFileInfo(projectItem.ITaskItem, projectWrapper.RootNamespace))
                .Cast<BizTalkFileInfo>()
                .Concat(schemaFiles);


            //для дальнейшей кодогенерации/компиляции нужно, чтобы все файлы располагались в Environment.CurrentDirectory, копируем их все в одну папку     
            var realFilePaths = filesToMapCompile.Select(item => Path.Combine(project.DirectoryPath, Path.GetFileName(item.FilePath.ToString())));
            foreach (var filePath in realFilePaths) {
                File.Copy(filePath, Path.GetFileName(filePath), true);
            }

            var schemaCodeGenerator = new SchemaCodeGenerator(schemaFiles, referencesPaths);

            
            var mapCodeGenerator = new MapCodeGenerator(filesToMapCompile, referencesPaths);
            schemaCodeGenerator.Execute();
            mapCodeGenerator.Execute();

            var filesToCompile = mapCodeGenerator.GeneratedCodeFiles.Concat(schemaCodeGenerator.GeneratedCodeFiles);

            filesToCompile = filesToCompile.Reverse();

            var assembly = CSharpCompiler.Compile(
                projectWrapper.GetReferences(),
                filesToCompile.Select(fileInfo => fileInfo.FullName).ToArray());

            Console.WriteLine(assembly.Location);
        }
    }
}
