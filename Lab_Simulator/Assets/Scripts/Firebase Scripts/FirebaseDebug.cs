using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseDebug : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var app = FirebaseApp.DefaultInstance;
            Debug.Log("Project ID = " + app.Options.ProjectId);
            Debug.Log("API Key = " + app.Options.ApiKey);
            Debug.Log("App ID = " + app.Options.AppId);
        });
    }
}
