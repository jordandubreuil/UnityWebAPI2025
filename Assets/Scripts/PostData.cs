using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PostData : MonoBehaviour
{
    string serverUrl = "http://localhost:3000/sentdatatodb";
    PlayerData player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupPlayerData("Cory", 300002, 99);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupPlayerData(string name, int score, int level)
    {
        player = new PlayerData();

        player.name = name;
        player.score = score;
        player.level = level;

        string json = JsonUtility.ToJson(player);
        Debug.Log(json);
        StartCoroutine(PostPlayerData(json));

    }

    IEnumerator PostPlayerData(string json)
    {
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            //Success
            Debug.Log($"Data Sent: {request.downloadHandler.text}");

            string newPlayerId = ExtractPlayerId(response);
            Debug.Log("New player id:" + newPlayerId);
        }
        else
        {
            //failed
            Debug.LogError($"Error sending data: {request.error}");
        }
    }

    string ExtractPlayerId(string jsonResponse)
    {
        int index = jsonResponse.IndexOf("\"playerid\":\"") + 12;
        if (index < 12) return "";
        int endIndex = jsonResponse.IndexOf("\"", index);
        return jsonResponse.Substring(index, endIndex - index);

    }
}
