// Opens the campfire the first time this project is opened, so a fresh clone doesn't start in an empty scene.
// Only once (a marker in Library/, which isn't in the repository), and only over an empty scene. Delete it if you like.
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
static class OpenFireSceneOnFirstOpen
{
    const string Scene = "Assets/Fire/Fire.unity";
    const string Marker = "Library/FireSceneOpened";

    static OpenFireSceneOnFirstOpen()
    {
        if (!UnityEngine.Application.isBatchMode && !File.Exists(Marker)) EditorApplication.update += Tick;
    }

    static void Tick()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        EditorApplication.update -= Tick;
        File.WriteAllText(Marker, "");
        Scene current = SceneManager.GetActiveScene();
        if (EditorApplication.isPlayingOrWillChangePlaymode || current.isDirty || !string.IsNullOrEmpty(current.path)) return;
        EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);
    }
}
