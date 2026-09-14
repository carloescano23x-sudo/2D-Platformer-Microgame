using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BodyPart : MonoBehaviour
{
    // Sprite used when this body part receives slicing damage
    public Sprite detachedSprite;

    // Sprite used when this body part receives burning damage
    public Sprite burnedSprite;

    // Position and rotation where the blood fountain will appear
    public Transform bloodFountainOrigin;

    // True after this body part has detached
    bool detached = false;

    public void Detach()
    {
        detached = true;

        tag = "Untagged";

        transform.SetParent(null, true);
    }

    void Update()
    {
        if (detached == false)
        {
            return;
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();

        if (body != null && body.IsSleeping())
        {
            foreach (Joint2D joint in GetComponentsInChildren<Joint2D>())
            {
                Destroy(joint);
            }

            foreach (Rigidbody2D rigidBody in GetComponentsInChildren<Rigidbody2D>())
            {
                Destroy(rigidBody);
            }

            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                Destroy(collider);
            }

            Destroy(this);
        }
    }

    public void ApplyDamageSprite(Gnome.DamageType damageType)
    {
        Sprite spriteToUse = null;

        switch (damageType)
        {
            case Gnome.DamageType.Burning:
                spriteToUse = burnedSprite;
                break;

            case Gnome.DamageType.Slicing:
                spriteToUse = detachedSprite;
                break;
        }

        if (spriteToUse != null)
        {
            GetComponent<SpriteRenderer>().sprite = spriteToUse;
        }
    }
}