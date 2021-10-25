using UnityEngine;

namespace View
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private GameObject point2;

        [SerializeField] private Tank tank;
        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                point2.transform.position = new Vector3(touch.position.x, touch.position.y, 0);
                Fire(touch.position);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                point2.transform.position = worldPosition;
                Fire(worldPosition);
            }
        }

        private void Fire(Vector2 point)
        {
            tank.Fire(point);
        }
    }
}
