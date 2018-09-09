using System;
using LitJson;

namespace charcolle.SpreadSheetConverter {

    public class SpreadSheetAPIClass {

        public string SheetName;
        public string[][] Data;
        public int RowCount;
        public int ColCount;

        private readonly static string KEY_VALUES = "values";
        private readonly static string KEY_ERROR  = "error_description";

        public SpreadSheetAPIClass( string res, string sheetName ) {
            SheetName = sheetName;
            var jsonObject = JsonMapper.ToObject( res );

            if( !checkAPIAvailable( jsonObject ) )
                throw new Exception( "Cannot get sheet data." );

            if( !checkSheetHasValue( jsonObject ) )
                throw new Exception( "The sheet has no data." );

            RowCount = jsonObject[ KEY_VALUES ].Count;
            ColCount = jsonObject[ KEY_VALUES ][ 0 ].Count;
            Data = new string[ RowCount ][];

            var valueObject = jsonObject[ KEY_VALUES ];
            for( int i = 0; i < RowCount; i++ ) {
                var row = new string[ ColCount ];
                var rowObject = valueObject[ i ];
                for( int j = 0; j < ColCount; j++ )
                    row[ j ] = rowObject[ j ].ToString();

                Data[ i ] = row;
            }
        }

        /// <summary>
        /// check API Error
        /// </summary>
        private bool checkAPIAvailable( JsonData data ) {
            if( data == null || data.Keys.Contains( KEY_ERROR ) )
                return false;
            return true;
        }

        /// <summary>
        /// check if the sheet has data or not.
        /// </summary>
        private bool checkSheetHasValue( JsonData data ) {
            if( !data.Keys.Contains( KEY_VALUES ) || data[ KEY_VALUES ].Count == 0 )
                return false;
            return true;
        }

    }
}