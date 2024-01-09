using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class readdata : MonoBehaviour
{   
    private int currentIndex = 0;
    [SerializeField]
    public TMP_Dropdown tmpDropdown;

    public firebase fb;

    public IEnumerable<DataSnapshot> alldata;

    List<DataSnapshot> childlastdata = new List<DataSnapshot>();

    public List<Dictionary<string, object>> shotdata = new List<Dictionary<string, object>>();
    [SerializeField]
    public GameObject spot;
    public GameObject legpain;
    [SerializeField]
    string name = "";
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            fb.ReadName(name, OnReadNameComplete);
        }
    }

private void OnReadNameComplete(IEnumerable<DataSnapshot> childrenList)
{
    tmpDropdown.ClearOptions();
    alldata = childrenList;
    if (alldata != null)
    {
        List<string> options = new List<string>();
        childlastdata.Clear();
        tmpDropdown.options.Clear();
        foreach (var childSnapshot in alldata)
        {
            string entryId = childSnapshot.Key;
            string timestamp = childSnapshot.Child("timestamp").Value.ToString();
            string entryData = $"ID: {entryId}, Timestamp: {timestamp}";

            tmpDropdown.options.Add(new TMP_Dropdown.OptionData(timestamp));
            childlastdata.Add(childSnapshot);
        }
        OnDropdownValueChanged(0);
        tmpDropdown.captionText.text = tmpDropdown.options[0].text;
        tmpDropdown.value = 0; // optional
        tmpDropdown.Select(); // optional
        tmpDropdown.RefreshShownValue();
    }
}

   public void OnDropdownValueChanged(int index)
{
    if (index >= 0 && index < childlastdata.Count)
    {   
        shotdata.Clear();
        Debug.Log("Selected index: " + index);
        currentIndex = 0;
        // Access the corresponding DataSnapshot from the child list
        DataSnapshot selectedSnapshot = childlastdata[index];

        // Access the "data" child
        DataSnapshot dataSnapshot = selectedSnapshot.Child("data");

        foreach (var typeSnapshot in dataSnapshot.Children)
        {
            string dataType = typeSnapshot.Key;
            foreach (var entrySnapshot in typeSnapshot.Children)
            {
                string entryKey = entrySnapshot.Key;
                var entryValue = entrySnapshot.Value;
                 Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                {
                    { entryKey, entryValue }
                };
               shotdata.Add(keyValuePairs);
            //Debug.Log( entryKey + " " + entryValue );
            }
        }
        DisplayCurrentIndex();
    }
}

public void ReduceIndex()
{
    if (shotdata.Count > 0)
    {
        currentIndex = (currentIndex - 1 + shotdata.Count) % shotdata.Count;
        DisplayCurrentIndex();
    }
}

public void IncreaseIndex()
{
    if (shotdata.Count > 0)
    {
        currentIndex = (currentIndex + 1) % shotdata.Count;
        DisplayCurrentIndex();
    }
}

private void DisplayCurrentIndex()
{
    Debug.Log($"Current Index: {currentIndex}");

    if (shotdata.Count > 0)
    {
        Dictionary<string, object> currentEntry = shotdata[currentIndex];

        if (currentEntry.ContainsKey("maxmin"))
        {
            Debug.Log("Type: MaxMin");
            // Perform actions specific to MaxMin type
        }
        else if (currentEntry.ContainsKey("position"))
        {
            Debug.Log("Type: Position");
            string positionString = currentEntry["position"].ToString();
            Vector3 position = ParseVector3(positionString);
            var spotf = Instantiate(spot, position, Quaternion.identity);
            spotf.transform.parent = legpain.transform;
            spotf.transform.localPosition = position;
            spotf.transform.localScale=3*Vector3.one;
        }
        else if (currentEntry.ContainsKey("acute_pain"))
        {
            Debug.Log("Type: Acute Pain");
            // Perform actions specific to Acute Pain type
        }
        else if (currentEntry.ContainsKey("ranged_pain"))
        {
            Debug.Log("Type: Ranged Pain");
            // Perform actions specific to Ranged Pain type
        }
    }
    else
    {
        Debug.Log("No data available.");
    }
}
private Vector3 ParseVector3(string vectorString)
{
    string[] components = vectorString.Trim('(', ')').Split(',');

    if (components.Length == 3)
    {
        float x = float.Parse(components[0]);
        float y = float.Parse(components[1]);
        float z = float.Parse(components[2]);

        return new Vector3(x, y, z);
    }

    return Vector3.zero; // Return a default value if parsing fails
}
}
