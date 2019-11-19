using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageHelper : MonoBehaviour
{
    public static void GetFromUrl(string _url, Image _image, Sprite _default = null)
    {
        if (UIManager.instance != null)
            UIManager.instance.StartCoroutine(DownloadImg(_url, _image, _default));
    }

    public static void GetFBProfilePicture(string facebookId, Image _image, Sprite _default = null)
    {
        string _url = "https://graph.facebook.com/" + facebookId + "/picture?width=150&height=150";
        if (UIManager.instance != null)
            UIManager.instance.StartCoroutine(DownloadImg(_url, _image, _default));
    }

    static IEnumerator DownloadImg(string _url, Image _image, Sprite _default = null)
    {
        if (_default = null)
            _default = _image.sprite;
        Texture2D texture = new Texture2D(1, 1);
        WWW www = new WWW(_url);
        yield return www;
        try
        {
            if (string.IsNullOrEmpty(www.error))
            {
                www.LoadImageIntoTexture(texture);
                _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                _image.sprite = _default;
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            _image.sprite = _default;
        }
        finally
        {
            www.Dispose();
            www = null;
        }
    }
}
