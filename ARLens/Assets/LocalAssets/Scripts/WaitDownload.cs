using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitDownload : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ProgressBar());
    }


    IEnumerator ProgressBar()
    {
        var chars = new List<string> { "/", "--", "\\", "|" };
        int i = 0;
        while (true)
        {
            GetComponentInChildren<TextMeshPro>().text = chars[i % 4];

            yield return new WaitForSeconds(0.3f);
            i++;
        }
    }
}
