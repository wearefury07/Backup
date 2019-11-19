using UnityEngine;
using UnityEngine.UI;

public static class ZenExtension
{
    public static void SetPosition(this Transform transform, Vector3 pos, Space space = Space.World)
    {
        if (space == Space.World)
            transform.position = pos;
        else
            transform.localPosition = pos;
    }
    public static void SetPosition(this Transform transform, Vector2 pos)
    {
        transform.position = pos;
    }
    public static void SetPosition(this Transform transform, float x, float y, float z = 0)
    {
        transform.position = new Vector3(x, y, z);
    }
    public static void SetZ(this Transform transform, float z, Space space = Space.World)
    {
        if (space == Space.World)
        {
            var pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }
        else
        {
            var pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
        }
    }

    public static void SetScale(this Transform transform, Vector3 scale)
    {
        transform.localScale = scale;
    }
    public static void SetScale(this Transform transform, float scale)
    {
        transform.localScale = scale * Vector3.one;
    }
    public static void SetRotation(this Transform transform, Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }
    public static void SetRotation(this Transform transform, float rotation, Space space = Space.World)
    {
        if(space == Space.World)
            transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        else
            transform.localRotation = Quaternion.Euler(Vector3.forward * rotation);
    }

    public static void SetAlpha(this SpriteRenderer sprite, float alpha)
    {
        var color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }

    public static void SetColor(this SpriteRenderer sprite, float r, float g, float b)
    {
        var color = sprite.color;
        color.r = r;
        color.g = Mathf.Clamp(g, 0, 1);
        color.b = b;
        sprite.color = color;
    }
    public static void SetColor(this SpriteRenderer sprite, Color color)
    {
        sprite.color = color;
    }

    public static void SetAlpha(this Image img, float alpha)
    {
        var color = img.color;
        color.a = alpha;
        img.color = color;
    }

    public static void SetColor(this Image img, float r, float g, float b)
    {
        var color = img.color;
        color.r = r;
        color.g = g;
        color.b = b;
        img.color = color;
    }
    public static void SetColor(this Image img, Color color)
    {
        img.color = color;
    }
}
