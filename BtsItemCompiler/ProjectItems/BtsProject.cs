using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler.ProjectItems
{
    public class BtsProject
    {
        readonly Project _project;
        readonly ITaskItem[] _projectTaskItems;

        Dictionary<Tuple<string, BtsProjectItemType>, BtsProjectItem> _types;
        Dictionary<Tuple<string, BtsProjectItemType>, BtsProjectItem> Types
        {
            get
            {
                return _types ?? (_types = new Dictionary<Tuple<string, BtsProjectItemType>, BtsProjectItem>());
            }
        }

        public string RootNamespace { get { return _project.GetPropertyValue("RootNamespace"); } }

        public BtsProject(Project project)
        {
            _project = project;
            _projectTaskItems = GetProjectItems(BtsProjectItemType.Schema).
                Concat( GetProjectItems(BtsProjectItemType.Map))
                .ToArray();
        }

        IEnumerable<TaskItem> GetProjectItems(BtsProjectItemType itemType)
        {
            return _project.GetItems(itemType.ToString())
               .Select(projectItem =>
               {
                   var metadata = projectItem.Metadata.ToDictionary(projectMetadata => projectMetadata.Name, projectMetadata => projectMetadata.EvaluatedValue);
                   var taskItem = new TaskItem(projectItem.EvaluatedInclude, metadata);
                   return taskItem;
               });
        }

        public BtsProjectItem GetTaskItemByTypeName(BtsProjectItemType itemType, string clrTypeName)
        {
            BtsProjectItem projectItemWrapper;
            if (Types.TryGetValue(Tuple.Create(clrTypeName, itemType), out projectItemWrapper)) {
                return projectItemWrapper;
            }

            var taskItem = _projectTaskItems.First(item => item.GetMetadata("TypeName") == clrTypeName);
            projectItemWrapper = BtsProjectItem.FromProject(_project, taskItem, clrTypeName);
            Types.Add(Tuple.Create(clrTypeName, itemType), projectItemWrapper);

            return projectItemWrapper;
        }

        public IEnumerable<string> GetReferences()
        {
            //считаем, что ссылок в проекте немного относительно общего кол-ва хранимых артефактов, поэтому пока не кэшируем.
            var projectItems = GetProjectItems(BtsProjectItemType.Reference);
            return projectItems
                .Select(item => item.GetMetadata("FullPath") + ".dll" );
        }

    }
}