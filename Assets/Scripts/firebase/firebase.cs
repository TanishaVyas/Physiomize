using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using UnityEngine.SceneManagement;
public class firebase : MonoBehaviour
{
 private DatabaseReference reference;
    
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }
    
public void AddDataEntry(string name, List<Dictionary<string, object>> yourList, string age, string gender)
{
    var time = DateTime.Now;
    string timestamp = time.ToString("M-d-yyyy h:mm:ss tt");

    string userId = name;
    DatabaseReference userReference = reference.Child(userId);
    DatabaseReference entryReference = userReference.Push(); // Use Push to generate a unique key

    entryReference.Child("timestamp").SetValueAsync(timestamp);
    entryReference.Child("age").SetValueAsync(age);
    entryReference.Child("gender").SetValueAsync(gender);
    entryReference.Child("scene").SetValueAsync(SceneManager.GetActiveScene().name);

    // Add yourList as child nodes under the "data" node
    DatabaseReference dataReference = entryReference.Child("data");

    foreach (var entry in yourList)
    {
        DatabaseReference entryNode = dataReference.Push(); // Use Push to generate a unique key for each entry

        foreach (var keyValuePair in entry)
        {
            entryNode.Child(keyValuePair.Key).SetValueAsync(keyValuePair.Value.ToString());
        }
    }
}

    
 public void ReadName(string name,Action<IEnumerable<DataSnapshot>> onComplete)
    {
        reference.Child(name).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IEnumerable<DataSnapshot> childrenList = snapshot.Children;
                onComplete?.Invoke(childrenList);
                // Iterate through the children of the snapshot
                /*foreach (var childSnapshot in snapshot.Children)
                {   
                    string entryId = childSnapshot.Key;
                    string entryData = childSnapshot.Value.ToString();

                    Debug.Log(entryId);
                foreach (var grandChildSnapshot in childSnapshot.Children)
                {
                    string dataEntryId = grandChildSnapshot.Key;
                    string dataEntry = grandChildSnapshot.Value.ToString();
                    Debug.Log(dataEntry);
                }
                Debug.Log("*************************************************************");
            }
            */
            }
        });
    }

}