//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using AssetBundles;

//public class UISplash_AssetBundle : MonoBehaviour
//{
//	public string sceneName;
//	public UISlider loadingBar;
//	public static string status;
//	private string AssetBundleName = "scene";
//	private float progress;
//	// Use this for initialization
//	IEnumerator Start()
//	{
//		yield return StartCoroutine(Initialize());
//		yield return StartCoroutine(InitializeLevelAsync(sceneName, true));
//	}

//	void Update()
//	{
//		if (progress < AssetBundleManager.progress)
//			progress = AssetBundleManager.progress;
		
//		var scaledPerc = 100f * progress;

//		if (progress > 0.9f)
//			loadingBar.Loading("Vui lòng đợi giây lát...", scaledPerc);
//		else
//			loadingBar.Loading("Đang tải...", scaledPerc);
//	}

//	IEnumerator LoadGameSceneAsync(string name)
//	{
//		AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
//		async.allowSceneActivation = false;
//		while (async.progress < 0.9f)
//		{
//			var scaledPerc = 100f * (0.75f * async.progress / 0.9f);
//			loadingBar.Loading("Đang tải...", scaledPerc);
//			status = "Đang tải " + scaledPerc.ToString("F0") + "...";
//		}

//		async.allowSceneActivation = true;
//		float perc = 0.75f;
//		while (!async.isDone)
//		{
//			yield return null;
//			perc = Mathf.Lerp(perc, 1f, 0.1f);
//			status = "Đang tải " + (100f * perc).ToString("F0") + "...";
//			loadingBar.Loading("Vui lòng chờ giây lát...", 100f * perc);
//		}

//		float fake = loadingBar.slider.value;
//		while (fake < 100)
//		{
//			yield return new WaitForEndOfFrame();
//			fake += 1;
//			loadingBar.Loading("Vui lòng chờ giây lát...", fake);
//		}
//		loadingBar.Loading("Vui lòng chờ giây lát...", 100);

//		CanvasGroup canvasGroup = loadingBar.GetComponent<RectTransform>().GetComponent<CanvasGroup>();
//		if (canvasGroup == null)
//			canvasGroup = loadingBar.GetComponent<RectTransform>().gameObject.AddComponent<CanvasGroup>();

//		while (canvasGroup.alpha > 0)
//		{
//			yield return new WaitForEndOfFrame();
//			canvasGroup.alpha -= 0.1f;
//		}
//		SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
//	}

//	// Initialize the downloading URL.
//	// eg. Development server / iOS ODR / web URL
//	void InitializeSourceURL()
//	{
//		// If ODR is available and enabled, then use it and let Xcode handle download requests.
//		#if ENABLE_IOS_ON_DEMAND_RESOURCES
//		if (UnityEngine.iOS.OnDemandResources.enabled)
//		{
//		AssetBundleManager.SetSourceAssetBundleURL("odr://");
//		return;
//		}
//		#endif
//		//#if DEVELOPMENT_BUILD || UNITY_EDITOR
//		// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
//		// (This is very dependent on the production workflow of the project.
//		//      Another approach would be to make this configurable in the standalone player.)
//		//AssetBundleManager.SetDevelopmentAssetBundleServer();
//		//return;
//		//#else
//		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
//		//AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "Assets/AssetBundles/");
//		// Or customize the URL based on your deployment or configuration
//		AssetBundleManager.SetSourceAssetBundleURL("https://tuquyk.com/test/");
//		return;
//		//#endif
//	}

//	// Initialize the downloading url and AssetBundleManifest object.
//	protected IEnumerator Initialize()
//	{
//		// Don't destroy the game object as we base on it to run the loading script.
//		DontDestroyOnLoad(gameObject);

//		InitializeSourceURL();

//		// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
//		var request = AssetBundleManager.Initialize();

//		if (request != null)
//			yield return StartCoroutine(request);
//	}

//	protected IEnumerator InitializeLevelAsync(string levelName, bool isAdditive)
//	{
//		// This is simply to get the elapsed time for this phase of AssetLoading.
//		float startTime = Time.realtimeSinceStartup;

//		// Load level from assetBundle.
//		float progress = AssetBundleManager.progress;
//		AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(AssetBundleName, levelName, isAdditive);

//		if (request == null)
//			yield break;
//		yield return StartCoroutine(request);
////
////		var scaledPerc = 100f * (0.75f * async.progress / 0.9f);
////		loadingBar.Loading("Đang tải...", scaledPerc);
////		status = "Đang tải " + scaledPerc.ToString("F0") + "...";
//		// Calculate and display the elapsed time.

////		float elapsedTime = Time.realtimeSinceStartup - startTime;
////		Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
////
//		CanvasGroup canvasGroup = loadingBar.GetComponent<RectTransform>().GetComponent<CanvasGroup>();
//		if (canvasGroup == null)
//			canvasGroup = loadingBar.GetComponent<RectTransform>().gameObject.AddComponent<CanvasGroup>();

//		while (canvasGroup.alpha > 0)
//		{
//			yield return new WaitForEndOfFrame();
//			canvasGroup.alpha -= 0.1f;
//		}
//	}
//}

