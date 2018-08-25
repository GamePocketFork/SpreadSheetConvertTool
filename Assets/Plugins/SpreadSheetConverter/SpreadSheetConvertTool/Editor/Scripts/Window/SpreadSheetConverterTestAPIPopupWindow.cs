using UnityEngine;
using UnityEditor;

namespace charcolle.SpreadSheetConverter {

    internal class SpreadSheetConverterTestAPIPopupWindow : PopupWindowContent {

        private Vector2 scrollView = Vector2.zero;
        private string Response;
        private string SheetName;

        public void Initialize( string res, string sheetName ) {
            Response  = res;
            SheetName = sheetName;
        }

        public override void OnGUI( Rect rect ) {
            EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
            {
                GUILayout.Label( SheetName );
            }
            EditorGUILayout.EndHorizontal();

            scrollView = EditorGUILayout.BeginScrollView( scrollView );
            {
                GUILayout.TextArea( Response );
            }
            EditorGUILayout.EndScrollView();
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 400f, 250f );
        }

    }

}