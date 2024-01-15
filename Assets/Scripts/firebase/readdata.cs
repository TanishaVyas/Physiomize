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
    public holdrotate hr;

    private int currentIndex = 0;

    [SerializeField]
    public Dropdown tmpDropdown;//TMP_Dropdown

    [SerializeField]
    TMP_InputField Inputname;

    public firebase fb;

    public IEnumerable<DataSnapshot> alldata;

    List<DataSnapshot> childlastdata = new List<DataSnapshot>();

    public List<Dictionary<string, object>>
        shotdata = new List<Dictionary<string, object>>();

    [SerializeField]
    public GameObject spot;

    GameObject legpain;

    GameObject Painpoint;

    [SerializeField]
    string name = "";

    [SerializeField]
    GameObject Knee1;

    [SerializeField]
    GameObject Elbow;

    string scene;

    // Update is called once per frame
    public void callReadName()
    {
        fb.ReadName(Inputname.text, OnReadNameComplete);
    }

    private void OnReadNameComplete(IEnumerable<DataSnapshot> childrenList)
    {
        tmpDropdown.ClearOptions();
        alldata = childrenList;
        string first= "";
        if (alldata != null)
        {
            List<string> options = new List<string>();
            childlastdata.Clear();
            foreach (var childSnapshot in alldata)
            {
                string entryId = childSnapshot.Key;
                string timestamp =
                    childSnapshot.Child("timestamp").Value.ToString();
                if(first=="")
                {
                    first=timestamp;
                }
                string entryData = $"ID: {entryId}, Timestamp: {timestamp}";
                var op = new Dropdown.OptionData(timestamp);
                tmpDropdown.options.Add(op);
                childlastdata.Add (childSnapshot);
            }
            Debug.Log("exe");
            tmpDropdown.captionText.text = first;
            OnDropdownValueChanged();
            tmpDropdown.RefreshShownValue();
        }
    }

    public void OnDropdownValueChanged()
    {   
        int index= tmpDropdown.value;
        if (index >= 0 && index < childlastdata.Count)
        {
            shotdata.Clear();
            Debug.Log("Selected index: " + index);
            currentIndex = 0;

            // Access the corresponding DataSnapshot from the child list
            DataSnapshot selectedSnapshot = childlastdata[index];

            // Access the "data" child
            DataSnapshot dataSnapshot = selectedSnapshot.Child("data");
            scene = selectedSnapshot.Child("scene").Value.ToString();
            foreach (var typeSnapshot in dataSnapshot.Children)
            {
                string dataType = typeSnapshot.Key;
                foreach (var entrySnapshot in typeSnapshot.Children)
                {
                    string entryKey = entrySnapshot.Key;
                    var entryValue = entrySnapshot.Value;
                    Dictionary<string, object> keyValuePairs =
                        new Dictionary<string, object> {
                            { entryKey, entryValue }
                        };
                    shotdata.Add (keyValuePairs);
                    //Debug.Log( entryKey + " " + entryValue );
                }
            }
            Knee1.SetActive(false);
            Elbow.SetActive(false);

            switch (scene)
            {
                case "knee 1":
                    Knee1.SetActive(true);
                    legpain = FindChildWithTag(Knee1, "painloc");
                    hr.hd = Knee1.GetComponent<Rotation>();
                    break;
                case "elbow":
                    Elbow.SetActive(true);
                    legpain = FindChildWithTag(Elbow, "painloc");
                    hr.hd = Elbow.GetComponent<Rotation>();
                    break;
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

            if (Painpoint != null)
            {
                Destroy (Painpoint);
            }
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
                spotf.transform.localScale = 4 * Vector3.one;
                Painpoint = spotf;
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

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (
                child != null &&
                child.gameObject != null &&
                child.gameObject.tag == tag
            )
            {
                return child.gameObject;
            }
        }

        // Child with the specified tag not found
        return null;
    }
}
