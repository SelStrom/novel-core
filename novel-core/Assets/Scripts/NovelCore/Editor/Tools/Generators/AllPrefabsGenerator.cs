using UnityEditor;
using UnityEngine;

namespace NovelCore.Editor.Tools.Generators
{
    /// <summary>
    /// Master generator for all NovelCore UI prefabs.
    /// Usage: Menu → NovelCore → Generate UI Prefabs → Generate All Prefabs
    /// </summary>
    public static class AllPrefabsGenerator
    {
        [MenuItem("NovelCore/Generate UI Prefabs/Generate All Prefabs", false, 0)]
        public static void GenerateAllPrefabs()
        {
            bool proceed = EditorUtility.DisplayDialog(
                "Generate All UI Prefabs",
                "This will generate the following prefabs:\n\n" +
                "• DialogueBox.prefab\n" +
                "• ChoiceButton.prefab\n" +
                "• SaveLoadUI.prefab\n\n" +
                "Existing prefabs will be overwritten without confirmation.\n\n" +
                "Do you want to continue?",
                "Generate All",
                "Cancel"
            );

            if (!proceed)
            {
                Debug.Log("[AllPrefabsGenerator] Generation cancelled by user.");
                return;
            }

            int successCount = 0;
            int totalCount = 3;

            Debug.Log("[AllPrefabsGenerator] Starting generation of all UI prefabs...");

            // Generate DialogueBox
            try
            {
                DialogueBoxPrefabGenerator.GenerateDialogueBoxPrefab();
                successCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AllPrefabsGenerator] Failed to generate DialogueBox: {e.Message}");
            }

            // Generate ChoiceButton
            try
            {
                ChoiceButtonPrefabGenerator.GenerateChoiceButtonPrefab();
                successCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AllPrefabsGenerator] Failed to generate ChoiceButton: {e.Message}");
            }

            // Generate SaveLoadUI
            try
            {
                SaveLoadUIPrefabGenerator.GenerateSaveLoadUIPrefab();
                successCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AllPrefabsGenerator] Failed to generate SaveLoadUI: {e.Message}");
            }

            // Final report
            if (successCount == totalCount)
            {
                Debug.Log($"[AllPrefabsGenerator] ✅ Successfully generated all {totalCount} UI prefabs!");
                EditorUtility.DisplayDialog(
                    "All Prefabs Generated",
                    $"Successfully generated all {totalCount} UI prefabs:\n\n" +
                    "✅ DialogueBox.prefab\n" +
                    "✅ ChoiceButton.prefab\n" +
                    "✅ SaveLoadUI.prefab\n\n" +
                    "You can find them in Assets/Resources/NovelCore/UI/",
                    "OK"
                );
            }
            else
            {
                Debug.LogWarning($"[AllPrefabsGenerator] ⚠️ Generated {successCount}/{totalCount} prefabs. Check console for errors.");
                EditorUtility.DisplayDialog(
                    "Partial Generation",
                    $"Generated {successCount} out of {totalCount} prefabs.\n\n" +
                    "Check the Unity Console for error details.",
                    "OK"
                );
            }
        }
    }
}
