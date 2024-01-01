using Microsoft.BizTalk.Studio.Extensibility;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using Microsoft.VisualStudio.BizTalkProject.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public sealed class MyMapperCompiler : BizTalkBaseTask
    {

        [Required]
        public bool EnableUnitTesting
        {
            get; set;
        }

        [Required]
        public ITaskItem[]
        MapItems
        {
            get; set;
        }

        [Required]
        public ITaskItem[] SchemaItems
        {
            get; set;
        }

        [Required]
        public ITaskItem[]
        ProjectReferences
        {
            get; set;
        }

        [Required]
        public int WarningLevel
        {
            get; set;
        }

        [Required]
        public string RootNamespace
        {
            get; set;
        }

        public bool TreatWarningAsError
        {
            get; set;
        }

        private IEnumerable<MapBuildFileInfo> MapFiles
        {
            get
            {
                foreach (ITaskItem mapItem in this.MapItems)
                    yield return MapBuildFileInfo.GetMapperFileInfo(mapItem, this.RootNamespace);
            }
        }

        private IEnumerable<SchemaBuildFileInfo> SchemaFiles
        {
            get
            {
                foreach (ITaskItem schemaItem in this.SchemaItems)
                    yield return SchemaBuildFileInfo.GetSchemaFileInfo(schemaItem, this.RootNamespace);
            }
        }

        private IEnumerable<string> References
        {
            get
            {
                foreach (ITaskItem projectReference in this.ProjectReferences)
                    yield return projectReference.GetMetadata("FullPath");
            }
        }

        public override bool Execute()
        {
            List<MapBuildFileInfo> mapFilesToCompile = new List<MapBuildFileInfo>(this.MapFiles);
            List<SchemaBuildFileInfo> schemaFilesToCompile = new List<SchemaBuildFileInfo>(this.SchemaFiles);
            List<string> projectReferences = new List<string>(this.References);
            MapperBuildSnapshot mapperBuildSnapshot = new MapperBuildSnapshot(this.GetServiceProvider(), projectReferences, mapFilesToCompile, schemaFilesToCompile);
            mapperBuildSnapshot.ProjectConfigProperties[DictionaryTags.WarningLevel] = (object)this.WarningLevel;
            mapperBuildSnapshot.ProjectConfigProperties[DictionaryTags.TreatWarningsAsErrors] = (object)this.TreatWarningAsError;
            mapperBuildSnapshot.ProjectConfigProperties[DictionaryTags.EnableUnitTesting] = (object)this.EnableUnitTesting;
            List<XsltFileInfo> xsltFiles = (List<XsltFileInfo>)null;
            List<FileInfo> generatedCodeFiles = (List<FileInfo>)null;
            BizTalkErrorCollection errors = (BizTalkErrorCollection)null;
            if (mapFilesToCompile.Count > 0)
                errors = new MapCompiler().Compile((BizTalkBuildSnapshot)mapperBuildSnapshot, mapperBuildSnapshot.MapFilesToCompile, mapperBuildSnapshot.SchemaFilesToCompile, out generatedCodeFiles, out xsltFiles);
            foreach (IBizTalkError error in errors) {
                if (error.get_Type() == BtsErrorType.Error || error.get_Type() == BtsErrorType.FatalError) {
                    Console.WriteLine(error.get_Text());
                    Console.WriteLine(error.get_Document());
                }
            }
            return true;
            // return !this.LogErrors(errors);
        }
    }
}
