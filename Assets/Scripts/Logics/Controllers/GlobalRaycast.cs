using System.Collections;
using TMPro;
using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class GlobalRaycast : MonoBehaviour
    {
        public float raycastDistance = 10f;
        public LayerMask layerMask;
        public new GameObject gameObject;

        private ItemPickup _detectedItem;

        public ItemPickup DetectedItem => _detectedItem;
        public TextMeshProUGUI titlePromp;
        public TextMeshProUGUI descriptionPrompt;
        private Coroutine _pickupTextCoroutine;
        public bool couroutineIsPlaying = false;

        void Start()
        {
            titlePromp.text = "";
            descriptionPrompt.text = "";
        }

        void Update()
        {
            RayCastLogic();
        }

        void OnDrawGizmos()
        {
            if (gameObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward * raycastDistance);
            }
        }

        public void ShowPickupMessage(string message)
        {
            // Dừng coroutine cũ nếu đang chạy
            if (_pickupTextCoroutine != null)
            {
                StopCoroutine(_pickupTextCoroutine);
            }

            // Bắt đầu coroutine mới
            _pickupTextCoroutine = StartCoroutine(ShowPickupMessageCoroutine(message));
        }

        private IEnumerator ShowPickupMessageCoroutine(string message)
        {
            if (descriptionPrompt != null)
            {
                couroutineIsPlaying = true;
                descriptionPrompt.text = message;
                titlePromp.text = ""; // Clear title khi show description

                yield return new WaitForSeconds(3f);

                descriptionPrompt.text = "";
                couroutineIsPlaying = false;

                // Sau khi coroutine kết thúc, cập nhật lại title nếu vẫn còn detect item
                if (_detectedItem != null)
                {
                    titlePromp.text = _detectedItem.displayName;
                }
            }
        }

        public void RayCastLogic()
        {
            _detectedItem = null;
            if (gameObject == null) return;

            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, raycastDistance, layerMask);

            if (hits.Length > 0)
            {
                // Sắp xếp theo khoảng cách gần nhất
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (RaycastHit hit in hits)
                {
                    GameObject target = hit.collider.gameObject;

                    if (target.CompareTag("Item"))
                    {
                        _detectedItem = target.GetComponent<ItemPickup>();

                        // Chỉ update title khi KHÔNG có coroutine đang chạy
                        if (_detectedItem != null && titlePromp != null && !couroutineIsPlaying)
                        {
                            titlePromp.text = _detectedItem.displayName;
                        }
                        return;
                    }
                    // if (target.CompareTag("NPC"))
                    // {
                    //     Debug.Log($"[GlobalRaycast] got raycast hit!! -> {target.name}");
                    // }
                    else
                    {
                        // Clear title chỉ khi không có coroutine đang chạy
                        if (titlePromp != null && !couroutineIsPlaying)
                            titlePromp.text = "";
                        return;
                    }
                }
            }
            else
            {
                // Clear title chỉ khi không có coroutine đang chạy  
                if (titlePromp != null && !couroutineIsPlaying)
                {
                    titlePromp.text = "";
                }
            }
        }
    }
}