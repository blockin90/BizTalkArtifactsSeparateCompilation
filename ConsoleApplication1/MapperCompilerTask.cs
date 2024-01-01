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

    public sealed class MapperCompilerTask : BizTalkBaseTask
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
                foreach (ITaskItem mapItem in MapItems)
                    yield return MapBuildFileInfo.GetMapperFileInfo(mapItem, RootNamespace);
            }
        }

        private IEnumerable<SchemaBuildFileInfo> SchemaFiles
        {
            get
            {
                foreach (ITaskItem schemaItem in SchemaItems)
                    yield return SchemaBuildFileInfo.GetSchemaFileInfo(schemaItem, RootNamespace);
            }
        }

        private IEnumerable<string> References
        {
            get
            {
                foreach (ITaskItem projectReference in ProjectReferences)
                    yield return projectReference.GetMetadata("FullPath");
            }
        }

        List<FileInfo> _lastGeneratedCodeFiles;
        public List<FileInfo> LastGeneratedCodeFiles
        {
            get
            {
                return _lastGeneratedCodeFiles;
            }
        }

        List<IBizTalkError> _lastErrors;
        public List<IBizTalkError> LastErrors
        {
            get
            {
                return _lastErrors ?? (_lastErrors = new List<IBizTalkError>());
            }
        }

        public override bool Execute()
        {
            List<MapBuildFileInfo> mapFilesToCompile = new List<MapBuildFileInfo>(MapFiles);
            List<SchemaBuildFileInfo> schemaFilesToCompile = new List<SchemaBuildFileInfo>(SchemaFiles);
            List<string> projectReferences = new List<string>(References);
            MapperBuildSnapshot mapperBuildSnapshot = new MapperBuildSnapshot(GetServiceProvider(), projectReferences, mapFilesToCompile, schemaFilesToCompile);
            mapperBuildSnapshot.ProjectConfigProperties[DictionaryTags.WarningLevel] = WarningLevel;
            mapperBuildSnapshot.ProjectConfigProperties[DictionaryTags.TreatWarningsAsErrors] = TreatWarningAsError;
            mapperBuildSnapshot.ProjectConfigProperties[DictionaryTags.EnableUnitTesting] = EnableUnitTesting;
            List<XsltFileInfo> xsltFiles = null;
            if (mapFilesToCompile.Count > 0) {
                var errors = new MapCompiler()
                    .Compile(mapperBuildSnapshot, mapperBuildSnapshot.MapFilesToCompile, mapperBuildSnapshot.SchemaFilesToCompile, out _lastGeneratedCodeFiles, out xsltFiles);

                foreach (IBizTalkError error in errors) {
                    var errorType = error.get_Type();
                    if (errorType == BtsErrorType.Error || errorType == BtsErrorType.FatalError) {
                        LastErrors.Add(error);
                    }
                }
            }
            return _lastErrors == null || _lastErrors.Count == 0;
        }
    }
}
