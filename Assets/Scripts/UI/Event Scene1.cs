using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventScene1 : MonoBehaviour
{
    public Text textLabel;
    public TextAsset textFile;
    public int textline;
    List<string> textList = new List<string>();
    // Start is called before the first frame update
    void Awake()
    {
        GetText(textFile);

    }
    private void OnEnable()
    {
        textLabel.text = textList[textline];
        textline++;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && textline == textList.Count)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            textline = 0;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            textLabel.text = textList[textline];
            textline++;
        }
    }
    void GetText (TextAsset file)
    {
        textList.Clear();
        textline = 0;
        var lineDate = file.text.Split('\n');
        foreach (var line in lineDate)
        {
            textList.Add(line);
        }
    }
}
