using UnityEngine;
using System.Collections;

namespace charcolle.SpreadSheetConverter {

    public class SpreadSheetConvertRuntime : MonoBehaviour {

        public GoogleSpreadSheetConfig SpreadSheet;
        public string ConverterName = "RuntimeConverter";

        IEnumerator Start() {

            var converter = SpreadSheet.GetConverter<RuntimeConverter>( ConverterName );
            if( converter == null )
                yield break;

            converter.OnFinish += ( result ) => { Debug.Log( result ); };
            var request   = SpreadSheet.GetWebRequest();
            yield return StartCoroutine( converter.DoProcess( request ) );

        }

    }

}