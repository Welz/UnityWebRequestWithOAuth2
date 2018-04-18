using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TokenManager : MonoBehaviour
{
    [Tooltip("The endpoind for authorization server. This is used to exchange the authorization code for an access token.")]
    [SerializeField] Text accessTokenUrl;

    [Header("Client")]
    [Tooltip("The client identifier issued to the client during the Application registration process.")]
    [SerializeField] Text clientId;
    [Tooltip("The client secret issued to the client during the Application registration process")]
    [SerializeField] Text clientSecret;

    [Header("User")]
    [SerializeField] Text username;
    [SerializeField] Text password;

    static string sAccessTokenUrl;
    static string sClientId;
    static string sClientSecret;
    static string sUsername;
    static string sPassword;

    //Using this method for demo purposes.
    public void InitializeVariables()
    {
        if (accessTokenUrl.text == "" ||
            clientId.text == ""       ||
            clientSecret.text == ""   ||
            username.text == ""       ||
            password.text == "")
        {
            Debug.LogError("Initialize all the variables in the inspector.");
        }
        else
        {
            sAccessTokenUrl = accessTokenUrl.text;
            sClientId = clientId.text;
            sClientSecret = clientSecret.text;
            sUsername = username.text;
            sPassword = password.text;
            Debug.Log("Variables initialized correctly"+ sAccessTokenUrl + sClientId + sClientSecret + sUsername + sPassword);
        }
    }


    [Serializable]
    public class AccessToken
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public string expires_in;
        public string scope;
    }

    public static IEnumerator FindAccessToken(Action<String> result)
    {
        //try to load last stored token
        AccessToken token = new AccessToken();
        LoadToken(token);

        //if an access token exists and it is not expired, return it
        if (token.access_token != String.Empty && DateTime.Compare(DateTime.Now, DateTime.Parse(token.expires_in)) < 0)
        {
            Debug.Log("Last used access_token: " + token.access_token + " will not expire until: " + token.expires_in.ToString());
            result(token.access_token);
        }
        else
        {
            //otherwise get new access token, save it and return it
            yield return GetAccessToken((tokenResult) => { token = tokenResult; });
            if (token.access_token != String.Empty)
            {
                token.expires_in = DateTime.Now.AddSeconds(Int32.Parse(token.expires_in)).ToString();
                Debug.Log("New access_token: " + token.access_token + " will not expire until: " + token.expires_in);
                SaveToken(token);
            }
            else
            {
                Debug.LogError("No access token returned");
            }

            result(token.access_token);
        }
    }

    private static string GetAccessToken()
    {
        return "";
    }

    ////dummy needs to correct the input
    private static IEnumerator GetAccessToken(Action<AccessToken> result)
    {
        Dictionary<string, string> content = new Dictionary<string, string>();

        //grant type -> refresh token
        content.Add("grant_type", "refresh_token");
        content.Add("client_id", sClientId);
        content.Add("client_secret", sClientSecret);
        content.Add("refresh_token", PlayerPrefs.GetString("refresh_token"));

        UnityWebRequest www = UnityWebRequest.Post(sAccessTokenUrl, content);

        //Send request
        yield return www.SendWebRequest();


        //Check if POST request is succesful
        if (!www.isNetworkError)
        {
            string resultContent = www.downloadHandler.text;
            print(resultContent);
            AccessToken token = new AccessToken();

            //check if refresh token is invalid
            if (ErrorManager.IsError(resultContent))
            {
                yield return GetRefreshAndAccessToken((tokenResult) => { token = tokenResult; });
            }
            else
            {
                token = JsonUtility.FromJson<AccessToken>(resultContent);
            }
            
            //Return result
            result(token);
        }
        else
        {
            //Return null
            Debug.Log("POST request unsuccesful");
            result(new AccessToken());
        }
    }

    private static IEnumerator GetRefreshAndAccessToken(Action<AccessToken> result)
    {
        Dictionary<string, string> content = new Dictionary<string, string>();
        //Fill key and value
        //grant type -> Password credentials
        content.Add("grant_type", "password");
        //TO DO: user login and save credentials to playerPrefs
        content.Add("username", sUsername);
        content.Add("password", sPassword);
        content.Add("client_id", sClientId);
        content.Add("client_secret", sClientSecret);

        UnityWebRequest www = UnityWebRequest.Post(sAccessTokenUrl, content);

        //Send request
        yield return www.SendWebRequest();

        //Check if POST request is succesful
        if (!www.isNetworkError)
        {
            string resultContent = www.downloadHandler.text;
            print(resultContent);
            //TO DO: need to check for invalid credential
            AccessToken token = JsonUtility.FromJson<AccessToken>(resultContent);
            //Return result
            result(token);

        }
        else
        {
            //Return null
            Debug.Log("POST request unsuccesful");
            result(new AccessToken());
        }
    }

    private static void LoadToken(AccessToken token)
    {
        token.access_token = PlayerPrefs.GetString("access_token");
        token.refresh_token = PlayerPrefs.GetString("refresh_token");
        token.expires_in = PlayerPrefs.GetString("expires_in");
    }

    private static void SaveToken(AccessToken token)
    {
        PlayerPrefs.SetString("access_token", token.access_token);
        PlayerPrefs.SetString("refresh_token", token.refresh_token);
        PlayerPrefs.SetString("expires_in", token.expires_in);
    }

    public static void CleanToken()
    {
        PlayerPrefs.SetString("access_token", "");
        PlayerPrefs.SetString("expires_in", "");
    }

}
