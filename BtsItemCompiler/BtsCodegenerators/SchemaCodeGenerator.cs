using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.Compiler;
using Microsoft.Build.Framework;

namespace BtsItemCompiler.BtsCodegenerators
{
    public class SchemaCodeGenerator : BtsCodeGeneratorBase
    {
        public SchemaCodeGenerator(IEnumerable<BizTalkFileInfo> filesToCompile, IEnumerable<string> projectReferencesPaths)
            : base(filesToCompile, projectReferencesPaths)
        {
        }

        protected override void GenerateCodeFiles(CodeGeneratorBuildSnapshot buildSnapshot, out BizTalkErrorCollection errors)
        {
            var schemaFiles = buildSnapshot.GetCompilableProjectFiles().OfType<SchemaFileInfo>();
            errors = new SchemaCompiler()
                .Compile(buildSnapshot, schemaFiles, out _generatedCodeFiles);
        }
    }
}