using LitJson;
using UnityEngine;

using WebRequest = charcolle.Utility.SpreadSheetConvertTool.GoogleSpreadSheetWebRequest;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public static class APIUtility {
        //======================================================================
        // public
        //======================================================================

        /// <summary>
        /// refresh access token if access token is expired,
        /// </summary>
        public static bool UpdateAccessToken() {
            if( !CheckAccessTokenAvailable() ) {
                var res = WebRequest.GetAccessToken();
                if( string.IsNullOrEmpty( res ) )
                    return false;

                var jsonObject = JsonMapper.ToObject( res );
                var keys = jsonObject.Keys;
                if( keys.Contains( "error_description" ) ) {
                    Debug.LogError( "SpreadSheetConvertTool Error: " + jsonObject[ "error_description" ].ToString() );
                    return false;
                } else {
                    GoogleAPIOAuthConfig.Instance.AccessToken = jsonObject[ "access_token" ].ToString();
                    return true;
                }
            } else {
                return true;
            }
        }

        public static string GetRange( int row, int col ) {
            return row.ToAlphabet() + col.ToString();
        }

        public static SpreadSheetAPIClass GetSpreadSheetData( GoogleSpreadSheetConfig spreadSheet, GoogleSpreadSheetConverter converter ) {
            if( spreadSheet == null || converter == null ) {
                Debug.LogError( "fatal error." );
                return null;
            }

            var res = WebRequest.GetSheetsAPI( spreadSheet.Id, converter.Range );
            if( string.IsNullOrEmpty( res ) )
                return null;

            var jsonObject = JsonMapper.ToObject( res );
            var keys = jsonObject.Keys;
            if( keys.Contains( "error" ) ) {
                Debug.LogError( "SpreadSheetConvertTool Error: " + jsonObject[ "error" ][ "message" ].ToString() );
                return null;
            } else {
                return new SpreadSheetAPIClass( res );
            }
        }

        //======================================================================
        // process
        //======================================================================

        private static bool CheckAccessTokenAvailable() {
            var res = WebRequest.CheckAccessToken();
            if( string.IsNullOrEmpty( res ) ) {
                return false;
            } else {
                var jsonObject = JsonMapper.ToObject( res );
                var keys = jsonObject.Keys;
                if( keys.Contains( "error_description" ) ) {
                    return false;
                } else {
                    return true;
                }
            }
        }

        /// <summary>
        /// http://zecl.hatenablog.com/entry/20080813/p1
        /// </summary>
        private static string ToAlphabet( this int self ) {
            if( self <= 0 )
                return "";
            int n = self % 26;
            n = ( n == 0 ) ? 26 : n;
            string s = ( ( char )( n + 64 ) ).ToString();
            if( self == n )
                return s;
            return ( ( self - n ) / 26 ).ToAlphabet() + s;
        }
    }
}