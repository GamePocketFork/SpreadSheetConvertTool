using UnityEditor;
using UnityEngine;

using WebRequest       = charcolle.Utility.SpreadSheetConvertTool.GoogleSpreadSheetWebRequest;
using GUIHelper        = charcolle.Utility.SpreadSheetConvertTool.GUIHelper;
using UndoHelper       = charcolle.Utility.SpreadSheetConvertTool.UndoHelper;
using WindowHelper     = charcolle.Utility.SpreadSheetConvertTool.SpreadSheetWindowHelper;
using APIUtility       = charcolle.Utility.SpreadSheetConvertTool.APIUtility;
using ConverterUtility = charcolle.Utility.SpreadSheetConvertTool.ConverterUtility;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public class SpreadSheetConvertToolWindow : EditorWindow {

        private static SpreadSheetConvertToolWindow win;
        private static SpreadSheetConvertToolPopupWindow popupWin;

        //======================================================================
        // initialize
        //======================================================================

        [MenuItem( "Window/SpreadSheetConvertTool" )]
        private static void Open() {
            win = GetWindow<SpreadSheetConvertToolWindow>();
            win.titleContent.text = WindowHelper.WINDOW_TITLE;
            win.minSize = WindowHelper.WINDOW_SIZE;

            Initialize();

            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        private static void Initialize() {
            popupWin = new SpreadSheetConvertToolPopupWindow();
            ConverterUtility.GetConverterSubClass();
            WindowHelper.Initialize( win.position );

            if( win != null )
                win.Repaint();
        }

        private void OnGUI() {
            if( win == null )
                Open();

            if( GoogleAPIOAuthConfig.Instance == null ) {
                EditorGUILayout.HelpBox( "fatal error.", MessageType.Error );
                return;
            }

            EditorGUI.BeginChangeCheck();
            WindowHelper.OnGUIStart();
            Draw();
            WindowHelper.OnGUIEnd();

            if( EditorGUI.EndChangeCheck() ) {
                if( GoogleAPIOAuthConfig.Instance != null )
                    EditorUtility.SetDirty( GoogleAPIOAuthConfig.Instance );
                if( SelectedConverter != null )
                    EditorUtility.SetDirty( SelectedConverter );
                if( SelectedSpreadSheet != null )
                    EditorUtility.SetDirty( SelectedSpreadSheet );
            }
        }

        #region drawer

        //======================================================================
        // window content
        //======================================================================

        private int selectedMenu;
        private void Draw() {
            SplitterGUI.BeginHorizontalSplit( WindowHelper.HorizontalState );
            {
                LightSide();
                RightSide();
            }
            SplitterGUI.EndHorizontalSplit();
        }

        private void LightSide() {
            EditorGUILayout.BeginVertical( GUIHelper.Styles.LeftArea );
            {
                var selected = GUILayout.Toolbar( selectedMenu, WindowHelper.MENU_WINDOW, EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) );
                if( selected != selectedMenu ) {
                    UndoHelper.WindowUndo( win, UndoHelper.UNDO_WIN_CHANGE );
                    EditorGUIUtility.keyboardControl = 0;
                }
                selectedMenu = selected;
                switch( selectedMenu ) {
                    case 0:
                        SheetContents();
                        break;
                    case 1:
                        GoogleAPIOAuthConfig.Instance.Draw();
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RightSide() {
            if( SelectedSpreadSheet == null )
                return;

            selectedConverter = SelectedSpreadSheet.SelectedCoverter;
            if( SelectedConverter == null )
                return;

            EditorGUILayout.BeginVertical();
            {

                SelectedConverter.Draw();

                if( SelectedConverter.IsContextClick ) {
                    var menu = new GenericMenu();
                    menu.AddItem( new GUIContent( "Edit" ), false, () => {
                        SelectedConverter.IsEdit = true;
                    } );
                    menu.AddItem( new GUIContent( "Delete Converter" ), false, () => {
                        UndoHelper.SpreadSheetUndo( SelectedSpreadSheet, UndoHelper.UNDO_SS_DELETE_CONVERTER );
                        SelectedSpreadSheet.Converter.Remove( SelectedConverter );
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    } );
                    menu.ShowAsContext();
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.Footer );
                {
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( "API Test", new GUILayoutOption[] { GUILayout.Width( 100 ), GUILayout.Height( 30 ) } ) ) {
                        EditorUtility.DisplayProgressBar( "Conecting to SpreadSheet...", "", 0 );
                        APIUtility.UpdateAccessToken();
                        SelectedConverter.TestAPIResponse = WebRequest.GetSheetsAPI( SelectedSpreadSheet.Id, SelectedConverter.Range );
                        EditorUtility.ClearProgressBar();
                    }
                    GUI.backgroundColor = Color.cyan;
                    if( GUILayout.Button( "Convert", new GUILayoutOption[] { GUILayout.Width( 150 ), GUILayout.Height( 30 ) } ) ) {
                        EditorUtility.DisplayProgressBar( "Conecting to SpreadSheet...", "", 0 );
                        APIUtility.UpdateAccessToken();
                        var apiData = APIUtility.GetSpreadSheetData( SelectedSpreadSheet, SelectedConverter );
                        SelectedConverter.Receive( apiData );
                        EditorUtility.ClearProgressBar();
                        EditorGUIUtility.keyboardControl = 0;
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private int selectedSpreadSheet = 0;
        private int selectedConverter   = 0;
        private void SheetContents() {
            EditorGUILayout.BeginVertical();
            {
                if( !WindowHelper.IsSpreadSheetDataExists ) {

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUI.backgroundColor = Color.green;
                        if( GUILayout.Button( "+", EditorStyles.toolbarButton, GUILayout.Width( 35 ) ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            PopupWindow.Show( GUILayoutUtility.GetLastRect(), popupWin );
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox( "no spreadsheets", MessageType.Warning );
                } else {

                    EditorGUILayout.BeginHorizontal();
                    {
                        var select = EditorGUILayout.Popup( selectedSpreadSheet, WindowHelper.SpreadSheetList, EditorStyles.toolbarDropDown );
                        if( select != selectedSpreadSheet ) {
                            UndoHelper.WindowUndo( win, UndoHelper.UNDO_WIN_CHANGE );
                            WindowHelper.SpreadSheets[ selectedSpreadSheet ].Initialize();
                            selectedConverter = 0;
                            EditorGUIUtility.keyboardControl = 0;
                        }
                        selectedSpreadSheet = select;
                        GUI.backgroundColor = Color.green;
                        if( GUILayout.Button( "+", EditorStyles.toolbarButton, GUILayout.Width( 35 ) ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            PopupWindow.Show( GUILayoutUtility.GetLastRect(), popupWin );
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 10 );

                    SelectedSpreadSheet.Draw();

                    if( SelectedSpreadSheet.IsContextClick ) {
                        var menu = new GenericMenu();
                        menu.AddItem( new GUIContent( "Delete " + SelectedSpreadSheet.Name ), false, () => {
                            if( EditorUtility.DisplayDialog( "Delete " + SelectedSpreadSheet.Name, " You cannot undo this. ", "ok", "cancel" ) ) {
                                AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( SelectedSpreadSheet ) );
                                AssetDatabase.Refresh();
                                WindowHelper.ConstructData();
                                selectedConverter = 0;
                                selectedSpreadSheet = 0;
                            }
                        } );
                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
                        {
                            GUILayout.FlexibleSpace();
                            if( GUILayout.Button( "Refresh", EditorStyles.toolbarButton, GUILayout.Width( 60 ) ) ) {
                                SelectedSpreadSheet.SelectedCoverter = -1;
                                EditorGUIUtility.keyboardControl = 0;
                            }
                            GUI.backgroundColor = Color.yellow;
                            if( GUILayout.Button( "new Converter Script", EditorStyles.toolbarButton, GUILayout.Width( 120 ) ) ) {
                                FileUtility.CreateConverterScript();
                                EditorGUIUtility.keyboardControl = 0;
                            }
                            GUI.backgroundColor = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                }
            }
            EditorGUILayout.EndVertical();
        }

        private GoogleSpreadSheetConfig SelectedSpreadSheet {
            get {
                if( WindowHelper.CheckSpreadSheetIdx( selectedSpreadSheet ) )
                    return WindowHelper.SpreadSheets[ selectedSpreadSheet ];
                selectedSpreadSheet = 0;
                return null;
            }
        }

        private GoogleSpreadSheetConverter SelectedConverter {
            get {
                if( selectedConverter == -1 || selectedConverter >= WindowHelper.SpreadSheets[ selectedSpreadSheet ].Converter.Count ) {
                    return null;
                }
                return WindowHelper.SpreadSheets[ selectedSpreadSheet ].Converter[ selectedConverter ];
            }
        }

        #endregion

    }
}