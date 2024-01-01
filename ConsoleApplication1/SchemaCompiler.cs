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
    public sealed class SchemaCompiler : BizTalkBaseTask
    {
        private ITaskItem[] schemaItems;
        private ITaskItem[] projectReferences;

        [Required]
        public bool EnableUnitTesting
        {
            get; set;
        }

        [Required]
        public ITaskItem[]
        SchemaItems
        {
            get;
            set;
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
            List<SchemaBuildFileInfo> schemaFilesToCompile = new List<SchemaBuildFileInfo>(this.SchemaFiles);
            List<string> projectReferences = new List<string>(this.References);
            SchemaBuildSnapshot schemaBuildSnapshot = new SchemaBuildSnapshot(this.GetServiceProvider(), projectReferences, schemaFilesToCompile);
            schemaBuildSnapshot.ProjectConfigProperties[DictionaryTags.WarningLevel] = (object)this.WarningLevel;
            schemaBuildSnapshot.ProjectConfigProperties[DictionaryTags.TreatWarningsAsErrors] = (object)this.TreatWarningAsError;
            schemaBuildSnapshot.ProjectConfigProperties[DictionaryTags.EnableUnitTesting] = (object)this.EnableUnitTesting;
            List<FileInfo> generatedCodeFiles = (List<FileInfo>)null;
            BizTalkErrorCollection errors = (BizTalkErrorCollection)null;
            if (schemaFilesToCompile.Count > 0)
                errors = new Microsoft.VisualStudio.BizTalkProject.Compiler.SchemaCompiler().Compile((BizTalkBuildSnapshot)schemaBuildSnapshot, schemaBuildSnapshot.SchemaFilesToCompile, out generatedCodeFiles);
            return !this.LogErrors(errors);
        }
    }
}
