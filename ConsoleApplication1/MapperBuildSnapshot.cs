using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    // Decompiled with JetBrains decompiler
    // Type: Microsoft.VisualStudio.BizTalkProject.BuildTasks.MapperBuildSnapshot
    // Assembly: Microsoft.VisualStudio.BizTalkProject.BuildTasks, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
    // MVID: FFBF4ACE-587B-4542-8875-C7A808F70AF8
    // Assembly location: C:\Program Files (x86)\Microsoft BizTalk Server 2016\Developer Tools\Microsoft.VisualStudio.BizTalkProject.BuildTasks.dll
    internal class MapperBuildSnapshot : BizTalkBuildSnapshot
    {
        private List<string> projectReferences;
        private List<MapBuildFileInfo> mapFilesToCompile;
        private List<SchemaBuildFileInfo> schemaFilesToCompile;

        public MapperBuildSnapshot(
          object serviceProvider,
          List<string> projectReferences,
          List<MapBuildFileInfo> mapFilesToCompile,
          List<SchemaBuildFileInfo> schemaFilesToCompile
          )
          : base(serviceProvider)
        {
            this.projectReferences = projectReferences;
            this.mapFilesToCompile = mapFilesToCompile;
            this.schemaFilesToCompile = schemaFilesToCompile;
        }

        public IEnumerable<MapFileInfo> MapFilesToCompile
        {
            get
            {
                foreach (MapBuildFileInfo mapBuildFileInfo in this.mapFilesToCompile)
                    yield return (MapFileInfo)mapBuildFileInfo;
            }
        }

        public IEnumerable<SchemaFileInfo> SchemaFilesToCompile
        {
            get
            {
                foreach (SchemaBuildFileInfo schemaBuildFileInfo in this.schemaFilesToCompile)
                    yield return (SchemaFileInfo)schemaBuildFileInfo;
            }
        }

        public override List<string> GetProjectReferences() => this.projectReferences;

        public override List<BizTalkFileInfo> GetCompilableProjectFiles()
        {
            List<BizTalkFileInfo> bizTalkFileInfoList = new List<BizTalkFileInfo>();
            foreach (BizTalkFileInfo bizTalkFileInfo in this.MapFilesToCompile)
                bizTalkFileInfoList.Add(bizTalkFileInfo);
            foreach (BizTalkFileInfo bizTalkFileInfo in this.SchemaFilesToCompile)
                bizTalkFileInfoList.Add(bizTalkFileInfo);
            return bizTalkFileInfoList;
        }
    }
}
