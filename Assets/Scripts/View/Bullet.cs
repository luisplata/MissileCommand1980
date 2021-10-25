using UnityEngine;

namespace View
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float distanceMin;
        [SerializeField] private float impulseForce;
        private Vector2 goal;
        
        public void Configure(Vector2 destiny, Vector2 diff)
        {
            goal = destiny;
            GetComponent<Rigidbody2D>().AddForce(diff * impulseForce, ForceMode2D.Force);
        }

        private void Update()
        {
            if (((Vector2)transform.position - goal).sqrMagnitude < distanceMin)
            {
                Explosion();
            }
        }

        private void Explosion()
        {
            Destroy(gameObject);
        }
    }
}
