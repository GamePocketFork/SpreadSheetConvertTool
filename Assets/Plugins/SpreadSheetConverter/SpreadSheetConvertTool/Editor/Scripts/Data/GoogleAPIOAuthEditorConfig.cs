using UnityEngine;

using FileUtility = charcolle.SpreadSheetConverter.FileUtility;

public class GoogleAPIOAuthEditorConfig : ScriptableObject {

    public string OAuthClientId;
    public string OAuthClientSecret;
    public string RefreshToken;

    [HideInInspector]
    public string AccessToken;

    private static GoogleAPIOAuthEditorConfig instance;

    public static GoogleAPIOAuthEditorConfig Instance {
        get {
            if( instance == null )
                instance = FileUtility.LoadGoogleAPIOAuthConfigData();
            return instance;
        }
    }

}