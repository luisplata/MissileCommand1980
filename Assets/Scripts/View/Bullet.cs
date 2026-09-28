using System;
using System.Collections.Generic;
using UnityEngine;

namespace View
{
    // M1b-1: tipos de enemigo. Warhead es runtime-only (hijo de un MIRV),
    // NUNCA entra en la cola de mezcla del sector ("no 4th type").
    public enum EnemyType
    {
        Normal,
        Mirv,
        Inteligente,
        Warhead
    }

    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float distanceMin;
        [SerializeField] private float impulseForce;
        [SerializeField] private Explosion explosion;
        [SerializeField] private LineRenderer linesRender;
        [SerializeField] private Vector2 goal;
        [SerializeField] public bool isPlayer;
        [SerializeField] private float damage;
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private int points = 25;
        [SerializeField] private float dodgeRadius = 1.5f;
        [SerializeField] private Bullet mirvChildPrefab;
        private Stack<GameObject> listOfChain;
        private GameObject originI;

        // M1b-1: eventos que el spawner (MissilesEnemies) suscribe para
        // materializar el split del MIRV y la esquiva del inteligente.
        public event Action<Bullet> SplitRequested;
        public event Action<Bullet> DodgeRequested;

        public int Points => points;

        // Acceso para el handler del spawner (HandleMirvSplit): el prefab hijo
        // se serializa en Bullet (self-ref) con fallback al prefab base.
        public Bullet MirvChildPrefab => mirvChildPrefab;

        private bool _split;
        private bool _hasDodged;
        private bool _killReported;
        private float _totalTravelSqr;

        // M1b-1: sella tipo + puntos (Normal 25, Mirv 40, Warhead 40, Inteligente 125).
        public void SetEnemyType(EnemyType type)
        {
            enemyType = type;
            points = type switch
            {
                EnemyType.Mirv => 40,
                EnemyType.Warhead => 40,
                EnemyType.Inteligente => 125,
                _ => 25
            };
        }
        
        
        public TankView.OnPlayerDestroyEnemy OnEnemyDestroy;

        // M1b-1 (SC-2): dedupe de kill sobre el MISMO objeto enemigo — tanto el
        // contacto directo como la nube leen este flag: cada enemigo puntúa UNA vez.
        public bool TryReportKill()
        {
            if (_killReported) return false;
            _killReported = true;
            return true;
        }

        private void Awake()
        {
            listOfChain = new Stack<GameObject>();
            linesRender.positionCount = listOfChain.Count;
            // M1b-1 (SC-2): la nube de explosión propia reporta kills por contacto
            // con enemigos (Explosion.OnTriggerEnter2D); el bullet la reenvía al
            // tanque solo si es del player. Suscripción única por instancia.
            if (explosion != null)
            {
                explosion.OnEnemyDestroy += points =>
                {
                    if (isPlayer)
                    {
                        OnEnemyDestroy?.Invoke(points);
                    }
                };
            }
        }

        public void Configure(Vector2 destiny, Vector2 diff, GameObject origin)
        {
            goal = destiny;
            _totalTravelSqr = (destiny - (Vector2)transform.position).sqrMagnitude;
            GetComponent<Rigidbody2D>().AddForce(diff * impulseForce, ForceMode2D.Force);
            originI = new GameObject("Origin");
            originI.transform.position = origin.transform.position;
            listOfChain.Push(originI);
            listOfChain.Push(gameObject);
        }

        // M1b-1 (IN-1): re-aim del inteligente tras esquivar — solo cambia la
        // dirección (meta + velocidad normalizada por la magnitud actual).
        // NO toca impulseForce/AddForce: el feel de impulso M0 queda intacto.
        public void ReAim(Vector2 newTarget)
        {
            goal = newTarget;
            var rigidbody = GetComponent<Rigidbody2D>();
            var currentMagnitude = rigidbody.linearVelocity.magnitude;
            rigidbody.linearVelocity = (newTarget - (Vector2)transform.position).normalized * currentMagnitude;
        }

        private void Update()
        {
            // M1b-1 (MV-1): el MIRV se parte al >=50% del recorrido
            // (queda <=25% de la distancia total al cuadrado) y solo una vez.
            if (enemyType == EnemyType.Mirv && !_split)
            {
                var remainingSqr = (goal - (Vector2)transform.position).sqrMagnitude;
                if (_totalTravelSqr > 0f && remainingSqr <= _totalTravelSqr * 0.25f)
                {
                    _split = true;
                    SplitRequested?.Invoke(this);
                    Explosion();
                }
            }
            // M1b-1 (IN-1): el inteligente esquiva UNA vez cuando un bullet del
            // player entra en su radio (dodgeRadius serializado).
            else if (enemyType == EnemyType.Inteligente && !_hasDodged)
            {
                var hits = Physics2D.OverlapCircleAll(transform.position, dodgeRadius);
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<Bullet>(out var bullet) && bullet.isPlayer)
                    {
                        _hasDodged = true;
                        DodgeRequested?.Invoke(this);
                        break;
                    }
                }
            }

            // Arrival fix (bases-arrival-fix): with AddForce the bullet accelerates and can
            // cross the goal BETWEEN frames — the sqrMagnitude < distanceMin check (prefab
            // distanceMin 0.01, a nearly-zero trigger radius) then misses because the bullet
            // is already PAST the goal, so it never explodes and flies forever (user report:
            // "se van de largo"). Real fix: explode as soon as the bullet stops APPROACHING
            // the goal (dot >= 0 = at or past closest approach). Velocity epsilon guards the
            // edge of a bullet that stopped far from its goal: ~0 velocity must NOT explode.
            var toGoal = (Vector2)transform.position - goal;
            var velocity = GetComponent<Rigidbody2D>().linearVelocity;
            var movingAway = velocity.sqrMagnitude > 0.01f && Vector2.Dot(velocity, toGoal) >= 0f;
            if (movingAway || (goal - (Vector2)transform.position).sqrMagnitude < distanceMin)
            {
                Explosion();
                return; // dead bullet: skip stale PrintLine and re-triggering
            }

            PrintLine();
        }

        public void MultiplyImpulseForce(float factor)
        {
            impulseForce *= factor;
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
                // M1b-1 (SC-2): contacto directo del bullet del player con un enemigo.
                if (isPlayer && other.TryGetComponent<Bullet>(out var enemy) && enemy.TryReportKill())
                {
                    OnEnemyDestroy?.Invoke(enemy.Points);
                }
            }
            else
            {
                // La nube de explosión (Untagged) de un bullet amigo no debe detonar este bullet.
                var cloudOwner = other.GetComponentInParent<Bullet>();
                if (cloudOwner != null && cloudOwner.isPlayer == isPlayer)
                {
                    return;
                }
                // M1b-1 (SC-2): nube ENEMIGA sobre este bullet → la kill del dueño
                // de la nube se reporta (dedupe por TryReportKill).
                if (cloudOwner != null && !cloudOwner.isPlayer && cloudOwner.TryReportKill())
                {
                    OnEnemyDestroy?.Invoke(cloudOwner.Points);
                }
            }
            Explosion();
        }
    }
}
