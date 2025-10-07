
// using System.Collections;
// using System.Collections.Generic;
// using DATN2.Assets.Scripts.Data;
// using DATN2.Assets.Scripts.Logics.Controllers;
// using UnityEngine;
// using UnityEngine.AI;

// public class NPCMover : MonoBehaviour
// {


//     [Header("Waypoint Settings")]
//     [SerializeField] private List<Transform> destinations = new List<Transform>(); // các điểm di chuyển trong Scene
//     [SerializeField] private float stopDistance = 1.5f;       // khoảng cách tính là "đã tới"
//     [SerializeField] private float waitTimeAtPoint = 0.5f;    // thời gian dừng giữa các điểm
//     [SerializeField] private float sampleRadius = 5f;         // bán kính để kiểm tra NavMesh hợp lệ

//     private NavMeshAgent agent;
//     private int currentIndex = 0;
//     private bool isMoving = false;

//     void Start()
//     {
//         agent = GetComponent<NavMeshAgent>();

//         if (agent == null)
//         {
//             Debug.LogError($"{name}: NavMeshAgent not found!");
//             return;
//         }

//         if (destinations.Count == 0)
//         {
//             Debug.LogWarning($"{name}: No destinations assigned in Inspector!");
//             return;
//         }
//         LoadDestinationsFromQuest();
//         //StartCoroutine(MoveThroughPoints());
//     }

//     void LoadDestinationsFromQuest()
//     {
//         QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData("Map_1_OpeningS_Q1");


//         // destinations.Clear();

//         string[] keys = { "NPC(s)_Move_Point_1", "NPC(s)_Move_Point_2", "NPC(s)_Move_Point_3" };

//         foreach (string key in keys)
//         {
//             if (questData.quests[0].targetPosition.ContainsKey(key))
//             {
//                 Vector3 targetPos = questData.quests[0].targetPosition[key];



//                 Debug.Log($"{name}: Loaded destination {key} at {targetPos}");

//             }
//         }

//         Debug.Log($"{name}: Total destinations loaded: {destinations.Count}");
//     }

//     private IEnumerator MoveThroughPoints()
//     {
//         isMoving = true;

//         while (currentIndex < destinations.Count)
//         {
//             Transform target = destinations[currentIndex];

//             if (target == null)
//             {
//                 //Debug.LogWarning($"{name}: Destination {currentIndex} is null — skipping.");
//                 currentIndex++;
//                 continue;
//             }

//             // đảm bảo target nằm trên NavMesh
//             NavMeshHit hit;
//             if (!NavMesh.SamplePosition(target.position, out hit, sampleRadius, NavMesh.AllAreas))
//             {
//                 //Debug.LogError($"{name}: Destination {target.name} is not on NavMesh!");
//                 currentIndex++;
//                 continue;
//             }

//             agent.isStopped = false;
//             agent.SetDestination(hit.position);
//             //Debug.Log($"{name}: Moving to destination {currentIndex}: {target.name} ({hit.position})");

//             // chờ tới khi agent tới gần đích
//             float stuckTimer = 0f;
//             float maxStuckTime = 10f;
//             Vector3 lastPosition = transform.position;

//             while (true)
//             {
//                 float distance = Vector3.Distance(transform.position, target.position);
//                 float remainingDist = agent.remainingDistance;

//                 // kiểm tra bị kẹt
//                 if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
//                 {
//                     stuckTimer += Time.deltaTime;
//                     if (stuckTimer > maxStuckTime)
//                     {
//                         //Debug.LogWarning($"{name}: Stuck too long — skipping {target.name}");
//                         break;
//                     }
//                 }
//                 else
//                 {
//                     stuckTimer = 0f;
//                     lastPosition = transform.position;
//                 }

//                 // đã tới đích
//                 bool hasReached = (distance <= stopDistance) ||
//                                   (!agent.pathPending && remainingDist <= stopDistance && remainingDist != Mathf.Infinity);

//                 if (hasReached)
//                 {
//                     //Debug.Log($"{name}: ✓ Reached destination {currentIndex}: {target.name}");
//                     break;
//                 }

//                 yield return null;
//             }

//             // dừng và chờ 1 lúc trước khi đi tiếp
//             agent.isStopped = true;
//             yield return new WaitForSeconds(waitTimeAtPoint);

//             currentIndex++;
//         }

//         // hoàn tất di chuyển
//         isMoving = false;
//         agent.isStopped = true;
//         Debug.Log($"{name}: ★ Completed all destinations ★");
//     }

// #if UNITY_EDITOR
//     private void OnDrawGizmos()
//     {
//         // vẽ sphere tại các điểm waypoint
//         Gizmos.color = Color.green;
//         foreach (var dest in destinations)
//         {
//             if (dest != null)
//             {
//                 Gizmos.DrawWireSphere(dest.position, stopDistance);
//                 Gizmos.DrawLine(dest.position, dest.position + Vector3.up * 2f);
//             }
//         }

//         // vẽ line nối các waypoint
//         Gizmos.color = Color.yellow;
//         for (int i = 0; i < destinations.Count - 1; i++)
//         {
//             if (destinations[i] && destinations[i + 1])
//                 Gizmos.DrawLine(destinations[i].position, destinations[i + 1].position);
//         }

//         // vẽ đường đi hiện tại khi đang chạy
//         if (Application.isPlaying && isMoving && currentIndex < destinations.Count)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawLine(transform.position, destinations[currentIndex].position);

//             if (agent != null && agent.hasPath)
//             {
//                 Gizmos.color = Color.cyan;
//                 Vector3[] pathCorners = agent.path.corners;
//                 for (int i = 0; i < pathCorners.Length - 1; i++)
//                 {
//                     Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
//                 }
//             }
//         }
//     }
// #endif
// }




using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Data;
using DATN2.Assets.Scripts.Logics.Controllers;
using UnityEngine;
using UnityEngine.AI;

public class NPCMover : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private List<Vector3> destinations = new List<Vector3>();
    [SerializeField] private List<Transform> destinations_debug = new List<Transform>(); // các điểm di chuyển trong Scene
    [SerializeField] private float stopDistance = 1.5f;       // khoảng cách tính là "đã tới"
    [SerializeField] private float waitTimeAtPoint = 0.5f;    // thời gian dừng giữa các điểm
    [SerializeField] private float sampleRadius = 5f;         // bán kính để kiểm tra NavMesh hợp lệ
    [SerializeField] private bool Debug_Mode = false;

    private NavMeshAgent agent;
    private int currentIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent not found!");
            return;
        }

        if (destinations.Count == 0)
        {
            Debug.LogWarning($"{name}: No destinations assigned in Inspector!");
        }

        LoadDestinationsFromQuest();
        if (destinations.Count > 0 && Debug_Mode == false)
        {
            StartCoroutine(MoveThroughPoints());
        }
        else
        {
            Debug.LogError($"{name}: No valid destinations loaded!");
        }
        if (Debug_Mode)
        {

            if (destinations_debug.Count > 0)
            {
                StartCoroutine(MoveThroughPoints_Debug_Mode());
            }

        }
    }

    void LoadDestinationsFromQuest()
    {
        QuestDataSO questData = KeyGameStateManager.Instance.GetQuestData("Map_1_OpeningS_Q1");
        destinations.Clear(); // Clear any existing destinations

        string[] keys = { "NPC(s)_Move_Point_1", "NPC(s)_Move_Point_2", "NPC(s)_Move_Point_3" };

        foreach (string key in keys)
        {
            if (questData.quests[0].targetPosition.ContainsKey(key))
            {
                Vector3 targetPos = questData.quests[0].targetPosition[key];
                destinations.Add(targetPos);
                Debug.Log($"{name}: Loaded destination {key} at {targetPos}");
            }
        }

        Debug.Log($"{name}: Total destinations loaded: {destinations.Count}");
    }

    private IEnumerator MoveThroughPoints()
    {
        isMoving = true;

        while (currentIndex < destinations.Count)
        {
            Vector3 targetPos = destinations[currentIndex];

            // đảm bảo target nằm trên NavMesh
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(targetPos, out hit, sampleRadius, NavMesh.AllAreas))
            {
                //Debug.LogError($"{name}: Destination {currentIndex} at {targetPos} is not on NavMesh!");
                currentIndex++;
                continue;
            }

            agent.isStopped = false;
            agent.SetDestination(hit.position);
            //Debug.Log($"{name}: Moving to destination {currentIndex}: {hit.position}");

            // chờ tới khi agent tới gần đích
            float stuckTimer = 0f;
            float maxStuckTime = 10f;
            Vector3 lastPosition = transform.position;

            while (true)
            {
                float distance = Vector3.Distance(transform.position, targetPos);
                float remainingDist = agent.remainingDistance;

                // kiểm tra bị kẹt
                if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer > maxStuckTime)
                    {
                        //Debug.LogWarning($"{name}: Stuck too long — skipping destination {currentIndex}");
                        break;
                    }
                }
                else
                {
                    stuckTimer = 0f;
                    lastPosition = transform.position;
                }

                // đã tới đích
                bool hasReached = (distance <= stopDistance) ||
                                  (!agent.pathPending && remainingDist <= stopDistance && remainingDist != Mathf.Infinity);

                if (hasReached)
                {
                    Debug.Log($"{name}: ✓ Reached destination {currentIndex}");
                    break;
                }

                yield return null;
            }

            // dừng và chờ 1 lúc trước khi đi tiếp
            agent.isStopped = true;
            yield return new WaitForSeconds(waitTimeAtPoint);

            currentIndex++;
        }

        // hoàn tất di chuyển
        isMoving = false;
        agent.isStopped = true;
        Debug.Log($"{name}: ★ Completed all destinations ★");
    }
    private IEnumerator MoveThroughPoints_Debug_Mode()
    {
        isMoving = true;

        while (currentIndex < destinations.Count)
        {
            Transform target = destinations_debug[currentIndex];

            if (target == null)
            {
                //Debug.LogWarning($"{name}: Destination {currentIndex} is null — skipping.");
                currentIndex++;
                continue;
            }

            // đảm bảo target nằm trên NavMesh
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(target.position, out hit, sampleRadius, NavMesh.AllAreas))
            {
                //Debug.LogError($"{name}: Destination {target.name} is not on NavMesh!");
                currentIndex++;
                continue;
            }

            agent.isStopped = false;
            agent.SetDestination(hit.position);
            Debug.Log($"{name}: Moving to destination {currentIndex}: {target.name} ({hit.position})");

            // chờ tới khi agent tới gần đích
            float stuckTimer = 0f;
            float maxStuckTime = 10f;
            Vector3 lastPosition = transform.position;

            while (true)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                float remainingDist = agent.remainingDistance;

                // kiểm tra bị kẹt
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

                // đã tới đích
                bool hasReached = (distance <= stopDistance) ||
                                  (!agent.pathPending && remainingDist <= stopDistance && remainingDist != Mathf.Infinity);

                if (hasReached)
                {
                    Debug.Log($"{name}: ✓ Reached destination {currentIndex}: {target.name}");
                    break;
                }

                yield return null;
            }

            // dừng và chờ 1 lúc trước khi đi tiếp
            agent.isStopped = true;
            yield return new WaitForSeconds(waitTimeAtPoint);

            currentIndex++;
        }

        // hoàn tất di chuyển
        isMoving = false;
        agent.isStopped = true;
        Debug.Log($"{name}: ★ Completed all destinations ★");
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // vẽ sphere tại các điểm waypoint
        Gizmos.color = Color.green;
        foreach (var dest in destinations)
        {
            Gizmos.DrawWireSphere(dest, stopDistance);
            Gizmos.DrawLine(dest, dest + Vector3.up * 2f);
        }

        // vẽ line nối các waypoint
        Gizmos.color = Color.yellow;
        for (int i = 0; i < destinations.Count - 1; i++)
        {
            Gizmos.DrawLine(destinations[i], destinations[i + 1]);
        }

        // vẽ đường đi hiện tại khi đang chạy
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