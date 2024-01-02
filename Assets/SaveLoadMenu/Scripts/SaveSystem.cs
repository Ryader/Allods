using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem {

    private static bool loaded;
    private static MetaState meta = new MetaState();
    private static DataState data = new DataState();
    public static int saveCount { get; private set; }
    public static string currentScene { get; private set; }
    public static List<FileList> fileLists { get; private set; }

    public static string GameResumeScene()
    {
        if (fileLists.Count > 0)
        {
            for (int i = 0; i < meta.items.Count; i++)
            {
                if (string.Compare(fileLists[0].meta, meta.items[i].Key) == 0)
                {
                    return meta.items[i].Scene;
                }
            }
        }

        return null;
    }

    public static bool IsData()
    {
        return data.items.Count > 0;
    }

    public static void Initialize()
    {
        if (!loaded)
        {
            if (GetFileList()) fileLists.Sort(new DateComparer());
            if (File.Exists(SaveMeta())) meta = SerializatorBinary.LoadMeta(SaveMeta());
            loaded = true;
        }
    }

    public static void RemoveKey(string key)
    {
        for (int i = 0; i < data.items.Count; i++)
        {
            if (string.Compare(key, Crypt(data.items[i].Key)) == 0)
            {
                data.items.RemoveAt(i);
                break;
            }
        }
    }

    public static void RemoveSave(System.DateTime date)
    {
        string key = date.ToFileTime().ToString();
        for (int i = 0; i < meta.items.Count; i++)
        {
            if (string.Compare(key, meta.items[i].Key) == 0)
            {
                meta.items.RemoveAt(i);
                break;
            }
        }

        if (meta.items.Count > 0)
        {
            SerializatorBinary.SaveMeta(meta, SaveMeta());
        }
        else if (meta.items.Count == 0)
        {
            if (File.Exists(SaveMeta())) File.Delete(SaveMeta());
        }

        for (int i = 0; i < fileLists.Count; i++)
        {
            if (System.DateTime.Compare(date, fileLists[i].date) == 0)
            {
                if (File.Exists(fileLists[i].path)) File.Delete(fileLists[i].path);
                if (File.Exists(fileLists[i].path.Replace(".bin", ".png"))) File.Delete(fileLists[i].path.Replace(".bin", ".png"));
                fileLists.RemoveAt(i);
                fileLists.Sort(new DateComparer());
                MenuSaveLoad.ReBuildList();
                break;
            }
        }
    }

    static void AddSaveToList(System.DateTime timeNow, string path, string title)
    {
        FileList fl = new FileList();
        fl.date = timeNow;
        fl.meta = timeNow.ToFileTime().ToString();
        fl.path = path;
        fl.title = title;
        fileLists.Add(fl);
        saveCount++;
    }

    public static string SavePath()
    {
        return Application.persistentDataPath + "/Save";
    }

    static string SaveMeta()
    {
        return Application.persistentDataPath + "/Save/Metadata.bin";
    }

    static bool GetFileList()
    {
        fileLists = new List<FileList>();
        saveCount = 0;

        if (!Directory.Exists(SavePath()))
        {
            Directory.CreateDirectory(SavePath());
            return false;
        }

        string[] list = Directory.GetFiles(SavePath(), "*.bin");

        if (list != null)
        {
            for (int i = 0; i < list.Length; i++)
            {
                string[] tmp = Path.GetFileNameWithoutExtension(list[i]).Split(new char[] { '-' });
                if (tmp.Length == 2)
                {
                    long value;
                    if (long.TryParse(tmp[1], out value))
                    {
                        FileList fl = new FileList();
                        fl.date = System.DateTime.FromFileTime(value);
                        fl.meta = tmp[1];
                        fl.path = list[i];
                        fl.title = tmp[0].Replace('_', ' ');
                        fileLists.Add(fl);
                        saveCount++;
                    }
                }
            }

            if (fileLists.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    static string SaveFile(System.DateTime timeNow, string file)
    {
        return string.Format
            (
            "{0}/{1}-{2}",
            SavePath(),
            file,
            timeNow.ToFileTime()
            );
    }

	public static void Load(System.DateTime date)
	{
        for (int i = 0; i < fileLists.Count; i++)
        {
            if (System.DateTime.Compare(date, fileLists[i].date) == 0)
            {
                data = SerializatorBinary.LoadBinary(fileLists[i].path);
                Debug.Log("[SaveSystem] --> Загрузка файла сохранения, по адресу: " + fileLists[i].path);
                break;
            }
        }
	}

	static void ReplaceItem(string name, string item)
	{
		bool j = false;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				data.items[i].Value = Crypt(item);
				j = true;
				break;
			}
		}

		if(!j) data.AddItem(new SaveData(Crypt(name), Crypt(item)));
	}

	public static bool HasKey(string name) // проверка, на наличие ключа
	{
		if(string.IsNullOrEmpty(name)) return false;

		foreach(SaveData k in data.items)
		{
			if(string.Compare(name, Crypt(k.Key)) == 0)
			{
				return true;
			}
		}

		return false;
	}

	public static void SetVector3(string name, Vector3 val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.x + "|" + val.y + "|" + val.z);
	}

	public static void SetVector2(string name, Vector2 val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.x + "|" + val.y);
	}

	public static void SetColor(string name, Color val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.r + "|" + val.g + "|" + val.b + "|" + val.a);
	}

	public static void SetBool(string name, bool val) // установка ключа и значения
	{
		if(string.IsNullOrEmpty(name)) return;
		string tmp = string.Empty;
		if(val) tmp = "1"; else tmp = "0";
		ReplaceItem(name, tmp);
	}

	public static void SetFloat(string name, float val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val.ToString());
	}

	public static void SetInt(string name, int val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val.ToString());
	}

	public static void SetString(string name, string val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val);
	}

	public static void SaveToDisk(string title, string metaValue, System.DateTime timeNow) // запись данных в файл
	{
        if (data.items.Count == 0 || string.IsNullOrEmpty(title)) return;
        string file = title.Replace(' ', '_');
        string pathFull = SaveFile(timeNow, file);
        SerializatorBinary.SaveBinary(data, pathFull + ".bin");
        CreateMetaFile(timeNow, metaValue);
        AddSaveToList(timeNow, pathFull + ".bin", title);
        fileLists.Sort(new DateComparer());
        MenuSaveLoad.ReBuildList();
        Debug.Log("[SaveSystem] --> Сохранение игровых данных: " + pathFull + ".bin");
    }

    static void CreateMetaFile(System.DateTime timeNow, string metaValue)
    {
        meta.AddItem(new SaveMeta(timeNow.ToFileTime().ToString(), SceneManager.GetActiveScene().name, metaValue));
        SerializatorBinary.SaveMeta(meta, SaveMeta());
    }

    public static string LoadMetaData(string key)
    {
        currentScene = null;

        for (int i = 0; i < meta.items.Count; i++)
        {
            if (string.Compare(key, meta.items[i].Key) == 0)
            {
                currentScene = meta.items[i].Scene;
                return meta.items[i].Data;
            }
        }

        return null;
    }

    public static Vector3 GetVector3(string name)
	{
		if(string.IsNullOrEmpty(name)) return Vector3.zero;
		return iVector3(name, Vector3.zero);
	}

	public static Vector3 GetVector3(string name, Vector3 defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iVector3(name, defaultValue);
	}

	static Vector3 iVector3(string name, Vector3 defaultValue)
	{
		Vector3 vector = Vector3.zero;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				string[] t = Crypt(data.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 3)
				{
					vector.x = floatParse(t[0]);
					vector.y = floatParse(t[1]);
					vector.z = floatParse(t[2]);
					return vector;
				}
				break;
			}
		}

		return defaultValue;
	}

	public static Vector2 GetVector2(string name)
	{
		if(string.IsNullOrEmpty(name)) return Vector2.zero;
		return iVector2(name, Vector2.zero);
	}

	public static Vector2 GetVector2(string name, Vector2 defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iVector2(name, defaultValue);
	}

	static Vector2 iVector2(string name, Vector2 defaultValue)
	{
		Vector2 vector = Vector2.zero;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				string[] t = Crypt(data.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 2)
				{
					vector.x = floatParse(t[0]);
					vector.y = floatParse(t[1]);
					return vector;
				}
				break;
			}
		}

		return defaultValue;
	}

	public static Color GetColor(string name)
	{
		if(string.IsNullOrEmpty(name)) return Color.white;
		return iColor(name, Color.white);
	}

	public static Color GetColor(string name, Color defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iColor(name, defaultValue);
	}

	static Color iColor(string name, Color defaultValue)
	{
		Color color = Color.clear;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				string[] t = Crypt(data.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 4)
				{
					color.r = floatParse(t[0]);
					color.g = floatParse(t[1]);
					color.b = floatParse(t[2]);
					color.a = floatParse(t[2]);
					return color;
				}
				break;
			}
		}

		return defaultValue;
	}

    public static bool GetBool(string name)
	{
		if(string.IsNullOrEmpty(name)) return false;
		return iBool(name, false);
	}

	public static bool GetBool(string name, bool defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iBool(name, defaultValue);
	}

	static bool iBool(string name, bool defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				if(string.Compare(Crypt(data.items[i].Value), "1") == 0) return true; else return false;
			}
		}

		return defaultValue;
	}

	public static float GetFloat(string name)
	{
		if(string.IsNullOrEmpty(name)) return 0;
		return iFloat(name, 0);
	}

	public static float GetFloat(string name, float defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iFloat(name, defaultValue);
	}

	static float iFloat(string name, float defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				return floatParse(Crypt(data.items[i].Value));
			}
		}

		return defaultValue;
	}

	public static int GetInt(string name)
	{
		if(string.IsNullOrEmpty(name)) return 0;
		return iInt(name, 0);
	}

	public static int GetInt(string name, int defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iInt(name, defaultValue);
	}

	static int iInt(string name, int defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				return intParse(Crypt(data.items[i].Value));
			}
		}

		return defaultValue;
	}

	public static string GetString(string name)
	{
		if(string.IsNullOrEmpty(name)) return null;
		return iString(name, null);
	}

	public static string GetString(string name, string defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iString(name, defaultValue);
	}

	static string iString(string name, string defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, Crypt(data.items[i].Key)) == 0)
			{
				return Crypt(data.items[i].Value);
			}
		}

		return defaultValue;
	}

	static int intParse(string val)
	{
		int value;
		if(int.TryParse(val, out value)) return value;
		return 0;
	}

	static float floatParse(string val)
	{
		float value;
		if(float.TryParse(val, out value)) return value;
		return 0;
	}
    
	static string Crypt(string text)
	{
        string result = string.Empty;
		foreach(char j in text) result += (char)((int)j ^ 47);
		return result;
	}
}

[System.Serializable]
class SaveData
{

    public string Key { get; set; }
    public string Value { get; set; }

    public SaveData() { }

    public SaveData(string key, string value)
    {
        this.Key = key;
        this.Value = value;
    }
}

[System.Serializable]
class DataState
{
    public List<SaveData> items = new List<SaveData>();

    public DataState() { }

    public void AddItem(SaveData item)
    {
        items.Add(item);
    }
}

[System.Serializable]
class SaveMeta
{

    public string Key { get; set; }
    public string Scene { get; set; }
    public string Data { get; set; }

    public SaveMeta() { }

    public SaveMeta(string key, string scene, string value)
    {
        this.Key = key;
        this.Scene = scene;
        this.Data = value;
    }
}

[System.Serializable]
class MetaState
{
    public List<SaveMeta> items = new List<SaveMeta>();

    public MetaState() { }

    public void AddItem(SaveMeta item)
    {
        items.Add(item);
    }
}

class SerializatorBinary
{

    public static void SaveBinary(DataState state, string dataPath)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Create);
        binary.Serialize(stream, state);
        stream.Close();
    }

    public static DataState LoadBinary(string dataPath)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Open);
        DataState state = (DataState)binary.Deserialize(stream);
        stream.Close();
        return state;
    }

    public static void SaveMeta(MetaState state, string dataPath)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Create);
        binary.Serialize(stream, state);
        stream.Close();
    }

    public static MetaState LoadMeta(string dataPath)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Open);
        MetaState state = (MetaState)binary.Deserialize(stream);
        stream.Close();
        return state;
    }
}

public class FileList
{
    public string title;
    public System.DateTime date;
    public string path;
    public string meta;
}

class DateComparer : IComparer<FileList>
{
    public int Compare(FileList x, FileList y)
    {
        if (x.date < y.date)
            return 1;
        if (x.date > y.date)
            return -1;
        else return 0;
    }
}
