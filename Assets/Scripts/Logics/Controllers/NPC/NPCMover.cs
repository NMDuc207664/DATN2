using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using UnityEngine;
using UnityEngine.AI;

public class NPCMover : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float waitTimeAtPoint = 0.5f;
    [SerializeField] private float sampleRadius = 5f;

    [Header("Debug Settings")]
    [SerializeField] private bool Debug_Mode = false;
    [SerializeField] private List<Transform> destinations_debug = new List<Transform>();
    [Header("Độ lệch sang trái hoặc phải (theo hướng di chuyển)")]
    [SerializeField] private float lateralOffset = 0f;

    private NavMeshAgent agent;
    private List<Vector3> destinations = new List<Vector3>();
    private int currentIndex = 0;
    private bool isMoving = false;
    private Coroutine currentMovementCoroutine;

    public bool IsMoving => isMoving;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent not found!");
            return;
        }

        // Chỉ chạy debug mode nếu được bật
        if (Debug_Mode && destinations_debug.Count > 0)
        {
            StartCoroutine(MoveThroughPoints_Debug_Mode());
        }
    }

    /// <summary>
    /// Nhận mảng keys và questKey, tự load destinations từ QuestData rồi bắt đầu di chuyển
    /// </summary>
    /// <param name="questKey">Key của QuestDataSO (vd: "Map_1_OpeningS_Q1")</param>
    /// <param name="moveKeys">Mảng các key để lấy vị trí (vd: ["NPC(s)_Move_Point_1", "NPC(s)_Move_Point_2"])</param>
    /// <param name="questIndex">Index của quest trong QuestDataSO (mặc định = 0)</param>
    public void StartMovementWithKeys(string questKey, string[] moveKeys, int questIndex = 0)
    {
        if (string.IsNullOrEmpty(questKey) || moveKeys == null || moveKeys.Length == 0)
        {
            Debug.LogWarning($"{name}: Invalid questKey or moveKeys!");
            return;
        }

        // Load QuestData
        QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData(questKey);
        if (questData == null)
        {
            Debug.LogError($"{name}: QuestData not found for key: {questKey}");
            return;
        }

        if (questData.quests.Count <= questIndex)
        {
            Debug.LogError($"{name}: Quest index {questIndex} out of range!");
            return;
        }

        // Load destinations từ keys
        List<Vector3> loadedDestinations = new List<Vector3>();
        var quest = questData.quests[questIndex];

        foreach (string key in moveKeys)
        {
            if (quest.targetPosition.ContainsKey(key))
            {
                Vector3 targetPos = quest.targetPosition[key];
                loadedDestinations.Add(targetPos);
                Debug.Log($"{name}: Loaded destination '{key}' at {targetPos}");
            }
            else
            {
                Debug.LogWarning($"{name}: Key '{key}' not found in quest '{questKey}'!");
            }
        }

        if (loadedDestinations.Count == 0)
        {
            Debug.LogWarning($"{name}: No valid destinations loaded from keys!");
            return;
        }

        // Bắt đầu di chuyển
        StartMovement(loadedDestinations);
    }

    /// <summary>
    /// Bắt đầu di chuyển với list Vector3 đã có sẵn
    /// </summary>
    private void StartMovement(List<Vector3> newDestinations)
    {
        // Dừng movement hiện tại nếu có
        StopMovement();

        // Set destinations mới
        destinations = new List<Vector3>(newDestinations);
        currentIndex = 0;

        // Bắt đầu di chuyển
        currentMovementCoroutine = StartCoroutine(MoveThroughPoints());
        Debug.Log($"{name}: Started movement with {destinations.Count} destinations");
    }

    public void StopMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }

        isMoving = false;
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private IEnumerator MoveThroughPoints()
    {
        isMoving = true;

        while (currentIndex < destinations.Count)
        {
            Vector3 targetPos = destinations[currentIndex];

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(targetPos, out hit, sampleRadius, NavMesh.AllAreas))
            {
                Debug.LogWarning($"{name}: Destination {currentIndex} at {targetPos} is not on NavMesh - skipping");
                currentIndex++;
                continue;
            }
            Vector3 direction = (hit.position - transform.position).normalized;
            Vector3 offset = Vector3.zero;

            if (Mathf.Abs(lateralOffset) > 0.01f)
            {
                Vector3 rightDir = Vector3.Cross(Vector3.up, direction).normalized;
                offset = rightDir * lateralOffset;
            }

            Vector3 finalTarget = hit.position + offset;


            agent.isStopped = false;
            agent.SetDestination(finalTarget);

            float stuckTimer = 0f;
            float maxStuckTime = 10f;
            Vector3 lastPosition = transform.position;

            while (true)
            {
                float distance = Vector3.Distance(transform.position, targetPos);
                float remainingDist = agent.remainingDistance;

                if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer > maxStuckTime)
                    {
                        Debug.LogWarning($"{name}: Stuck too long - skipping destination {currentIndex}");
                        break;
                    }
                }
                else
                {
                    stuckTimer = 0f;
                    lastPosition = transform.position;
                }

                bool hasReached = (distance <= stopDistance) ||
                                  (!agent.pathPending && remainingDist <= stopDistance && remainingDist != Mathf.Infinity);

                if (hasReached)
                {
                    Debug.Log($"{name}: ✓ Reached destination {currentIndex}");
                    break;
                }

                yield return null;
            }

            agent.isStopped = true;
            yield return new WaitForSeconds(waitTimeAtPoint);

            currentIndex++;
        }

        isMoving = false;
        agent.isStopped = true;
        currentMovementCoroutine = null;
        Debug.Log($"{name}: ★ Completed all destinations ★");
    }

    private IEnumerator MoveThroughPoints_Debug_Mode()
    {
        isMoving = true;

        while (currentIndex < destinations_debug.Count)
        {
            Transform target = destinations_debug[currentIndex];

            if (target == null)
            {
                currentIndex++;
                continue;
            }

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(target.position, out hit, sampleRadius, NavMesh.AllAreas))
            {
                currentIndex++;
                continue;
            }

            agent.isStopped = false;
            agent.SetDestination(hit.position);
            Debug.Log($"{name}: Moving to destination {currentIndex}: {target.name} ({hit.position})");

            float stuckTimer = 0f;
            float maxStuckTime = 10f;
            Vector3 lastPosition = transform.position;

            while (true)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                float remainingDist = agent.remainingDistance;

                if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer > maxStuckTime)
                    {
                        Debug.LogWarning($"{name}: Stuck too long — skipping {target.name}");
                        break;
                    }
                }
                else
                {
                    stuckTimer = 0f;
                    lastPosition = transform.position;
                }

                bool hasReached = (distance <= stopDistance) ||
                                  (!agent.pathPending && remainingDist <= stopDistance && remainingDist != Mathf.Infinity);

                if (hasReached)
                {
                    Debug.Log($"{name}: ✓ Reached destination {currentIndex}: {target.name}");
                    break;
                }

                yield return null;
            }

            agent.isStopped = true;
            yield return new WaitForSeconds(waitTimeAtPoint);

            currentIndex++;
        }

        isMoving = false;
        agent.isStopped = true;
        Debug.Log($"{name}: ★ Completed all destinations ★");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var dest in destinations)
        {
            Gizmos.DrawWireSphere(dest, stopDistance);
            Gizmos.DrawLine(dest, dest + Vector3.up * 2f);
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < destinations.Count - 1; i++)
        {
            Gizmos.DrawLine(destinations[i], destinations[i + 1]);
        }

        if (Application.isPlaying && isMoving && currentIndex < destinations.Count)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, destinations[currentIndex]);

            if (agent != null && agent.hasPath)
            {
                Gizmos.color = Color.cyan;
                Vector3[] pathCorners = agent.path.corners;
                for (int i = 0; i < pathCorners.Length - 1; i++)
                {
                    Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
                }
            }
        }
    }
#endif
}