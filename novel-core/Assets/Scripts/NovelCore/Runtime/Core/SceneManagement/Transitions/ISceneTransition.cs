using System.Threading.Tasks;
using UnityEngine;

namespace NovelCore.Runtime.Core.SceneManagement.Transitions
{
    /// <summary>
    /// Interface for scene transition effects.
    /// </summary>
    public interface ISceneTransition
    {
        /// <summary>
        /// Plays the transition effect.
        /// </summary>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <returns>Task that completes when transition is finished</returns>
        Task PlayAsync(float duration);
        
        /// <summary>
        /// Initializes the transition with required components.
        /// </summary>
        void Initialize(Camera camera);
        
        /// <summary>
        /// Cleans up transition resources.
        /// </summary>
        void Cleanup();
    }
}
