using UnityEngine;

public class RayGun : MonoBehaviour
{
    public LayerMask layerMask;
    public OVRInput.RawButton shootingButton;
    public LineRenderer linePrefab;
    public Transform shootingPoint; // start of the line
    public float maxLineDistance = 5;
    public float lineShowTimer = 0.3f;
    public AudioSource audioSource;
    public AudioClip shootingAudioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(shootingButton)) {
            Shoot();
        }
    }

    public void Shoot() {
        audioSource.PlayOneShot(shootingAudioClip);

        Debug.Log("Pew pew");
        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, maxLineDistance, layerMask);

        Vector3 endPoint = Vector3.zero;

        if (hasHit) { 
            endPoint = hit.point;

            Target target = hit.transform.GetComponentInParent<Target>();

            if (target) {
                hit.collider.enabled = false;
                target.Kill();
            }
        } else {
            endPoint = shootingPoint.position + shootingPoint.forward * maxLineDistance;
        }

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, shootingPoint.position);

        line.SetPosition(1, endPoint);

        Destroy(line.gameObject, lineShowTimer);
    }
}
