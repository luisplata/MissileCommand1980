using System;
using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float distanceMin;
        [SerializeField] private float impulseForce;
        [SerializeField] private Explosion explosion;
        [SerializeField] private LineRenderer linesRender;
        [SerializeField]private Vector2 goal;
        [SerializeField] private bool isPlayer;
        [SerializeField] private float damage;
        private Stack<GameObject> listOfChain;
        private GameObject originI;
        
        
        public TankView.OnPlayerDestroyEnemy OnEnemyDestroy;

        private void Awake()
        {
            listOfChain = new Stack<GameObject>();
            linesRender.positionCount = listOfChain.Count;
        }

        public void Configure(Vector2 destiny, Vector2 diff, GameObject origin)
        {
            goal = destiny;
            GetComponent<Rigidbody2D>().AddForce(diff * impulseForce, ForceMode2D.Force);
            originI = new GameObject("Origin");
            originI.transform.position = origin.transform.position;
            listOfChain.Push(originI);
            listOfChain.Push(gameObject);
        }

        private void Update()
        {
            if ((goal - (Vector2)transform.position).sqrMagnitude < distanceMin)
            {
                Explosion();
            }

            PrintLine();
        }
        private void PrintLine()
        {
            if (listOfChain.Count <= 0)
            {
                linesRender.positionCount = 0;
                return;
            }
            var countPosition = 0;
            if (listOfChain.Count > 0)
            {
                countPosition = listOfChain.Count;
            }
            var positions = new Vector3[countPosition];

            var positionCount = 0;
            foreach (var chain in listOfChain)
            {
                positions[positionCount] = chain.transform.position;
                positionCount++;
            }
            linesRender.positionCount = listOfChain.Count;
            linesRender.SetPositions(positions);
        }
        private void Explosion()
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            explosion.Configuration(originI);
        }

        private bool yaAplicoDanio;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Destruible"))
            {
                if (isPlayer) return;
                if (other.TryGetComponent<ObjectDestroyer>(out var destruibleComponent) && !yaAplicoDanio)
                {
                    yaAplicoDanio = true;
                    destruibleComponent.GetImpact(damage);   
                }
            }
            else if (other.CompareTag("Respawn"))
            {
                // Ignorar el propio bando: los bullets del player no deben detonar entre sí
                // (ambos comparten la tag "Respawn", que también identifica al enemigo).
                if (other.TryGetComponent<Bullet>(out var otherBullet) && otherBullet.isPlayer == isPlayer)
                {
                    return;
                }
                if (isPlayer)
                {
                    OnEnemyDestroy?.Invoke();    
                }
            }
            else
            {
                // La nube de explosión (Untagged) de un bullet amigo no debe detonar este bullet.
                if (other.GetComponentInParent<Bullet>() is Bullet cloudOwner && cloudOwner.isPlayer == isPlayer)
                {
                    return;
                }
                explosion.OnEnemyDestroy += () =>
                {
                    if (isPlayer)
                    {
                        OnEnemyDestroy?.Invoke();    
                    }
                };
            }
            Explosion();
        }
    }
}
