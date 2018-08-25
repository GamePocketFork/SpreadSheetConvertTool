using UnityEngine;
using UnityEditor;

using GUIHelper        = charcolle.SpreadSheetConverter.GUIHelper;
using FileUtility      = charcolle.SpreadSheetConverter.FileUtility;
using ConverterUtility = charcolle.SpreadSheetConverter.ConverterEditorUtility;

namespace charcolle.SpreadSheetConverter {

    internal class SpreadSheetConvertToolConverterPopupWindow : PopupWindowContent {

        private string newConverterName = "New Sheet Converter";
        private int selectedType;
        private GoogleSpreadSheetConfig parentConfig;

        public void Initialize( GoogleSpreadSheetConfig parent ) {
            selectedType = 0;
            parentConfig = parent;
            ConverterEditorUtility.GetConverterSubClass();
        }

        public override void OnGUI( Rect rect ) {
            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Label( new GUIContent( "Create New Converter Config", GUIHelper.Textures.ScriptableObjectIcon ), GUILayout.Height( 20 ) );

                GUILayout.Space( 5 );

                selectedType = EditorGUILayout.Popup( selectedType, ConverterEditorUtility.ConverterMenu );

                newConverterName = EditorGUILayout.TextField( newConverterName );

                GUILayout.Space( 5 );

                if( GUILayout.Button( "Create" ) ) {
                    UndoHelper.SpreadSheetUndo( parentConfig, UndoHelper.UNDO_SS_CREATE_CONVERTER );
                    ConverterEditorUtility.CreateConverter( selectedType, newConverterName, parentConfig );
                    this.editorWindow.Close();
                }
            }
            EditorGUILayout.EndVertical();
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 250f, 105f );
        }

    }

}