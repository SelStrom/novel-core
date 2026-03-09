using System.Collections.Generic;

namespace NovelCore.Runtime.Data.GameState
{
    /// <summary>
    /// Interface for managing game state (flags, variables) used in conditional transitions.
    /// Provides methods for setting/getting flags and variables, evaluating conditions,
    /// and creating/restoring state snapshots.
    /// </summary>
    public interface IGameStateManager
    {
        /// <summary>
        /// Sets a boolean flag to the specified value.
        /// </summary>
        /// <param name="key">Flag identifier (e.g., "metCharacter", "hasKey")</param>
        /// <param name="value">Flag value (true/false)</param>
        void SetFlag(string key, bool value);

        /// <summary>
        /// Gets a boolean flag value.
        /// </summary>
        /// <param name="key">Flag identifier</param>
        /// <param name="defaultValue">Default value if flag doesn't exist</param>
        /// <returns>Flag value or default</returns>
        bool GetFlag(string key, bool defaultValue = false);

        /// <summary>
        /// Sets an integer variable to the specified value.
        /// </summary>
        /// <param name="key">Variable identifier (e.g., "chapter", "health", "score")</param>
        /// <param name="value">Variable value</param>
        void SetVariable(string key, int value);

        /// <summary>
        /// Gets an integer variable value.
        /// </summary>
        /// <param name="key">Variable identifier</param>
        /// <param name="defaultValue">Default value if variable doesn't exist</param>
        /// <returns>Variable value or default</returns>
        int GetVariable(string key, int defaultValue = 0);

        /// <summary>
        /// Increments an integer variable by the specified amount.
        /// </summary>
        /// <param name="key">Variable identifier</param>
        /// <param name="amount">Amount to add (can be negative)</param>
        void IncrementVariable(string key, int amount = 1);

        /// <summary>
        /// Evaluates a condition expression against current game state.
        /// Supports simple expressions like "flagName == true" or "variableName >= 5".
        /// </summary>
        /// <param name="expression">Condition expression to evaluate</param>
        /// <returns>True if condition is met, false otherwise</returns>
        bool EvaluateCondition(string expression);

        /// <summary>
        /// Creates a snapshot of current game state for history/save purposes.
        /// </summary>
        /// <returns>Dictionary containing all current flags and variables</returns>
        Dictionary<string, object> CreateSnapshot();

        /// <summary>
        /// Restores game state from a snapshot.
        /// </summary>
        /// <param name="snapshot">State snapshot to restore</param>
        void RestoreSnapshot(Dictionary<string, object> snapshot);

        /// <summary>
        /// Clears all game state (flags and variables).
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the count of stored flags and variables.
        /// </summary>
        int StateCount { get; }
    }
}
