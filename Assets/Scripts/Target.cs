using UnityEngine;

public class Target : MonoBehaviour
{
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kill() {
        animator.SetTrigger("Death");
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}
