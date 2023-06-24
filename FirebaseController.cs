using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseController : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference databaseReference;

    [SerializeField] string child="data0";
    [SerializeField] Data data;
    [SerializeField] Data loadedData;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += (object sender, System.EventArgs eventArgs) =>
        {
            print("Auth State Changed.");
            if(auth.CurrentUser == null || auth.CurrentUser != user)
            {
                user = auth.CurrentUser;

                if (user == null) GetUser();
                else HandleDatabase();
            }
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string data = JsonUtility.ToJson(this.data);

            print($"Create data at child : {data}");

            CreateData(child, data);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            string data = JsonUtility.ToJson(this.data);

            print($"Create data at root : {data}");

            CreateData(data);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("Clear data at child");

            ClearData(child);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            print("Clear all data.");

            ClearData();
        }
    }

    void GetUser()
    {
        print("Logging in Anonymously.");
        auth.SignInAnonymouslyAsync();
    }

    void HandleDatabase()
    {
        print("Handling Database");

        databaseReference = FirebaseDatabase.GetInstance("https://realtime-db-demo-df199-default-rtdb.firebaseio.com/").RootReference;

        databaseReference.ValueChanged += (object sender, ValueChangedEventArgs eventArgs) =>
        {
            string data = eventArgs.Snapshot.GetRawJsonValue();

            print($"Database value has changed : {data}");
            
            try
            {
                loadedData = JsonUtility.FromJson<Data>(data);
            }
            catch (System.Exception)
            {
                //do nothing
            }
        };
    }

    void CreateData(string child, string jsonData)
    {
        databaseReference.Child(child).SetRawJsonValueAsync(jsonData);
    }

    void CreateData(string jsonData)
    {
        databaseReference.SetRawJsonValueAsync(jsonData);
    }

    void ClearData(string child)
    {
        databaseReference.Child(child).RemoveValueAsync();
    }

    void ClearData()
    {
        databaseReference.RemoveValueAsync();
    }
}

[System.Serializable]
class Data
{
    public int index;
    public string employeeName;
    public EmergencyContact[] emergencyContacts;

    public Data()
    {
    }

    public Data(int index, string employeeName, EmergencyContact[] emergencyContacts)
    {
        this.index = index;
        this.employeeName = employeeName;
        this.emergencyContacts = emergencyContacts;
    }
}

[System.Serializable]
class EmergencyContact
{
    public string contactName;
    public string contactPhoneNumber;

    public EmergencyContact()
    {
    }

    public EmergencyContact(string contactName, string contactPhoneNumber)
    {
        this.contactName = contactName;
        this.contactPhoneNumber = contactPhoneNumber;
    }
}
