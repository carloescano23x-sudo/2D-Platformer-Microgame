using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SignalOnTouch : MonoBehaviour
{
    public UnityEvent onTouch;

    public bool playAudioOnTouch = true;

    void OnTriggerEnter2D(Collider2D collider)
    {
        SendSignal(collider.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        SendSignal(collision.gameObject);
    }

    void SendSignal(GameObject objectThatHit)
    {
        if (objectThatHit.CompareTag("Player"))
        {
            if (playAudioOnTouch)
            {
                AudioSource audio =
                    GetComponent<AudioSource>();

                if (audio != null &&
                    audio.gameObject.activeInHierarchy)
                {
                    audio.Play();
                }
            }

            onTouch.Invoke();
        }
    }
}