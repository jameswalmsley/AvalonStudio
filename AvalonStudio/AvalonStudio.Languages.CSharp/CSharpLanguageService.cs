namespace AvalonStudio.Languages.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AvalonStudio.Projects;
    using Microsoft.CodeAnalysis;
    using Microsoft.CSharp;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.IO;
    using TextEditor.Document;
    using TextEditor.Rendering;
    using System.Runtime.CompilerServices;
    using Rendering;
    using Microsoft.CodeAnalysis.Text;
    class CSharpDataAssociation
    {
        public CSharpDataAssociation(TextDocument textDocument)
        {
            BackgroundRenderers = new List<IBackgroundRenderer>();
            DocumentLineTransformers = new List<IDocumentLineTransformer>();

            // Maybe the C++ one can be shared?
            TextColorizer = new TextColoringTransformer(textDocument);
            TextMarkerService = new TextMarkerService(textDocument);

            DocumentLineTransformers.Add(TextColorizer);
            DocumentLineTransformers.Add(TextMarkerService);            
        }

        public CSharpSyntaxTree SyntaxTree { get; set; }
        public TextColoringTransformer TextColorizer { get; private set; }
        public TextMarkerService TextMarkerService { get; private set; }
        public List<IBackgroundRenderer> BackgroundRenderers { get; private set; }
        public List<IDocumentLineTransformer> DocumentLineTransformers { get; private set; }
    }

    public class CSharpLanguageService : ILanguageService
    {
        private static ConditionalWeakTable<ISourceFile, CSharpDataAssociation> dataAssociations = new ConditionalWeakTable<ISourceFile, CSharpDataAssociation>();

        public CSharpLanguageService()
        {

        }

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
                return "C#";
            }
        }

        public bool CanHandle(ISourceFile file)
        {
            bool result = false;

            switch (Path.GetExtension(file.Location))
            {
                case ".cs":                
                    result = true;
                    break;
            }

            return result;            
        }

        private CSharpDataAssociation GetAssociatedData(ISourceFile sourceFile)
        {
            CSharpDataAssociation result = null;

            if (!dataAssociations.TryGetValue(sourceFile, out result))
            {
                throw new Exception("Tried to parse file that has not been registered with the language service.");
            }

            return result;
        }

        private SyntaxTree GetAndParseTranslationUnit(ISourceFile sourceFile)
        {
            var dataAssociation = GetAssociatedData(sourceFile);

            if (dataAssociation.SyntaxTree == null)
            {
                dataAssociation.SyntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(File.OpenRead(sourceFile.Location))) as CSharpSyntaxTree;                
            }
            else
            {
                //dataAssociation.SyntaxTree.Reparse(unsavedFiles.ToArray(), ReparseTranslationUnitFlags.None);
            }

            return dataAssociation.SyntaxTree;
        }

        public List<CodeCompletionData> CodeCompleteAt(ISourceFile sourceFile, int line, int column, List<UnsavedFile> unsavedFiles, string filter)
        {
            throw new NotImplementedException();
        }

        public IList<IDocumentLineTransformer> GetDocumentLineTransformers(ISourceFile file)
        {
            var associatedData = GetAssociatedData(file);

            return associatedData.DocumentLineTransformers;
        }

        public IList<IBackgroundRenderer> GetBackgroundRenderers(ISourceFile file)
        {
            var associatedData = GetAssociatedData(file);

            return associatedData.BackgroundRenderers;
        }

        public void RegisterSourceFile(ISourceFile file, AvalonStudio.TextEditor.Document.TextDocument textDocument)
        {
            CSharpDataAssociation existingAssociation = null;

            if (dataAssociations.TryGetValue(file, out existingAssociation))
            {
                throw new Exception("Source file already registered with language service.");
            }
            else
            {
                dataAssociations.Add(file, new CSharpDataAssociation(textDocument));
            }
        }

        public CodeAnalysisResults RunCodeAnalysis(ISourceFile file, List<UnsavedFile> unsavedFiles, Func<bool> interruptRequested)
        {
            var result = new CodeAnalysisResults();
            
            var dataAssociation = GetAssociatedData(file);

            var syntaxTree = GetAndParseTranslationUnit(file);

            

            var root = syntaxTree.GetCompilationUnitRoot();
            var tokens = root.DescendantNodesAndTokensAndSelf();            

            foreach (var token in tokens)
            {
                Console.WriteLine(token.GetType());
                if(token.AsNode() is TypeDeclarationSyntax)
                {
                    var node = token.AsNode() as TypeDeclarationSyntax;

                    Console.WriteLine(node);
                                                            
                    result.SyntaxHighlightingData.Add(new SyntaxHighlightingData() { Start = node.Identifier.Span.Start, Length = node.Identifier.Span.Length, Type = HighlightType.Keyword });
                }                                      
            }

            result.SyntaxHighlightingData.Add(new SyntaxHighlightingData() { Start = 0, Length = 22, Type = HighlightType.UserType });

            dataAssociation.TextColorizer.SetTransformations(result.SyntaxHighlightingData);

            return result;
        }

        public void UnregisterSourceFile(ISourceFile file)
        {
            dataAssociations.Remove(file);
        }
    }
}
