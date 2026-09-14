using System.Collections;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    // Object that the camera should follow
    public Transform cameraFollowTarget;

    // Leg connected to the rope
    public Rigidbody2D ropeBody;

    // Arm sprites
    public Sprite armHoldingEmpty;
    public Sprite armHoldingTreasure;

    // Arm SpriteRenderer
    public SpriteRenderer holdingArm;

    // Damage effect prefabs
    public GameObject deathPrefab;
    public GameObject flameDeathPrefab;

    // Ghost prefab
    public GameObject ghostPrefab;

    // Delay before removing the dead gnome
    public float delayBeforeRemoving = 3.0f;

    // Delay before spawning the ghost
    public float delayBeforeReleasingGhost = 0.25f;

    // Blood fountain particle prefab
    public GameObject bloodFountainPrefab;

    bool dead = false;

    bool _holdingTreasure = false;

    public bool holdingTreasure
    {
        get
        {
            return _holdingTreasure;
        }

        set
        {
            if (dead == true)
            {
                return;
            }

            _holdingTreasure = value;

            if (holdingArm != null)
            {
                if (_holdingTreasure)
                {
                    holdingArm.sprite = armHoldingTreasure;
                }
                else
                {
                    holdingArm.sprite = armHoldingEmpty;
                }
            }
        }
    }

    public enum DamageType
    {
        Slicing,
        Burning
    }

    public void ShowDamageEffect(DamageType type)
    {
        switch (type)
        {
            case DamageType.Burning:

                if (flameDeathPrefab != null)
                {
                    Instantiate(
                        flameDeathPrefab,
                        cameraFollowTarget.position,
                        cameraFollowTarget.rotation
                    );
                }

                break;

            case DamageType.Slicing:

                if (deathPrefab != null)
                {
                    Instantiate(
                        deathPrefab,
                        cameraFollowTarget.position,
                        cameraFollowTarget.rotation
                    );
                }

                break;
        }
    }

    public void DestroyGnome(DamageType type)
    {
        holdingTreasure = false;

        dead = true;

        foreach (BodyPart part in GetComponentsInChildren<BodyPart>())
        {
            switch (type)
            {
                case DamageType.Burning:

                    bool shouldBurn = Random.Range(0, 2) == 0;

                    if (shouldBurn)
                    {
                        part.ApplyDamageSprite(type);
                    }

                    break;

                case DamageType.Slicing:

                    part.ApplyDamageSprite(type);

                    break;
            }

            bool shouldDetach = Random.Range(0, 2) == 0;

            if (shouldDetach)
            {
                part.Detach();

                if (type == DamageType.Slicing)
                {
                    if (part.bloodFountainOrigin != null &&
                        bloodFountainPrefab != null)
                    {
                        GameObject fountain = Instantiate(
                            bloodFountainPrefab,
                            part.bloodFountainOrigin.position,
                            part.bloodFountainOrigin.rotation
                        );

                        fountain.transform.SetParent(
                            cameraFollowTarget,
                            false
                        );
                    }
                }

                Joint2D[] allJoints =
                    part.GetComponentsInChildren<Joint2D>();

                foreach (Joint2D joint in allJoints)
                {
                    Destroy(joint);
                }
            }
        }

        RemoveAfterDelay remove =
            gameObject.AddComponent<RemoveAfterDelay>();

        remove.delay = delayBeforeRemoving;

        StartCoroutine(ReleaseGhost());
    }

    IEnumerator ReleaseGhost()
    {
        if (ghostPrefab == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(
            delayBeforeReleasingGhost
        );

        Instantiate(
            ghostPrefab,
            transform.position,
            Quaternion.identity
        );
    }
}