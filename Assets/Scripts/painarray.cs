using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class painarray : MonoBehaviour
{   
    int i=0;
    bool up=false;
    public List<string> data = new List<string>();
    public List<Dictionary<string, object>> data1 = new List<Dictionary<string, object>>();
    [SerializeField]
    TMP_Text Text;
    [SerializeField]
    TMP_InputField register_username;
    [SerializeField]
    TMP_InputField age;
    [SerializeField]
    Dropdown gender;
    public firebase fb;
    public movesoham mv;
    public void singledata(float init)
    {   
        i=0;
        string s = "acute pain at "+init+"°";
        Dictionary<string, object> entry = new Dictionary<string, object>
        {
            { "acute_pain", init }
        };
        data1.Add(entry);
        data.Add(s);
        up=true;
    }

    public void rangedata(float init,float last)
    {
        i=0;
        Dictionary<string, object> entry = new Dictionary<string, object>
        {
            { "ranged_pain", $"({init}, {last})" }
        };
        data1.Add(entry);
        string s = "ranged pain at "+init+"° to "+last+"°";
        data.Add(s);
        up=true;
    }

    void Update()
    {
       if (up)
       {
        up=false;
        string s="";
        if(data.Count>5)
        {
            for (int i=data.Count-1;i>=data.Count-5;i--)
            {
                s=s+data[i]+"\n";
            }
        }
        else{
            for (int i=data.Count-1;i>=0;i--)
            {
                s=s+data[i]+"\n";
            }
        }
            Text.text=s;
       }
    }

    public void senddata()
    {   GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("Finish");
        foreach (GameObject gameObject in gameObjects)
        {
            Dictionary<string, object> entry = new Dictionary<string, object>
            {
                { "position", gameObject.transform.position }
            };
            data1.Add(entry);
            Destroy(gameObject);
        }
         Dictionary<string, object> minMaxEntry = new Dictionary<string, object>
        {
            { "maxmin", $"({mv.max}, {mv.min})" },
        };
        data1.Add(minMaxEntry);
        fb.AddDataEntry(register_username.text,data1,age.text,gender.options[gender.value].text);
        data.Clear();
        Text.text="";
    }
}


