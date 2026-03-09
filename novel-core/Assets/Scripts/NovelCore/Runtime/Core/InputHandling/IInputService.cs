using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NovelCore.Runtime.Core.InputHandling
{

/// <summary>
/// Interface for input handling across different platforms.
/// Abstracts mouse, touch, and keyboard inputs.
/// </summary>
public interface IInputService
{
    /// <summary>
    /// Event fired when primary action (click/tap) is performed.
    /// </summary>
    event System.Action OnPrimaryAction;

    /// <summary>
    /// Event fired when secondary action (right-click/long-press) is performed.
    /// </summary>
    event System.Action OnSecondaryAction;

    /// <summary>
    /// Event fired when cancel action (ESC/back button) is performed.
    /// </summary>
    event System.Action OnCancelAction;

    /// <summary>
    /// Gets the current pointer position in screen space.
    /// </summary>
    Vector2 PointerPosition { get; }

    /// <summary>
    /// Checks if primary action was performed this frame.
    /// </summary>
    bool WasPrimaryActionPerformed { get; }

    /// <summary>
    /// Checks if the pointer is currently pressed.
    /// </summary>
    bool IsPointerPressed { get; }

    /// <summary>
    /// Enables or disables input processing.
    /// </summary>
    bool InputEnabled { get; set; }
}

}