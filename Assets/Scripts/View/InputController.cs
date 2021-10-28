using UnityEngine;

namespace View
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private GameObject point2;

        [SerializeField] private TankView tankView;
        [SerializeField] private MenuController menu;

        private Camera _camera;

        // Update is called once per frame
        private void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                point2.transform.position = worldPosition;
                Fire(worldPosition);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menu.LoadScene(0);
            }
        }

        private void Fire(Vector2 point)
        {
            tankView.Fire(point);
        }
    }
}
