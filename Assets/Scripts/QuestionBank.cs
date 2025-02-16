using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question {
    public string question;
    public string[] answers;
    public int correctAnswer;
}

[System.Serializable]
public class QuestionData {
    public int id;
    public string question;
    public string wrong_answer_1;
    public string wrong_answer_2;
    public string wrong_answer_3;
    public string correct_answer;
}

public class QuestionBank : MonoBehaviour {
    public int questionIndex = 0;
    public List<Question> questions = new List<Question>();
    public TextController textController;
    public int retryTimes = 0;

    private void ShuffleQuestions() {
        // Shuffle the order of the questions in the bank
        for (int i = 0; i < questions.Count; i++) {
            int j = Random.Range(i, questions.Count);
            Question temp = questions[i];
            questions[i] = questions[j];
            questions[j] = temp;
        }
    }

    public Question GetNextQuestion() {
        if (questionIndex >= questions.Count) {
            return null;
        }

        return questions[questionIndex++];
    }

    // New method to load and initialize questions from a JSON string.
    public void LoadFromJSON(string json) {
        // Cannot keep going
        if (textController.progressCounter == 7) {
            if (retryTimes <= 2) {
                retryTimes++;

                return;
            } else {
                textController.ResetProgress();
                retryTimes = 0;
            }
        }

        // Use the helper to parse a top-level JSON array.
        QuestionData[] questionDataArray = JsonHelper.FromJson<QuestionData>(json);
        questions.Clear();
        questionIndex = 0;

        foreach (QuestionData qd in questionDataArray) {
            // Combine answers into a list; start with the correct answer.
            List<string> answerList = new List<string> {
                qd.correct_answer,
                qd.wrong_answer_1,
                qd.wrong_answer_2,
                qd.wrong_answer_3
            };

            // Shuffle answers using Fisher-Yates.
            for (int i = 0; i < answerList.Count; i++) {
                int j = Random.Range(i, answerList.Count);
                string temp = answerList[i];
                answerList[i] = answerList[j];
                answerList[j] = temp;
            }

            // Find where the correct answer landed after shuffling.
            int correctIndex = answerList.IndexOf(qd.correct_answer);

            // Create our internal Question object.
            Question newQuestion = new Question {
                question = qd.question,
                answers = answerList.ToArray(),
                correctAnswer = correctIndex
            };

            questions.Add(newQuestion);
        }

        // Optionally, shuffle the overall questions order.
        ShuffleQuestions();
    }
}

// Helper class to parse JSON arrays with Unity's JsonUtility.
public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T> {
        public T[] array;
    }
}