using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LeaderboardController : MonoBehaviour
{
    private string[] usernames;

    private readonly string[] events = new string[] { "100m", "200m", "400m", "Hurdles", "Rowing", "Javelin", "Hammer", "Shot Put", "Long Jump", "Triple Jump", "High Jump", "Pole Vault",
        "100m Freestyle", "Weightlifting", "Archery", "Skeet", "Speed Climbing", "Ski Jump" };
    private Dictionary<string, bool> eventRecordLowerIsBetter = new Dictionary<string, bool>();
    private Dictionary<string, string[]> courses = new Dictionary<string, string[]>();
    private Dictionary<string, int> resolution = new Dictionary<string, int>();

    [Header("References")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject leaderboardTilePrefab;
    [SerializeField]
    private TMP_Dropdown eventDropdown;
    [SerializeField]
    private TMP_Dropdown courseDropdown;

    private string course = "";

    private const float verticalSpacing = 2.5f;
    private const float startingY = 8f;
    private float y;

    private GameObject tile;
    private string key;
    private string value;

    private List<GameObject> leaderboardTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        y = startingY;

        eventRecordLowerIsBetter.Add("100m", true);
        eventRecordLowerIsBetter.Add("200m", true);
        eventRecordLowerIsBetter.Add("400m", true);
        eventRecordLowerIsBetter.Add("Hurdles", true);
        eventRecordLowerIsBetter.Add("Rowing", true);
        eventRecordLowerIsBetter.Add("Javelin", false);
        eventRecordLowerIsBetter.Add("Long Jump", false);
        eventRecordLowerIsBetter.Add("Triple Jump", false);
        eventRecordLowerIsBetter.Add("High Jump", false);
        eventRecordLowerIsBetter.Add("Pole Vault", false);
        eventRecordLowerIsBetter.Add("Weightlifting", false);
        eventRecordLowerIsBetter.Add("Archery", false);
        eventRecordLowerIsBetter.Add("Skeet", false);
        eventRecordLowerIsBetter.Add("Speed Climbing", true);
        eventRecordLowerIsBetter.Add("Ski Jump", false);
        eventRecordLowerIsBetter.Add("Hammer", false);
        eventRecordLowerIsBetter.Add("Shot Put", false);
        eventRecordLowerIsBetter.Add("100m Freestyle", true); eventRecordLowerIsBetter.Add("Swimming Freestyle", true);

        courses.Add("Speed Climbing", new string[] { "Test", "Everest", "Tower" });

        resolution.Add("100m", 3);
        resolution.Add("200m", 3);
        resolution.Add("400m", 3);
        resolution.Add("Hurdles", 3);
        resolution.Add("Rowing", 3);
        resolution.Add("Javelin", 3);
        resolution.Add("Long Jump", 3);
        resolution.Add("Triple Jump", 3);
        resolution.Add("High Jump", 0);
        resolution.Add("Pole Vault", 0);
        resolution.Add("Weightlifting", 0);
        resolution.Add("Archery", 0);
        resolution.Add("Skeet", 0);
        resolution.Add("Speed Climbing", 3);
        resolution.Add("Ski Jump", 3);
        resolution.Add("Hammer", 3);
        resolution.Add("Shot Put", 3);
        resolution.Add("100m Freestyle", 3); resolution.Add("Swimming Freestyle", 3);

        /// Set up event dropdown ///
        eventDropdown.options.Add(new TMP_Dropdown.OptionData("World Records"));
        foreach (string i in events)
        {
            eventDropdown.options.Add(new TMP_Dropdown.OptionData(i));
        }
        eventDropdown.captionText.text = "World Records";
        eventDropdown.value = 0;

        //courseDropdown.captionText.text = "Course";

        //usernames = PlayerPrefsX.GetStringArray("Usernames", "", 0);
        usernames = new string[] { "Blue", "Purple", "Green", "Yellow", "William", "Joe" };
        //Debug.Log("Usernames: " + usernames.ToString());
        //Debug.Log("Usernames: " + usernames.Length.ToString());
        //Debug.Log("Usernames: " + usernames[0]);

        //if (usernames == new string[] { })
        if (usernames.Length == 0)
        {
            Debug.Log("Setting usernames PlayerPrefsX.");
            usernames = new string[] { "Blue", "Purple", "Green", "Yellow", "William", "Joe" };
            PlayerPrefsX.SetStringArray("Usernames", usernames);
        }
        else
        {
            //Debug.Log("Saved usernames are: " + usernames.ToString());
            //Debug.Log("Saved usernames are:");
            foreach (string i in usernames)
            {
                //Debug.Log(i);
            }

            DisplayLeaderboard();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DisplayLeaderboard()
    {
        DisplayLeaderboard(true);
    }
    public void DisplayLeaderboard(bool redisplayCourseDropdown)
    {
        ClearLeaderboardTiles();

        string _event;
        if (eventDropdown.value == 0)
        {
            _event = "World Records";
            for (int i = 0; i < events.Length; i++)
            {
                if (courses.ContainsKey(events[i]))
                {
                    for (int c = 0; c < courses[events[i]].Length; c++)
                    {
                        key = events[i] + " Course " + courses[events[i]][c] + " Record";

                        string displayKey;
                        if (c == 0)
                        {
                            displayKey = events[i] + " WR";
                        }
                        else
                        {
                            displayKey = "";
                        }

                        AddLeaderboardTile(events[i], key, displayKey, Records.GetRecordOwners(events[i], courses[events[i]][c]), courses[events[i]][c]);
                    }
                }
                else
                {
                    if (events[i] == "100m Freestyle")
                    {
                        key = "Swimming Freestyle Record";
                    }
                    else
                    {
                        key = events[i] + " Record";
                    }

                    AddLeaderboardTile(events[i], key, events[i] + " WR", Records.GetRecordOwners(events[i]));
                }
            }

            if (redisplayCourseDropdown)
            {
                courseDropdown.options = new List<TMP_Dropdown.OptionData>();
                courseDropdown.options.Add(new TMP_Dropdown.OptionData("N/A"));
                courseDropdown.value = 0;
                courseDropdown.captionText.text = "N/A";
            }
        }
        else
        {
            _event = events[eventDropdown.value - 1];
            if (_event == "100m Freestyle")
            {
                _event = "Swimming Freestyle";
            }

            if (courses.ContainsKey(_event))
            {
                if (courseDropdown.value == 0)
                {
                    course = "All";
                }
                else
                {
                    course = courses[_event][courseDropdown.value - 1];
                }
            }
            else
            {
                course = "";
            }

            //key = events[i] + " Record";
            //AddLeaderboardTile(key, events[i] + " WR");

            if (course == "All")
            {
                for (int c = 0; c < courses[_event].Length; c++)
                {
                    key = _event + " Course " + courses[_event][c] + " Record";

                    //AddLeaderboardTile(key, _event + " WR", GetRecordOwners(_event, courses[_event][c]), courses[_event][c]);
                    AddLeaderboardTile(_event, key, "WR", Records.GetRecordOwners(_event, courses[_event][c]), courses[_event][c]);

                    key = _event + " Course " + courses[_event][c] + " PB ";

                    /// Sort usernames based on record, so records appear in order ///
                    string[] sortableUsernames = usernames;
                    float[] records = new float[sortableUsernames.Length];
                    for (int i = 0; i < sortableUsernames.Length; i++)
                    {
                        if (PlayerPrefs.HasKey(key + sortableUsernames[i]))
                        {
                            records[i] = PlayerPrefs.GetFloat(key + sortableUsernames[i]);
                        }
                        else
                        {
                            if (eventRecordLowerIsBetter[_event])
                            {
                                records[i] = float.PositiveInfinity;
                            }
                            else
                            {
                                records[i] = float.NegativeInfinity;
                            }
                        }
                    }

                    Array.Sort(records, sortableUsernames);

                    if (!eventRecordLowerIsBetter[_event])
                    {
                        Array.Reverse(sortableUsernames);
                    }

                    for (int j = 0; j < sortableUsernames.Length; j++)
                    {
                        //AddLeaderboardTile(key + sortableUsernames[j], _event + " PB " + sortableUsernames[j], courses[_event][c]);
                        AddLeaderboardTile(_event, key + sortableUsernames[j], "PB " + sortableUsernames[j], courses[_event][c]);
                    }
                }
            }
            else
            {
                if (course == "")
                {
                    key = _event + " Record";
                }
                else
                {
                    key = _event + " Course " + course + " Record";
                }

                //AddLeaderboardTile(key, _event + " WR", GetRecordOwners(_event, course), course);
                AddLeaderboardTile(_event, key, "WR", Records.GetRecordOwners(_event, course), course);

                /*for (int j = 0; j < usernames.Length; j++)
                {
                    //key = events[i] + " PB " + usernames[j];
                    key = _event + " PB " + usernames[j];
                    AddLeaderboardTile(key, key);
                    //AddLeaderboardTile(key, usernames[j]);
                }*/

                if (course == "")
                {
                    key = _event + " PB ";
                }
                else
                {
                    key = _event + " Course " + course + " PB ";
                }

                /// Sort usernames based on record, so records appear in order ///
                string[] sortableUsernames = usernames;
                float[] records = new float[sortableUsernames.Length];
                for (int i = 0; i < sortableUsernames.Length; i++)
                {
                    if (PlayerPrefs.HasKey(key + sortableUsernames[i]))
                    {
                        records[i] = PlayerPrefs.GetFloat(key + sortableUsernames[i]);
                    }
                    else
                    {
                        if (eventRecordLowerIsBetter[_event])
                        {
                            records[i] = float.PositiveInfinity;
                        }
                        else
                        {
                            records[i] = float.NegativeInfinity;
                        }
                    }
                }

                Array.Sort(records, sortableUsernames);

                if (!eventRecordLowerIsBetter[_event])
                {
                    Array.Reverse(sortableUsernames);
                }

                for (int j = 0; j < sortableUsernames.Length; j++)
                {
                    //key = key + sortableUsernames[j];
                    //AddLeaderboardTile(key + sortableUsernames[j], _event + " PB " + sortableUsernames[j], course);
                    AddLeaderboardTile(_event, key + sortableUsernames[j], "PB " + sortableUsernames[j], course);
                }
            }

            if (redisplayCourseDropdown)
            {
                courseDropdown.options = new List<TMP_Dropdown.OptionData>();
                if (courses.ContainsKey(_event))
                {
                    courseDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
                    for (int i = 0; i < courses[_event].Length; i++)
                    {
                        courseDropdown.options.Add(new TMP_Dropdown.OptionData(courses[_event][i]));
                    }
                    courseDropdown.value = 0;
                    courseDropdown.captionText.text = courseDropdown.options[0].text;
                }
                else
                {
                    courseDropdown.options.Add(new TMP_Dropdown.OptionData("N/A"));
                    courseDropdown.value = 0;
                    courseDropdown.captionText.text = "N/A";
                }
            }
        }
    }

    public void AddLeaderboardTile(string _event, string recordKey, string displayKey)
    {
        AddLeaderboardTile(_event, recordKey, displayKey, new string[0], "");
    }
    public void AddLeaderboardTile(string _event, string recordKey, string displayKey, string course)
    {
        AddLeaderboardTile(_event, recordKey, displayKey, new string[0], course);
    }
    public void AddLeaderboardTile(string _event, string recordKey, string displayKey, string[] owners)
    {
        AddLeaderboardTile(_event, recordKey, displayKey, owners, "");
    }
    public void AddLeaderboardTile(string _event, string recordKey, string displayKey, string[] owners, string course)
    {
        tile = Instantiate(leaderboardTilePrefab, canvas.transform);
        tile.transform.localPosition = new Vector3(0f, y, 0f);
        //tile = Instantiate(leaderboardTilePrefab);
        //tile.transform.parent = canvas.transform;
        //tile.transform.position =

        if (PlayerPrefs.HasKey(recordKey))
        {
            value = PlayerPrefs.GetFloat(recordKey).ToString("n" + resolution[_event].ToString());
        }
        else
        {
            value = "N/A";
        }

        //tile.GetComponent<LeaderboardTileController>().SetText(recordKey, value);
        tile.GetComponent<LeaderboardTileController>().SetText(displayKey, course, value, owners);

        y -= verticalSpacing;

        leaderboardTiles.Add(tile);
    }

    public void ClearLeaderboardTiles()
    {
        y = startingY;

        while (leaderboardTiles.Count > 0)
        {
            Destroy(leaderboardTiles[0]);
            leaderboardTiles.RemoveAt(0);
        }
    }

    public void SetCourse()
    {
        DisplayLeaderboard(false);
    }
}
