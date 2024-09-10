using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsDisplay : MonoBehaviour
{
    float timeA;
    public int fps;
    public int lastFPS;
    public GUIStyle textStyle;
    [SerializeField] private TextMeshProUGUI teks;
    // Start is called before the first frame update
    void Start()
    {
        timeA = Time.timeSinceLevelLoad;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeSinceLevelLoad - timeA <= 1)
        {
            fps++;
            teks.text = fps.ToString();
        }    
    }

    void OnGUI()
    {
        GUI.Label(new Rect(450, 5, 30, 30), "" + lastFPS, textStyle);
    }
}
