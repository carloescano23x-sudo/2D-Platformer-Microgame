using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private float _sidewaysMotion = 0.0f;

    public float sidewaysMotion
    {
        get
        {
            return _sidewaysMotion;
        }
    }

    void Update()
    {
        Vector3 accel = Input.acceleration;
        _sidewaysMotion = accel.x;
    }
}