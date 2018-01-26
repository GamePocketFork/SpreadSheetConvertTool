using System;
using UnityEngine;
using UnityEditor;
using charcolle.Utility.SpreadSheetConvertTool;

using GUIHelper   = charcolle.Utility.SpreadSheetConvertTool.GUIHelper;
using UndoHelper  = charcolle.Utility.SpreadSheetConvertTool.UndoHelper;
using APIUtility  = charcolle.Utility.SpreadSheetConvertTool.APIUtility;
using FileUtility = charcolle.Utility.SpreadSheetConvertTool.FileUtility;

public abstract class GoogleSpreadSheetConverter : ScriptableObject, ISpreadSheetConverter {

    public string ConverterName;
    public string SheetName;
    public string SheetDesc = "about this converter";
    public string ScriptName;

    public Vector2 RowRange = Vector2.one;
    public Vector2 ColRange = Vector2.one;
    public int StartRow     = 1;
    public int StartCol     = 1;
    public int MaxRow       = 20;
    public int MaxCol       = 20;
    public bool UseAutomaticEnd;

    public string SavePath;
    public string SaveFileName = "hoge.asset";

    //======================================================================
    // property
    //======================================================================

    public string Range {
        get {
            return SheetName + "!" + StartCell + ":" + EndCell;
        }
    }

    public string StartCell {
        get {
            if( UseAutomaticEnd ) {
                return APIUtility.GetRange( StartRow, StartCol );
            } else {
                return APIUtility.GetRange( ( int )RowRange.x, ( int )ColRange.x );
            }
        }
    }

    public string EndCell {
        get {
            if( UseAutomaticEnd ) {
                return "ZZ";
            } else {
                return APIUtility.GetRange( ( int )RowRange.y, ( int )ColRange.y );
            }
        }
    }

    //======================================================================
    // abstract
    //======================================================================

    public abstract void Convert( SpreadSheetAPIClass data );

    //======================================================================
    // public
    //======================================================================

    public void Receive( SpreadSheetAPIClass data ) {
        try {
            if( data != null )
                Convert( data );
        } catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    protected virtual void Save( ScriptableObject data ) {
        try {
            FileUtility.SaveData( data, SavePath, SaveFileName );
        } catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    protected virtual void Save( string data ) {
        try {
            FileUtility.SaveData( data, SavePath, SaveFileName );
        } catch( Exception ex ) {
            Debug.LogError( "GoogleSpreadSheetConverter Error :" + ex );
        }
    }

    public virtual void Draw() {
        DrawSettings();
    }

    #region draw process

    //======================================================================
    // drawer
    //======================================================================

    [NonSerialized]
    public bool IsContextClick;
    [NonSerialized]
    public bool IsEdit;
    [NonSerialized]
    public string TestAPIResponse;
    private int converterEditMenu;
    private Vector2 scrollView;
    protected void DrawSettings() {

        var rect = EditorGUILayout.BeginVertical( GUILayout.ExpandHeight( true ) );
        {
            EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
            {
                if( IsEdit ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                    ConverterName = EditorGUILayout.TextField( ConverterName, EditorStyles.toolbarTextField );
                } else {
                    GUILayout.Label( ConverterName );
                }

                GUILayout.Label( string.Format( "({0})", ScriptName ) );

                GUILayout.FlexibleSpace();

                if( GUILayout.Button( "Open Script", EditorStyles.toolbarButton, GUILayout.Width( 80 ) ) ) {
                    FileUtility.OpenInEditor( ScriptName );
                }
                var edit = GUILayout.Toggle( IsEdit, "Edit", EditorStyles.toolbarButton, GUILayout.Width( 50 ) );
                if( edit != IsEdit ) {
                    TestAPIResponse = "";
                    EditorGUIUtility.keyboardControl = 0;
                }
                IsEdit = edit;
            }
            EditorGUILayout.EndHorizontal();

            scrollView = EditorGUILayout.BeginScrollView( scrollView );
            {
                GUILayout.Space( 2 );

                if( IsEdit ) {
                    SheetDesc = EditorGUILayout.TextArea( SheetDesc );
                } else {
                    GUILayout.Label( SheetDesc, GUIHelper.Styles.LabelWordWrap );
                }

                GUILayout.Label( "Basic Setting".ToBold() );

                drawSheetRange();
                DrawTestAPIResponse();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        eventProcess( Event.current, rect );
    }

    private void drawSheetRange() {
        EditorGUILayout.BeginVertical( EditorStyles.helpBox );
        {
            GUILayout.Space( 5 );

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( "Sheet Name".ToBold(), GUILayout.Width( 100 ) );
                SheetName = EditorGUILayout.TextField( SheetName );
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 3 );

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Range".ToBold(), GUILayout.Width( 100 ) );
                GUILayout.Label( ( StartCell + " : " + EndCell ).ToMiddleBold() );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( UseAutomaticEnd ? "Start Row" : "Row Range", GUILayout.Width( 100 ) );
                if( UseAutomaticEnd ) {
                    RowRange.x = EditorGUILayout.IntSlider( ( int )RowRange.x, 1, MaxRow );
                    RowRange.y = RowRange.x + 1;
                } else {
                    EditorGUILayout.MinMaxSlider( ref RowRange.x, ref RowRange.y, 1, MaxRow );
                }

                RowRange.x = ( int )RowRange.x;
                RowRange.y = ( int )RowRange.y;
                StartRow   = ( int )RowRange.x;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( UseAutomaticEnd ? "Start Col" : "Col Range", GUILayout.Width( 100 ) );
                if( UseAutomaticEnd ) {
                    ColRange.x = EditorGUILayout.IntSlider( ( int )ColRange.x, 1, MaxCol );
                    ColRange.y = ColRange.x + 1;
                } else {
                    EditorGUILayout.MinMaxSlider( ref ColRange.x, ref ColRange.y, 1, MaxCol );
                }
                ColRange.x = ( int )ColRange.x;
                ColRange.y = ( int )ColRange.y;
                StartCol   = ( int )ColRange.x;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 5 );

            EditorGUI.BeginDisabledGroup( !IsEdit );
            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( "Max Row Range", GUILayout.Width( 100 ) );
                MaxRow = EditorGUILayout.IntField( MaxRow, GUILayout.Width( 50 ) );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( "Max Col Range", GUILayout.Width( 100 ) );
                MaxCol = EditorGUILayout.IntField( MaxCol, GUILayout.Width( 50 ) );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                Undo.IncrementCurrentGroup();
                UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                GUILayout.Label( "Use Automatic End", GUILayout.Width( 150 ) );
                UseAutomaticEnd = EditorGUILayout.Toggle( UseAutomaticEnd );
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            GUILayout.Space( 5 );

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Save Path".ToBold(), GUILayout.Width( 100 ) );
                EditorGUILayout.TextField( SavePath );
                {
                    var path = FileUtility.GetDraggedObjectPath( Event.current, GUILayoutUtility.GetLastRect() );
                    if( !string.IsNullOrEmpty( path ) ) {
                        UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                        SavePath = path;
                    }
                }
                if( GUILayout.Button( " Select ", GUILayout.Width( 60 ) ) ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                    var path = EditorUtility.OpenFolderPanel( "Select Folder", "", "" );
                    if( !string.IsNullOrEmpty( path ) ) {
                        UndoHelper.ConverterUndo( this, UndoHelper.UNDO_CONVERTER_EDIT );
                        SavePath = path;
                    }
                    EditorGUIUtility.keyboardControl = 0;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Save FileName".ToBold(), GUILayout.Width( 100 ) );
                SaveFileName = EditorGUILayout.TextField( SaveFileName );
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 5 );
        }
        EditorGUILayout.EndVertical();
    }

    private Vector2 testScrollView;
    private void DrawTestAPIResponse() {
        if( string.IsNullOrEmpty( TestAPIResponse ) )
            return;

        GUILayout.Space( 5 );
        GUILayout.Label( "API Result" );
        testScrollView = EditorGUILayout.BeginScrollView( testScrollView );
        {
            EditorGUILayout.TextArea( TestAPIResponse );
        }
        EditorGUILayout.EndScrollView();
    }

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

    #endregion

}