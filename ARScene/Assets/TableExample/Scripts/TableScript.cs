using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TableScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTimetable());
    }

    // Update is called once per frame
    IEnumerator UpdateTimetable()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            using (UnityWebRequest timetableRequest = UnityWebRequest.Get("https://busstops.nikolaishieiko.repl.co/id"))
            {
                yield return timetableRequest.SendWebRequest();
                Debug.Log(timetableRequest.downloadHandler.text);
                FindObjectOfType<TextMeshPro>().GetComponent<TextMeshPro>().text = timetableRequest.downloadHandler.text;
            }
            yield return new WaitForSeconds(10f);
        }
    }
}
