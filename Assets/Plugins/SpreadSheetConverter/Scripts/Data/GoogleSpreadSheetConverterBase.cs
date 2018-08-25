using System;
using System.Collections;
using UnityEngine;
using charcolle.SpreadSheetConverter;

public abstract class GoogleSpreadSheetConverter : ScriptableObject {

    public string ConverterName;
    public string SheetName;
    public string SheetDesc = "about this converter";
    public string ScriptName;

    public bool MultiSheetConvertMode;
    public Vector2 SheetIndexRange = Vector2.one;
    public int MinSheetIndex = 1;
    public int MaxSheetIndex = 20;

    public Vector2 RowRange = Vector2.one;
    public Vector2 ColRange = Vector2.one;
    public int StartRow     = 1;
    public int StartCol     = 1;
    public int MaxRow       = 20;
    public int MaxCol       = 20;
    public bool UseAutomaticEnd;

    public string SavePath;
    public string SaveFileName      = "hoge";
    public string SaveFileExtension = "asset";

    //======================================================================
    // property
    //======================================================================

    public string CellRange {
        get {
            return SheetName + "!" + StartCell + ":" + EndCell;
        }
    }

    public string StartCell {
        get {
            if( UseAutomaticEnd ) {
                return SpreadSheetAPIUtility.GetRange( StartRow, StartCol );
            } else {
                return SpreadSheetAPIUtility.GetRange( ( int )RowRange.x, ( int )ColRange.x );
            }
        }
    }

    public string EndCell {
        get {
            if( UseAutomaticEnd ) {
                return "ZZ";
            } else {
                return SpreadSheetAPIUtility.GetRange( ( int )RowRange.y, ( int )ColRange.y );
            }
        }
    }

    public int StartSheetIndex {
        get {
            return ( int )SheetIndexRange.x;
        }
    }

    public int EndSheetIndex {
        get {
            return ( int )SheetIndexRange.y;
        }
    }


    //======================================================================
    // abstract
    //======================================================================

    protected abstract void Convert( SpreadSheetAPIClass data );

    //======================================================================
    // spread sheet process
    //======================================================================

    public IEnumerator DoProcess( GoogleSpreadSheetWebRequest request ) {
        if( MultiSheetConvertMode ) {
            var list = new SpreadSheetAPIClass[ EndSheetIndex - StartSheetIndex + 1 ];
            for( int i = StartSheetIndex; i <= EndSheetIndex; i++ ) {
                var requestCo = request.GetSheetsAPI( CellRange.Replace( "#NUM#", i.ToString() ) );
                while( requestCo.MoveNext() )
                    yield return requestCo.Current;
                var res = ( string )requestCo.Current;

                list[ i - 1 ] = new SpreadSheetAPIClass( res, SheetName );
            }
            Receive( list );
        } else {
            var requestCo = request.GetSheetsAPI( CellRange );
            while( requestCo.MoveNext() )
                yield return requestCo.Current;
            var res = ( string )requestCo.Current;

            var apiData = new SpreadSheetAPIClass( res, SheetName );
            Receive( apiData );
        }
    }

    public void Receive( SpreadSheetAPIClass data ) {
        try {
            if( data != null )
                Convert( data );
        }
        catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    public void Receive( SpreadSheetAPIClass[] data ) {
        try {
            if( data != null ) {
                for( int i = 0; i < data.Length; i++ )
                    Convert( data[ i ] );
            }
        }
        catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    //======================================================================
    // save process
    //======================================================================

    protected virtual void Save( ScriptableObject data, string saveFileName = "" ) {
        try {
#if UNITY_EDITOR
            if( string.IsNullOrEmpty( saveFileName ) )
                saveFileName = SaveFileNameWithExtension;
            SpreadSheetAPIUtility.SaveData( data, SavePath, saveFileName );
#else
            throw new NotSupportedException( "You should implement Save method by yourself." );
#endif
        }
        catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    protected virtual void Save( string data, string saveFileName = "" ) {
        try {
#if UNITY_EDITOR
            if( string.IsNullOrEmpty( saveFileName ) )
                saveFileName = SaveFileNameWithExtension;
            SpreadSheetAPIUtility.SaveData( data, SavePath, saveFileName );
#else
            throw new NotSupportedException( "You should implement Save method by yourself." );
#endif
        }
        catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    public string SaveFileNameWithExtension {
        get {
            return SaveFileName + "." + SaveFileExtension;
        }
    }

    public string GetSaveFileNameWithExtension( string fileName ) {
        return fileName + "." + SaveFileExtension;

    }

}