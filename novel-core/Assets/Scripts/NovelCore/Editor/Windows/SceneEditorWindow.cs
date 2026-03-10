using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Runtime.Data.Choices;

namespace NovelCore.Editor.Windows
{
    /// <summary>
    /// Unity Editor window for creating and editing visual novel scenes.
    /// Provides a visual interface for managing scene data, dialogue, characters, and choices.
    /// </summary>
    public class SceneEditorWindow : EditorWindow
    {
        private SceneData _currentScene;
        private Vector2 _scrollPosition;

        // UI state
        private bool _showSceneInfo = true;
        private bool _showVisualContent = true;
        private bool _showCharacters = true;
        private bool _showDialogue = true;
        private bool _showChoices = true;
        private bool _showSettings = true;

        // Dialogue editing
        private int _selectedDialogueIndex = -1;
        private DialogueLineData _dialogueLineBeingEdited;

        // Character editing
        private int _selectedCharacterIndex = -1;

        // Choice editing
        private int _selectedChoiceIndex = -1;

        [MenuItem("Window/NovelCore/Scene Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneEditorWindow>("Scene Editor");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnEnable()
        {
            // Load the currently selected SceneData if any
            if (Selection.activeObject is SceneData sceneData)
            {
                LoadScene(sceneData);
            }
        }

        private void OnSelectionChange()
        {
            // Auto-load SceneData when selected in Project window
            if (Selection.activeObject is SceneData sceneData)
            {
                LoadScene(sceneData);
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (_currentScene == null)
            {
                DrawNoSceneSelected();
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawSceneInfo();
            DrawVisualContent();
            DrawCharacters();
            DrawDialogue();
            DrawChoices();
            DrawSettings();

            EditorGUILayout.EndScrollView();

            DrawBottomToolbar();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("New Scene", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                CreateNewScene();
            }

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                SaveCurrentScene();
            }

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(_currentScene == null);
            if (GUILayout.Button("Preview Scene", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                PreviewScene();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoSceneSelected()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.HelpBox(
                "No scene selected.\n\n" +
                "• Select a SceneData asset in the Project window\n" +
                "• Or click 'New Scene' to create a new scene",
                MessageType.Info
            );

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private void DrawSceneInfo()
        {
            _showSceneInfo = EditorGUILayout.BeginFoldoutHeaderGroup(_showSceneInfo, "Scene Information");

            if (_showSceneInfo)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Scene ID", _currentScene.SceneId);

                var serializedObject = new SerializedObject(_currentScene);
                serializedObject.Update();

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_sceneName"),
                    new GUIContent("Scene Name")
                );

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawVisualContent()
        {
            _showVisualContent = EditorGUILayout.BeginFoldoutHeaderGroup(_showVisualContent, "Visual Content");

            if (_showVisualContent)
            {
                EditorGUI.indentLevel++;

                var serializedObject = new SerializedObject(_currentScene);
                serializedObject.Update();

                // Background image drag-and-drop
                DrawAssetReferenceField(
                    serializedObject,
                    "_backgroundImage",
                    "Background Image",
                    "Drag and drop a background sprite here"
                );

                // Background music drag-and-drop
                DrawAssetReferenceField(
                    serializedObject,
                    "_backgroundMusic",
                    "Background Music",
                    "Drag and drop an audio clip here"
                );

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawCharacters()
        {
            _showCharacters = EditorGUILayout.BeginFoldoutHeaderGroup(_showCharacters, "Characters");

            if (_showCharacters)
            {
                EditorGUI.indentLevel++;

                var serializedObject = new SerializedObject(_currentScene);
                serializedObject.Update();

                var charactersProperty = serializedObject.FindProperty("_characters");

                // List characters
                for (int i = 0; i < charactersProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var charProp = charactersProperty.GetArrayElementAtIndex(i);
                    var characterRef = charProp.FindPropertyRelative("character");

                    bool isSelected = _selectedCharacterIndex == i;

                    if (GUILayout.Toggle(isSelected, $"Character {i + 1}", EditorStyles.foldoutHeader))
                    {
                        _selectedCharacterIndex = i;
                    }
                    else if (isSelected)
                    {
                        _selectedCharacterIndex = -1;
                    }

                    if (GUILayout.Button("×", GUILayout.Width(25)))
                    {
                        charactersProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (_selectedCharacterIndex == i)
                    {
                        EditorGUI.indentLevel++;
                        DrawCharacterPlacement(charProp);
                        EditorGUI.indentLevel--;
                    }
                }

                // Add character button
                if (GUILayout.Button("+ Add Character"))
                {
                    charactersProperty.arraySize++;
                    _selectedCharacterIndex = charactersProperty.arraySize - 1;
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawCharacterPlacement(SerializedProperty charProp)
        {
            EditorGUILayout.PropertyField(charProp.FindPropertyRelative("character"), new GUIContent("Character"));
            EditorGUILayout.PropertyField(charProp.FindPropertyRelative("position"), new GUIContent("Position"));
            EditorGUILayout.PropertyField(charProp.FindPropertyRelative("initialEmotion"),
                new GUIContent("Initial Emotion"));
            EditorGUILayout.PropertyField(charProp.FindPropertyRelative("sortingOrder"),
                new GUIContent("Sorting Order"));
        }

        private void DrawDialogue()
        {
            _showDialogue = EditorGUILayout.BeginFoldoutHeaderGroup(_showDialogue, "Dialogue");

            if (_showDialogue)
            {
                EditorGUI.indentLevel++;

                var serializedObject = new SerializedObject(_currentScene);
                serializedObject.Update();

                var dialogueProperty = serializedObject.FindProperty("_dialogueLines");

                // List dialogue lines
                for (int i = 0; i < dialogueProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var lineProp = dialogueProperty.GetArrayElementAtIndex(i);
                    var lineData = lineProp.objectReferenceValue as DialogueLineData;

                    string label = lineData != null && !string.IsNullOrEmpty(lineData.FallbackText)
                        ? $"Line {i + 1}: {lineData.FallbackText.Substring(0, Math.Min(30, lineData.FallbackText.Length))}..."
                        : $"Line {i + 1}";

                    bool isSelected = _selectedDialogueIndex == i;

                    if (GUILayout.Toggle(isSelected, label, EditorStyles.foldoutHeader))
                    {
                        _selectedDialogueIndex = i;
                    }
                    else if (isSelected)
                    {
                        _selectedDialogueIndex = -1;
                    }

                    if (GUILayout.Button("×", GUILayout.Width(25)))
                    {
                        dialogueProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (_selectedDialogueIndex == i)
                    {
                        EditorGUI.indentLevel++;
                        DrawDialogueLine(lineProp);
                        EditorGUI.indentLevel--;
                    }
                }

                // Add dialogue line button
                if (GUILayout.Button("+ Add Dialogue Line"))
                {
                    CreateNewDialogueLine(serializedObject, dialogueProperty);
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawDialogueLine(SerializedProperty lineProp)
        {
            var lineData = lineProp.objectReferenceValue as DialogueLineData;

            if (lineData == null)
            {
                EditorGUILayout.HelpBox("Dialogue line data is missing", MessageType.Warning);
                return;
            }

            var lineSerializedObject = new SerializedObject(lineData);
            lineSerializedObject.Update();

            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_speaker"), new GUIContent("Speaker"));
            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_emotion"), new GUIContent("Emotion"));
            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_textKey"), new GUIContent("Text Key"));
            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_fallbackText"),
                new GUIContent("Fallback Text"));
            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_voiceClip"),
                new GUIContent("Voice Clip"));
            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_soundEffect"),
                new GUIContent("Sound Effect"));
            EditorGUILayout.PropertyField(lineSerializedObject.FindProperty("_displayDuration"),
                new GUIContent("Display Duration"));

            lineSerializedObject.ApplyModifiedProperties();
        }

        private void DrawChoices()
        {
            _showChoices = EditorGUILayout.BeginFoldoutHeaderGroup(_showChoices, "Choices");

            if (_showChoices)
            {
                EditorGUI.indentLevel++;

                var serializedObject = new SerializedObject(_currentScene);
                serializedObject.Update();

                var choicesProperty = serializedObject.FindProperty("_choices");

                // List choices
                for (int i = 0; i < choicesProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var choiceProp = choicesProperty.GetArrayElementAtIndex(i);
                    var choiceData = choiceProp.objectReferenceValue as ChoiceData;

                    string label = choiceData != null && !string.IsNullOrEmpty(choiceData.FallbackPromptText)
                        ? $"Choice {i + 1}: {choiceData.FallbackPromptText}"
                        : $"Choice {i + 1}";

                    bool isSelected = _selectedChoiceIndex == i;

                    if (GUILayout.Toggle(isSelected, label, EditorStyles.foldoutHeader))
                    {
                        _selectedChoiceIndex = i;
                    }
                    else if (isSelected)
                    {
                        _selectedChoiceIndex = -1;
                    }

                    if (GUILayout.Button("×", GUILayout.Width(25)))
                    {
                        choicesProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (_selectedChoiceIndex == i && choiceData != null)
                    {
                        EditorGUI.indentLevel++;
                        DrawChoice(choiceData);
                        EditorGUI.indentLevel--;
                    }
                }

                // Add choice button
                if (GUILayout.Button("+ Add Choice"))
                {
                    CreateNewChoice(serializedObject, choicesProperty);
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawChoice(ChoiceData choiceData)
        {
            var choiceSerializedObject = new SerializedObject(choiceData);
            choiceSerializedObject.Update();

            EditorGUILayout.PropertyField(choiceSerializedObject.FindProperty("_promptTextKey"),
                new GUIContent("Prompt Text Key"));
            EditorGUILayout.PropertyField(choiceSerializedObject.FindProperty("_fallbackPromptText"),
                new GUIContent("Fallback Prompt"));
            EditorGUILayout.PropertyField(choiceSerializedObject.FindProperty("_options"), new GUIContent("Options"),
                true);
            EditorGUILayout.PropertyField(choiceSerializedObject.FindProperty("_timerSeconds"),
                new GUIContent("Timer Seconds"));
            EditorGUILayout.PropertyField(choiceSerializedObject.FindProperty("_defaultOptionIndex"),
                new GUIContent("Default Option"));

            choiceSerializedObject.ApplyModifiedProperties();
        }

        private void DrawSettings()
        {
            _showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showSettings, "Settings");

            if (_showSettings)
            {
                EditorGUI.indentLevel++;

                var serializedObject = new SerializedObject(_currentScene);
                serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionType"),
                    new GUIContent("Transition Type"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionDuration"),
                    new GUIContent("Transition Duration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoAdvance"),
                    new GUIContent("Auto Advance"));

                if (_currentScene.AutoAdvance)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoAdvanceDelay"),
                        new GUIContent("Auto Advance Delay"));
                    EditorGUI.indentLevel--;
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawBottomToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.FlexibleSpace();

            if (_currentScene != null)
            {
                EditorGUILayout.LabelField(
                    $"Scene: {_currentScene.SceneName} | Dialogue: {_currentScene.DialogueLines.Count} | Choices: {_currentScene.Choices.Count}",
                    EditorStyles.miniLabel
                );
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAssetReferenceField(SerializedObject serializedObject, string propertyName, string label,
            string dropHint)
        {
            var property = serializedObject.FindProperty(propertyName);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            var rect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));

            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.3f));
            GUI.Box(rect, dropHint, EditorStyles.centeredGreyMiniLabel);

            var evt = Event.current;

            if (evt.type == EventType.DragUpdated && rect.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
            }
            else if (evt.type == EventType.DragPerform && rect.Contains(evt.mousePosition))
            {
                DragAndDrop.AcceptDrag();

                if (DragAndDrop.objectReferences.Length > 0)
                {
                    var asset = DragAndDrop.objectReferences[0];
                    // Handle asset assignment through Addressables
                    // For now, we'll just show the asset path
                    EditorGUILayout.HelpBox($"Asset dropped: {asset.name}", MessageType.Info);
                }

                evt.Use();
            }

            EditorGUILayout.PropertyField(property, GUIContent.none);

            EditorGUILayout.EndVertical();
        }

        private void LoadScene(SceneData sceneData)
        {
            _currentScene = sceneData;
            _selectedDialogueIndex = -1;
            _selectedCharacterIndex = -1;
            _selectedChoiceIndex = -1;
        }

        private void CreateNewScene()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Scene",
                "NewScene",
                "asset",
                "Choose where to save the new scene"
            );

            if (string.IsNullOrEmpty(path))
                return;

            var newScene = CreateInstance<SceneData>();
            AssetDatabase.CreateAsset(newScene, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            LoadScene(newScene);
            Selection.activeObject = newScene;
        }

        private void CreateNewDialogueLine(SerializedObject serializedObject, SerializedProperty dialogueProperty)
        {
            var newLine = CreateInstance<DialogueLineData>();
            newLine.name = $"DialogueLine_{dialogueProperty.arraySize + 1}";

            AssetDatabase.AddObjectToAsset(newLine, _currentScene);
            EditorUtility.SetDirty(_currentScene);
            AssetDatabase.SaveAssets();

            dialogueProperty.arraySize++;
            var newElement = dialogueProperty.GetArrayElementAtIndex(dialogueProperty.arraySize - 1);
            newElement.objectReferenceValue = newLine;

            serializedObject.ApplyModifiedProperties();

            _selectedDialogueIndex = dialogueProperty.arraySize - 1;
        }

        private void CreateNewChoice(SerializedObject serializedObject, SerializedProperty choicesProperty)
        {
            var newChoice = CreateInstance<ChoiceData>();
            newChoice.name = $"Choice_{choicesProperty.arraySize + 1}";

            AssetDatabase.AddObjectToAsset(newChoice, _currentScene);
            EditorUtility.SetDirty(_currentScene);
            AssetDatabase.SaveAssets();

            choicesProperty.arraySize++;
            var newElement = choicesProperty.GetArrayElementAtIndex(choicesProperty.arraySize - 1);
            newElement.objectReferenceValue = newChoice;

            serializedObject.ApplyModifiedProperties();

            _selectedChoiceIndex = choicesProperty.arraySize - 1;
        }

        private void SaveCurrentScene()
        {
            if (_currentScene == null)
                return;

            EditorUtility.SetDirty(_currentScene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Scene saved: {_currentScene.SceneName}");
        }

        private void PreviewScene()
        {
            if (_currentScene == null)
            {
                Debug.LogError("SceneEditorWindow: Cannot preview null scene!");
                return;
            }

            string scenePath = AssetDatabase.GetAssetPath(_currentScene);
            
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError($"SceneEditorWindow: Scene {_currentScene.SceneName} has invalid asset path!");
                return;
            }

            // Store current scene for preview (Constitution Principle VIII: Editor-Runtime Bridge)
            EditorPrefs.SetString("NovelCore_PreviewScene", scenePath);
            
            Debug.Log($"[Preview Mode] Set preview scene: {_currentScene.SceneName}");
            Debug.Log($"[Preview Mode] Path: {scenePath}");
            Debug.Log($"[Preview Mode] Entering Play Mode...");

            // Enter Play mode
            EditorApplication.isPlaying = true;
        }
    }
}
