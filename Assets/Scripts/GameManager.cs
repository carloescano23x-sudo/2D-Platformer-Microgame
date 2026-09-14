using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject startingPoint;
    public Rope rope;
    public CameraFollow cameraFollow;

    private Gnome currentGnome;

    public GameObject gnomePrefab;

    public RectTransform mainMenu;
    public RectTransform gameplayMenu;
    public RectTransform gameOverMenu;

    public bool gnomeInvincible { get; set; }

    public float delayAfterDeath = 1.0f;

    public AudioClip gnomeDiedSound;
    public AudioClip gameOverSound;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.gameObject.SetActive(false);
        }

        if (mainMenu != null)
        {
            mainMenu.gameObject.SetActive(false);
        }

        if (gameplayMenu != null)
        {
            gameplayMenu.gameObject.SetActive(true);
        }

        Resettable[] resetObjects =
    FindObjectsByType<Resettable>();

        foreach (Resettable r in resetObjects)
        {
            r.Reset();
        }

        if (gnomePrefab != null &&
            startingPoint != null &&
            rope != null &&
            cameraFollow != null)
        {
            CreateNewGnome();
        }

        Time.timeScale = 1.0f;
    }

    void CreateNewGnome()
    {
        RemoveGnome();

        GameObject newGnome = Instantiate(
            gnomePrefab,
            startingPoint.transform.position,
            Quaternion.identity
        );

        currentGnome =
            newGnome.GetComponent<Gnome>();

        if (currentGnome == null)
        {
            Debug.LogError(
                "The Gnome Prefab does not contain a Gnome component."
            );

            return;
        }

        rope.gameObject.SetActive(true);

        rope.connectedObject =
            currentGnome.ropeBody;

        rope.ResetLength();

        cameraFollow.target =
            currentGnome.cameraFollowTarget;
    }

    void RemoveGnome()
    {
        if (gnomeInvincible)
        {
            return;
        }

        if (rope != null)
        {
            rope.gameObject.SetActive(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.target = null;
        }

        if (currentGnome != null)
        {
            currentGnome.holdingTreasure = false;

            currentGnome.gameObject.tag =
                "Untagged";

            foreach (Transform child in
                     currentGnome.transform)
            {
                child.gameObject.tag =
                    "Untagged";
            }

            currentGnome = null;
        }
    }

    void KillGnome(Gnome.DamageType damageType)
    {
        if (currentGnome == null)
        {
            return;
        }

        AudioSource audio =
            GetComponent<AudioSource>();

        if (audio != null &&
            gnomeDiedSound != null)
        {
            audio.PlayOneShot(
                gnomeDiedSound
            );
        }

        currentGnome.ShowDamageEffect(
            damageType
        );

        if (gnomeInvincible == false)
        {
            currentGnome.DestroyGnome(
                damageType
            );

            RemoveGnome();

            StartCoroutine(
                ResetAfterDelay()
            );
        }
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(
            delayAfterDeath
        );

        Reset();
    }

    public void TrapTouched()
    {
        KillGnome(
            Gnome.DamageType.Slicing
        );
    }

    public void FireTrapTouched()
    {
        KillGnome(
            Gnome.DamageType.Burning
        );
    }

    public void TreasureCollected()
    {
        if (currentGnome != null)
        {
            currentGnome.holdingTreasure =
                true;
        }
    }

    public void ExitReached()
    {
        if (currentGnome != null &&
            currentGnome.holdingTreasure)
        {
            AudioSource audio =
                GetComponent<AudioSource>();

            if (audio != null &&
                gameOverSound != null)
            {
                audio.PlayOneShot(
                    gameOverSound
                );
            }

            Time.timeScale = 0.0f;

            if (gameOverMenu != null)
            {
                gameOverMenu.gameObject
                    .SetActive(true);
            }

            if (gameplayMenu != null)
            {
                gameplayMenu.gameObject
                    .SetActive(false);
            }
        }
    }

    public void SetPaused(bool paused)
    {
        Time.timeScale =
            paused ? 0.0f : 1.0f;

        if (mainMenu != null)
        {
            mainMenu.gameObject
                .SetActive(paused);
        }

        if (gameplayMenu != null)
        {
            gameplayMenu.gameObject
                .SetActive(!paused);
        }
    }

    public void RestartGame()
    {
        if (currentGnome != null)
        {
            Destroy(
                currentGnome.gameObject
            );

            currentGnome = null;
        }

        Reset();
    }
}