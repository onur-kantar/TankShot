using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    DatabaseReference DBreference;
    [SerializeField] Transform scoreboardContent;
    [SerializeField] GameObject contentPrefab;
    int row;
    Color color;
    void Awake()
    {
        StartCoroutine(LoadScoreboardData());
    }
    public IEnumerator LoadScoreboardData()
    {
        row = 0;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        var DBTask = DBreference.Child("users").OrderByChild("score").LimitToFirst(10).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse())
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
                string username = childSnapshot.Child("username").Value.ToString();
                int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                GameObject scoreboardElement = Instantiate(contentPrefab, scoreboardContent);
                scoreboardElement.GetComponent<ScoreContent>().NewScoreElement(color, row, username, score);
            }
        }
    }
}
