using Model;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private GameObject point2;

        [SerializeField] private TankView tankView;
        [SerializeField] private MenuController menu;
        [SerializeField] private FloorController floor;
        [SerializeField] private UI ui;
        [SerializeField] private MissilesEnemies missilesCreator;
        [SerializeField] private Button again, exit;

        private Camera _camera;
        private bool _usage;

        // Update is called once per frame
        private void Start()
        {
            _camera = Camera.main;

            RunSession.Current.RunOver += GameOver;
            RunSession.Current.RunWon += Victory;

            again.onClick.AddListener(() =>
            {
                GetComponent<MenuController>().LoadScene(MenuController.MainMenuScene);
            });
            exit.onClick.AddListener(() =>
            {
                GetComponent<MenuController>().ExitGame();
            });
            _usage = true;
        }

        private void GameOver()
        {
            missilesCreator.StopCreatingMissile();
            tankView.StopAllMovements();
            ui.ShowGameOverAndOptions();
            _usage = false;
        }

        private void Victory()
        {
            _usage = false;
            ui.ShowVictoryAndOptions();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menu.LoadScene(MenuController.MainMenuScene);
            }

            if (!_usage) return;
            if (Input.GetMouseButton(0))
            {
                Vector3 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                point2.transform.position = worldPosition;
                Fire(worldPosition);
            }
        }

        public void SetUsage(bool usage)
        {
            _usage = usage;
        }

        private void Fire(Vector2 point)
        {
            tankView.Fire(point);
        }
    }
}