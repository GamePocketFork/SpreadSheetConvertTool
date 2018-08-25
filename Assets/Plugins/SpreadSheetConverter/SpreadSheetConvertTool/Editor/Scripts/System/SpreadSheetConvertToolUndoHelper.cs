using UnityEditor;

namespace charcolle.SpreadSheetConverter {

    internal class UndoHelper {

        public readonly static string UNDO_WIN_CHANGE = "window change";

        public readonly static string UNDO_SS_CHANGE = "change spreadsheet";
        public readonly static string UNDO_SS_EDIT = "edit spreadsheet config";
        public readonly static string UNDO_SS_CREATE_CONVERTER = "create converter";
        public readonly static string UNDO_SS_DELETE_CONVERTER = "delete converter";

        public readonly static string UNDO_CONVERTER_EDIT = "edit converter";

        public static void ConverterUndo( GoogleSpreadSheetConverter converter, string text ) {
            if( converter != null )
                Undo.RecordObject( converter, text );
        }

        public static void SpreadSheetUndo( GoogleSpreadSheetConfig spreadSheet, string text ) {
            if( spreadSheet != null )
                Undo.RecordObject( spreadSheet, text );
        }

        public static void WindowUndo( SpreadSheetConvertToolWindow win, string text ) {
            if( win != null )
                Undo.RecordObject( win, text );
        }

    }

}