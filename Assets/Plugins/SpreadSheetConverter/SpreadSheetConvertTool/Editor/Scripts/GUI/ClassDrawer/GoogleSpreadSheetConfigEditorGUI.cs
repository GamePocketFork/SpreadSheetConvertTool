using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UndoHelper       = charcolle.SpreadSheetConverter.UndoHelper;
using GUIHelper        = charcolle.SpreadSheetConverter.GUIHelper;
using ConverterUtility = charcolle.SpreadSheetConverter.ConverterEditorUtility;

namespace charcolle.SpreadSheetConverter {

    internal class GoogleSpreadSheetConfigEditorGUI : EditorWindowItem<GoogleSpreadSheetConfig> {

        public GoogleSpreadSheetConfigEditorGUI( GoogleSpreadSheetConfig data ) : base( data ) {
            SelectedCoverter = -1;
        }

        /// <summary>
        /// Update AccessToken and check Sheets.
        /// </summary>
        public void Refresh() {
            if( data == null )
                return;

            if( string.IsNullOrEmpty( OAuthClientId ) || string.IsNullOrEmpty( OAuthClientSecret ) || string.IsNullOrEmpty( RefreshToken ) || string.IsNullOrEmpty( Id ) )
                return;
            EditorUtility.DisplayProgressBar( "Refreshing SpreadSheet...", "", 0 );
            try{
                var webRequest = data.GetWebRequest();
                Sheets = EditorAPIUtility.GetSheetName( webRequest );
                AccessToken = webRequest.GetAccessToken();
            }catch( System.Exception ex ) {
                Debug.LogError( "GoogleSpreadSheetConfig Error: " + ex );
            }
            EditorUtility.ClearProgressBar();
        }

        protected override void Draw() {
            var rect = EditorGUILayout.BeginVertical();
            {
                Header();
                DrawConverterList();
                DrawSheetName();
            }
            EditorGUILayout.EndVertical();

            EventProcess( Event.current, rect );
        }

        //======================================================================
        // contents
        //======================================================================

        public bool IsContextClick;
        public int SelectedCoverter = -1;
        private Vector2 converterListView;
        private void DrawConverterList() {
            if( ConverterEditorUtility.ConverterTypeMenu == null || ConverterEditorUtility.ConverterTypeMenu.Count == 0 ) {
                EditorGUILayout.HelpBox( "You have to create your own converter class.", MessageType.Error );
                return;
            }

            GUILayout.Space( 3 );

            EditorGUILayout.BeginHorizontal( GUIHelper.Styles.SheetConverterLabel );
            {
                GUILayout.Label( " Converters" );
                GUILayout.FlexibleSpace();
                if( GUILayout.Button( "", GUIHelper.Styles.PlusButton ) ) {
                    popupWin = new SpreadSheetConvertToolConverterPopupWindow();
                    popupWin.Initialize( data );
                    PopupWindow.Show( Rect.zero, popupWin );
                    EditorGUIUtility.keyboardControl = 0;
                }
            }
            EditorGUILayout.EndHorizontal();

            if( Converter == null || Converter.Count == 0 ) {
                EditorGUILayout.HelpBox( "no converers.", MessageType.Warning );
                return;
            }

            GUILayout.Space( 5 );

            converterListView = EditorGUILayout.BeginScrollView( converterListView );
            {
                for( int i = 0; i < Converter.Count; i++ ) {
                    GUI.backgroundColor = i == SelectedCoverter ? Color.yellow : Color.white;
                    if( GUILayout.Button( Converter[ i ].ConverterName, GUIHelper.Styles.ConverterList, GUILayout.Height( 30 ) ) ) {
                        UndoHelper.SpreadSheetUndo( data, UndoHelper.UNDO_SS_CHANGE );
                        SelectedCoverter = i;
                        EditorGUIUtility.keyboardControl = 0;
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            GUI.backgroundColor = Color.white;
        }

        private Vector2 sheetNameView;
        private void DrawSheetName() {
            GUILayout.Space( 10 );

            EditorGUILayout.BeginHorizontal( GUIHelper.Styles.SheetConverterLabel );
            {
                GUILayout.Label( " Sheet Names" );
            }
            EditorGUILayout.EndHorizontal();

            if( Sheets == null || Sheets.Length == 0 ) {
                EditorGUILayout.HelpBox( "no sheets data.", MessageType.Warning );
                return;
            }

            sheetNameView = EditorGUILayout.BeginScrollView( sheetNameView, EditorStyles.helpBox );
            {
                for( int i = 0; i < Sheets.Length; i++ ) {
                    if( GUILayout.Button( Sheets[i], GUIHelper.Styles.SheetName, GUILayout.ExpandWidth( true ) ) ) {
                        Debug.Log( "SpreadSheetConverter: Copy sheet name. " + Sheets[i] );
                        EditorGUIUtility.systemCopyBuffer = Sheets[ i ];
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private SpreadSheetConvertToolConverterPopupWindow popupWin;
        private void Header() {
            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.SpreadSheetUndo( data, UndoHelper.UNDO_SS_EDIT );
                GUILayout.Label( "Name", GUILayout.Width( 100 ) );
                Name = EditorGUILayout.TextField( Name );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.SpreadSheetUndo( data, UndoHelper.UNDO_SS_EDIT );
                GUILayout.Label( "SpreadSheet Id", GUILayout.Width( 100 ) );
                Id = EditorGUILayout.TextField( Id );
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 5 );

            EditorGUILayout.BeginHorizontal();
            {
                var usetoken = GUILayout.Toggle( UseDefaultAccessToken, "Use Default OAuth Config" );
                if( usetoken != UseDefaultAccessToken ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.SpreadSheetUndo( data, UndoHelper.UNDO_SS_EDIT );
                    if( usetoken ) {
                        OAuthClientId = GoogleAPIOAuthEditorConfig.Instance.OAuthClientId;
                        OAuthClientSecret = GoogleAPIOAuthEditorConfig.Instance.OAuthClientSecret;
                        RefreshToken= GoogleAPIOAuthEditorConfig.Instance.RefreshToken;
                    }
                    UseDefaultAccessToken = usetoken;
                }
            }
            EditorGUILayout.EndHorizontal();

            var defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            if( !UseDefaultAccessToken ) {
                OAuthClientId     = EditorGUILayout.TextField( "ClientID", OAuthClientId, GUIHelper.Styles.TextFieldWordWrap );
                OAuthClientSecret = EditorGUILayout.TextField( "ClientSecret", OAuthClientSecret, GUIHelper.Styles.TextFieldWordWrap );
                RefreshToken      = EditorGUILayout.TextField( "RefreshToken", RefreshToken, GUIHelper.Styles.TextFieldWordWrap );
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( "Check Config", new GUILayoutOption[] { GUILayout.Width( 120 ), GUILayout.Height( 20 ) } ) ) {
                        try {
                            EditorUtility.DisplayProgressBar( "Spread Sheet Converter", "Checking Config...", 0f );
                            var webRequest = data.GetWebRequest();
                            Sheets = EditorAPIUtility.GetSheetName( webRequest );
                            EditorUtility.DisplayDialog( "Spread Sheet Converter", "The setting is correct.", "ok" );
                            AccessToken = webRequest.GetAccessToken();
                        } catch( Exception ex ) {
                            Debug.LogError( "SpreadSheetConfig Error: " + ex );
                        }
                        EditorUtility.ClearProgressBar();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUIUtility.labelWidth = defaultWidth;
        }

        private void EventProcess( Event e, Rect rect ) {
            switch( e.type ) {
                case EventType.ContextClick:
                    if( rect.Contains( e.mousePosition ) ) {
                        IsContextClick = true;
                        e.Use();
                    } else {
                        IsContextClick = false;
                    }
                    break;
                default:
                    IsContextClick = false;
                    break;
            }
        }

        #region property

        //======================================================================
        // property
        //======================================================================

        private List<GoogleSpreadSheetConverter> Converter {
            get {
                return data.Converter;
            }
        }

        private string Name {
            get {
                return data.Name;
            }
            set {
                data.Name = value;
            }
        }

        private string Id {
            get {
                return data.Id;
            }
            set {
                data.Id = value;
            }
        }

        private bool UseDefaultAccessToken {
            get {
                return data.UseDefaultAccessToken;
            }
            set {
                data.UseDefaultAccessToken = value;
            }
        }

        private string OAuthClientId {
            get {
                return data.OAuthClientId;
            }
            set {
                data.OAuthClientId = value;
            }
        }

        private string OAuthClientSecret {
            get {
                return data.OAuthClientSecret;
            }
            set {
                data.OAuthClientSecret = value;
            }
        }

        private string RefreshToken {
            get {
                return data.RefreshToken;
            }
            set {
                data.RefreshToken = value;
            }
        }

        private string AccessToken {
            get {
                return data.AccessToken;
            }
            set {
                data.AccessToken = value;
            }
        }

        private string[] Sheets {
            get {
                return data.Sheets;
            }
            set {
                data.Sheets = value;
            }
        }

        #endregion

    }

}