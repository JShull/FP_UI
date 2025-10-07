using FuzzPhyte.UI.Samples;
using UnityEngine;
using TMPro;
public class CVScore : MonoBehaviour
{
    public MatchTestBankController Controller;
    public TMP_Text ScoreText;
    public TMP_Text NumberAttemptsText;
    public int NumAttempts = 0;

    public void CheckScore()
    {
        NumAttempts++;
        NumberAttemptsText.text = NumAttempts.ToString();
        ScoreText.text = $"{Controller.RunningMatchScore:D3}";
        //$"Player 1 Score: {score1:D3}"
    }

}
