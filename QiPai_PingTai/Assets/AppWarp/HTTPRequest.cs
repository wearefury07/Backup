using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Text;

public class HttpRequest
{
    private static HttpRequest instance;
    public static HttpRequest Instance
    {
        get
        {
            if (instance == null)
                instance = new HttpRequest();
            return instance;
        }
    }

    public static string keyyRRCC4 = "uoyjvexhmjaqfqc";
    private static string url_getPaypaypage = "http://win360game.com/deposit";

    private static string url_initialize = OldWarpChannel.httpRequest + "initialize";
    private static string url_forgot_password = OldWarpChannel.httpRequest + "auth/forgot_password";
    private static string url_register = OldWarpChannel.httpRequest + "auth/register";
    private static string url_version_cfg = OldWarpChannel.httpRequest + "version_cfg";
    //private static string url_payment_initialize = OldWarpChannel.httpRequest + "payment/initialize";
    //private static string url_payment_complete = OldWarpChannel.httpRequest + "payment/complete";
    //private static string url_cfg_by_key = OldWarpChannel.httpRequest + "cfg_by_key";
    private static string url_dealer_cfg = OldWarpChannel.httpRequest + "dealer_cfg";

    #region ActionOnResponse
    public delegate void GetAPITokenDelegate(WarpResponseResultCode status, string apiToken);
    public static event GetAPITokenDelegate OnGetAPITokenDone;

    public delegate void ForgotPasswordDelegate(WarpResponseResultCode status, string data);
    public static event ForgotPasswordDelegate OnForgotPasswordDone;

    public delegate void GetVersionConfigureDelegate(WarpResponseResultCode status);
    public static event GetVersionConfigureDelegate OnGetVersionConfigureDone;

    public delegate void RegisterDelegate(WarpResponseResultCode status, UserData data);
    public static event RegisterDelegate OnRegisterDone;

    public delegate void GetDealerConfigDelegate(WarpResponseResultCode status, List<UserData> data);
    public static event GetDealerConfigDelegate OnGetDealerConfigDone;

	public delegate void CheckLocationDelegate(WarpResponseResultCode status, bool result);
	public static event CheckLocationDelegate OnCheckLocationDone;
    #endregion

    public IEnumerator GetAPIToken()
    {
        WWWForm form = new WWWForm();
        form.AddField("api_key", GameBase.api_key);
        form.AddField("api_secret", GameBase.api_secret);
        WWW www = new WWW(url_initialize, form);
        yield return www;

        try
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("Error " + www.error);
                OnGetAPITokenDone(WarpResponseResultCode.BAD_REQUEST, GameBase.apiToken);
            }
            else
            {
                var data = JsonUtility.FromJson<InitializeData>(www.text);
                if (data != null && data.status == 0)
                    GameBase.apiToken = data.data;
                OnGetAPITokenDone(WarpResponseResultCode.SUCCESS, GameBase.apiToken);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
			OnGetAPITokenDone(WarpResponseResultCode.BAD_REQUEST, GameBase.apiToken);
        }
    }

    public IEnumerator GetAPIPaypay(string tokenStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("", tokenStr);
        WWW www = new WWW(url_getPaypaypage, form);

        yield return www;
    }

    public IEnumerator GetVersionConfigure()
    {
        var form = new WWWForm();
        form.AddField("os_type", GameBase.osType.ToString());
        form.AddField("version", GameBase.clientVersion);
        form.AddField("providerCode", GameBase.providerCode);

        var www = new WWW(url_version_cfg, form);
        yield return www;

        try
        {
            HandleGetVersionConfig(www.text);
            if (OnGetVersionConfigureDone != null)
                OnGetVersionConfigureDone(WarpResponseResultCode.SUCCESS);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());

            var _backupData = PlayerPrefs.GetString("versionConfig");
            HandleGetVersionConfig(_backupData);
            if (OnGetVersionConfigureDone != null)
                OnGetVersionConfigureDone(WarpResponseResultCode.BAD_REQUEST);
        }
    }

	public IEnumerator GetVersionConfigureV2()
	{
		var form = new WWWForm();
		form.AddField("os_type", GameBase.osType.ToString());
		form.AddField("version", GameBase.clientVersion);
		form.AddField("providerCode", GameBase.providerCode);

		var www = new WWW(url_version_cfg, form);
		yield return www;

		try
		{
			var root = new JSONObject(www.text);
			if (root != null && root["status"].i == 0)
			{
				var data = new JSONObject(root["data"].ToString());
				if (data != null)
				{

					var tempData = new ConfigData();

					GameBase.website = data["web"] != null ? data["web"].str : "";
					GameBase.fbFanpage = data["fb"] != null ? data["fb"].str : "";
					GameBase.emailSupport = data["email"] != null ? data["email"].str : "";

					tempData.hotline = new List<string>();
					var hotlineListTemp = new JSONObject(data["hotline"].ToString());
					for (int i = 0; i < hotlineListTemp.Count; i++)
					{
						try
						{
							tempData.hotline.Add(hotlineListTemp[i].str);
						}
						catch (Exception ex)
						{
							Debug.LogError("GetVersionConfigure: hotlineListTemp: " + ex.Message);
						}
					}

					if (tempData.hotline.Any())
						GameBase.hotline = tempData.hotline.FirstOrDefault();

					tempData.unsupportVersion = new List<Version>();
					var unsupportVersionListTemp = new JSONObject(data["unsupportVersion"].ToString());
					for (int i = 0; i < unsupportVersionListTemp.Count; i++)
					{
						try
						{
							tempData.unsupportVersion.Add(GameBase.StringToVersion(unsupportVersionListTemp[i].str));
						}
						catch (Exception ex)
						{
							Debug.LogError("GetVersionConfigure: unsupportVersion: " + ex.Message);
						}
					}

					if (tempData.unsupportVersion.Any(x => x == GameBase.currentVersion))
						GameBase.needUpdateToPlay = true;

					tempData.providers = new List<Provider>();
					var providersListTemp = new JSONObject(data["providers"].ToString());
					for (int i = 0; i < providersListTemp.Count; i++)
					{
						try
						{
							var provider = new Provider();
							var providerTemp = new JSONObject(providersListTemp[i].ToString());
							if (providerTemp != null)
							{
								provider.providerCode = providerTemp["providerCode"] != null ? providerTemp["providerCode"].str : "";
								provider.fb = providerTemp["fb"] != null ? providerTemp["fb"].str : "";
								provider.socketIp = providerTemp["socketIp"] != null ? providerTemp["socketIp"].str : "";
								provider.socketPort = providerTemp["socketPort"] != null ? providerTemp["socketPort"].str : "";

								if (!string.IsNullOrEmpty(provider.fb))
									GameBase.fbFanpage = provider.fb;

								var packages = new List<Package>();
								var packagesTemp = new JSONObject(providerTemp["packages"].ToString());
								if (packagesTemp != null)
								{
									for (int j = 0; j < packagesTemp.Count; j++)
									{
										try
										{
											var package = new Package();
											Debug.Log(packagesTemp[j].ToString());
											var packTemp = new JSONObject(packagesTemp[j].ToString());
											if (packTemp != null)
											{
												package.appId = packTemp["appId"] != null ? packTemp["appId"].str : "";
												package.link = packTemp["link"] != null ? packTemp["link"].str : "";
												package.latestVersion = packTemp["latestVersion"] != null ? GameBase.StringToVersion(packTemp["latestVersion"].str) : new Version();
												package.reviewVersion = packTemp["reviewVersion"] != null ? GameBase.StringToVersion(packTemp["reviewVersion"].str) : new Version();
												packages.Add(package);
											}
										}
										catch (Exception ex)
										{
											Debug.LogError("GetVersionConfigure: packages: " + ex.Message + " " + ex.StackTrace);
										}
									}
									provider.packages = packages;
								}
								tempData.providers.Add(provider);
							}
						}
						catch (Exception ex)
						{
							Debug.LogError("GetVersionConfigure: providers: " + ex.Message);
						}
					}

					var checkProvider = tempData.providers.FirstOrDefault(x => x.packages.Any(xx => xx.appId == GameBase.appPackageName));
					if (checkProvider != null)
					{
						GameBase.providerCode = checkProvider.providerCode;
						if (!string.IsNullOrEmpty(checkProvider.socketIp) && !string.IsNullOrEmpty(checkProvider.socketIp))
						{
							OldWarpChannel.warp_host = checkProvider.socketIp ;
							OldWarpChannel.warp_port = Convert.ToInt32(checkProvider.socketPort);
						}

						var checkPack = checkProvider.packages.FirstOrDefault(x => x.appId == GameBase.appPackageName);
						if (checkPack != null)
						{
							//Check underReview
							if (GameBase.currentVersion == checkPack.reviewVersion)
								GameBase.underReview = true;
							else
								GameBase.underReview = false;

							if (GameBase.currentVersion < checkPack.latestVersion)
								GameBase.newVersionAvaiable = true;
							GameBase.downloadURL = checkPack.link;
						}
					}
					GameBase.versionConfigDataV2 = tempData;
				}
				if (OnGetVersionConfigureDone != null)
					OnGetVersionConfigureDone(WarpResponseResultCode.SUCCESS);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
			if (OnGetVersionConfigureDone != null)
				OnGetVersionConfigureDone(WarpResponseResultCode.BAD_REQUEST);
		}
	}

    public IEnumerator ForgotPassword(string username, int mobile)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("mobile", mobile);

        var headers = form.headers;
        headers["Content-Type"] = "application/x-www-form-urlencoded";
        headers["token"] = GameBase.apiToken;
        var www = new WWW(url_forgot_password, form.data, headers);
        yield return www;

        try
        {
            var data = JsonUtility.FromJson<InitializeData>(www.text);
            if (data != null)
            {
                OnForgotPasswordDone((WarpResponseResultCode)data.status, data.message);
            }
            else
            {
                OnForgotPasswordDone(WarpResponseResultCode.BAD_REQUEST, "");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            OnForgotPasswordDone(WarpResponseResultCode.BAD_REQUEST, "");
        }
    }

    public IEnumerator Register(string _username, string _password, string _mobile = "")
    {
        var form = new WWWForm();
        form.AddField("username", _username);
        form.AddField("password", _password);
        form.AddField("mobile", _mobile);

        form.AddField("provider_code", GameBase.providerCode);
        form.AddField("client_version", GameBase.clientVersion);
        form.AddField("platform", GameBase.platform);
        form.AddField("model", GameBase.model);
        form.AddField("device_uuid", GameBase.device_uuid);
        form.AddField("refCode", GameBase.refCode);

        var headers = form.headers;
        headers["Content-Type"] = "application/x-www-form-urlencoded";
        headers["token"] = GameBase.apiToken;
        var www = new WWW(url_register, form.data, headers);
        yield return www;

        try
        {
            var data = JsonUtility.FromJson<InitializeData>(www.text);
            if (data != null)
            {
                OnRegisterDone((WarpResponseResultCode)data.status, new UserData { username = _username, password = _password, mobile = _mobile });
            } 
            else
            {
                OnRegisterDone(WarpResponseResultCode.BAD_REQUEST, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            OnRegisterDone(WarpResponseResultCode.BAD_REQUEST, null);
        }
    }

    public IEnumerator GetDealerConfig()
    {
        var headers = new Dictionary<string, string>();
        headers["token"] = GameBase.apiToken;
        var www = new WWW(url_dealer_cfg, null, headers);
        yield return www;

        try
        {
            //var temp = JsonUtility.FromJson<RootAgencyData>(www.text.ToString());
            if (string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
            {
                var listData = new List<UserData>();
                var root = new JSONObject(www.text);
                if (root["status"].i == 0)
                {
                    var data = new JSONObject(root["data"].ToString());
                    var locked = new JSONObject(data["locked"].ToString());
                    if (!locked.b)
                    {
                        var agencyList = new JSONObject(data["agency"].ToString());
                        for (int i = 0; i < agencyList.Count; i++)
                        {
                            try
                            {
                                var agency = new JSONObject(agencyList[i].ToString());
                                if (agency != null)
                                {
                                    string add = agency["add"].str;
                                    string name = agency["name"].str;
                                    string tel = agency["tel"].str;
                                    string fb = agency["fb"].str;
                                    string userId = agency["userId"].str;
                                    listData.Add(new UserData { displayName = name, mobile = tel, passport = add, faceBookId = fb, id = int.Parse(userId) });
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("Wc_OnGetSMSConfig Passer: " + ex.Message);
                            }
                        }
                    }

                    if (listData.Any())
                        OnGetDealerConfigDone(WarpResponseResultCode.SUCCESS, listData);
                    else
                        OnGetDealerConfigDone(WarpResponseResultCode.BAD_REQUEST, null);
                }
                else
                    OnGetDealerConfigDone(WarpResponseResultCode.BAD_REQUEST, null);
            }
            else
            {
                OnGetDealerConfigDone(WarpResponseResultCode.BAD_REQUEST, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            OnGetDealerConfigDone(WarpResponseResultCode.BAD_REQUEST, null);
        }
    }

	public IEnumerator CheckLocation()
	{	
		string url = "http://ipinfo.io/geo";
		WWW www = new WWW(url);
		yield return www;

		try
		{
			var data = JsonUtility.FromJson<LocationData>(www.text);
			if (data != null)
			{
				if (data.country == "VN")
					GameBase.underReview = false;
                else
                    GameBase.underReview = true;
                Debug.Log("Under REV: " + GameBase.underReview);
				OnCheckLocationDone(WarpResponseResultCode.SUCCESS, !GameBase.underReview);
			}
			else
			{
				OnCheckLocationDone(WarpResponseResultCode.BAD_REQUEST, false);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
			OnCheckLocationDone(WarpResponseResultCode.BAD_REQUEST, false);
		}
			
	}
    public void HandleGetVersionConfig(string _data)
    {
        if (String.IsNullOrEmpty(_data))
            return;
        Debug.Log(_data);
        var root = new JSONObject(_data);
        var data = new JSONObject(root["data"].ToString());
        if (data != null)
        {
            JSONObject under_review_versions = new JSONObject();
            JSONObject need_update_versions = new JSONObject();
            JSONObject download_urls = new JSONObject();
            JSONObject packageData = data["provider_by_package"];
            if (packageData != null)
            {
                int numOfPack = 0;
                // check number of urls
                while (true)
                {
                    if (packageData[numOfPack] != null)
                    {
                        if (Application.identifier == packageData.keys[numOfPack])
                        {
#if UNITY_ANDROID || UNITY_IOS
                            GameBase.providerCode = packageData[numOfPack].str;
#endif
                        }
                        numOfPack = numOfPack + 1;
                    }
                    else
                        break;
                }

            }

            if (data["hotline"] != null && data["hotline"][0] != null)
                GameBase.hotline = data["hotline"][0].str;

            download_urls = data["download_urls"];
            if (download_urls != null)
            {
                int numOfUrls = 0;
                // check number of urls
                while (true)
                {
                    if (download_urls[numOfUrls] != null)
                        numOfUrls = numOfUrls + 1;
                    else
                        break;
                }

                for (int i = 0; i < numOfUrls; i++)
                {
                    if (GameBase.providerCode == download_urls[i]["provider_code"].str)
                    {
                        GameBase.downloadURL = download_urls[i]["link"].str;
                        GameBase.latest_version = download_urls[i]["latest_version"].str;
                        GameBase.fbFanpage = download_urls[i]["fanpage"].str;
                        under_review_versions = download_urls[i]["under_review_versions"];
                        need_update_versions = download_urls[i]["need_update_versions"];
                    }
                }
            }

#if UNITY_ANDROID || UNITY_IOS
            Debug.Log("under_review_versions:" + under_review_versions.ToString());
            if (under_review_versions != null && under_review_versions.Count > 0)
            {
                 Debug.Log("under_review_versions:" + under_review_versions);
                // check number of urls
                if (under_review_versions.ToString().Contains(GameBase.clientVersion))
                {
                    GameBase.underReview = true;
                }
                Debug.Log("under_review_versions  2:" + GameBase.underReview);
            }
            Debug.Log("need_update_versions:" + need_update_versions.ToString());
            if (need_update_versions != null && need_update_versions.Count > 0)
            {
                if (need_update_versions.ToString().Contains(GameBase.clientVersion))
                {
                    GameBase.needUpdateToPlay = true;
                }
                Debug.Log("under_review_versions  2:" + GameBase.needUpdateToPlay);
            }

            // Request download when lastest ver higher than current ver
            int clientVer = int.Parse(GameBase.clientVersion.Replace(".", ""));
            int lastestVer = int.Parse(GameBase.latest_version.Replace(".", ""));
            if (lastestVer > clientVer)
                GameBase.newVersionAvaiable = true;

#endif
        }
        PlayerPrefs.SetString("versionConfig", _data);
        PlayerPrefs.Save();
    }
}
