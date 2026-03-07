using UnityEngine;
using UnityEditor;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Editor.Data
{

/// <summary>
/// Custom inspector for SceneData to provide enhanced editing experience.
/// Shows nextScene field prominently and provides validation warnings.
/// </summary>
[CustomEditor(typeof(SceneData))]
public class SceneDataEditor : UnityEditor.Editor
{
    private SerializedProperty _sceneIdProp;
    private SerializedProperty _sceneNameProp;
    private SerializedProperty _backgroundImageProp;
    private SerializedProperty _backgroundMusicProp;
    private SerializedProperty _charactersProp;
    private SerializedProperty _dialogueLinesProp;
    private SerializedProperty _choicesProp;
    private SerializedProperty _transitionTypeProp;
    private SerializedProperty _transitionDurationProp;
    private SerializedProperty _nextSceneProp;
    private SerializedProperty _autoAdvanceProp;
    private SerializedProperty _autoAdvanceDelayProp;

    private void OnEnable()
    {
        _sceneIdProp = serializedObject.FindProperty("_sceneId");
        _sceneNameProp = serializedObject.FindProperty("_sceneName");
        _backgroundImageProp = serializedObject.FindProperty("_backgroundImage");
        _backgroundMusicProp = serializedObject.FindProperty("_backgroundMusic");
        _charactersProp = serializedObject.FindProperty("_characters");
        _dialogueLinesProp = serializedObject.FindProperty("_dialogueLines");
        _choicesProp = serializedObject.FindProperty("_choices");
        _transitionTypeProp = serializedObject.FindProperty("_transitionType");
        _transitionDurationProp = serializedObject.FindProperty("_transitionDuration");
        _nextSceneProp = serializedObject.FindProperty("_nextScene");
        _autoAdvanceProp = serializedObject.FindProperty("_autoAdvance");
        _autoAdvanceDelayProp = serializedObject.FindProperty("_autoAdvanceDelay");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SceneData sceneData = (SceneData)target;

        // Scene Information Header
        EditorGUILayout.LabelField("Scene Information", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_sceneIdProp);
        EditorGUILayout.PropertyField(_sceneNameProp);
        EditorGUILayout.Space();

        // Visual Content Header
        EditorGUILayout.LabelField("Visual Content", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_backgroundImageProp);
        EditorGUILayout.PropertyField(_backgroundMusicProp);
        EditorGUILayout.Space();

        // Characters Header
        EditorGUILayout.LabelField("Characters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_charactersProp, true);
        EditorGUILayout.Space();

        // Narrative Content Header
        EditorGUILayout.LabelField("Narrative Content", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_dialogueLinesProp, true);
        EditorGUILayout.PropertyField(_choicesProp, true);
        EditorGUILayout.Space();

        // Scene Transition Header with Warning
        EditorGUILayout.LabelField("Scene Transition", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_transitionTypeProp);
        EditorGUILayout.PropertyField(_transitionDurationProp);
        
        // Next Scene field with help box
        EditorGUILayout.PropertyField(_nextSceneProp, new GUIContent("Next Scene", 
            "Scene to load after dialogue completes (for linear progression). Ignored if choices are present."));
        
        // Check if AssetReference has a valid asset set
        bool hasNextScene = IsAssetReferenceValid(_nextSceneProp);
        
        // Show warning if both choices and nextScene are defined
        if (_choicesProp.arraySize > 0 && hasNextScene)
        {
            EditorGUILayout.HelpBox(
                "⚠️ Both choices and nextScene are defined. Choices will take priority, and nextScene will be ignored.",
                MessageType.Warning
            );
        }
        
        // Show info if nextScene is set
        if (hasNextScene && _choicesProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox(
                "✓ Linear progression enabled: This scene will automatically transition to the next scene after dialogue completes.",
                MessageType.Info
            );
        }
        
        // Show info if no next scene and no choices
        if (!hasNextScene && _choicesProp.arraySize == 0 && _dialogueLinesProp.arraySize > 0)
        {
            EditorGUILayout.HelpBox(
                "ℹ️ No nextScene or choices defined. Dialogue will end here with no automatic progression.",
                MessageType.Info
            );
        }

        EditorGUILayout.Space();

        // Auto-Advance Settings
        EditorGUILayout.PropertyField(_autoAdvanceProp);
        if (_autoAdvanceProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_autoAdvanceDelayProp);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Validate button
        if (GUILayout.Button("Validate Scene"))
        {
            if (sceneData.Validate())
            {
                Debug.Log($"✓ Scene '{sceneData.SceneName}' validation passed!");
            }
            else
            {
                Debug.LogError($"✗ Scene '{sceneData.SceneName}' validation failed. Check console for details.");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Checks if an AssetReference SerializedProperty has a valid asset assigned.
    /// </summary>
    private bool IsAssetReferenceValid(SerializedProperty assetRefProperty)
    {
        if (assetRefProperty == null)
            return false;

        // AssetReference stores the asset GUID in a nested property
        var assetGuidProp = assetRefProperty.FindPropertyRelative("m_AssetGUID");
        if (assetGuidProp != null && !string.IsNullOrEmpty(assetGuidProp.stringValue))
        {
            return true;
        }

        return false;
    }
}

}
