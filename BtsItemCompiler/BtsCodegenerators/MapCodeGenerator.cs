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
    public class MapCodeGenerator : BtsCodeGeneratorBase
    {
        protected List<XsltFileInfo> _generatedXsltFiles;
        public List<XsltFileInfo> GeneratedXsltFiles
        {
            get
            {
                return _generatedXsltFiles;
            }
        }

        public MapCodeGenerator(IEnumerable<BizTalkFileInfo> filesToCompile, IEnumerable<string> projectReferencesPaths)
            : base(filesToCompile, projectReferencesPaths)
        {
        }

        protected override void GenerateCodeFiles(CodeGeneratorBuildSnapshot buildSnapshot, out BizTalkErrorCollection errors)
        {
            var schemaFiles = buildSnapshot.GetCompilableProjectFiles().OfType<SchemaFileInfo>();
            var mapFiles = buildSnapshot.GetCompilableProjectFiles().OfType<MapFileInfo>();
            errors = new MapCompiler()
                .Compile(buildSnapshot,  mapFiles, schemaFiles, out _generatedCodeFiles, out _generatedXsltFiles );
        }
    }
}