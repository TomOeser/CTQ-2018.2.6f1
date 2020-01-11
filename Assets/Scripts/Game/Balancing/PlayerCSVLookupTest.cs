using UnityEngine;

public class PlayerCSVLookupTest : MonoBehaviour {

    [SerializeField] private float p_movespeed = -1;
    [SerializeField] private int p_health = -1;

    private CSVValueLookup cSVValueLookup;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // CSVValueLookup lookup = GameObject.Find("CSVConverter").GetComponent<CSVValueLookup>/
        Debug.Log(nameof(p_movespeed));
        Debug.Log(CSVValueLookup.Instance.name, CSVValueLookup.Instance);

        Debug.Log(CSVValueLookup.Instance.ValueList.Count);

        p_movespeed = CSVValueLookup.Instance.ValueList.Find(csvv =>  { return csvv.name == nameof(p_movespeed); }).value;
        p_health = (int) CSVValueLookup.Instance.ValueList.Find(csvv =>  { return csvv.name == nameof(p_health); }).value;
    }

}