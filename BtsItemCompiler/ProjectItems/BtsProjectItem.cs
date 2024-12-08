using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler.ProjectItems
{
    public class BtsProjectItem
    {
        //public Project Project { get; }
        public ITaskItem ITaskItem { get; }
        public string FullPath { get; }
        public string TypeName { get; }

        public static BtsProjectItem FromProject(Project project, ITaskItem taskItem, string typeName)
        {
            return new BtsProjectItem(project, taskItem, typeName);
        }

        private BtsProjectItem(Project project, ITaskItem taskItem, string typeName)
        {
            ITaskItem = taskItem;
            FullPath = Path.Combine(project.DirectoryPath, Path.GetFileName(taskItem.ToString()));
            TypeName = typeName;
        }

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is BtsProjectItem) {
                var second = (BtsProjectItem)obj;
                return FullPath.Equals(second.FullPath);
            }
            return false;            
        }

        public override string ToString()
        {
            return ITaskItem.ToString();
        }
    }
}