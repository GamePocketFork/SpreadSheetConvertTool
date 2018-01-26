using UnityEngine;
using UnityEditor;

using WindowHelper = charcolle.Utility.SpreadSheetConvertTool.SpreadSheetWindowHelper;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public class SpreadSheetConvertToolPopupWindow : PopupWindowContent {

        private string newSpreadSheetConfigName = "new spreadsheet config";

        public override void OnGUI( Rect rect ) {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label( "Create New SpreadSheet Config" );
                newSpreadSheetConfigName = EditorGUILayout.TextField( newSpreadSheetConfigName );
                if( GUILayout.Button( "Create" ) ) {
                    WindowHelper.CreateNewSpreadSheetConfig( newSpreadSheetConfigName );
                    WindowHelper.ConstructData();
                    this.editorWindow.Close();
                }
            }
            EditorGUILayout.EndVertical();
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 250f, 70f );
        }

    }
}