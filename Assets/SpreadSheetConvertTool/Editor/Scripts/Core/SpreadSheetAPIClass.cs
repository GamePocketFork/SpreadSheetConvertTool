using LitJson;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public class SpreadSheetAPIClass {

        public string[][] Data;
        public int RowCount;
        public int ColCount;

        private readonly static string KEY_VALUES = "values";
        private readonly static string KEY_ERROR = "error_description";

        public SpreadSheetAPIClass( string res ) {
            var jsonObject = JsonMapper.ToObject( res );

            if( checkAPIAvailable( jsonObject ) ) {
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
        }

        private bool checkAPIAvailable( JsonData data ) {
            if( data == null || data.Keys.Contains( KEY_ERROR ) )
                return false;
            if( data[ KEY_VALUES ].Count == 0 )
                return false;
            return true;
        }
    }
}