using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GetQuestionsAnswers : MonoBehaviour
{
    public TextController textController;

    public void GetData(string url)
    {
        if (textController.progressCounter == 7) {
            QuestionBank qb = FindObjectOfType<QuestionBank>();
            qb.LoadFromJSON("request again");

            return;
        }

        textController.SetProblemText("Loading questions...");
        StartCoroutine(GetQuestionsAnswersData(url));
    }

    IEnumerator GetQuestionsAnswersData(string url) {
        Debug.Log("Getting data from " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                textController.SetProblemText("Error: " + webRequest.error);
            } else {
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("Data received: " + responseText);

                // Find the QuestionBank instance and load the JSON data into it.
                QuestionBank qb = FindObjectOfType<QuestionBank>();
                if (qb != null) {
                    qb.LoadFromJSON(responseText);
                } else {
                    Debug.LogError("QuestionBank instance not found!");
                }
            }
        }
    }
}