using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] private GameObject canion;
    [SerializeField] private float angleMore;
    [SerializeField] private float min;

    private bool _rotating;
    private Vector2 point;

    private Quaternion concurrentAngle;
    // Update is called once per frame
    void Update()
    {
        if (_rotating)
        {
            var angle = GetAngle();
            var diff = Quaternion.Slerp(canion.transform.rotation, Quaternion.Euler(0, 0, angle), angleMore * Time.deltaTime);
            var diffRotationZ = canion.transform.rotation.z - Quaternion.Euler(0, 0, angle).z;
            if (Mathf.Abs(diffRotationZ) < min)
            {
                Debug.Log($"angle {angle} diffRotationZ {diffRotationZ}");
            }
            canion.transform.rotation = diff;
        }
    }

    private float GetAngle()
    {
        var position = canion.transform.position;
        var diff = point - (Vector2)position;
        var angle = Vector2.Angle(diff,Vector2.up);
        var cross = Vector3.Cross(position, point);
        if (cross.z > 0)
        {
            angle *= -1;
        }
        return angle;
    }

    private void RotateTo(Vector2 toPoint)
    {
        _rotating = true;
        point = toPoint;
    }

    public void Fire(Vector2 vector2)
    {
        RotateTo(vector2);
    }
}
