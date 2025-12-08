using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
namespace D.Editor
{

#if UNITY_EDITOR

    using UnityEditor;

    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : Editor
    {
        public Dictionary<string, string> prefInfos;
        private bool foldOut;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (prefInfos == null)
            {
                foldOut = true;

                if (GUILayout.Button("Show Prefs (PlayerPrefs)"))
                {
                    string data = PlayerPrefs.GetString(DataManager.DATAPATH, "");

                    if (string.IsNullOrEmpty(data)) return;

                    var userData = JsonConvert.DeserializeObject<PlayerData>(data);

                    if (string.IsNullOrEmpty(userData.prefData)) return;

                    prefInfos = JsonConvert.DeserializeObject<Dictionary<string, string>>(userData.prefData);
                }
            }
            else
            {
                ShowPrefs();

                if (GUILayout.Button("Save Prefs → PlayerPrefs"))
                {
                    string rawData = PlayerPrefs.GetString(DataManager.DATAPATH, "");
                    if (string.IsNullOrEmpty(rawData)) return;

                    var userData = JsonConvert.DeserializeObject<PlayerData>(rawData);

                    string newData = JsonConvert.SerializeObject(prefInfos);
                    userData.prefData = newData;

                    string json = JsonConvert.SerializeObject(userData);
                    PlayerPrefs.SetString(DataManager.DATAPATH, json);
                    PlayerPrefs.Save();

                    Debug.Log("Saved to PlayerPrefs: " + json);

                    if (Application.isPlaying)
                    {
                        DataManager dataManager = target as DataManager;
                        dataManager.data.prefData = newData;
                        dataManager.SendMessage("ReloadPref");
                    }
                }
            }
        }

        private void ShowPrefs()
        {
            int index = 0;
            foldOut = EditorGUILayout.Foldout(foldOut, "List Prefs");
            if (foldOut)
            {
                EditorGUI.indentLevel++;
                var copy = new Dictionary<string, string>(prefInfos);

                foreach (var item in copy)
                {
                    EditorGUILayout.LabelField($"Element {index}");
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(item.Key);

                    string newValue;
                    if (int.TryParse(item.Value, out int intValue))
                        newValue = EditorGUILayout.IntField(intValue).ToString();
                    else
                        newValue = EditorGUILayout.TextField(item.Value);

                    if (newValue != item.Value)
                        prefInfos[item.Key] = newValue;

                    EditorGUILayout.EndHorizontal();

                    index++;
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
        }
    }

#endif

}
public class DataManager : Singleton<DataManager>
{
    public static readonly string DATAPATH = "player_data"; // key PlayerPrefs
    public PlayerData data;
    private Dictionary<string, string> userDicDataPref;

    public void OnAwake()
    {
        LoadData();
    }

    void LoadData()
    {
        string myData = PlayerPrefs.GetString(DATAPATH, "");

        if (!string.IsNullOrWhiteSpace(myData))
        {
            Debug.Log("Load:" + myData);
            data = JsonUtility.FromJson<PlayerData>(myData);

            if (!string.IsNullOrEmpty(data.prefData))
            {
                userDicDataPref = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.prefData);
            }
            else
            {
                userDicDataPref = new Dictionary<string, string>();
            }
        }
        else
        {
            data = new PlayerData();
            userDicDataPref = new Dictionary<string, string>();
        }
    }

    void SaveData()
    {
        data.prefData = JsonConvert.SerializeObject(userDicDataPref);
        string dt = JsonConvert.SerializeObject(data);
        Debug.Log("Save:" + dt);

        PlayerPrefs.SetString(DATAPATH, dt);
        PlayerPrefs.Save();
    }

    private void ReloadPref()
    {
        Debug.Log("Reload");
        if (!string.IsNullOrEmpty(data.prefData))
        {
            userDicDataPref = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.prefData);
        }
    }

    #region Save Sync

    public string GetString(string key, string defaultValue)
    {
        return userDicDataPref.ContainsKey(key) ? userDicDataPref[key] : defaultValue;
    }

    public void SetString(string key, string newValue)
    {
        userDicDataPref[key] = newValue;
        SaveData();
    }

    public int GetInt(string key, int defaultValue)
    {
        if (!userDicDataPref.ContainsKey(key)) return defaultValue;

        return Convert.ToInt32(userDicDataPref[key]);
    }

    public void SetInt(string key, int newValue)
    {
        userDicDataPref[key] = newValue.ToString();
        SaveData();
    }

    #endregion

    public GameDataSave GetGameData()
    {
        string loadData = GetString("game_data", "");

        if (string.IsNullOrWhiteSpace(loadData))
        {
            Debug.Log("No save found → Creating new save...");

            GameDataSave newData = new GameDataSave();
            string json = JsonConvert.SerializeObject(newData);

            SetString("game_data", json);
            return newData;
        }

        return JsonConvert.DeserializeObject<GameDataSave>(loadData);
    }

    public void CheckInitData()
    {
        int isInit = GetInt("init", -1);
        
        if (isInit <= 0)
        {
            SetLevelData(1);
            SetMoneyData(0);
            SetLifeData(5);
            SetFrozenData(0);
            SetBombData(0);
            SetHammerData(0);

            SetInt("init", 1);
        }
    }

    public int GetLevelData()
    {
        int level = GetInt("level", 1);
        return level;
    }
    public void SetLevelData(int value)
    {
        SetInt("level", value);
    }

    public int GetMoneyData()
    {
        int money = GetInt("money", 0);
        return money;
    }
    public void SetMoneyData(int value)
    {
        SetInt("money", value);
    }

    public int GetLifeData()
    {
        int life = GetInt("life", 5);
        return life;
    }
    public void SetLifeData(int value)
    {
        SetInt("life", value);
    }

    public int GetFrozenData()
    {
        int life = GetInt("frozen", 0);
        return life;
    }
    public void SetFrozenData(int value)
    {
        SetInt("frozen", value);
    }

    public int GetBombData()
    {
        int life = GetInt("bomb", 0);
        return life;
    }
    public void SetBombData(int value)
    {
        SetInt("bomb", value);
    }

    public int GetHammerData()
    {
        int life = GetInt("hammer", 0);
        return life;
    }
    public void SetHammerData(int value)
    {
        SetInt("hammer", value);
    }
}

[System.Serializable]
public class PlayerData
{
    public string prefData;

    public PlayerData()
    {
        prefData = "";
    }
}
