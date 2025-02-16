using UnityEngine;
using TMPro;
using System.Data.Common;
// using UnityEngine.ProBuilder.MeshOperations;

// [ExecuteInEditMode] // Ensures text fits properly in the editor before the game starts
public class McqCube : MonoBehaviour
{
    private Transform cubeTransform; // Parent Cube
    private TextMeshPro textMeshPro; // Text component

    // public Transform target = Camera.main; // Camera or user
    private float speed = 0.6f;

    [TextArea]
    public string textContent = "Default Text Default Text Default Text Default Text"; // Editable text
    public bool isAnswer = false; // Indicates if it's an answer
    public float padding = 0.2f; // Padding inside the cube
    public float maxTextSize = 20f;
    public float minTextSize = 5f;

    private bool isHit = false;
    private Rigidbody rb;
    private Vector3 initialDirection;

    // Explosion properties
    private float smallCubeSize = 0.5f; // Fixed perfect cube size
    public float explosionDelay = 10f;
    public float explosionForce = 400f;
    public float explosionRadius = 2f;
    private int id;


    // Set text and answer properties. Call after Instantiate.
    public void SetMcqProperties(string text, bool isAnswer, int id)
    {
        textContent = text;
        this.isAnswer = isAnswer;
        this.id = id;
        UpdateText();
    }

    void Awake()
    {
        Initialize();
    }

    void Reset()
    {
        Initialize();
    }

    void Initialize()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>(); // Get text component

        if (transform.childCount > 0) {
            AdjustTextSizeAndPosition();
        }
        else
        {
            Debug.LogError("TextMeshPro object not found! Ensure it is a child of the cube.");
        }

        UpdateText();
    }

    void Start()
    {
        // Ensure we have a Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;

        cubeTransform = transform;
        AdjustTextSizeAndPosition();

        Vector3 origindirection = -transform.position;
        initialDirection = new Vector3(origindirection.x, 0, origindirection.z).normalized;

        // Start explosion countdown
        Invoke(nameof(TimedTriggerExplosion), explosionDelay);
    }

    void Update()
    {
        if (!Application.isPlaying) return; // Prevents movement in Edit Mode

        if (!isHit )
        {
            transform.position += initialDirection * speed * Time.deltaTime;
        }
    }

    void AdjustTextSizeAndPosition()
    {
        if (cubeTransform == null || textMeshPro == null) return;

        // Get cube size
        Vector3 cubeSize = cubeTransform.localScale;

        textMeshPro.transform.localPosition = new Vector3(0, -0.3f, cubeSize.z / 2 - 1f);

        if (cubeTransform.position.z > 0) {
            textMeshPro.transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else if (cubeTransform.position.z < 0) {
            textMeshPro.transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else if (cubeTransform.position.x > 0) {
            textMeshPro.transform.localPosition = new Vector3(0, -0.4f, cubeSize.z / 2 + 1f);
            // Ensure the text is rotated properly
            textMeshPro.transform.localRotation = Quaternion.Euler(0, 180, 0);
        } else if (cubeTransform.position.x < 0) {
            textMeshPro.transform.localPosition = new Vector3(0, -0.4f, cubeSize.z / 2 + 1f);
            // Ensure the text is rotated properly
            textMeshPro.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        // Resize TextMeshPro to fit inside the cube
        RectTransform textRect = textMeshPro.GetComponent<RectTransform>();
        if (textRect != null) {
            // adjust and scale according to local transform
            textRect.sizeDelta = new Vector2((cubeSize.x * cubeTransform.localScale.x - padding) * 2.7f, cubeSize.y * cubeTransform.localScale.y - padding) ;
        }

        // Enable auto-sizing for proper text fitting
        // textMeshPro.enableAutoSizing = true;
        textMeshPro.fontSizeMin = minTextSize;
        textMeshPro.fontSizeMax = maxTextSize;

        // Ensure the text is active
        textMeshPro.gameObject.SetActive(true);
    }

    public void UpdateText()
    {
        if (textMeshPro != null) {
            textMeshPro.text = textContent; // Set text dynamically
            textMeshPro.ForceMeshUpdate(); // Ensure it updates immediately
        }
    }

    private void TimedTriggerExplosion() {
        TriggerExplosion(true);

        if (!isAnswer) {
            TextController textController = Object.FindFirstObjectByType<TextController>();
            textController.AddProgressText();
        }
    }

    public void TriggerExplosion(bool finalExplosion = false) {
        // Calculate how many cubes fit per axis based on the rotation of the cube

        int cubesX = Mathf.Max(1, Mathf.RoundToInt(transform.localScale.x / smallCubeSize));
        int cubesY = Mathf.Max(1, Mathf.RoundToInt(transform.localScale.y / smallCubeSize));
        int cubesZ = Mathf.Max(1, Mathf.RoundToInt(transform.localScale.z / smallCubeSize));

        if (id > 1) {
            int cubesTemp = cubesX;
            cubesX = cubesZ;
            cubesZ = cubesTemp;
        }

        Debug.Log("Explosion triggered!");

        for (int x = 0; x < cubesX; x++)
        {
            for (int y = 0; y < cubesY; y++)
            {
                for (int z = 0; z < cubesZ; z++)
                {
                    CreateCube(new Vector3(x, y, z), cubesX, cubesY, cubesZ, finalExplosion);
                }
            }
        }

        // Ensure we only destroy in Play Mode, not Edit Mode
        if (Application.isPlaying)
        {
            Destroy(gameObject); // Works in Play Mode
        }
        else
        {
            DestroyImmediate(gameObject); // Works in Edit Mode
        }
    }

    void CreateCube(Vector3 coordinates, int cubesX, int cubesY, int cubesZ, bool finalExplosion)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Renderer rd = cube.GetComponent<Renderer>();
        rd.material = GetComponent<Renderer>().material;

        if (finalExplosion) {
            if (isAnswer) {
                rd.material.color = Color.green; 
            } else {
                rd.material.color = Color.red;
            } 
        } else {
            if (isAnswer) {
                rd.material.color = Color.red;
            }
        }

        // Set fixed perfect cube size
        cube.transform.localScale = new Vector3(smallCubeSize, smallCubeSize, smallCubeSize);

        // Calculate first cube position to align properly
        Vector3 firstCube = transform.position - new Vector3(cubesX * smallCubeSize, cubesY * smallCubeSize, cubesZ * smallCubeSize) / 2 + new Vector3(smallCubeSize, smallCubeSize, smallCubeSize) / 2;

        // Position each cube correctly
        cube.transform.position = firstCube + coordinates * smallCubeSize;

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

        Destroy(cube, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StrikerVRProjectile"))
        {
            isHit = true;
            rb.useGravity = true;

            // Trigger explosion when hit
            TriggerExplosion();
        }
    }
}
