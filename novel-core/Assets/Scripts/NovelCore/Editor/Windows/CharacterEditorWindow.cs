using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NovelCore.Runtime.Data.Characters;

namespace NovelCore.Editor.Windows
{
    /// <summary>
    /// Unity Editor window for creating and editing characters.
    /// Provides UI for managing character data and emotion sprites.
    /// </summary>
    public class CharacterEditorWindow : EditorWindow
    {
        private CharacterData _currentCharacter;
        private Vector2 _scrollPosition;
        
        // UI state
        private bool _showBasicInfo = true;
        private bool _showEmotions = true;
        private bool _showAdvanced = true;
        
        // Emotion editing
        private string _newEmotionName = "";
        
        [MenuItem("Window/NovelCore/Character Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<CharacterEditorWindow>("Character Editor");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnEnable()
        {
            // Load the currently selected CharacterData if any
            if (Selection.activeObject is CharacterData characterData)
            {
                LoadCharacter(characterData);
            }
        }
        
        private void OnSelectionChange()
        {
            // Auto-load CharacterData when selected in Project window
            if (Selection.activeObject is CharacterData characterData)
            {
                LoadCharacter(characterData);
                Repaint();
            }
        }
        
        private void OnGUI()
        {
            DrawToolbar();
            
            if (_currentCharacter == null)
            {
                DrawNoCharacterSelected();
                return;
            }
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            DrawBasicInfo();
            DrawEmotions();
            DrawAdvanced();
            
            EditorGUILayout.EndScrollView();
            
            DrawBottomToolbar();
        }
        
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            if (GUILayout.Button("New Character", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                CreateNewCharacter();
            }
            
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                SaveCurrentCharacter();
            }
            
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawNoCharacterSelected()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.HelpBox(
                "No character selected.\n\n" +
                "• Select a CharacterData asset in the Project window\n" +
                "• Or click 'New Character' to create a new character",
                MessageType.Info
            );
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawBasicInfo()
        {
            _showBasicInfo = EditorGUILayout.BeginFoldoutHeaderGroup(_showBasicInfo, "Basic Information");
            
            if (_showBasicInfo)
            {
                EditorGUI.indentLevel++;
                
                var serializedObject = new SerializedObject(_currentCharacter);
                serializedObject.Update();
                
                EditorGUILayout.LabelField("Character ID", _currentCharacter.CharacterId);
                
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_characterName"),
                    new GUIContent("Character Name")
                );
                
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_displayName"),
                    new GUIContent("Display Name")
                );
                
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_defaultScale"),
                    new GUIContent("Default Scale")
                );
                
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_animationType"),
                    new GUIContent("Animation Type")
                );
                
                serializedObject.ApplyModifiedProperties();
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawEmotions()
        {
            _showEmotions = EditorGUILayout.BeginFoldoutHeaderGroup(_showEmotions, "Emotions");
            
            if (_showEmotions)
            {
                EditorGUI.indentLevel++;
                
                var serializedObject = new SerializedObject(_currentCharacter);
                serializedObject.Update();
                
                var emotionsProperty = serializedObject.FindProperty("_emotions");
                
                // Display existing emotions
                EditorGUILayout.LabelField("Emotion List", EditorStyles.boldLabel);
                
                for (int i = 0; i < emotionsProperty.arraySize; i++)
                {
                    var emotionProp = emotionsProperty.GetArrayElementAtIndex(i);
                    var emotionNameProp = emotionProp.FindPropertyRelative("emotionName");
                    var spriteProp = emotionProp.FindPropertyRelative("sprite");
                    
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    
                    EditorGUILayout.BeginVertical();
                    
                    // Emotion name
                    EditorGUILayout.LabelField(emotionNameProp.stringValue, EditorStyles.boldLabel);
                    
                    // Sprite field
                    EditorGUI.BeginChangeCheck();
                    Sprite newSprite = (Sprite)EditorGUILayout.ObjectField(
                        "Sprite",
                        spriteProp.objectReferenceValue,
                        typeof(Sprite),
                        false,
                        GUILayout.Height(60)
                    );
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        spriteProp.objectReferenceValue = newSprite;
                    }
                    
                    // Preview
                    if (spriteProp.objectReferenceValue != null)
                    {
                        Sprite sprite = (Sprite)spriteProp.objectReferenceValue;
                        Rect previewRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64));
                        
                        if (sprite.texture != null)
                        {
                            GUI.DrawTextureWithTexCoords(
                                previewRect,
                                sprite.texture,
                                GetSpriteTextureCoords(sprite)
                            );
                        }
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                    // Delete button
                    if (GUILayout.Button("×", GUILayout.Width(25), GUILayout.Height(60)))
                    {
                        emotionsProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(5);
                }
                
                // Add new emotion
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Add New Emotion", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                
                _newEmotionName = EditorGUILayout.TextField("Emotion Name", _newEmotionName);
                
                EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_newEmotionName));
                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    AddNewEmotion(_newEmotionName.Trim());
                    _newEmotionName = "";
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.EndHorizontal();
                
                // Quick add common emotions
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Quick Add:", EditorStyles.miniLabel);
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Neutral", EditorStyles.miniButton))
                {
                    AddNewEmotion("neutral");
                    serializedObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Happy", EditorStyles.miniButton))
                {
                    AddNewEmotion("happy");
                    serializedObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Sad", EditorStyles.miniButton))
                {
                    AddNewEmotion("sad");
                    serializedObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Angry", EditorStyles.miniButton))
                {
                    AddNewEmotion("angry");
                    serializedObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Surprised", EditorStyles.miniButton))
                {
                    AddNewEmotion("surprised");
                    serializedObject.ApplyModifiedProperties();
                }
                
                EditorGUILayout.EndHorizontal();
                
                serializedObject.ApplyModifiedProperties();
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawAdvanced()
        {
            _showAdvanced = EditorGUILayout.BeginFoldoutHeaderGroup(_showAdvanced, "Advanced");
            
            if (_showAdvanced)
            {
                EditorGUI.indentLevel++;
                
                var serializedObject = new SerializedObject(_currentCharacter);
                serializedObject.Update();
                
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_spineData"),
                    new GUIContent("Spine Data Asset")
                );
                
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(
                    "Spine Data is only used when Animation Type is set to 'Spine'.\n" +
                    "For sprite-based animation, use the Emotions section above.",
                    MessageType.Info
                );
                
                serializedObject.ApplyModifiedProperties();
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawBottomToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUILayout.FlexibleSpace();
            
            if (_currentCharacter != null)
            {
                EditorGUILayout.LabelField(
                    $"Character: {_currentCharacter.CharacterName} | Emotions: {_currentCharacter.Emotions.Count}",
                    EditorStyles.miniLabel
                );
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private Rect GetSpriteTextureCoords(Sprite sprite)
        {
            if (sprite == null || sprite.texture == null)
                return new Rect(0, 0, 1, 1);
            
            Rect rect = sprite.textureRect;
            return new Rect(
                rect.x / sprite.texture.width,
                rect.y / sprite.texture.height,
                rect.width / sprite.texture.width,
                rect.height / sprite.texture.height
            );
        }
        
        private void LoadCharacter(CharacterData characterData)
        {
            _currentCharacter = characterData;
        }
        
        private void CreateNewCharacter()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Character",
                "NewCharacter",
                "asset",
                "Choose where to save the new character"
            );
            
            if (string.IsNullOrEmpty(path))
                return;
            
            var newCharacter = ScriptableObject.CreateInstance<CharacterData>();
            AssetDatabase.CreateAsset(newCharacter, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            LoadCharacter(newCharacter);
            Selection.activeObject = newCharacter;
        }
        
        private void AddNewEmotion(string emotionName)
        {
            if (_currentCharacter == null || string.IsNullOrWhiteSpace(emotionName))
                return;
            
            var serializedObject = new SerializedObject(_currentCharacter);
            serializedObject.Update();
            
            var emotionsProperty = serializedObject.FindProperty("_emotions");
            
            // Check if emotion already exists
            for (int i = 0; i < emotionsProperty.arraySize; i++)
            {
                var emotionProp = emotionsProperty.GetArrayElementAtIndex(i);
                var nameProp = emotionProp.FindPropertyRelative("emotionName");
                
                if (nameProp.stringValue.Equals(emotionName, StringComparison.OrdinalIgnoreCase))
                {
                    EditorUtility.DisplayDialog(
                        "Emotion Exists",
                        $"An emotion named '{emotionName}' already exists for this character.",
                        "OK"
                    );
                    return;
                }
            }
            
            // Add new emotion
            emotionsProperty.arraySize++;
            var newEmotionProp = emotionsProperty.GetArrayElementAtIndex(emotionsProperty.arraySize - 1);
            newEmotionProp.FindPropertyRelative("emotionName").stringValue = emotionName.ToLower();
            newEmotionProp.FindPropertyRelative("sprite").objectReferenceValue = null;
            
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log($"Added emotion '{emotionName}' to character {_currentCharacter.CharacterName}");
        }
        
        private void SaveCurrentCharacter()
        {
            if (_currentCharacter == null)
                return;
            
            EditorUtility.SetDirty(_currentCharacter);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Character saved: {_currentCharacter.CharacterName}");
        }
    }
}
