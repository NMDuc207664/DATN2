using UnityEngine;
namespace DATN2.Assets.Scripts.Logics.Controllers
{
    public class GlobalRaycast : MonoBehaviour
    {
        public float raycastDistance = 10f;
        public LayerMask layerMask;
        // private Camera camera;
        public GameObject gameObject;

        private ItemPickup _detectedItem;

        public ItemPickup DetectedItem => _detectedItem;
        void Start()
        {
            // camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            _detectedItem = null;
            if (gameObject == null) return;
            // Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            {
                GameObject target = hit.collider.gameObject;
                if (target.CompareTag("Item"))
                {
                    // Nếu bấm E thì nhặt
                    // if (Input.GetKeyDown(KeyCode.E))
                    // {
                    //     Debug.Log("Nhìn thấy item: " + target.name);
                    //     var pickup = target.GetComponent<ItemPickup>();
                    //     if (pickup != null)
                    //     {
                    //         // pickup.PickUp();
                    //         Debug.Log("Nhắt item: " + pickup.itemData.itemName);
                    //     }
                    // }
                    _detectedItem = hit.collider.GetComponent<ItemPickup>();
                }
                // if (hit.collider.CompareTag("Item"))
                // {
                //     _detectedItem = hit.collider.GetComponent<ItemPickup>();
                // }
            }
        }
        void OnDrawGizmos()
        {
            if (gameObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward * raycastDistance);
            }
        }
    }
}