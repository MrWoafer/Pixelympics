using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Records
{
    public static string[] usernames = new string[] { "Blue", "Green", "Purple", "Yellow", "William", "Joe" };

    public static string[] GetRecordOwners(string _event)
    {
        return GetRecordOwners(_event, "");
    }
    public static string[] GetRecordOwners(string _event, string course)
    {
        if (_event == "100m Freestyle")
        {
            _event = "Swimming Freestyle";
        }

        List<string> owners = new List<string>();

        foreach (string user in usernames)
        {
            if (course == "")
            {
                if (PlayerPrefs.HasKey(_event + " Record") && PlayerPrefs.GetFloat(_event + " Record").ToString("n5") == PlayerPrefs.GetFloat(_event + " PB " + user).ToString("n5"))
                {
                    owners.Add(user);
                }
            }
            else
            {
                if (PlayerPrefs.HasKey(_event + " Course " + course + " Record") && PlayerPrefs.GetFloat(_event + " Course " + course + " Record").ToString("n5") == PlayerPrefs.GetFloat(_event + " Course " + course + " PB " + user).ToString("n5"))
                {
                    owners.Add(user);
                }
            }
        }

        return owners.ToArray();
    }
}
