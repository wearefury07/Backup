using UnityEngine;
using System.Collections.Generic;

public class TimeOutHelper
{
    Dictionary<string, Coroutine> handleDict;

    public TimeOutHelper()
    {
        handleDict = new Dictionary<string, Coroutine>();
    }

    public bool Start(string id, Coroutine handle)
    {
        if (handleDict.ContainsKey(id))
            return false;
        else
        {
            handleDict.Add(id, handle);
            return true;
        }
    }

    public void Remove(string id)
    {
        if (handleDict.ContainsKey(id))
            handleDict.Remove(id);
    }
    public void Clear()
    {
        handleDict.Clear();
    }

    public bool IsWorking(string id)
    {
        return handleDict.ContainsKey(id);
    }

    public bool TryGetValue(string id, out Coroutine handle)
    {
        return handleDict.TryGetValue(id, out handle);
    }
}
