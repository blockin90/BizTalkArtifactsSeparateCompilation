using Microsoft.VisualStudio.BizTalkProject.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler
{
    public class CodeGeneratorBuildSnapshot : BizTalkBuildSnapshot
    {
        private List<string> _projectReferences;
        private List<BizTalkFileInfo> _filesToCompile;

        public CodeGeneratorBuildSnapshot(object serviceProvider, List<string> projectReferences, List<BizTalkFileInfo> filesToCompile)
          : base(serviceProvider)
        {
            _projectReferences = projectReferences;
            _filesToCompile = filesToCompile;
        }

        public override List<string> GetProjectReferences() => _projectReferences;

        public override List<BizTalkFileInfo> GetCompilableProjectFiles()
        {
            return _filesToCompile.ToList();//create copy of source object
        }
    }
}
