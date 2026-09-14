using UnityEngine;

public class Swinging : MonoBehaviour
{
    public float swingSensitivity = 100.0f;

    void FixedUpdate()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();

        if (body == null)
        {
            Destroy(this);
            return;
        }

        float swing = InputManager.instance.sidewaysMotion;

        Vector2 force =
            new Vector2(swing * swingSensitivity, 0);

        body.AddForce(force);
    }
}