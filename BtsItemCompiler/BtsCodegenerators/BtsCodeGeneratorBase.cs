using Microsoft.BizTalk.Studio.Extensibility;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.BizTalkProject.Base;
using Microsoft.VisualStudio.BizTalkProject.BuildTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtsItemCompiler.BtsCodegenerators
{

    public abstract class BtsCodeGeneratorBase : BizTalkBaseTask
    {
        readonly string _rootNamespace;
        readonly IEnumerable<string> _projectReferencesFullPaths;
        readonly IEnumerable<BizTalkFileInfo> _filesToCompile;

        protected List<FileInfo> _generatedCodeFiles;
        public List<FileInfo> GeneratedCodeFiles
        {
            get
            {
                return _generatedCodeFiles;
            }
        }

        BizTalkErrorCollection _errors;
        public IEnumerable<IBizTalkError> Errors
        {
            get
            {
                return _errors?.OfType<IBizTalkError>() ?? Enumerable.Empty<IBizTalkError>();
            }
        }

        public IEnumerable<IBizTalkError> CriticalErrors
        {
            get
            {
                return Errors.Where(error =>
                {
                    var errorType = error.get_Type();
                    return errorType == BtsErrorType.Error || errorType == BtsErrorType.FatalError;
                });
            }
        }


        public BtsCodeGeneratorBase(IEnumerable<BizTalkFileInfo> filesToCompile, IEnumerable<string> projectReferencesPaths)
        {
            _filesToCompile = filesToCompile;
            _projectReferencesFullPaths = projectReferencesPaths;
        }

        public override bool Execute()
        {
            ThrowIfInputFilesIsEmpty();
            GenerateCodeFiles(CreateBuildSnapshot(), out _errors);
            ThrowIfAnyCriticalErrors();
            return true;
        }

        private CodeGeneratorBuildSnapshot CreateBuildSnapshot()
        {
            var buildSnapshot = new CodeGeneratorBuildSnapshot(GetServiceProvider(), _projectReferencesFullPaths.ToList(), _filesToCompile.ToList());
            buildSnapshot.ProjectConfigProperties[DictionaryTags.WarningLevel] = 4;
            buildSnapshot.ProjectConfigProperties[DictionaryTags.TreatWarningsAsErrors] = false;
            buildSnapshot.ProjectConfigProperties[DictionaryTags.EnableUnitTesting] = false;
            return buildSnapshot;
        }

        void ThrowIfInputFilesIsEmpty()
        {
            if (!_filesToCompile.Any()) {
                throw new ArgumentException("Cannot execute code generation on empty input files collection");
            }
        }

        void ThrowIfAnyCriticalErrors()
        {
            var critErrors = CriticalErrors.ToArray();
            if (critErrors.Any()) {
                throw new Exception(GetErrorsDescriptionSummary(critErrors));
            }
        }

        string GetErrorsDescriptionSummary(IEnumerable<IBizTalkError> errors)
        {
            var errorsSummary = string.Join(Environment.NewLine,
                errors.Where(error =>
                {
                    var errorType = error.get_Type();
                    return errorType == BtsErrorType.Error || errorType == BtsErrorType.FatalError;
                })
                .Select(error => error.get_Text()));

            return errorsSummary;
        }


        protected abstract void GenerateCodeFiles(CodeGeneratorBuildSnapshot buildSnapshot, out BizTalkErrorCollection errors);
    }
}