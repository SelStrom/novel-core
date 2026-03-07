using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Choices;

namespace NovelCore.Editor.Windows
{
    /// <summary>
    /// Unity Editor window for visualizing story flow and scene connections.
    /// Shows a graph of all scenes with choice connections.
    /// </summary>
    public class StoryFlowWindow : EditorWindow
    {
        private List<SceneData> _allScenes = new List<SceneData>();
        private Vector2 _scrollPosition;
        private Vector2 _graphOffset = Vector2.zero;
        private float _zoom = 1.0f;
        
        // Node layout
        private Dictionary<SceneData, Rect> _nodePositions = new Dictionary<SceneData, Rect>();
        private Dictionary<SceneData, Vector2> _nodeTargetPositions = new Dictionary<SceneData, Vector2>();
        private const float NodeWidth = 200f;
        private const float NodeHeight = 80f;
        private const float NodeSpacing = 250f;
        
        // Selection
        private SceneData _selectedScene;
        
        // Validation
        private HashSet<SceneData> _scenesWithErrors = new HashSet<SceneData>();
        private HashSet<SceneData> _scenesWithWarnings = new HashSet<SceneData>();
        
        [MenuItem("Window/NovelCore/Story Flow")]
        public static void ShowWindow()
        {
            var window = GetWindow<StoryFlowWindow>("Story Flow");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        
        private void OnEnable()
        {
            RefreshSceneList();
        }
        
        private void OnGUI()
        {
            DrawToolbar();
            
            if (_allScenes.Count == 0)
            {
                DrawNoScenesMessage();
                return;
            }
            
            DrawGraph();
            
            // Handle events
            HandleEvents();
        }
        
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                RefreshSceneList();
            }
            
            if (GUILayout.Button("Auto Layout", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                AutoLayoutNodes();
            }
            
            if (GUILayout.Button("Validate", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                ValidateAllScenes();
            }
            
            GUILayout.FlexibleSpace();
            
            // Zoom controls
            GUILayout.Label("Zoom:", EditorStyles.miniLabel);
            _zoom = GUILayout.HorizontalSlider(_zoom, 0.5f, 2.0f, GUILayout.Width(100));
            GUILayout.Label($"{_zoom:F2}x", EditorStyles.miniLabel, GUILayout.Width(40));
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawNoScenesMessage()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.HelpBox(
                "No scenes found in the project.\n\n" +
                "Create a scene by:\n" +
                "• Right-click in Project → Create → NovelCore → Scene Data\n" +
                "• Or use Window → NovelCore → Scene Editor",
                MessageType.Info
            );
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawGraph()
        {
            Rect graphRect = new Rect(0, EditorGUIUtility.singleLineHeight + 4, position.width, position.height - EditorGUIUtility.singleLineHeight - 4);
            
            GUI.BeginGroup(graphRect);
            
            // Draw grid
            DrawGrid(graphRect);
            
            // Draw connections first (so they're behind nodes)
            DrawConnections();
            
            // Draw nodes
            DrawNodes();
            
            GUI.EndGroup();
        }
        
        private void DrawGrid(Rect rect)
        {
            Handles.BeginGUI();
            
            Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            Handles.color = gridColor;
            
            float gridSpacing = 50f * _zoom;
            
            // Vertical lines
            for (float x = _graphOffset.x % gridSpacing; x < rect.width; x += gridSpacing)
            {
                Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, rect.height, 0));
            }
            
            // Horizontal lines
            for (float y = _graphOffset.y % gridSpacing; y < rect.height; y += gridSpacing)
            {
                Handles.DrawLine(new Vector3(0, y, 0), new Vector3(rect.width, y, 0));
            }
            
            Handles.EndGUI();
        }
        
        private void DrawNodes()
        {
            foreach (var scene in _allScenes)
            {
                if (!_nodePositions.ContainsKey(scene))
                {
                    // Initialize position if not set
                    _nodePositions[scene] = new Rect(_graphOffset + new Vector2(100, 100), new Vector2(NodeWidth, NodeHeight));
                }
                
                Rect nodeRect = _nodePositions[scene];
                
                // Determine node color based on validation
                Color nodeColor = Color.white;
                if (_scenesWithErrors.Contains(scene))
                {
                    nodeColor = new Color(1f, 0.5f, 0.5f); // Light red
                }
                else if (_scenesWithWarnings.Contains(scene))
                {
                    nodeColor = new Color(1f, 1f, 0.5f); // Light yellow
                }
                else
                {
                    nodeColor = new Color(0.7f, 0.9f, 1f); // Light blue
                }
                
                // Highlight selected node
                if (_selectedScene == scene)
                {
                    nodeColor = new Color(0.5f, 1f, 0.5f); // Light green
                }
                
                // Draw node background
                GUI.backgroundColor = nodeColor;
                GUI.Box(nodeRect, "", EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;
                
                // Draw node content
                GUILayout.BeginArea(nodeRect);
                
                GUILayout.Label(scene.SceneName, EditorStyles.boldLabel);
                GUILayout.Label($"ID: {scene.SceneId}", EditorStyles.miniLabel);
                GUILayout.Label($"Dialogue: {scene.DialogueLines.Count}", EditorStyles.miniLabel);
                GUILayout.Label($"Choices: {scene.Choices.Count}", EditorStyles.miniLabel);
                
                GUILayout.EndArea();
                
                // Handle node click
                if (Event.current.type == EventType.MouseDown && nodeRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0) // Left click
                    {
                        _selectedScene = scene;
                        Event.current.Use();
                    }
                    else if (Event.current.button == 1) // Right click
                    {
                        ShowNodeContextMenu(scene);
                        Event.current.Use();
                    }
                }
            }
        }
        
        private void DrawConnections()
        {
            Handles.BeginGUI();
            
            foreach (var scene in _allScenes)
            {
                if (!_nodePositions.ContainsKey(scene))
                    continue;
                
                Rect startNodeRect = _nodePositions[scene];
                Vector2 startPos = new Vector2(startNodeRect.center.x, startNodeRect.yMax);
                
                // Draw connections for each choice
                foreach (var choice in scene.Choices)
                {
                    foreach (var option in choice.Options)
                    {
                        if (option.targetScene == null || !option.targetScene.RuntimeKeyIsValid())
                            continue;
                        
                        // Find target scene
                        SceneData targetScene = FindSceneByAssetReference(option.targetScene);
                        if (targetScene == null || !_nodePositions.ContainsKey(targetScene))
                            continue;
                        
                        Rect endNodeRect = _nodePositions[targetScene];
                        Vector2 endPos = new Vector2(endNodeRect.center.x, endNodeRect.yMin);
                        
                        // Draw arrow
                        Handles.color = new Color(0.5f, 0.5f, 0.8f, 0.8f);
                        DrawArrow(startPos, endPos);
                        
                        // Draw label
                        Vector2 midPoint = (startPos + endPos) / 2f;
                        GUI.Label(new Rect(midPoint.x - 50, midPoint.y - 10, 100, 20), option.fallbackText, EditorStyles.miniLabel);
                    }
                }
            }
            
            Handles.EndGUI();
        }
        
        private void DrawArrow(Vector2 start, Vector2 end)
        {
            // Draw line
            Handles.DrawLine(start, end);
            
            // Draw arrowhead
            Vector2 direction = (end - start).normalized;
            Vector2 right = new Vector2(-direction.y, direction.x);
            
            float arrowSize = 10f;
            Vector2 arrowTip = end - direction * arrowSize;
            Vector2 arrowLeft = arrowTip - direction * arrowSize + right * arrowSize * 0.5f;
            Vector2 arrowRight = arrowTip - direction * arrowSize - right * arrowSize * 0.5f;
            
            Handles.DrawAAConvexPolygon(end, arrowLeft, arrowRight);
        }
        
        private void ShowNodeContextMenu(SceneData scene)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Open in Scene Editor"), false, () =>
            {
                OpenSceneInEditor(scene);
            });
            
            menu.AddItem(new GUIContent("Select in Project"), false, () =>
            {
                Selection.activeObject = scene;
                EditorGUIUtility.PingObject(scene);
            });
            
            menu.AddSeparator("");
            
            menu.AddItem(new GUIContent("Validate Scene"), false, () =>
            {
                ValidateScene(scene);
            });
            
            menu.ShowAsContext();
        }
        
        private void HandleEvents()
        {
            Event e = Event.current;
            
            // Pan with middle mouse or Alt+Left mouse
            if ((e.type == EventType.MouseDrag && e.button == 2) ||
                (e.type == EventType.MouseDrag && e.button == 0 && e.alt))
            {
                _graphOffset += e.delta;
                
                // Update all node positions
                List<SceneData> keys = new List<SceneData>(_nodePositions.Keys);
                foreach (var key in keys)
                {
                    Rect rect = _nodePositions[key];
                    rect.position += e.delta;
                    _nodePositions[key] = rect;
                }
                
                e.Use();
                Repaint();
            }
            
            // Zoom with scroll wheel
            if (e.type == EventType.ScrollWheel)
            {
                float zoomDelta = -e.delta.y * 0.05f;
                _zoom = Mathf.Clamp(_zoom + zoomDelta, 0.5f, 2.0f);
                e.Use();
                Repaint();
            }
        }
        
        private void RefreshSceneList()
        {
            _allScenes.Clear();
            
            // Find all SceneData assets in the project
            string[] guids = AssetDatabase.FindAssets("t:SceneData");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneData scene = AssetDatabase.LoadAssetAtPath<SceneData>(path);
                
                if (scene != null)
                {
                    _allScenes.Add(scene);
                }
            }
            
            Debug.Log($"StoryFlowWindow: Found {_allScenes.Count} scenes");
            
            // Auto-layout if positions not set
            if (_nodePositions.Count == 0)
            {
                AutoLayoutNodes();
            }
            
            ValidateAllScenes();
            Repaint();
        }
        
        private void AutoLayoutNodes()
        {
            if (_allScenes.Count == 0)
                return;
            
            _nodePositions.Clear();
            
            // Simple grid layout
            int columns = Mathf.CeilToInt(Mathf.Sqrt(_allScenes.Count));
            
            for (int i = 0; i < _allScenes.Count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                
                Vector2 position = new Vector2(
                    100 + col * NodeSpacing,
                    100 + row * (NodeHeight + 50)
                );
                
                _nodePositions[_allScenes[i]] = new Rect(position, new Vector2(NodeWidth, NodeHeight));
            }
            
            Repaint();
        }
        
        private void ValidateAllScenes()
        {
            _scenesWithErrors.Clear();
            _scenesWithWarnings.Clear();
            
            foreach (var scene in _allScenes)
            {
                ValidateScene(scene);
            }
        }
        
        private void ValidateScene(SceneData scene)
        {
            bool hasErrors = false;
            bool hasWarnings = false;
            
            // Check for missing background
            if (scene.BackgroundImage == null || !scene.BackgroundImage.RuntimeKeyIsValid())
            {
                hasWarnings = true;
            }
            
            // Check for empty dialogue
            if (scene.DialogueLines.Count == 0 && scene.Choices.Count == 0)
            {
                hasWarnings = true;
            }
            
            // Check for broken choice links
            foreach (var choice in scene.Choices)
            {
                foreach (var option in choice.Options)
                {
                    if (option.targetScene == null || !option.targetScene.RuntimeKeyIsValid())
                    {
                        hasErrors = true;
                    }
                }
            }
            
            // Check if scene is unreachable (no scenes link to it)
            if (!IsSceneReachable(scene))
            {
                hasWarnings = true;
            }
            
            if (hasErrors)
            {
                _scenesWithErrors.Add(scene);
            }
            else if (hasWarnings)
            {
                _scenesWithWarnings.Add(scene);
            }
        }
        
        private bool IsSceneReachable(SceneData targetScene)
        {
            // Check if any scene has a choice that links to this scene
            foreach (var scene in _allScenes)
            {
                if (scene == targetScene)
                    continue;
                
                foreach (var choice in scene.Choices)
                {
                    foreach (var option in choice.Options)
                    {
                        SceneData linkedScene = FindSceneByAssetReference(option.targetScene);
                        if (linkedScene == targetScene)
                            return true;
                    }
                }
            }
            
            // If it's the first scene (or no other scenes), consider it reachable
            return _allScenes.Count == 1 || _allScenes.IndexOf(targetScene) == 0;
        }
        
        private SceneData FindSceneByAssetReference(UnityEngine.AddressableAssets.AssetReference assetRef)
        {
            if (assetRef == null || !assetRef.RuntimeKeyIsValid())
                return null;
            
            // Try to load the asset directly
            string path = AssetDatabase.GUIDToAssetPath(assetRef.AssetGUID);
            return AssetDatabase.LoadAssetAtPath<SceneData>(path);
        }
        
        private void OpenSceneInEditor(SceneData scene)
        {
            var window = GetWindow<SceneEditorWindow>("Scene Editor");
            Selection.activeObject = scene;
            window.Show();
        }
    }
}
