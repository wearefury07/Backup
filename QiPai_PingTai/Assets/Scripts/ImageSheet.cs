using System.Collections.Generic;
using UnityEngine;
//using AssetBundles;

public class ImageSheet : MonoBehaviour
{
    public static ImageSheet Instance;
    private Sprite[] resources;
    public Dictionary<string, Sprite> resourcesDics;

    void Awake()
    {
        Instance = this;
		//string error;
		//LoadedAssetBundle _assetBundle = AssetBundleManager.GetLoadedAssetBundle ("image", out error);

		//if (_assetBundle != null)
		//	resources = _assetBundle.m_AssetBundle.LoadAllAssets<Sprite> ();
		//else
		resources = Resources.LoadAll<Sprite> ("");
		
        resourcesDics = new Dictionary<string, Sprite>();
        foreach (var i in resources)
        {
            try
            {
                resourcesDics.Add(i.name, i);
            }
            catch(System.Exception ex)
            {
                Debug.Log("Duplicate: " + i.name + " " + ex.ToString());
            }
        }
    }
}
