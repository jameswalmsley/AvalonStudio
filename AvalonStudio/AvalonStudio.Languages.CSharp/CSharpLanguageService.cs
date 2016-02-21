namespace AvalonStudio.Languages.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AvalonStudio.Projects;

    public class CSharpLanguageService : ILanguageService
    {
        public Type BaseTemplateType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Title
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanHandle(ISourceFile file)
        {
            throw new NotImplementedException();
        }

        public List<CodeCompletionData> CodeCompleteAt(ISourceFile sourceFile, int line, int column, List<UnsavedFile> unsavedFiles, string filter)
        {
            throw new NotImplementedException();
        }

        public IList<AvalonStudio.TextEditor.Rendering.IBackgroundRenderer> GetBackgroundRenderers(ISourceFile file)
        {
            throw new NotImplementedException();
        }

        public IList<AvalonStudio.TextEditor.Rendering.IDocumentLineTransformer> GetDocumentLineTransformers(ISourceFile file)
        {
            throw new NotImplementedException();
        }

        public void RegisterSourceFile(ISourceFile file, AvalonStudio.TextEditor.Document.TextDocument textDocument)
        {
            throw new NotImplementedException();
        }

        public CodeAnalysisResults RunCodeAnalysis(ISourceFile file, List<UnsavedFile> unsavedFiles, Func<bool> interruptRequested)
        {
            throw new NotImplementedException();
        }

        public void UnregisterSourceFile(ISourceFile file)
        {
            throw new NotImplementedException();
        }
    }
}
