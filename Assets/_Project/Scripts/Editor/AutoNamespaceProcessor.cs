using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace GameplaySystemsAndTools.Editor
{
    /// <summary>
    /// Automatically stamps folder-structure-based namespaces onto newly created C#
    /// scripts, following the project architecture (AGENTS.md / PROJECT_ARCHITECTURE.md).
    /// The namespace mirrors the folder path down to the FEATURE ROOT only: structural
    /// folders (Logic, Components, View, Data, Input, States, ...) do not extend it.
    /// Editor is the one exception - it is always appended (asmdef rule).
    /// Ex: Assets/_Project/Scripts/Features/Player/Logic/States/RootStates/PlayerIdleState.cs
    /// -> namespace GameplaySystemsAndTools.Features.Player
    /// </summary>
    public class AutoNamespaceProcessor : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (!path.EndsWith(".cs")) return;

            // Wait one frame so the script template has been written to disk.
            EditorApplication.delayCall += () =>
            {
                if (!File.Exists(path)) return;

                string expectedNamespace = GetNamespaceFromPath(path);
                if (string.IsNullOrEmpty(expectedNamespace)) return;

                string fileContent = File.ReadAllText(path);

                if (!fileContent.Contains("namespace "))
                {
                    var classMatch = Regex.Match(fileContent, @"(public\s+(class|interface|struct|enum)\s+\w+)");
                    if (classMatch.Success)
                    {
                        int classIndex = fileContent.IndexOf(classMatch.Value);
                        string usingStatements = fileContent.Substring(0, classIndex).TrimEnd();
                        string classBody = fileContent.Substring(classIndex);

                        string indentedBody = "    " + classBody.Replace("\n", "\n    ");

                        string newContent = $"{usingStatements}\n\nnamespace {expectedNamespace}\n{{\n{indentedBody}\n}}";
                        File.WriteAllText(path, newContent);
                        AssetDatabase.ImportAsset(path);
                    }
                }
                else
                {
                    // Replace a template-provided namespace with the folder-based one.
                    string newContent = Regex.Replace(fileContent, @"namespace\s+[\w\.]+", $"namespace {expectedNamespace}");
                    if (newContent != fileContent)
                    {
                        File.WriteAllText(path, newContent);
                        AssetDatabase.ImportAsset(path);
                    }
                }
            };
        }

        // Folders that organize files INSIDE a feature (or a Shared mechanic) without
        // being a semantic boundary themselves. The namespace stops before these.
        // Note: direct children of Shared/ (Data, Input, Events, ...) ARE domains and
        // extend the namespace - the depth check below handles that.
        private static readonly string[] StructuralFolders =
        {
            "Logic", "Components", "View", "Data", "Input", "States", "RootStates",
            "Interfaces", "Structs", "Enums", "Events",
            "Weapons", "Tools", "Shields", "Targeting"
        };

        internal static string GetNamespaceFromPath(string path)
        {
            const string targetPath = "Assets/_Project/Scripts/";
            const string rootNamespace = "GameplaySystemsAndTools";

            if (!path.StartsWith(targetPath)) return null;

            string relativePath = path.Substring(targetPath.Length);

            int lastSlash = relativePath.LastIndexOf('/');
            if (lastSlash < 0) return rootNamespace;

            string[] folders = relativePath.Substring(0, lastSlash).Split('/');
            var parts = new System.Collections.Generic.List<string> { rootNamespace };

            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];

                // Editor code must live in an .Editor namespace (asmdef requirement),
                // and nothing below an Editor folder refines it further.
                if (folder == "Editor")
                {
                    parts.Add("Editor");
                    break;
                }

                // A structural folder ends the namespace - unless it sits directly
                // under Shared/, where Data/Input/Events are real domain folders.
                bool directChildOfShared = i > 0 && folders[i - 1] == "Shared";
                if (!directChildOfShared && System.Array.IndexOf(StructuralFolders, folder) >= 0) break;

                parts.Add(folder);
            }

            return string.Join(".", parts);
        }
    }
}
