using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CSVValueLookup : MonoBehaviour {

	public static CSVValueLookup Instance {
		get; private set;
	}

	[Header("CSV file")]
	public TextAsset CSVFile;

	[Header("Values from CSV"), SerializeField]
	private List<CSVValue> valueList;

	[HideInInspector]
	public List<CSVValue> ValueList => valueList;

	public void Awake(){
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}

		Instance = this;
			
		if(Application.isPlaying)
			DontDestroyOnLoad(gameObject);

		// ParseCSVFile();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[ContextMenu("Load CSV Values")]
	public void LoadCSVValues(){
		if(CSVFile == null || CSVFile.text.Length < 1) {
			Debug.LogError("No CSV file set or CSV file empty");
			return;
		}

		valueList = new List<CSVValue>();

		string[] lines = CSVFile.text.Trim().Split('\n');
		for(int l = 0; l < lines.Length; l++){
			string[] data = lines[l].Split(',');
			float result = float.MaxValue;

			if(!float.TryParse(data[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result)) {
				Debug.LogError(data[0] + " can not be parsed");
				continue;
			}

			CSVValue newValue = valueList.Find(csvv => { return csvv.name == data[0];});
			if(newValue == null){
				valueList.Add(new CSVValue(){
					name = data[0],
					value = result
				});
			} else {
				newValue.value = result;
			}
		}
	}

	#if UNITY_EDITOR
	[ContextMenu("Save CSV Values")]
	public void SaveCSVValues(){
		
		string temp = "";
			
		for(int i = 0; i < valueList.Count; i++){
			temp += valueList[i].ToCSVString() + "\n";
		}
			
		File.WriteAllText(Application.dataPath + "/Scripts/Game/Balancing/CSVValues.csv", temp);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
	#endif 
	
}

[System.Serializable]
public class CSVValue {
	public string name;
	public float value;

	public string ToCSVString(){
		return name + "," + value.ToString().Replace(",",".");
	}
}

[System.Serializable]
public class PrefabValues {
	public GameObject prefab;
	public List<string> valueNames;
}

// public static class CSVValues {
// 	private static List<CSVValue> valueList;
// 	public static List<CSVValue> ValueList {
// 		get;
// 	}

// 	public static void SetCSVValueList(List<CSVValue> list){
// 		valueList = list;
// 	}
// }

#if UNITY_EDITOR
[CustomEditor(typeof(CSVValueLookup))]
public class CSVValueLookupEditor : Editor {
	public override void OnInspectorGUI(){
		CSVValueLookup target = serializedObject.targetObject as CSVValueLookup;
		
		DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Load CSV Values")){
			target.LoadCSVValues();
		}
		if(GUILayout.Button("Save CSV Values")){
			target.SaveCSVValues();
		}
		EditorGUILayout.EndHorizontal();
	}
}
#endif