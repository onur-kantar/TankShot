using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour
{
    [SerializeField] GameObject loading;
    [SerializeField] Button refreshButton;
    [SerializeField] TMP_Text date;
    [SerializeField] Transform scoreboardContent;
    [SerializeField] GameObject contentPrefab;
    
    void Awake()
    {
        LoadScoreboard();
    }
    public void LoadScoreboard()
    {
        loading.SetActive(true);
        StartCoroutine(LoadScoreboardData());
        StartCoroutine(ToggleButton());
    }
    IEnumerator ToggleButton()
    {
        refreshButton.interactable = false;
        yield return new WaitForSeconds(60);
        refreshButton.interactable = true;
    }
    IEnumerator LoadScoreboardData()
    {
        foreach (Transform child in scoreboardContent.transform)
        {
            Destroy(child.gameObject);
        }

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        var DBTask = DBreference.Child("users").OrderByChild("score").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            date.text = "Loading Failed";
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            bool hasUser = false;
            string username;
            int score;
            int row = 0;
            Color color;
            GameObject scoreboardElement;
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
            DataSnapshot snapshot = DBTask.Result;
            
            for (int i = 0; i < 6; i++)
            {
                if (i == 5 && !hasUser)
                {
                    int j = 0;
                    foreach (DataSnapshot child in snapshot.Children.Reverse())
                    {
                        j++;
                        if (child.Key == user.UserId)
                        {
                            color = new Color32(181, 23, 158, 255);
                            username = child.Child("username").Value.ToString();
                            score = int.Parse(child.Child("score").Value.ToString());
                            scoreboardElement = Instantiate(contentPrefab, scoreboardContent);
                            scoreboardElement.GetComponent<ScoreContent>().NewScoreElement(color, j, username, score);
                            break;
                        }
                    }
                }
                else
                {
                    DataSnapshot childSnapshot = snapshot.Children.Reverse().ElementAt(i);
                    if (childSnapshot.Key == user.UserId)
                    {
                        color = new Color32(181, 23, 158, 255);
                        hasUser = true;
                    }
                    else
                    {
                        row++;
                        if (row % 2 == 0)
                        {
                            color = new Color32(76, 150, 51, 255);
                        }
                        else
                        {
                            color = new Color32(76, 173, 51, 255);
                        }
                    }
                    username = childSnapshot.Child("username").Value.ToString();
                    score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    scoreboardElement = Instantiate(contentPrefab, scoreboardContent);
                    scoreboardElement.GetComponent<ScoreContent>().NewScoreElement(color, i + 1, username, score);
                }
            }
            date.text = "Last Updated On: " + DateTime.Now;
            loading.SetActive(false);
        }
    }
}
