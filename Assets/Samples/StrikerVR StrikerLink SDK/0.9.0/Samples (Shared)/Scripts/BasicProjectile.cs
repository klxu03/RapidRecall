using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrikerLink.Unity.Runtime.Samples
{
    public class BasicProjectile : MonoBehaviour
    {
        public float speed = 50f;
        public float maxLifespan = 10f;

        public GameObject explosionPrefab;
        Rigidbody rigidBody;

        float aliveTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateVelocity();
            CheckLifespan();
        }

        void CheckLifespan()
        {
            aliveTime += Time.deltaTime;

            if (aliveTime >= maxLifespan)
                Die();
        }

        void UpdateVelocity()
        {
            if (rigidBody != null)
                rigidBody.AddForce(transform.forward.normalized * speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CustomOnCollisionEnter(collision);
            Die();
        }

        void Die()
        {
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        private void CustomOnCollisionEnter(Collision collision) {
            // Target target = collision.transform.GetComponentInParent<Target>();
            McqCube mcqCube = collision.transform.GetComponentInParent<McqCube>();
            if (mcqCube != null) {
                // collision.collider.enabled = false;
                
                if (mcqCube.isAnswer) {
                    TextController textController = Object.FindFirstObjectByType<TextController>();
                    textController.AddProgressText();
                }

                mcqCube.TriggerExplosion();
            } else {
                // Debug what was hit
                Debug.Log("Hit: " + collision.gameObject.name);
            }
        }
    }
}