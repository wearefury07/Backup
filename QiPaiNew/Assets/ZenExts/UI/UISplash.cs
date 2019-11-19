using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISplash : MonoBehaviour
{
    public string sceneName;
    public UISlider loadingBar;
    public static string status;

    // Use this for initialization
    private void Start()
	{	
		#if UNITY_WEBGL
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive); 
		CanvasGroup canvasGroup = loadingBar.GetComponent<RectTransform>().GetComponent<CanvasGroup>(); 
		if (canvasGroup == null)
			canvasGroup = loadingBar.GetComponent<RectTransform>().gameObject.AddComponent<CanvasGroup>();

		if (canvasGroup != null)
			canvasGroup.alpha = 0f;
        //if (SceneManager.GetSceneByName(sceneName) != null)
        //    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
		#else
        LoadGameScene(sceneName);
		#endif
    }

    public void LoadGameScene(string name)
    {
        StartCoroutine(LoadGameSceneAsync(name));
    }

    IEnumerator LoadGameSceneAsync(string name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            var scaledPerc = 100f * (0.75f * async.progress / 0.9f);
            loadingBar.Loading("Đang tải...", scaledPerc);
            status = "Đang tải " + scaledPerc.ToString("F0") + "...";
        }

        async.allowSceneActivation = true;
        float perc = 0.75f;
        while (!async.isDone)
        {
            yield return null;
            perc = Mathf.Lerp(perc, 1f, 0.1f);
            status = "Đang tải " + (100f * perc).ToString("F0") + "...";
            loadingBar.Loading("Vui lòng chờ giây lát...", 100f * perc);
        }

        CanvasGroup canvasGroup = loadingBar.GetComponent<RectTransform>().GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = loadingBar.GetComponent<RectTransform>().gameObject.AddComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            yield return new WaitForEndOfFrame();
            canvasGroup.alpha -= 0.1f;
        }

        float fake = loadingBar.slider.value;
        while (fake < 100)
        {
            yield return new WaitForEndOfFrame();
            fake += 1;
            loadingBar.Loading("Vui lòng chờ giây lát...", fake);
        }
        loadingBar.Loading("Vui lòng chờ giây lát...", 100);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
    }
}
