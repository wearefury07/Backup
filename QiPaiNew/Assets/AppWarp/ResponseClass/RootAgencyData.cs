using System.Collections.Generic;

public class RootAgencyData
{
    public int status { get; set; }
    public string message { get; set; }
    public AgencyData data { get; set; }
}

public class Agency
{
    public int numb { get; set; }
    public string add { get; set; }
    public string name { get; set; }
    public string tel { get; set; }
    public string fb { get; set; }
    public string userId { get; set; }
}

public class AgencyData
{
    public List<Agency> agency { get; set; }
    public bool locked { get; set; }
}
