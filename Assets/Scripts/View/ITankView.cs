using UnityEngine;

namespace View
{
    public interface ITankView
    {
        float GetAngleUpFrom(Vector2 diff);
        bool IsLeft(Vector2 position, Vector2 point);
    }
}