using Microsoft.BizTalk.Studio.Extensibility;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public sealed class SchemaCompilerTask : BizTalkBaseTask
    {
        private ITaskItem[] schemaItems;
        private ITaskItem[] projectReferences;

        [Required]
        public bool EnableUnitTesting
        {
            get; set;
        }

        [Required]
        public ITaskItem[] SchemaItems
        {
            get;
            set;
        }

        [Required]
        public ITaskItem[] ProjectReferences
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
            get;
            set;
        }

        public bool TreatWarningAsError
        {
            get; set;
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
            List<SchemaBuildFileInfo> schemaFilesToCompile = new List<SchemaBuildFileInfo>(SchemaFiles);
            List<string> projectReferences = new List<string>(References);
            SchemaBuildSnapshot schemaBuildSnapshot = new SchemaBuildSnapshot(GetServiceProvider(), projectReferences, schemaFilesToCompile);
            schemaBuildSnapshot.ProjectConfigProperties[DictionaryTags.WarningLevel] = WarningLevel;
            schemaBuildSnapshot.ProjectConfigProperties[DictionaryTags.TreatWarningsAsErrors] = TreatWarningAsError;
            schemaBuildSnapshot.ProjectConfigProperties[DictionaryTags.EnableUnitTesting] = EnableUnitTesting;
            BizTalkErrorCollection errors = null;
            if (schemaFilesToCompile.Count > 0) {
                errors = new Microsoft.VisualStudio.BizTalkProject.Compiler.SchemaCompiler()
                    .Compile((BizTalkBuildSnapshot)schemaBuildSnapshot, schemaBuildSnapshot.SchemaFilesToCompile, out _lastGeneratedCodeFiles);

                foreach (IBizTalkError error in errors) {
                    var errorType = error.get_Type();
                    if (errorType  == BtsErrorType.Error || errorType == BtsErrorType.FatalError) {
                        LastErrors.Add(error);
                    }
                }
            }
            return _lastErrors == null || _lastErrors.Count == 0;
        }
    }
}
