using System.Collections;
using UnityEngine;

public class RemoveAfterDelay : MonoBehaviour
{
    public float delay = 1.0f;

    void Start()
    {
        StartCoroutine(Remove());
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }
}