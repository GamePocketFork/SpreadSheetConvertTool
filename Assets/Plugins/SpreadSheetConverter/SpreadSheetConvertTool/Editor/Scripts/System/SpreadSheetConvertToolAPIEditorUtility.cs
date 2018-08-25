using LitJson;
using UnityEngine;
using System.Collections.Generic;

using WebRequest = charcolle.SpreadSheetConverter.GoogleSpreadSheetWebRequest;

namespace charcolle.SpreadSheetConverter {

    internal static class EditorAPIUtility {

        public static string[] GetSheetName( WebRequest webRequest ) {
            var requestCo = webRequest.GetSheetsNameAPI();
            while( requestCo.MoveNext() ) { }
            var res = ( string )requestCo.Current;

            var jsonObject = JsonMapper.ToObject( res );
            if( jsonObject.Keys.Contains( "error_description" ) || jsonObject[ "sheets" ].Count == 0 )
                return null;

            var sheets = new List<string>();
            try {
                for( int i = 0; i < jsonObject[ "sheets" ].Count; i++ ) {
                    var propertyObj = jsonObject[ "sheets" ][ i ][ "properties" ];
                    sheets.Add( propertyObj[ "title" ].ToString() );
                    //Debug.Log( propertyObj[ "title" ].ToString() );
                }
            } catch( System.Exception ex ) {
                Debug.LogError( "SpreadSheetConverterError: " + ex );
            }

            return sheets.ToArray();
        }

    }
}