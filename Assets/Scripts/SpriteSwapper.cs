using UnityEngine;

public class SpriteSwapper : MonoBehaviour
{
    public Sprite spriteToUse;
    public SpriteRenderer spriteRenderer;

    private Sprite originalSprite;

    public void SwapSprite()
    {
        if (spriteRenderer == null)
            return;

        if (spriteToUse != spriteRenderer.sprite)
        {
            originalSprite = spriteRenderer.sprite;
            spriteRenderer.sprite = spriteToUse;
        }
    }

    public void ResetSprite()
    {
        if (spriteRenderer == null)
            return;

        if (originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
    }
}