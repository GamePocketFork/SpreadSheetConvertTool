using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using charcolle.SpreadSheetConverter;

public class GoogleSpreadSheetConfig : ScriptableObject {

    public string Name;
    public string Id;

    public bool UseDefaultAccessToken = true;
    public string OAuthClientId       = "";
    public string OAuthClientSecret   = "";
    public string RefreshToken        = "";
    public string AccessToken         = "";
    public string[] Sheets = new string[] { };

    public List<GoogleSpreadSheetConverter> Converter = new List<GoogleSpreadSheetConverter>();

    //======================================================================
    // convert process
    //======================================================================

    public GoogleSpreadSheetWebRequest GetWebRequest() {
        return new GoogleSpreadSheetWebRequest( OAuthClientId, OAuthClientSecret, RefreshToken, AccessToken, Id );
    }

    //======================================================================
    // get converter
    //======================================================================

    public GoogleSpreadSheetConverter GetConverter( int idx ) {
        if( idx >= Converter.Count )
            return null;
        return Converter[ idx ];
    }

    public GoogleSpreadSheetConverter GetConverter( string converterName ) {
        return Converter.FirstOrDefault( c => c.ConverterName.Equals( converterName ) );
    }

    public T GetConverter<T>( int idx ) where T : GoogleSpreadSheetConverter {
        if( idx >= Converter.Count )
            return null;
        return (T)Converter[ idx ];
    }

    public T GetConverter<T>( string converterName ) where T : GoogleSpreadSheetConverter {
        var converter = Converter.FirstOrDefault( c => c.ConverterName.Equals( converterName ) );
        return (T)converter;
    }

}