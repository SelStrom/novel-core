using UnityEngine;

namespace NovelCore.Runtime.Core.SceneManagement.Transitions
{
    /// <summary>
    /// Factory for creating scene transitions based on transition type.
    /// </summary>
    public class SceneTransitionFactory
    {
    private readonly Camera _camera;
    
    public SceneTransitionFactory(Camera camera = null)
    {
        _camera = camera ?? Camera.main;
    }
    
    /// <summary>
    /// Creates a transition effect based on the specified type.
    /// </summary>
    public ISceneTransition CreateTransition(TransitionType transitionType)
    {
        ISceneTransition transition = transitionType switch
        {
            TransitionType.Fade => new FadeTransition(),
            TransitionType.Slide => new SlideTransition(),
            TransitionType.Cut => new CutTransition(),
            _ => new CutTransition()
        };
        
        transition.Initialize(_camera);
        return transition;
    }
}
}
