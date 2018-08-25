using System.Text;
using charcolle.SpreadSheetConverter;

public class CSVConverter : GoogleSpreadSheetConverter {

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

        // You can save your data with this method.
        Save( sb.ToString() );

    }

}
