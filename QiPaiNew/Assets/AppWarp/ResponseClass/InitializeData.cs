using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class InitializeData
{
    public int status;
    public string message;
    public string data;
}

public class RootVersionConfig
{
    public int status;
    public string message;
    public VersionConfigData data;
}

public class VersionConfigData
{
    public string website;
    public string fb_fanpage;
    public string email_support;
    public List<string> hotline;
    public List<Version> unsupportVersions;
    public List<DownloadUrl> download_urls;
    public Dictionary<string, string> provider_by_package;
}

public class DownloadUrl
{
    public string latest_version;
    public string link;
    public string provider_code;
    public List<Version> under_review_versions;
}

public class RootVersionConfig_v2
{
    public int status;
    public string message;
    public ConfigData data;
}

public class ConfigData
{
    public string web;
    public string fb;
    public List<string> hotline;
    public List<Provider> providers;
    public List<Version> unsupportVersion;
}

public class Provider
{
    public string providerCode;
    public string fb;
    public List<Package> packages;
	public string socketIp;
	public string socketPort;
}


public class Package
{
    public string appId;
    public Version reviewVersion;
    public Version latestVersion;
    public string link;
}

public class LocationData
{
	public string ip;
	public string city;
	public string region;
	public string country;
	public string loc;
	public string postal;
}