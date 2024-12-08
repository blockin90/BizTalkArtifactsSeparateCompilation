using BtsItemCompiler.ProjectItems;
using BtsItemCompiler.Resolvers;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BtsItemCompiler.Compilers
{
    public static class BtsCompiler
    {
        public static Type CompileSchema(string assemblyName, string typeName)
        {
            var project = ProjectResolver.GetProjectByAssemblyName(assemblyName);
            var projectWrapper = new BtsProject(project);

            var schemas = SchemaResolver.GetSchemasWithDependencies(projectWrapper, typeName);
            
            return Type.GetType("");
        }
        
    }
}
