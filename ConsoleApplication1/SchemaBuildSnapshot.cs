using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    internal class SchemaBuildSnapshot : BizTalkBuildSnapshot
    {
        private List<string> projectReferences;
        private List<SchemaBuildFileInfo> schemaFilesToCompile;

        public SchemaBuildSnapshot(
          object serviceProvider,
          List<string> projectReferences,
          List<SchemaBuildFileInfo> schemaFilesToCompile)
          : base(serviceProvider)
        {
            this.projectReferences = projectReferences;
            this.schemaFilesToCompile = schemaFilesToCompile;
        }

        public IEnumerable<SchemaFileInfo> SchemaFilesToCompile
        {
            get
            {
                foreach (SchemaBuildFileInfo schemaBuildFileInfo in schemaFilesToCompile)
                    yield return schemaBuildFileInfo;
            }
        }

        public override List<string> GetProjectReferences() => projectReferences;

        public override List<BizTalkFileInfo> GetCompilableProjectFiles()
        {
            List<BizTalkFileInfo> bizTalkFileInfoList = new List<BizTalkFileInfo>();
            foreach (BizTalkFileInfo bizTalkFileInfo in SchemaFilesToCompile)
                bizTalkFileInfoList.Add(bizTalkFileInfo);
            return bizTalkFileInfoList;
        }
    }
}
