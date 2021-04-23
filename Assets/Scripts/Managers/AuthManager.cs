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
    DependencyStatus dependencyStatus;
    FirebaseAuth auth;
    FirebaseUser user;
    [SerializeField] TMP_InputField displayNameInput;
    [SerializeField] GameObject displayNamePanel;
    DatabaseReference databaseRef;
    ScoreBoardManager scoreBoardManager;

    [SerializeField] TMP_Text ss;

    void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestServerAuthCode(false /* Don't force refresh */)
        .Build();
        //TODO: -- Düzenleme
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        InitializeFirebase();
        SignInWithPlayGames();
        
        //scoreBoardManager = GetComponent<ScoreBoardManager>();
        //StartCoroutine(InitializeFirebaseCoroutine());
    }
    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void SignInWithPlayGames()
    {
        // Initialize Firebase Auth
        //auth = FirebaseAuth.DefaultInstance;

        // Sign In and Get a server auth code.
        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                Debug.LogError("SignInOnClick: Failed to Sign into Play Games Services.");
                return;
            }

            string authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            if (string.IsNullOrEmpty(authCode))
            {
                Debug.LogError("SignInOnClick: Signed into Play Games Services but failed to get the server auth code.");
                return;
            }
            Debug.LogFormat("SignInOnClick: Auth code is: {0}", authCode);

            // Use Server Auth Code to make a credential
            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

            // Sign In to Firebase with the credential
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInOnClick was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInOnClick encountered an error: " + task.Exception);
                    return;
                }

                FirebaseUser newUser = task.Result;
                Debug.LogFormat("SignInOnClick: User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                StartCoroutine(WriteNewUserCoroutine(newUser.UserId, newUser.DisplayName, 0));
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
                StartCoroutine(scoreBoardManager.LoadScoreboardData());//TODO: -- kullanıcı kendini leaderboard'da aynı görecek
                displayNamePanel.SetActive(false);
            }
        }
    }
    //////////
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