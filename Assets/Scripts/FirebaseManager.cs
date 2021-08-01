using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviourPun
{
    //TODO: -- Dont destroy on load
    DatabaseReference dbReference;
    FirebaseAuth auth;
    FirebaseUser user;
    private void Awake()
    {
        InitializeFirebase();
    }
    void InitializeFirebase()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }
    public string GetUserID()
    {
        return user != null ? user.UserId : "";
    }
    public IEnumerator GetUserName(GameObject go, int point)
    {
        var task = dbReference.Child("users").Child(GetUserID()).GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogWarning(message: $"Failed to reader task with {task.Exception}");
        }
        else if (task.IsCompleted)
        {
            DataSnapshot ds = task.Result;

            photonView.RPC("AddUserID",
                            RpcTarget.All,
                            go.GetPhotonView().ViewID,
                            GetUserID(),
                            point,
                            ds.HasChild("username") == true ? ds.Child("username").Value.ToString() : "",
                            ds.Child("score").Value.ToString());
        }
    }
    public void UpdateScore(string userId, int score = 0)
    {
        dbReference.Child("users").Child(userId).Child("score").GetValueAsync().ContinueWith(
        task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(score + "s2");
                Debug.LogWarning(message: $"Failed to reader task with {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                int baseScore = Convert.ToInt32(task.Result.Value);
                baseScore += score;

                dbReference.Child("users").Child(userId).Child("score").SetValueAsync(baseScore).ContinueWith(
                task => {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogWarning(message: $"Failed to reader task with {task.Exception}");
                    }
                    else if (task.IsCompleted)
                    {
                        // Score Update Process Completed
                    }
                });
            }
        });
    }
}
