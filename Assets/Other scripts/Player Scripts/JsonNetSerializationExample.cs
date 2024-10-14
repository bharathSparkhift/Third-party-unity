using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JsonNetSerializationExample : MonoBehaviour
{
    void Start()
    {
        
    }

    [ContextMenu("Fetch Data")]
    public void FetchData()
    {
        // Create sample data
        List<Player> playerList = new List<Player>
        {
            new Player { name = "Alice", level = 5, health = 75.5f },
            new Player { name = "Bob", level = 3, health = 60.0f },
            new Player { name = "Charlie", level = 8, health = 90.2f }
        };

        // Define the file path
        string filePath = Path.Combine(Application.persistentDataPath, "Player.json");
        Debug.Log($"File Path: {filePath}");

        // Serialize the list to JSON
        string json = JsonConvert.SerializeObject(playerList, Formatting.Indented);
        Debug.Log("Serialized JSON:\n" + json);

        // Save JSON to file
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"JSON successfully saved to {filePath}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to write to file: {ex.Message}");
            return;
        }

        // Read JSON from file
        string jsonFromFile;
        try
        {
            jsonFromFile = File.ReadAllText(filePath);
            Debug.Log("JSON successfully read from file.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to read from file: {ex.Message}");
            return;
        }

        // Deserialize JSON back to list of Player objects
        List<Player> deserializedList;
        try
        {
            deserializedList = JsonConvert.DeserializeObject<List<Player>>(jsonFromFile);
            if (deserializedList == null)
            {
                Debug.LogError("Deserialization returned null.");
                return;
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON Deserialization error: {ex.Message}");
            return;
        }

        // Output deserialized data
        Debug.Log("Deserialized Players:");
        foreach (Player player in deserializedList)
        {
            Debug.Log($"Name: {player.name}, Level: {player.level}, Health: {player.health}");
        }
        
    }
    
}

