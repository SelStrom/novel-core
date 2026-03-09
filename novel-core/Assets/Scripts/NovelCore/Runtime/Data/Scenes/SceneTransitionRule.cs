using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NovelCore.Runtime.Data.Scenes
{
    /// <summary>
    /// Defines a conditional transition rule for scene navigation.
    /// Rules are evaluated in priority order, first matching rule determines target scene.
    /// </summary>
    [Serializable]
    public class SceneTransitionRule
    {
        /// <summary>
        /// Priority for rule evaluation (lower number = higher priority).
        /// Rules are evaluated in ascending priority order.
        /// </summary>
        [SerializeField]
        [Tooltip("Lower number = higher priority (e.g., priority 0 evaluated before priority 10)")]
        private int _priority = 0;

        /// <summary>
        /// Condition expression to evaluate (e.g., "metCharacter == true", "chapter >= 2").
        /// Simple syntax: "flag_name == true/false" or "variable_name >= value".
        /// </summary>
        [SerializeField]
        [Tooltip("Condition expression (e.g., 'hasKey == true', 'health >= 50')")]
        private string _conditionExpression = "";

        /// <summary>
        /// Target scene to load if condition evaluates to true.
        /// </summary>
        [SerializeField]
        [Tooltip("Scene to load if condition is met")]
        private AssetReference _targetScene;

        /// <summary>
        /// Optional description for debugging/editor display.
        /// </summary>
        [SerializeField]
        [Tooltip("Human-readable description of this rule")]
        private string _description = "";

        public int Priority => _priority;
        public string ConditionExpression => _conditionExpression;
        public AssetReference TargetScene => _targetScene;
        public string Description => _description;

        /// <summary>
        /// Constructor for creating rules programmatically.
        /// </summary>
        public SceneTransitionRule(int priority, string conditionExpression, AssetReference targetScene, string description = "")
        {
            _priority = priority;
            _conditionExpression = conditionExpression;
            _targetScene = targetScene;
            _description = description;
        }

        /// <summary>
        /// Parameterless constructor for Unity serialization.
        /// </summary>
        public SceneTransitionRule()
        {
        }

        /// <summary>
        /// Validates that the rule has valid data.
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(_conditionExpression))
            {
                Debug.LogWarning("SceneTransitionRule: Empty condition expression");
                return false;
            }

            if (_targetScene == null || !_targetScene.RuntimeKeyIsValid())
            {
                Debug.LogWarning($"SceneTransitionRule: Invalid target scene for condition '{_conditionExpression}'");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a debug string representation of this rule.
        /// </summary>
        public override string ToString()
        {
            return $"Rule[P{_priority}]: IF ({_conditionExpression}) THEN {_targetScene?.AssetGUID ?? "null"}";
        }
    }
}
