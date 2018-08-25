using System;
using UnityEngine;
using UnityEditor;

using GUIHelper   = charcolle.SpreadSheetConverter.GUIHelper;
using UndoHelper  = charcolle.SpreadSheetConverter.UndoHelper;
using APIUtility  = charcolle.SpreadSheetConverter.EditorAPIUtility;
using FileUtility = charcolle.SpreadSheetConverter.FileUtility;

namespace charcolle.SpreadSheetConverter {

    internal class GoogleSpreadSheetConverterEditorGUI : EditorWindowItem<GoogleSpreadSheetConverter> {

        public GoogleSpreadSheetConverterEditorGUI( GoogleSpreadSheetConverter data ) : base( data ) { }

        [NonSerialized]
        public bool IsContextClick;
        [NonSerialized]
        public bool IsEdit;

        protected override void Draw() {
            DrawSettings();
        }

        //======================================================================
        // drawer
        //======================================================================

        private int converterEditMenu;
        private Vector2 scrollView;
        protected void DrawSettings() {
            Header();

            var rect = EditorGUILayout.BeginVertical( GUILayout.ExpandHeight( true ) );
            {
                scrollView = EditorGUILayout.BeginScrollView( scrollView );
                {
                    EditorGUILayout.BeginVertical( GUIHelper.Styles.Header );
                    {
                        GUILayout.Space( 2 );

                        if( IsEdit )
                            SheetDesc = EditorGUILayout.TextArea( SheetDesc );
                        else
                            GUILayout.Label( SheetDesc, GUIHelper.Styles.LabelWordWrap );
                    }
                    EditorGUILayout.EndVertical();

                    DrawSheetConfig();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();

            eventProcess( Event.current, rect );
        }

        private void Header() {
            EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
            {
                if( IsEdit ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                    ConverterName = EditorGUILayout.TextField( ConverterName, EditorStyles.toolbarTextField );
                } else {
                    GUILayout.Label( ConverterName );
                }

                GUILayout.Label( string.Format( "({0})", ScriptName ) );

                GUILayout.FlexibleSpace();

                if( GUILayout.Button( new GUIContent( "Open", GUIHelper.Textures.ScriptIcon ), EditorStyles.toolbarButton, GUILayout.Width( 80 ) ) ) {
                    FileUtility.OpenInEditor( ScriptName );
                }

                var edit = GUILayout.Toggle( IsEdit, "Edit", EditorStyles.toolbarButton, GUILayout.Width( 50 ) );
                if( edit != IsEdit ) {
                    EditorGUIUtility.keyboardControl = 0;
                    IsEdit = edit;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        #region draw sheet config
        private void DrawSheetConfig() {
            GUILayout.Space( 2 );

            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                DrawSheetNameConfig();
                DrawCellRangeConfig();
                DrawSaveDataConfig();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSheetNameConfig() {
            GUILayout.Space( 5 );

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( "Sheet Name".ToBold(), GUILayout.Width( 100 ) );
                SheetName = EditorGUILayout.TextField( SheetName );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup( !IsEdit );
            {
                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                    GUILayout.Label( "Multi Sheet Convert".ToBold(), GUILayout.Width( 150 ) );
                    MultiSheetConvertMode = EditorGUILayout.Toggle( MultiSheetConvertMode );
                }
                EditorGUILayout.EndHorizontal();
                if( MultiSheetConvertMode ) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                        GUILayout.Label( "Min Sheet Index", GUILayout.Width( 150 ) );
                        MinSheetIndex = EditorGUILayout.IntField( MinSheetIndex, GUILayout.Width( 50 ) );
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                        GUILayout.Label( "Max Sheet Index", GUILayout.Width( 150 ) );
                        MaxSheetIndex = EditorGUILayout.IntField( MaxSheetIndex, GUILayout.Width( 50 ) );
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.EndDisabledGroup();

            if( MultiSheetConvertMode ) {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( ( "Sheet Range : " + data.SheetIndexRange.x + " - " + data.SheetIndexRange.y ).ToMiddleBold() );
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 100 );
                    EditorGUILayout.MinMaxSlider( ref data.SheetIndexRange.x, ref data.SheetIndexRange.y, MinSheetIndex, MaxSheetIndex );
                }
                EditorGUILayout.EndHorizontal();
                data.SheetIndexRange.x = ( int )SheetIndexRange.x;
                data.SheetIndexRange.y = ( int )SheetIndexRange.y;
                EditorGUILayout.HelpBox( "SheetName Example: mysheet_#NUM#", MessageType.Info );
            }

        }

        private void DrawCellRangeConfig() {

            GUILayout.Space( 3 );
            GUILayout.Box( "", new GUILayoutOption[] { GUILayout.Height( 2f ), GUILayout.ExpandWidth( true ) } );
            GUILayout.Space( 3 );

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Cell Range".ToBold(), GUILayout.Width( 100 ) );
                GUILayout.Label( ( StartCell + " : " + EndCell ).ToMiddleBold() );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( UseAutomaticEnd ? "Start Row" : "Row Range", GUILayout.Width( 100 ) );
                if( UseAutomaticEnd ) {
                    data.RowRange.x = EditorGUILayout.IntSlider( ( int )RowRange.x, 1, MaxRow );
                    data.RowRange.y = RowRange.x + 1;
                } else {
                    EditorGUILayout.MinMaxSlider( ref data.RowRange.x, ref data.RowRange.y, 1, MaxRow );
                }
                data.RowRange.x = ( int )RowRange.x;
                data.RowRange.y = ( int )RowRange.y;
                StartRow = ( int )RowRange.x;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( UseAutomaticEnd ? "Start Col" : "Col Range", GUILayout.Width( 100 ) );
                if( UseAutomaticEnd ) {
                    data.ColRange.x = EditorGUILayout.IntSlider( ( int )ColRange.x, 1, MaxCol );
                    data.ColRange.y = ColRange.x + 1;
                } else {
                    EditorGUILayout.MinMaxSlider( ref data.ColRange.x, ref data.ColRange.y, 1, MaxCol );
                }
                data.ColRange.x = ( int )ColRange.x;
                data.ColRange.y = ( int )ColRange.y;
                StartCol = ( int )ColRange.x;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 5 );

            EditorGUI.BeginDisabledGroup( !IsEdit );
            {

                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                    GUILayout.Label( "Use Automatic End", GUILayout.Width( 150 ) );
                    UseAutomaticEnd = EditorGUILayout.Toggle( UseAutomaticEnd );
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                    GUILayout.Label( "Max Row Range", GUILayout.Width( 100 ) );
                    MaxRow = EditorGUILayout.IntField( MaxRow, GUILayout.Width( 50 ) );
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                    GUILayout.Label( "Max Col Range", GUILayout.Width( 100 ) );
                    MaxCol = EditorGUILayout.IntField( MaxCol, GUILayout.Width( 50 ) );
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawSaveDataConfig() {
            GUILayout.Space( 3 );
            GUILayout.Box( "", new GUILayoutOption[] { GUILayout.Height( 2f ), GUILayout.ExpandWidth( true ) } );
            GUILayout.Space( 3 );

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Save Path".ToBold(), GUILayout.Width( 100 ) );
                EditorGUILayout.TextField( SavePath );
                {
                    var path = FileUtility.GetDraggedObjectPath( Event.current, GUILayoutUtility.GetLastRect() );
                    if( !string.IsNullOrEmpty( path ) ) {
                        UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                        SavePath = path;
                    }
                }
                if( GUILayout.Button( " Select ", GUILayout.Width( 60 ) ) ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                    var path = EditorUtility.OpenFolderPanel( "Select Folder", "", "" );
                    if( !string.IsNullOrEmpty( path ) ) {
                        UndoHelper.ConverterUndo( data, UndoHelper.UNDO_CONVERTER_EDIT );
                        SavePath = path;
                    }
                    EditorGUIUtility.keyboardControl = 0;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Save FileName".ToBold(), GUILayout.Width( 150 ) );
                SaveFileName = EditorGUILayout.TextField( SaveFileName );
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Save File Extension".ToBold(), GUILayout.Width( 150 ) );
                SaveFileExtension = EditorGUILayout.TextField( SaveFileExtension );
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 5 );
        }

        #endregion

        private void eventProcess( Event e, Rect rect ) {
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

        private string ConverterName {
            get {
                return data.ConverterName;
            }
            set {
                data.ConverterName = value;
            }
        }

        private string SheetName {
            get {
                return data.SheetName;
            }
            set {
                data.SheetName = value;
            }
        }

        private string SheetDesc {
            get {
                return data.SheetDesc;
            }
            set {
                data.SheetDesc = value;
            }
        }

        private string ScriptName {
            get {
                return data.ScriptName;
            }
            set {
                data.ScriptName = value;
            }
        }

        private Vector2 SheetIndexRange {
            get {
                return data.SheetIndexRange;
            }
            set {
                data.SheetIndexRange = value;
            }
        }

        private Vector2 RowRange {
            get {
                return data.RowRange;
            }
            set {
                data.RowRange = value;
            }
        }

        private Vector2 ColRange {
            get {
                return data.ColRange;
            }
            set {
                data.ColRange = value;
            }
        }

        private int StartRow {
            get {
                return data.StartRow;
            }
            set {
                data.StartRow = value;
            }
        }

        private int StartCol {
            get {
                return data.StartCol;
            }
            set {
                data.StartCol = value;
            }
        }

        private int MaxRow {
            get {
                return data.MaxRow;
            }
            set {
                data.MaxRow = value;
            }
        }

        private int MaxCol {
            get {
                return data.MaxCol;
            }
            set {
                data.MaxCol = value;
            }
        }

        private bool UseAutomaticEnd {
            get {
                return data.UseAutomaticEnd;
            }
            set {
                data.UseAutomaticEnd = value;
            }
        }

        private string SavePath {
            get {
                return data.SavePath;
            }
            set {
                data.SavePath = value;
            }
        }

        private string SaveFileName {
            get {
                return data.SaveFileName;
            }
            set {
                data.SaveFileName = value;
            }
        }

        private string SaveFileExtension {
            get {
                return data.SaveFileExtension;
            }
            set {
                data.SaveFileExtension = value;
            }
        }

        private string CellRange {
            get {
                return data.CellRange;
            }
        }

        private string StartCell {
            get {
                return data.StartCell;
            }
        }

        private string EndCell {
            get {
                return data.EndCell;
            }
        }

        private bool MultiSheetConvertMode {
            get {
                return data.MultiSheetConvertMode;
            }
            set {
                data.MultiSheetConvertMode = value;
            }
        }

        private int MaxSheetIndex {
            get {
                return data.MaxSheetIndex;
            }
            set {
                data.MaxSheetIndex = value;
            }
        }

        private int MinSheetIndex {
            get {
                return data.MinSheetIndex;
            }
            set {
                data.MinSheetIndex = value;
            }
        }

        #endregion
    }

}