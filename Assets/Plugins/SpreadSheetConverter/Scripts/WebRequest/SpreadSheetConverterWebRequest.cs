using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

namespace charcolle.SpreadSheetConverter {

    public class GoogleSpreadSheetWebRequest {

        private static readonly string URI_TOKENINFO    = "https://www.googleapis.com/oauth2/v3/tokeninfo";
        private static readonly string URI_REFRESHTOKEN = "https://www.googleapis.com/oauth2/v4/token";

        private static readonly string URI_SPREADSHEET  = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}";
        private static readonly string URI_SHEETGET     = "https://sheets.googleapis.com/v4/spreadsheets/{0}?&fields=sheets.properties";

        private readonly string OAuthClientId;
        private readonly string OAuthClientSecret;
        private readonly string RefreshToken;
        private readonly string SheetId;

        private string AccessToken;
        private bool IsAccessTokenOK;

        public GoogleSpreadSheetWebRequest( string OAuthClientId, string OAuthClientSecret, string RefreshToken, string AccessToken = "", string SheetId = "" ) {
            if( string.IsNullOrEmpty( OAuthClientId ) )
                throw new ArgumentException( "OAuthCliendId is null." );
            this.OAuthClientId     = OAuthClientId;
            if( string.IsNullOrEmpty( OAuthClientSecret ) )
                throw new ArgumentException( "OAuthClientId is null." );
            this.OAuthClientSecret = OAuthClientSecret;
            if( string.IsNullOrEmpty( RefreshToken ) )
                throw new ArgumentException( "RefreshToken is null." );
            this.RefreshToken      = RefreshToken;
            this.SheetId           = SheetId;
            this.AccessToken       = AccessToken;
        }

        //======================================================================
        // public
        //======================================================================

        public string GetAccessToken() {
            return AccessToken;
        }

        public IEnumerator GetSheetsAPI( string sheetRange ) {
            if( string.IsNullOrEmpty( SheetId ) )
                throw new ArgumentException( "SheetId is null." );

            var checkCo = CheckAccessToken();
            while( checkCo.MoveNext() )
                yield return checkCo.Current;
            if( !IsAccessTokenOK )
                throw new ArgumentException( "GoogleSpreadSheet Config is invalid." );

            var uri = string.Format( URI_SPREADSHEET, SheetId, sheetRange );
            var requestoCo = GetCo( uri );
            while( requestoCo.MoveNext() )
                yield return requestoCo.Current;
            yield return ( string )requestoCo.Current;
        }

        public IEnumerator GetSheetsNameAPI() {
            if( string.IsNullOrEmpty( SheetId ) )
                throw new ArgumentException( "SheetId is null." );

            var checkCo = CheckAccessToken();
            while( checkCo.MoveNext() )
                yield return checkCo.Current;
            if( !IsAccessTokenOK )
                throw new ArgumentException( "GoogleSpreadSheet Config is invalid." );

            var uri = string.Format( URI_SHEETGET, SheetId );
            var requestoCo = GetCo( uri );
            while( requestoCo.MoveNext() )
                yield return requestoCo.Current;
            yield return ( string )requestoCo.Current;
        }

        //======================================================================
        // check accesstoken
        //======================================================================

        /// <summary>
        /// refresh access token if access token is expired.
        /// </summary>
        public IEnumerator CheckAccessToken() {
            WWWForm form = new WWWForm();
            form.AddField( "access_token", AccessToken );
            //form.headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // argument exception
            form.headers[ "Content-Type" ] = "application/x-www-form-urlencoded";

            var requestoCo = PostCo( URI_TOKENINFO, form );
            while( requestoCo.MoveNext() )
                yield return requestoCo.Current;

            var res = ( string )requestoCo.Current;
            if( string.IsNullOrEmpty( res ) ) {
                IsAccessTokenOK = false;
                yield return false;
            } else {
                var jsonObject = JsonMapper.ToObject( res );
                var keys = jsonObject.Keys;
                if( keys.Contains( "error_description" ) ) {
                    var getAccessTokenCo = UpdateAccessToken();
                    while( getAccessTokenCo.MoveNext() )
                        yield return getAccessTokenCo.Current;
                } else {
                    IsAccessTokenOK = true;
                    yield return true;
                }
            }
        }

        public IEnumerator UpdateAccessToken() {
            WWWForm form = new WWWForm();
            form.AddField( "refresh_token", RefreshToken );
            form.AddField( "client_id"    , OAuthClientId );
            form.AddField( "client_secret", OAuthClientSecret );
            form.AddField( "grant_type"   , "refresh_token" );
            //form.headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // argument exception
            form.headers[ "Content-Type" ] = "application/x-www-form-urlencoded";
            var requestoCo = PostCo( URI_REFRESHTOKEN, form );

            while( requestoCo.MoveNext() )
                yield return requestoCo.Current;

            var res = ( string )requestoCo.Current;
            var jsonObject = JsonMapper.ToObject( res );
            var keys = jsonObject.Keys;
            if( keys.Contains( "error_description" ) ) {
                Debug.LogError( "SpreadSheetWebRequest Error: " + jsonObject[ "error_description" ].ToString() );
                IsAccessTokenOK = false;
                yield return false;
            } else {
                AccessToken = jsonObject[ "access_token" ].ToString();
                IsAccessTokenOK = true;
                yield return true;
            }
        }

        //======================================================================
        // process
        //======================================================================

        private IEnumerator PostCo( string uri, WWWForm form ) {
            var request = UnityWebRequest.Post( uri, form );
            request.SendWebRequest();

            while( !request.isDone )
                yield return null;

            if( request.isNetworkError ) {
                Debug.LogError( "SpreadSheetWebRequest Error: " + request.error );
                yield return null;
            } else {
                //Debug.Log( request.downloadHandler.text );
                yield return request.downloadHandler.text;
            }
        }

        private IEnumerator GetCo( string uri ) {
            var request = UnityWebRequest.Get( uri );
            request.SetRequestHeader( "Content-Type", "application/json" );
            request.SetRequestHeader( "Authorization", string.Format( "Bearer {0}", AccessToken ) );
            request.SendWebRequest();

            while( !request.isDone )
                yield return null;

            if( request.isNetworkError ) {
                Debug.LogError( "SpreadSheetWebRequest Error: " + request.error );
                yield return null;
            } else {
                //Debug.Log( request.downloadHandler.text );
                yield return request.downloadHandler.text;
            }
        }

    }
}