using UnityEngine;
using TMPro;  // If you're using TextMeshPro

public class TextController : MonoBehaviour
{
    // Drag your TextMeshPro component here in the Inspector
    public TextMeshProUGUI problemText;
    public TextMeshProUGUI progressText;
    public QuestionBank qb;

    public int progressCounter = 0;

    // Option A: Public method for other scripts to call
    public void SetProblemText(string newText)
    {
        // limit the text to 310 characters
        if (newText.Length > 310) {
            newText = newText.Substring(0, 310);
        }

        problemText.text = newText;
    }

    public void AddProgressText()
    {
        progressCounter++;

        if (progressCounter > 7)
            progressCounter = 7;

        int numCorrect = progressCounter * 3;

        // ◼▭▭▭▭▭▭▭ 12%

        int totalChars = 21;
        int numIncorrect = totalChars - numCorrect;

        // 5. Build the string
        string newText = new string('█', numCorrect) + new string('░', numIncorrect) + " " + (progressCounter * (100f/7)) + "%";

        progressText.text = newText;

        if (progressCounter == 7) {
            qb.questionIndex = 0;
            qb.questions.Clear();
            qb.retryTimes = 0;
            SetProblemText("Game Over!! To reset, please try clicking a problem set three more times");
        }
    }

    public void ResetProgress() {
        progressCounter = 0;

        int totalChars = 21;
        string newText = new string('░', totalChars) + " " + (progressCounter * (100f/7)) + "%";

        progressText.text = newText;
    }
}
