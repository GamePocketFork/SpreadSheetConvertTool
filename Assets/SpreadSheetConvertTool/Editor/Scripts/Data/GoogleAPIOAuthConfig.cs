using UnityEditor;
using UnityEngine;

using FileUtility = charcolle.Utility.SpreadSheetConvertTool.FileUtility;
using GUIHelper = charcolle.Utility.SpreadSheetConvertTool.GUIHelper;

public class GoogleAPIOAuthConfig : ScriptableObject {

    public string OAuthClientId;
    public string OAuthClientSecret;
    public string RefreshToken;

    [HideInInspector]
    public string AccessToken;

    private static GoogleAPIOAuthConfig instance;

    public static GoogleAPIOAuthConfig Instance {
        get {
            if( instance == null )
                instance = FileUtility.LoadGoogleAPIOAuthConfigData();
            return instance;
        }
    }

    public void Draw() {
        EditorGUILayout.BeginVertical();
        {
            GUILayout.Space( 10 );
            OAuthClientId = EditorGUILayout.TextField( "ClientID", OAuthClientId, GUIHelper.Styles.TextFieldWordWrap );
            OAuthClientSecret = EditorGUILayout.TextField( "ClientSecret", OAuthClientSecret, GUIHelper.Styles.TextFieldWordWrap );
            RefreshToken = EditorGUILayout.TextField( "RefreshToken", RefreshToken, GUIHelper.Styles.TextFieldWordWrap );

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if( GUILayout.Button( "Check Config", new GUILayoutOption[] { GUILayout.Width( 120 ), GUILayout.Height( 25 ) } ) ) {
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 15 );

            GUILayout.Label( "<b>Your Current AccessToken</b>" );
            GUILayout.Label( AccessToken, GUIHelper.Styles.LabelWordWrap );
        }
        EditorGUILayout.EndHorizontal();
    }

}