using UnityEngine;
using UnityEditor;

using GUIHelper    = charcolle.SpreadSheetConverter.GUIHelper;
using FileUtility  = charcolle.SpreadSheetConverter.FileUtility;
using WindowHelper = charcolle.SpreadSheetConverter.SpreadSheetWindowHelper;

namespace charcolle.SpreadSheetConverter {

    internal class SpreadSheetConvertToolPopupWindow : PopupWindowContent {

        private string newSpreadSheetConfigName = "new spreadsheet config";
        private string newConverterScriptName   = "MyConverter";

        public override void OnGUI( Rect rect ) {
            GUI.skin.label.richText = true;
            GUILayout.Space( 10 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Label( new GUIContent( "Create New SpreadSheet Config".ToBold(), GUIHelper.Textures.ScriptableObjectIcon ), GUILayout.Height( 20 ) );
                newSpreadSheetConfigName = EditorGUILayout.TextField( newSpreadSheetConfigName );
                if( GUILayout.Button( "Create" ) ) {
                    WindowHelper.CreateNewSpreadSheetConfig( newSpreadSheetConfigName );
                    WindowHelper.ConstructData();
                    SpreadSheetConvertToolWindow.win.Initialize();
                    this.editorWindow.Close();
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space( 5 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Label( new GUIContent( "Create New Converter Script".ToBold(), GUIHelper.Textures.ScriptIcon ), GUILayout.Height( 20 ) );
                newConverterScriptName = EditorGUILayout.TextField( newConverterScriptName );
                if( GUILayout.Button( "Create" ) ) {
                    FileUtility.CreateConverterScript( newConverterScriptName );
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space( 10 );
            GUI.skin.label.richText = false;
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 250f, 160f );
        }

    }
}