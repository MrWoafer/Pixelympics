using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteEdit
{
    public static Sprite ReplaceColours(Sprite sprite, Color[] originalColours, Color[] newColours)
    {
        List<int> colouredPixels = new List<int>();
        Texture2D tex = sprite.texture;
        Color32[] colourArray = tex.GetPixels32();
        for (int j = 0; j < originalColours.Length; j++)
        {
            for (int i = 0; i < colourArray.Length; i++)
            {
                if (colourArray[i] == originalColours[j] && !colouredPixels.Contains(i))
                {
                    colourArray[i] = newColours[j];
                    colouredPixels.Add(i);
                }
            }
        }
        tex.SetPixels32(colourArray);
        tex.Apply();
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 64f);
        return newSprite;
    }

    public static Sprite ReplaceColour(Sprite sprite, Color originalColour, Color newColour)
    {
        return ReplaceColours(sprite, new Color[] { originalColour }, new Color[] { newColour });
    }

    public static Sprite CopySprite(Sprite sprite)
    {
        return Sprite.Create(sprite.texture, new Rect(0, 0, sprite.texture.width, sprite.texture.height), new Vector2(0f, 0f), sprite.pixelsPerUnit, 0, SpriteMeshType.Tight);
    }
}
