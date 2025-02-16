using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public float spawnTimer = 1;
    public GameObject prefabToSpawn;
    public TextController textController;
    public QuestionBank qb;

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update() {
        if (qb.questionIndex >= qb.questions.Count) {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnTimer) {
            SpawnTargets();
            timer = 0;
        }
    }

    public void SpawnTargets() {
        Vector3[] positions = new Vector3[] {
            new Vector3(0, 4, 10),
            new Vector3(0, 4, -10),
            new Vector3(10, 4, 0),
            new Vector3(-10, 4, 0),
        };

        Quaternion[] rotations = new Quaternion[] {
            Quaternion.identity,
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, -90, 0),
            Quaternion.Euler(0, 90, 0),
        };

        Question q = qb.GetNextQuestion();

        textController.SetProblemText(q.question);

        for (int i = 0; i < 4; i++) {
            GameObject created = Instantiate(prefabToSpawn, positions[i], rotations[i]);
            if (created.TryGetComponent(out McqCube cubeScript)) {
                cubeScript.SetMcqProperties(q.answers[i], q.correctAnswer == i, i);
            } else {
                Debug.LogWarning("Spawned object is missing MoveCubeWithTextAndExplode script!");
            }
        }
    }
}