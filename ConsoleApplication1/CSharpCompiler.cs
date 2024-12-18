﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class CSharpCompiler
    {
        public string[] AdditionalReferences { get; set; }

        public Assembly Compile(params string[] filePath)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters parameters = new CompilerParameters();

            // parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("Microsoft.XLANGs.BaseTypes.dll");
            
            if (AdditionalReferences != null) {
                parameters.ReferencedAssemblies.AddRange(AdditionalReferences);
            }
            parameters.OutputAssembly = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() +".dll");

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
