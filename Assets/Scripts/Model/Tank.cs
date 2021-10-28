using UnityEngine;
using View;

namespace Model
{
    public class Tank
    {
        private readonly ITankView _view;
        private readonly Vector2 _cannon;
        private readonly float _cooldown;
        private readonly float _min;
        
        private bool _rotating;
        private Vector2 _point;
        private float _cooldownDeltaTime;

        public Tank(ITankView view, Vector2 cannon, float cooldown, float min)
        {
            _view = view;
            _cannon = cannon;
            _cooldown = cooldown;
            _min = min;
        }

        public float GetAngle()
        {
            var diff = _point - _cannon;
            var angle = _view.GetAngleUpFrom(diff);
            if (_view.IsLeft(_cannon, _point))
            {
                angle *= -1;
            }
            return angle;
        }

        public void Rotate(Vector2 vector2)
        {
            _rotating = true;
            _point = vector2;
        }

        public bool CanRotate()
        {
            return _rotating;
        }

        public void StopRotating()
        {
            _rotating = false;
        }

        public Vector2 GetGoal()
        {
            return _point;
        }

        public Vector2 GetDirectionNormalize()
        {
            return (_point - _cannon).normalized;
        }

        public bool CanShoot(float diffRotationZ)
        {
            var can= diffRotationZ < _min && _cooldownDeltaTime > _cooldown;
            if (can)
            {
                _cooldownDeltaTime = 0;
            }
            return can;
        }

        public void AddDelta(float deltaTime)
        {
            _cooldownDeltaTime += deltaTime;
        }
    }
}