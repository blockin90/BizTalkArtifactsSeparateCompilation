using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler.Resolvers
{
    public static class ProjectResolver
    {
        const string baseDirRelativePath = "../../../"; // bin/debug/testprojectdir

        static Dictionary<string, Project> _projects;
        static Dictionary<string, Project> Projects
        {
            get
            {
                return _projects ?? (_projects = new Dictionary<string, Project>());
            }
        }
        private static string ResolveProjectPath(string assemblyName)
        {
            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, baseDirRelativePath, $@"{assemblyName}\{assemblyName}.btproj");
        }

        public static Project GetProjectByAssemblyName(string assemblyName)
        {
            Project project;
            if (Projects.TryGetValue(assemblyName, out project)) {
                return project;
            }
            var projectPath = ResolveProjectPath(assemblyName);
            project = new Project(projectPath);
            Projects.Add(assemblyName, project);
            return project;
        }
    }
}
