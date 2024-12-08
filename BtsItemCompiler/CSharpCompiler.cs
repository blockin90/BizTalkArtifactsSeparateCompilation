using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler
{
    public static class CSharpCompiler
    {
        public static Assembly Compile( IEnumerable<string> references, params string[] filePath)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters parameters = new CompilerParameters();

            // parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(@"D:\Develop\Microsoft Visual Studio 14.0\Common7\IDE\PublicAssemblies\Microsoft.XLANGs.BaseTypes.dll");
                        
           // parameters.ReferencedAssemblies.AddRange(references.ToArray());

            parameters.OutputAssembly = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".dll");

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, filePath);

            if (results.Errors.HasErrors) {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors) {
                    sb.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }

                throw new InvalidOperationException(sb.ToString());
            }

            return results.CompiledAssembly;
        }
    }
}
