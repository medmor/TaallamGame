using System.Collections;
using UnityEngine;
using TaallamGame.Dialogue;

namespace TaallamGame.NPC
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PatrolAI : MonoBehaviour
    {
        [Header("Patrol")]
        [SerializeField] private Transform patrolCenter; // if null, uses own transform
        [SerializeField] private float patrolRadius = 3f;
        [SerializeField] private Collider2D patrolAreaCollider; // optional: constrain inside collider (takes precedence)
        [SerializeField] private float pointThreshold = 0.1f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float pauseMin = 0.8f;
        [SerializeField] private float pauseMax = 2.0f;

        private Rigidbody2D rb;
        private Vector2 currentTarget;
        private Coroutine patrolRoutine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (patrolCenter == null) patrolCenter = transform;
        }

        private void OnEnable()
        {
            StartPatrol();
        }

        private void OnDisable()
        {
            StopPatrol();
        }

        public void StartPatrol()
        {
            if (patrolRoutine == null)
                patrolRoutine = StartCoroutine(PatrolLoop());
        }

        public void StopPatrol()
        {
            if (patrolRoutine != null)
            {
                StopCoroutine(patrolRoutine);
                patrolRoutine = null;
            }
            rb.linearVelocity = Vector2.zero;
        }

        private IEnumerator PatrolLoop()
        {
            while (true)
            {
                // pause if dialogue is active
                var dm = DialogueManager.GetInstance();
                while (dm != null && dm.dialogueIsPlaying)
                    yield return null;

                currentTarget = GetNextPoint();
                // move toward target
                while (Vector2.Distance(rb.position, currentTarget) > pointThreshold)
                {
                    // pause movement during dialogue
                    dm = DialogueManager.GetInstance();
                    if (dm != null && dm.dialogueIsPlaying)
                    {
                        rb.linearVelocity = Vector2.zero;
                        yield return null;
                        continue;
                    }

                    Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget, moveSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);
                    yield return null;
                }

                rb.linearVelocity = Vector2.zero;
                // arrive: pause a bit
                yield return new WaitForSeconds(Random.Range(pauseMin, pauseMax));
            }
        }

        private Vector2 GetNextPoint()
        {
            if (patrolAreaCollider != null)
            {
                // try random points until one is inside the collider (bounded attempts)
                for (int i = 0; i < 12; i++)
                {
                    Vector2 candidate = (Vector2)patrolCenter.position + Random.insideUnitCircle * patrolRadius;
                    if (patrolAreaCollider.OverlapPoint(candidate)) return candidate;
                }
                // fallback: nearest point on collider bounds
                return (Vector2)patrolCenter.position + Random.insideUnitCircle * patrolRadius;
            }
            else
            {
                return (Vector2)patrolCenter.position + Random.insideUnitCircle * patrolRadius;
            }
        }

        // Optional helper API
        public void PauseForSeconds(float seconds)
        {
            StartCoroutine(PauseCoroutine(seconds));
        }

        private IEnumerator PauseCoroutine(float s)
        {
            StopPatrol();
            yield return new WaitForSeconds(s);
            StartPatrol();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var center = patrolCenter != null ? patrolCenter.position : transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(center, patrolRadius);
        }
#endif
    }
}