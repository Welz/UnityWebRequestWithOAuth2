using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoRequest : MonoBehaviour
{
    public DemoDataClass demoData;

    [SerializeField] Text url;
    [SerializeField] Text input;


    public void GetRequestDemo()
    {
        print(url.text);
        StartCoroutine(WebRequest.GetRequest(url.text, (resultContent) =>
        {
            print(resultContent);
            demoData = JsonUtility.FromJson<DemoDataClass>(resultContent);
        }));
    }

    public void PostRequestDemo()
    {
        StartCoroutine(WebRequest.PostRequest(url.text, input.text));
    }

    public void PutRequestDemo()
    {
        StartCoroutine(WebRequest.PutRequest(url.text, input.text));
    }

    public void DeleteRequestDemo()
    {
        StartCoroutine(WebRequest.DeleteRequest(url.text));
    }
}