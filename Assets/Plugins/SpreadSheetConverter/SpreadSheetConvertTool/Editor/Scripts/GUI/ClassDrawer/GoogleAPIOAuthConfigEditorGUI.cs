using System;
using UnityEditor;
using UnityEngine;

namespace charcolle.SpreadSheetConverter {

    internal class GoogleAPIOAuthConfigEditorGUI : EditorWindowItem<GoogleAPIOAuthEditorConfig> {

        public GoogleAPIOAuthConfigEditorGUI( GoogleAPIOAuthEditorConfig data ) : base( data ) { }

        protected override void Draw() {

            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space( 5 );
                GUILayout.Label( "<b>Default Setting</b>" );
                GUILayout.Space( 5 );

                var defaultWidth            = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;
                OAuthClientId               = EditorGUILayout.TextField( "ClientID", OAuthClientId, GUIHelper.Styles.TextFieldWordWrap );
                OAuthClientSecret           = EditorGUILayout.TextField( "ClientSecret", OAuthClientSecret, GUIHelper.Styles.TextFieldWordWrap );
                RefreshToken                = EditorGUILayout.TextField( "RefreshToken", RefreshToken, GUIHelper.Styles.TextFieldWordWrap );
                EditorGUIUtility.labelWidth = defaultWidth;

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( "Check Config", new GUILayoutOption[] { GUILayout.Width( 120 ), GUILayout.Height( 25 ) } ) ) {
                        EditorUtility.DisplayProgressBar( "Spread Sheet Converter", "Checking Config...", 0f );
                        try {
                            var webRequest = new GoogleSpreadSheetWebRequest( OAuthClientId, OAuthClientSecret, RefreshToken );
                            var requestCo = webRequest.CheckAccessToken();
                            while( requestCo.MoveNext() ) { }
                            EditorUtility.DisplayDialog( "Spread Sheet Converter", "The setting is correct.", "ok" );
                            AccessToken = webRequest.GetAccessToken();
                        } catch( Exception ex ) {
                            Debug.LogError( ex );
                        }
                        EditorUtility.ClearProgressBar();
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 15 );

                GUILayout.Label( "<b>Your Current AccessToken</b>" );
                GUILayout.Label( AccessToken, GUIHelper.Styles.LabelWordWrap );
            }
            EditorGUILayout.EndHorizontal();

        }

        //======================================================================
        // property
        //======================================================================

        private string OAuthClientId {
            get {
                return data.OAuthClientId;
            }
            set {
                data.OAuthClientId = value;
            }
        }

        private string OAuthClientSecret {
            get {
                return data.OAuthClientSecret;
            }
            set {
                data.OAuthClientSecret = value;
            }
        }

        private string RefreshToken {
            get {
                return data.RefreshToken;
            }
            set {
                data.RefreshToken = value;
            }
        }

        private string AccessToken {
            get {
                return data.AccessToken;
            }
            set {
                data.AccessToken = value;
            }
        }


    }

}