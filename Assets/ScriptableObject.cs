using UnityEngine;

[CreateAssetMenu(fileName = "SceneSettings", menuName = "Audio/SceneSettings", order = 1)]
public class SceneSettings : ScriptableObject
{
    public string sceneName;
    public AudioClip sceneMusic;
    // Add other scene-specific settings here if needed
}
