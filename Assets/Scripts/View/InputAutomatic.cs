using UnityEngine;

namespace View
{
    public class InputAutomatic : MonoBehaviour
    {
        [SerializeField] private TankView tankView;
        [SerializeField] private GameObject center;
        [SerializeField] private float time;
        [SerializeField] private float ratio;
        private float _deltaTimeLocal;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            _deltaTimeLocal += Time.deltaTime;
            if (_deltaTimeLocal > time)
            {
                _deltaTimeLocal = 0;
                var position = (Random.insideUnitCircle * ratio) + (Vector2)center.transform.position;
                Debug.Log($"position {position}");
                tankView.Fire(position);
            }
        }
    }
}
