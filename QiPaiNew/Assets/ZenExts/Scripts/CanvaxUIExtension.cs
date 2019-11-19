using UnityEngine.UI;

public static class CanvaxUIExtension
{
    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        var color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
}
