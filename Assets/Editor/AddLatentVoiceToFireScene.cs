// Adds the Latent Voice object to the campfire scene once the SDK is in the project, using the SDK's own menu item
// (GameObject > Latent Voice > Latent Voice). The scene in this repository doesn't hold it. Save the scene afterwards.
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[InitializeOnLoad]
static class AddLatentVoiceToFireScene
{
    const string Scene = "Assets/Fire/Fire.unity";
    const string Menu = "GameObject/Latent Voice/Latent Voice";

    static AddLatentVoiceToFireScene()
    {
        if (UnityEngine.Application.isBatchMode) return;
        EditorApplication.update += Tick;
        EditorSceneManager.sceneOpened += (scene, mode) => AddIfMissing();
    }

    static void Tick()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        EditorApplication.update -= Tick;
        AddIfMissing();
    }

    static void AddIfMissing()
    {
        Type runner = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType("Latent.VoiceRunner", false)).FirstOrDefault(type => type != null);
        if (runner == null || SceneManager.GetActiveScene().path != Scene) return;
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (Object.FindFirstObjectByType(runner, UnityEngine.FindObjectsInactive.Include) != null) return;
        EditorApplication.ExecuteMenuItem(Menu);
    }
}
