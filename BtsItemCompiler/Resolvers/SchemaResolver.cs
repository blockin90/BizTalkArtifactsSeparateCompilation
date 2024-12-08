using BtsItemCompiler.ProjectItems;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BtsItemCompiler.Resolvers
{
    public class SchemaResolver
    {
        public static IEnumerable<SchemaBuildFileInfo> GetSchemasWithDependencies(BtsProject btsProject, string typeName)
        {
            BtsProjectItem schemaItem = btsProject.GetTaskItemByTypeName(BtsProjectItemType.Schema, typeName);            
            var schemaBuildInfo = SchemaBuildFileInfo.GetSchemaFileInfo(schemaItem.ITaskItem, btsProject.RootNamespace);
            var schemasList = new List<SchemaBuildFileInfo>()
            {
                schemaBuildInfo
            };
            ResolveDependencies(schemaBuildInfo, schemasList);

            
            return schemasList;
        }

        static void ResolveDependencies(SchemaBuildFileInfo currentItem, List<SchemaBuildFileInfo> dependencies)
        {
            var schemaDependencies = GetSchemaDependencies(currentItem);            
            foreach(var child in schemaDependencies) {
                if (!dependencies.Contains(child)) {
                    //todo: нужно поменять логику получения зависимостей - брать из проекта. если читать просто из файла - теряем CLR Namespace + имя типа, потом получаем ошибку сборки
                    //нужно как-то вытягивать из проекта, пока костыль - просто копируем эти атрибуты из родительского элемента
                    var schemaBuildInfo = new SchemaBuildFileInfo(child.FilePath, Path.GetFileNameWithoutExtension(child.FilePath), currentItem.NamespaceName, currentItem.DotNetName);
                    dependencies.Add(schemaBuildInfo);
                    ResolveDependencies(schemaBuildInfo, dependencies);
                }
            }
        }

        static IEnumerable<SchemaBuildFileInfo> GetSchemaDependencies(SchemaBuildFileInfo currentProjectItem)
        {
            var schemaDir = Path.GetDirectoryName(currentProjectItem.FilePath);

            XmlDocument currentSchemaDoc = new XmlDocument();
            currentSchemaDoc.Load(currentProjectItem.FilePath);
            var dependenciesSchemasNodes = currentSchemaDoc.SelectNodes("/*[local-name()='schema']/*[local-name()='include' or local-name()='import']");

            return dependenciesSchemasNodes.OfType<XmlElement>()
                .Select(e => e.GetAttribute("schemaLocation"))
                .Select(location => new FileInfo( Path.Combine(schemaDir, location) ).FullName) //через FileInfo избавляемся от относительных путей
                .Select( location=> new SchemaBuildFileInfo(location));
        }

    }
}
