using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Database;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
//using Firebase.Database;
//TODO: -- baştan sona her şeyi incele
public class AuthManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference databaseRef;
    ScoreBoardManager scoreBoardManager;
    LoadingManager loadingManager;

    void Awake()
    {
        loadingManager = GetComponent<LoadingManager>();
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestServerAuthCode(false)
        .Build();
        //TODO: -- Düzenleme
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        InitializeFirebase();
        SignInWithPlayGames();
    }
    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        if (auth != null)
        {
            auth.SignOut();
        }
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    void SignInWithPlayGames()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                Debug.LogError("SignInOnClick: Failed to Sign into Play Games Services.");
                loadingManager.hasError = true;
                return;
            }
            string authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            if (string.IsNullOrEmpty(authCode))
            {
                Debug.LogError("SignInOnClick: Signed into Play Games Services but failed to get the server auth code.");
                loadingManager.hasError = true;
                return;
            }
            Debug.LogFormat("SignInOnClick: Auth code is: {0}", authCode);

            PlayGamesPlatform.Instance.GetAnotherServerAuthCode(true, (authCode) =>
            {
                Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
                auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInOnClick was canceled.");
                        loadingManager.hasError = true;
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInOnClick encountered an error: ");
                        loadingManager.hasError = true;
                        return;
                    }

                    FirebaseUser newUser = task.Result;
                    Debug.LogFormat("SignInOnClick: User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                    loadingManager.firebaseIsLoaded = true;
                    StartCoroutine(WriteNewUserCoroutine(newUser.UserId, newUser.DisplayName, 0));
                });
            });

        });
    }

    IEnumerator WriteNewUserCoroutine(string userId, string name, int score)
    {
        var DBGetTask = databaseRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBGetTask.IsCompleted);
        if (!DBGetTask.Result.Exists)
        {
            User myUser = new User(name, score);
            string json = JsonUtility.ToJson(myUser);
            var DBSetTask = databaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
            yield return new WaitUntil(predicate: () => DBSetTask.IsCompleted);
            if (DBSetTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBSetTask.Exception}");
            }
            else
            {
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                scoreBoardManager.LoadScoreboard();//TODO: -- kullanıcı kendini leaderboard'da aynı görecek
            }
        }
    }
    //////////
    ///DependencyStatus dependencyStatus;
    ///
    //IEnumerator InitializeFirebaseCoroutine()
    //{
    //    var DBTask = FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    //    {
    //        dependencyStatus = task.Result;
    //        if (dependencyStatus == DependencyStatus.Available)
    //        {
    //            InitializeFirebase();
    //        }
    //        else
    //        {
    //            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
    //        }
    //    });
    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    //    }
    //    else
    //    {
    //        StartCoroutine(SignInCoroutine());
    //    }
    //}

    //IEnumerator SignInCoroutine()
    //{
    //    var DBTask = auth.SignInAnonymouslyAsync();
    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    //    }
    //    else
    //    {
    //        user = auth.CurrentUser;
    //        Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
    //        if (user.DisplayName == "")
    //        {
    //            displayNamePanel.SetActive(true);
    //        }
    //    }
    //}

    //IEnumerator UpdateDisplayNameCoroutine()
    //{
    //    user = auth.CurrentUser;
    //    if (user != null)
    //    {
    //        UserProfile profile = new UserProfile
    //        {
    //            DisplayName = displayNameInput.text.ToUpper()
    //        };
    //        var ProfileTask = user.UpdateUserProfileAsync(profile);
    //        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
    //        if (ProfileTask.Exception != null)
    //        {
    //            Debug.LogWarning(message: $"Failed to task with {ProfileTask.Exception}");
    //        }
    //        else
    //        {
    //            StartCoroutine(WriteNewUserCoroutine(user.UserId, user.DisplayName, 0));
    //        }
    //    }
    //}
    //public void UpdateDisplayName()
    //{
    //    StartCoroutine(UpdateDisplayNameCoroutine());
    //}
}