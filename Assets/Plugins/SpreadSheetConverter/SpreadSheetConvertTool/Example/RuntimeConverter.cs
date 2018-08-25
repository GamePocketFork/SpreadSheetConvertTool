using System;
using System.Text;
using charcolle.SpreadSheetConverter;

public class RuntimeConverter : GoogleSpreadSheetConverter {

    public Action<string> OnFinish;

    protected override void Convert( SpreadSheetAPIClass data ) {

        StringBuilder sb = new StringBuilder();
        for( int i = 0; i < data.RowCount; i++ ) {
            for( int j = 0; j < data.ColCount; j++ ) {
                sb.Append( data.Data[ i ][ j ] );
                if( j + 1 < data.ColCount )
                    sb.Append( "," );
            }
            sb.Append( "\n" );
        }

        OnFinish( sb.ToString() );

    }

}
