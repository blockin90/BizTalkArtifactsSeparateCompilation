using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler
{
    public class SchemaBuildFileInfoEqualityComparer : IEqualityComparer<SchemaBuildFileInfo>
    {
        public bool Equals(SchemaBuildFileInfo x, SchemaBuildFileInfo y)
        {
            return x.FilePath.Equals(y.FilePath) || x.TypeName.Equals(y.TypeName);
        }

        public int GetHashCode(SchemaBuildFileInfo obj)
        {
            //todo: поменять логику для случаев, когда используем схему по имени типа
            return obj.FilePath.GetHashCode();
        }
    }
}
