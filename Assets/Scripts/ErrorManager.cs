using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ErrorManager{

    public static bool IsError(string resultContent)
    {
        Error error = JsonUtility.FromJson<Error>(resultContent);
        if (error.error == "invalid_grant")
        {
            Debug.Log("Invalid Grant. Getting new Refresh Token.");
            return true;
        }
        if(error.error == "invalid_token")
        {
            Debug.Log("Invalid Token. Getting new Access Token.");
            return true;
        }
        return false;
    }

}

public class Error
{
    public string error;
    public string error_description;
}
