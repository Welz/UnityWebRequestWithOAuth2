using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest : MonoBehaviour
{
    
    private static bool isRunning = false;

    public static IEnumerator GetRequest(string url, Action<string> result)
    {
        while(isRunning)
        {
            yield return new WaitForSeconds(0.1f);
        }
        isRunning = true;
        UnityWebRequest www = new UnityWebRequest(url, "GET");

        //--- for upload handler
        //UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
        //www.uploadHandler = uH;

        //--- for downloadhandler
        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
        www.downloadHandler = dH;

        string token = null;
        yield return TokenManager.FindAccessToken((tokenResult) => { token = tokenResult; });

        www.SetRequestHeader("Authorization", "Bearer " + token);
        www.SendWebRequest();

        if (!www.isNetworkError )
        {
            while (!www.isDone)
            {
                //code for checking my received strings....
                yield return new WaitForSeconds(0.1f);
            }
            result(www.downloadHandler.text); 
        }
        else
        {
            Debug.Log("GET request unsuccesful");
            //Return null
            result("");
        }
        isRunning = false;

    }
    
    public static IEnumerator PostRequest(string url, string content)
    {

        UnityWebRequest www = new UnityWebRequest(url, "POST");

        //--- for upload handler
        UploadHandlerRaw uH = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(content));
        uH.contentType = "application/json";
        www.uploadHandler = uH;
              
        //--- for downloadhandler
        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
        www.downloadHandler = dH;

        //string token = null;
        string token = null;
        yield return TokenManager.FindAccessToken((tokenResult) => { token = tokenResult; });

        www.SetRequestHeader("Authorization", "Bearer " + token);

        yield return www.SendWebRequest();

        if (!www.isNetworkError)
        {
            string resultContent = www.downloadHandler.text;
            Debug.Log("Succesful posting " + resultContent);
        }
        else
        {
            Debug.Log("POST request unsuccesful");
        }
    }

    public static IEnumerator PutRequest(string url, string content)
    {
       
        UnityWebRequest www = new UnityWebRequest(url, "PUT");

        //--- for upload handler
        UploadHandlerRaw uH = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(content));
        uH.contentType = "application/json";
        www.uploadHandler = uH;

        //--- for downloadhandler
        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
        www.downloadHandler = dH;

        string token = null;
        yield return TokenManager.FindAccessToken((tokenResult) => { token = tokenResult; });

        www.SetRequestHeader("Authorization", "Bearer " + token);
        www.SendWebRequest();
        //yield return www.Send();

        if (!www.isNetworkError)
        {
            while (!www.isDone)
            {
                //code for checking my received strings....
                yield return new WaitForSeconds(0.1f);
            }
            string resultContent = www.downloadHandler.text;
            Debug.Log("Succesful posting " + resultContent);
        }
        else
        {
            Debug.Log("PUT request unsuccesful");
        }
    }

    public static IEnumerator DeleteRequest(string url)
    {
        UnityWebRequest www = new UnityWebRequest(url, "DELETE");

        //--- for downloadhandler
        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
        www.downloadHandler = dH;

        //string token = null;
        string token = null;
        yield return TokenManager.FindAccessToken((tokenResult) => { token = tokenResult; });

        www.SetRequestHeader("Authorization", "Bearer " + token);

        yield return www.SendWebRequest();

        if (!www.isNetworkError)
        {
            string resultContent = www.downloadHandler.text;
            Debug.Log("Succesful posting " + resultContent);
        }
        else
        {
            Debug.Log("DELETE request unsuccesful");
        }
    }
}
