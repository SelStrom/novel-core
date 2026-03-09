using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NovelCore.Runtime.Data.GameState
{
    /// <summary>
    /// Simple condition expression evaluator for game state checks.
    /// Supports basic comparisons: ==, !=, <, >, <=, >=
    /// Example expressions:
    ///   - "metCharacter == true"
    ///   - "chapter >= 2"
    ///   - "health > 50"
    ///   - "hasKey == false"
    /// </summary>
    public class ConditionEvaluator
    {
        private readonly IGameStateManager _gameState;

        // Regex pattern for parsing condition expressions
        // Format: "variable_name operator value"
        // Example: "chapter >= 2" or "hasKey == true"
        private static readonly Regex ConditionPattern = new Regex(
            @"^\s*(\w+)\s*(==|!=|>=|<=|>|<)\s*(.+?)\s*$",
            RegexOptions.Compiled
        );

        public ConditionEvaluator(IGameStateManager gameState)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        }

        /// <summary>
        /// Evaluates a condition expression against current game state.
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>True if condition is met, false otherwise</returns>
        public bool Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                Debug.LogWarning("ConditionEvaluator: Empty expression");
                return false;
            }

            var match = ConditionPattern.Match(expression.Trim());
            if (!match.Success)
            {
                Debug.LogError($"ConditionEvaluator: Invalid expression format: '{expression}'. Expected format: 'variable operator value'");
                return false;
            }

            string variableName = match.Groups[1].Value;
            string operatorSymbol = match.Groups[2].Value;
            string valueString = match.Groups[3].Value.Trim();

            // Try to parse as boolean
            if (bool.TryParse(valueString, out bool boolValue))
            {
                return EvaluateBooleanCondition(variableName, operatorSymbol, boolValue);
            }

            // Try to parse as integer
            if (int.TryParse(valueString, out int intValue))
            {
                return EvaluateIntegerCondition(variableName, operatorSymbol, intValue);
            }

            Debug.LogError($"ConditionEvaluator: Unsupported value type: '{valueString}'. Supported types: bool, int");
            return false;
        }

        private bool EvaluateBooleanCondition(string variableName, string operatorSymbol, bool expectedValue)
        {
            bool currentValue = _gameState.GetFlag(variableName, false);

            switch (operatorSymbol)
            {
                case "==":
                    return currentValue == expectedValue;
                case "!=":
                    return currentValue != expectedValue;
                default:
                    Debug.LogError($"ConditionEvaluator: Operator '{operatorSymbol}' not supported for boolean values");
                    return false;
            }
        }

        private bool EvaluateIntegerCondition(string variableName, string operatorSymbol, int expectedValue)
        {
            int currentValue = _gameState.GetVariable(variableName, 0);

            switch (operatorSymbol)
            {
                case "==":
                    return currentValue == expectedValue;
                case "!=":
                    return currentValue != expectedValue;
                case ">":
                    return currentValue > expectedValue;
                case "<":
                    return currentValue < expectedValue;
                case ">=":
                    return currentValue >= expectedValue;
                case "<=":
                    return currentValue <= expectedValue;
                default:
                    Debug.LogError($"ConditionEvaluator: Unsupported operator: '{operatorSymbol}'");
                    return false;
            }
        }

        /// <summary>
        /// Validates that an expression has valid syntax (without evaluating it).
        /// </summary>
        public static bool IsValidExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return false;
            }

            return ConditionPattern.IsMatch(expression.Trim());
        }
    }
}
