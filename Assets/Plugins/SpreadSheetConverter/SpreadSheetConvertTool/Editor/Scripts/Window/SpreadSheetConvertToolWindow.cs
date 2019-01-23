using System;
using UnityEditor;
using UnityEngine;

using WebRequest       = charcolle.SpreadSheetConverter.GoogleSpreadSheetWebRequest;
using GUIHelper        = charcolle.SpreadSheetConverter.GUIHelper;
using UndoHelper       = charcolle.SpreadSheetConverter.UndoHelper;
using WindowHelper     = charcolle.SpreadSheetConverter.SpreadSheetWindowHelper;
using APIUtility       = charcolle.SpreadSheetConverter.EditorAPIUtility;
using ConverterUtility = charcolle.SpreadSheetConverter.ConverterEditorUtility;
using FileUtility      = charcolle.SpreadSheetConverter.FileUtility;

namespace charcolle.SpreadSheetConverter {

    internal class SpreadSheetConvertToolWindow : EditorWindow {

        public static SpreadSheetConvertToolWindow win;
        private static bool IsInitialized;
        private SpreadSheetConvertToolPopupWindow popupWin;
        private SpreadSheetConverterTestAPIPopupWindow textAPIWin;

        [SerializeField]
        private GoogleAPIOAuthConfigEditorGUI OAuthConfigItem;
        [SerializeField]
        private GoogleSpreadSheetConfigEditorGUI SpreadSheetConfigItem;
        [SerializeField]
        private GoogleSpreadSheetConverterEditorGUI ConverterItem;

        //======================================================================
        // initialize
        //======================================================================

        [MenuItem( "Window/SpreadSheetConvertTool" )]
        private static void Open() {
            IsInitialized = false;
            win               = GetWindow<SpreadSheetConvertToolWindow>();
            win.titleContent.text = WindowHelper.WINDOW_TITLE;
            win.minSize           = WindowHelper.WINDOW_SIZE;
            win.Show();
        }

        private void OnEnable() {
            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        public void Initialize() {
            popupWin   = new SpreadSheetConvertToolPopupWindow();
            textAPIWin = new SpreadSheetConverterTestAPIPopupWindow();

            ConverterUtility.GetConverterSubClass();
            WindowHelper.Initialize( position );

            if( OAuthConfigItem == null )
                OAuthConfigItem = new GoogleAPIOAuthConfigEditorGUI( GoogleAPIOAuthEditorConfig.Instance );
            if( SpreadSheetConfigItem == null ) {
                SpreadSheetConfigItem = new GoogleSpreadSheetConfigEditorGUI( SelectedSpreadSheet );
                OnSelectedSpreadSheetChange();
            }

            Repaint();

            IsInitialized = true;
        }

        private void OnGUI() {
            if( GoogleAPIOAuthEditorConfig.Instance == null ) {
                EditorGUILayout.HelpBox( "fatal error.", MessageType.Error );
                return;
            }

            if( !IsInitialized )
                Initialize();

            EditorGUI.BeginChangeCheck();

            WindowHelper.OnGUIStart();
            Draw();
            WindowHelper.OnGUIEnd();

            if( EditorGUI.EndChangeCheck() ) {
                if( GoogleAPIOAuthEditorConfig.Instance != null )
                    EditorUtility.SetDirty( GoogleAPIOAuthEditorConfig.Instance );
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
                    UndoHelper.WindowUndo( this, UndoHelper.UNDO_WIN_CHANGE );
                    EditorGUIUtility.keyboardControl = 0;
                    selectedMenu = selected;
                }
                switch( selectedMenu ) {
                    case 0:
                        SheetContents();
                        break;
                    case 1:
                        if( OAuthConfigItem != null )
                            OAuthConfigItem.OnGUI();
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RightSide() {
            if( SelectedSpreadSheet == null || SpreadSheetConfigItem == null )
                return;

            var selected = SpreadSheetConfigItem.SelectedCoverter;
            if( selected != selectedConverter ) {
                selectedConverter = selected;
                OnSelectedConverterChange();
            }

            if( SelectedConverter == null )
                return;

            EditorGUILayout.BeginVertical();
            {
                //SelectedConverter.Draw();
                ConverterItem.OnGUI();

                if( ConverterItem.IsContextClick ) {
                    var menu = new GenericMenu();
                    menu.AddItem( new GUIContent( "Edit" ), false, () => {
                        ConverterItem.IsEdit = true;
                    } );
                    menu.AddItem( new GUIContent( "Delete Converter" ), false, () => {
                        if( EditorUtility.DisplayDialog( "Delete " + SelectedConverter.ConverterName, " You cannot undo this. ", "ok", "cancel" ) ) {
                            var converter = SelectedConverter;
                            SelectedSpreadSheet.Converter.Remove( converter );
                            DestroyImmediate( converter, true );
                            AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( SelectedSpreadSheet ), ImportAssetOptions.ForceUpdate );
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    } );
                    menu.ShowAsContext();
                }

                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.Footer );
                {
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( "API Test", new GUILayoutOption[] { GUILayout.Width( 100 ), GUILayout.Height( 30 ) } ) ) {
                        //EditorUtility.DisplayProgressBar( "Conecting to SpreadSheet...", "", 0 );
                        //try {
                            OnEditorTestAPIProcess();
                        //} catch (System.Exception ex){
                            //Debug.LogError( ex );
                        //}
                        //EditorUtility.ClearProgressBar();
                    }
                    GUI.backgroundColor = Color.cyan;
                    if( GUILayout.Button( "Convert", new GUILayoutOption[] { GUILayout.Width( 150 ), GUILayout.Height( 30 ) } ) ) {
                        OnEditorConvertProcess();
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
            if( SpreadSheetConfigItem == null )
                return;

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
                            selectedSpreadSheet = select;
                            OnSelectedSpreadSheetChange();
                        }
                        GUI.backgroundColor = Color.green;
                        if( GUILayout.Button( "+", EditorStyles.toolbarButton, GUILayout.Width( 35 ) ) ) {
                            EditorGUIUtility.keyboardControl = 0;
                            PopupWindow.Show( GUILayoutUtility.GetLastRect(), popupWin );
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 10 );

                    SpreadSheetConfigItem.OnGUI();

                    if( SpreadSheetConfigItem.IsContextClick ) {
                        var menu = new GenericMenu();
                        menu.AddItem( new GUIContent( "Delete " + SelectedSpreadSheet.Name ), false, () => {
                            if( EditorUtility.DisplayDialog( "Delete " + SelectedSpreadSheet.Name, " You cannot undo this. ", "ok", "cancel" ) ) {
                                AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( SelectedSpreadSheet ) );
                                AssetDatabase.Refresh();
                                WindowHelper.ConstructData();
                                selectedConverter   = 0;
                                selectedSpreadSheet = 0;
                                OnSelectedSpreadSheetChange();
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
                            if( GUILayout.Button( "Refresh", EditorStyles.toolbarButton, GUILayout.Width( 80 ) ) ) {
                                SpreadSheetConfigItem.SelectedCoverter = -1;
                                SpreadSheetConfigItem.Refresh();
                                EditorGUIUtility.keyboardControl = 0;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region actions
        private void OnSelectedSpreadSheetChange() {
            UndoHelper.WindowUndo( this, UndoHelper.UNDO_WIN_CHANGE );
            SpreadSheetConfigItem = new GoogleSpreadSheetConfigEditorGUI( SelectedSpreadSheet );
            selectedConverter = 0;
            EditorGUIUtility.keyboardControl = 0;
        }

        private void OnSelectedConverterChange() {
            UndoHelper.WindowUndo( this, UndoHelper.UNDO_WIN_CHANGE );
            EditorGUIUtility.keyboardControl = 0;
            if( SelectedConverter != null )
                ConverterItem = new GoogleSpreadSheetConverterEditorGUI( SelectedConverter );
        }

        private void OnEditorConvertProcess() {
            EditorUtility.DisplayProgressBar( "Conecting to SpreadSheet...", "", 0 );
            try {
                var requestCo = SelectedConverter.DoProcess( SelectedSpreadSheet.GetWebRequest() );
                while( requestCo.MoveNext() ) { }
            } catch( Exception ex ) {
                Debug.LogError( ex );
            }
            EditorUtility.ClearProgressBar();
        }

        private void OnEditorTestAPIProcess() {
            var res = "";
            var cellRange = SelectedConverter.MultiSheetConvertMode ? SelectedConverter.CellRange.Replace( "#NUM#", SelectedConverter.StartSheetIndex.ToString() ) : SelectedConverter.CellRange;

            var webRequest = SelectedSpreadSheet.GetWebRequest();
            var requestCo = webRequest.GetSheetsAPI( cellRange );
            while( requestCo.MoveNext() ) { }
            res = ( string )requestCo.Current;

            textAPIWin.Initialize( res, cellRange );
            PopupWindow.Show( Rect.zero, textAPIWin );
        }

        #endregion

        #region utilities

        //======================================================================
        // utilities
        //======================================================================

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