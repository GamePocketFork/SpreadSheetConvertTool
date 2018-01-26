using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public static class GoogleSpreadSheetWebRequest {

        private static readonly string URI_TOKENINFO    = "https://www.googleapis.com/oauth2/v3/tokeninfo";
        private static readonly string URI_REFRESHTOKEN = "https://www.googleapis.com/oauth2/v4/token";

        private static readonly string URI_SPREADSHEET  = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}";

        //======================================================================
        // public
        //======================================================================

        public static string GetAccessToken() {
            WWWForm form = new WWWForm();
            form.AddField( "refresh_token", GoogleAPIOAuthConfig.Instance.RefreshToken );
            form.AddField( "client_id", GoogleAPIOAuthConfig.Instance.OAuthClientId );
            form.AddField( "client_secret", GoogleAPIOAuthConfig.Instance.OAuthClientSecret );
            form.AddField( "grant_type", "refresh_token" );
            //form.headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // argument exception
            form.headers[ "Content-Type" ] = "application/x-www-form-urlencoded";
            var requestoCo = PostCo( URI_REFRESHTOKEN, form );
            while( requestoCo.MoveNext() ) { }

            return ( string )requestoCo.Current;
        }

        public static string CheckAccessToken() {
            WWWForm form = new WWWForm();
            form.AddField( "access_token", GoogleAPIOAuthConfig.Instance.AccessToken );
            //form.headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // argument exception
            form.headers[ "Content-Type" ] = "application/x-www-form-urlencoded";
            var requestoCo = PostCo( URI_TOKENINFO, form );
            while( requestoCo.MoveNext() ) { }
            return ( string )requestoCo.Current;
        }

        public static string GetSheetsAPI( string spreadSheetId, string sheetRange ) {
            var uri = string.Format( URI_SPREADSHEET, spreadSheetId, sheetRange );
            var requestoCo = GetCo( uri );
            while( requestoCo.MoveNext() ) { }
            return ( string )requestoCo.Current;
        }

        //======================================================================
        // process
        //======================================================================

        private static IEnumerator PostCo( string uri, WWWForm form ) {
            var request = UnityWebRequest.Post( uri, form );
            request.SendWebRequest();

            while( !request.isDone )
                yield return null;

            if( request.isNetworkError ) {
                Debug.LogError( "SpreadSheetConvertTool: " + request.error );
                yield return null;
            } else {
                //Debug.Log( request.downloadHandler.text );
                yield return request.downloadHandler.text;
            }
        }

        private static IEnumerator GetCo( string uri ) {
            var request = UnityWebRequest.Get( uri );
            request.SetRequestHeader( "Content-Type", "application/json" );
            request.SetRequestHeader( "Authorization", string.Format( "Bearer {0}", GoogleAPIOAuthConfig.Instance.AccessToken ) );
            request.SendWebRequest();

            while( !request.isDone )
                yield return null;

            if( request.isNetworkError ) {
                Debug.LogError( "SpreadSheetConvertTool: " + request.error );
                yield return null;
            } else {
                //Debug.Log( request.downloadHandler.text );
                yield return request.downloadHandler.text;
            }
        }
    }
}