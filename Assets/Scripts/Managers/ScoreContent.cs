using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreContent : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text row;
    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text scoreText;
    public void NewScoreElement(Color _color, int _row, string _username, int _xp)
    {
        image.color = _color;
        row.text = _row + ".";
        usernameText.text = _username;
        scoreText.text = _xp.ToString();
    }
}
