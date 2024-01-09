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
    [SerializeField]
    public TMP_Dropdown tmpDropdown;

    public firebase fb;

    public IEnumerable<DataSnapshot> alldata;

    List<DataSnapshot> childlastdata = new List<DataSnapshot>();

    List<string> shotdata = new List<string>();
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
                Debug.Log( entryKey + " " + entryValue);
            }
        }
    }
}
}
