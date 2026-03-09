using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovelCore.Runtime.Data.GameState
{
    /// <summary>
    /// Implementation of IGameStateManager for managing game state (flags and variables).
    /// Thread-safe storage with simple condition evaluation.
    /// </summary>
    public class GameStateManager : IGameStateManager
    {
        private Dictionary<string, object> _state = new Dictionary<string, object>();
        private readonly object _lock = new object();

        public int StateCount
        {
            get
            {
                lock (_lock)
                {
                    return _state.Count;
                }
            }
        }

        public void SetFlag(string key, bool value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning("GameStateManager: Cannot set flag with empty key");
                return;
            }

            lock (_lock)
            {
                _state[key] = value;
                Debug.Log($"GameStateManager: Set flag '{key}' = {value}");
            }
        }

        public bool GetFlag(string key, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            lock (_lock)
            {
                if (_state.TryGetValue(key, out var value) && value is bool boolValue)
                {
                    return boolValue;
                }
                return defaultValue;
            }
        }

        public void SetVariable(string key, int value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning("GameStateManager: Cannot set variable with empty key");
                return;
            }

            lock (_lock)
            {
                _state[key] = value;
                Debug.Log($"GameStateManager: Set variable '{key}' = {value}");
            }
        }

        public int GetVariable(string key, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            lock (_lock)
            {
                if (_state.TryGetValue(key, out var value))
                {
                    if (value is int intValue)
                    {
                        return intValue;
                    }
                    // Try to convert if stored as different numeric type
                    if (value is float floatValue)
                    {
                        return (int)floatValue;
                    }
                    if (value is long longValue)
                    {
                        return (int)longValue;
                    }
                }
                return defaultValue;
            }
        }

        public void IncrementVariable(string key, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning("GameStateManager: Cannot increment variable with empty key");
                return;
            }

            lock (_lock)
            {
                int currentValue = GetVariable(key, 0);
                int newValue = currentValue + amount;
                _state[key] = newValue;
                Debug.Log($"GameStateManager: Incremented variable '{key}' from {currentValue} to {newValue}");
            }
        }

        public bool EvaluateCondition(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                Debug.LogWarning("GameStateManager: Cannot evaluate empty expression");
                return false;
            }

            try
            {
                // Simple condition evaluator using ConditionEvaluator
                var evaluator = new ConditionEvaluator(this);
                return evaluator.Evaluate(expression);
            }
            catch (Exception ex)
            {
                Debug.LogError($"GameStateManager: Error evaluating condition '{expression}': {ex.Message}");
                return false;
            }
        }

        public Dictionary<string, object> CreateSnapshot()
        {
            lock (_lock)
            {
                var snapshot = new Dictionary<string, object>(_state);
                Debug.Log($"GameStateManager: Created snapshot with {snapshot.Count} entries");
                return snapshot;
            }
        }

        public void RestoreSnapshot(Dictionary<string, object> snapshot)
        {
            if (snapshot == null)
            {
                Debug.LogWarning("GameStateManager: Cannot restore null snapshot");
                return;
            }

            lock (_lock)
            {
                _state = new Dictionary<string, object>(snapshot);
                Debug.Log($"GameStateManager: Restored snapshot with {_state.Count} entries");
            }
        }

        /// <summary>
        /// Saves game state to SaveData format (flags and variables separated).
        /// </summary>
        public void SaveToSaveData(NovelCore.Runtime.Core.SaveSystem.SaveData saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("GameStateManager: Cannot save to null SaveData");
                return;
            }

            lock (_lock)
            {
                // Separate flags and variables
                var flags = new Dictionary<string, bool>();
                var variables = new Dictionary<string, int>();

                foreach (var kvp in _state)
                {
                    if (kvp.Value is bool boolValue)
                    {
                        flags[kvp.Key] = boolValue;
                    }
                    else if (kvp.Value is int intValue)
                    {
                        variables[kvp.Key] = intValue;
                    }
                }

                saveData.flags.FromDictionary(flags);
                saveData.variables.FromDictionary(variables);

                Debug.Log($"GameStateManager: Saved {flags.Count} flags and {variables.Count} variables to SaveData");
            }
        }

        /// <summary>
        /// Loads game state from SaveData format.
        /// </summary>
        public void LoadFromSaveData(NovelCore.Runtime.Core.SaveSystem.SaveData saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("GameStateManager: Cannot load from null SaveData");
                return;
            }

            lock (_lock)
            {
                _state.Clear();

                // Load flags
                if (saveData.flags != null)
                {
                    var flagDict = saveData.flags.ToDictionary();
                    foreach (var kvp in flagDict)
                    {
                        _state[kvp.Key] = kvp.Value;
                    }
                }

                // Load variables
                if (saveData.variables != null)
                {
                    var varDict = saveData.variables.ToDictionary();
                    foreach (var kvp in varDict)
                    {
                        _state[kvp.Key] = kvp.Value;
                    }
                }

                Debug.Log($"GameStateManager: Loaded {_state.Count} state entries from SaveData");
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                int count = _state.Count;
                _state.Clear();
                Debug.Log($"GameStateManager: Cleared {count} state entries");
            }
        }
    }
}
