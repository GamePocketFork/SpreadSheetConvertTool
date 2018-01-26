using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ConverterUtility = charcolle.Utility.SpreadSheetConvertTool.ConverterUtility;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public class SpreadSheetConvertToolConverterPopupWindow : PopupWindowContent {

        private string newConverterName = "New Sheet Converter";
        private int selectedType;
        private GoogleSpreadSheetConfig parentConfig;

        public void Initialize( GoogleSpreadSheetConfig parent ) {
            selectedType = 0;
            parentConfig = parent;
            ConverterUtility.GetConverterSubClass();
        }

        public override void OnGUI( Rect rect ) {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label( "Create New Converter Config" );

                GUILayout.Space( 5 );

                newConverterName = EditorGUILayout.TextField( newConverterName );
                selectedType = EditorGUILayout.Popup( selectedType, ConverterUtility.ConverterMenu );

                GUILayout.Space( 5 );

                if( GUILayout.Button( "Create" ) ) {
                    UndoHelper.SpreadSheetUndo( parentConfig, UndoHelper.UNDO_SS_CREATE_CONVERTER );
                    ConverterUtility.CreateConverter( selectedType, newConverterName, parentConfig );
                    this.editorWindow.Close();
                }
            }
            EditorGUILayout.EndVertical();
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 250f, 100f );
        }

    }

}