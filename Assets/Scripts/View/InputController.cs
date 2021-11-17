using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private GameObject point2;

        [SerializeField] private TankView tankView;
        [SerializeField] private MenuController menu;
        [SerializeField] private FloorController floor;
        [SerializeField] private List<HouseController> houses;
        [SerializeField] private UI ui;
        [SerializeField] private MissilesEnemies missilesCreator;

        private Camera _camera;

        // Update is called once per frame
        private void Start()
        {
            _camera = Camera.main;
            floor.OnCollisionFromBullet += OnCollisionFromBullet;
            foreach (var house in houses)
            {
                house.OnCollisionFromBullet += OnCollisionFromBullet;
            }
            
            ui.GameOver += GameOver;
            ui.AumentoDeMissiles += AumentoDeMissiles;
        }

        private void AumentoDeMissiles()
        {
            missilesCreator.AddOneMoreMissile();
        }

        private void GameOver()
        {
            missilesCreator.StopCreatingMissile();
            tankView.StopAllMovements();
            ui.ShowGameOverAndOptions();
        }

        private void OnCollisionFromBullet(float damage)
        {
            //Logica para la vida y puntuacion
            ui.ApllyDamange(damage);
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
