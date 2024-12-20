using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] Text tittleTxt;
    [SerializeField] Text scoreTxt;

    private string[] difficulty = { "Easy", "Medium", "Hard" };
    private int index = 1;

    void Update()
    {
        ManageUI(difficulty[index - 1], $"HighScore{difficulty[index - 1]}");
    }

    public void Tambah()
    {
        index = index % 3 + 1; // Cycles index from 1 to 3
    }

    public void Kurang()
    {
        index = (index + 1) % 3 + 1; // Cycles index from 3 to 1
    }

    void ManageUI(string tittle, string scoreKey)
    {
        tittleTxt.text = tittle;
        scoreTxt.text = PlayerPrefs.GetInt(scoreKey).ToString();
    }
}
