using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using charcolle.Utility.SpreadSheetConvertTool;

using UndoHelper       = charcolle.Utility.SpreadSheetConvertTool.UndoHelper;
using GUIHelper        = charcolle.Utility.SpreadSheetConvertTool.GUIHelper;
using ConverterUtility = charcolle.Utility.SpreadSheetConvertTool.ConverterUtility;

public class GoogleSpreadSheetConfig : ScriptableObject {

    public string Name;
    public string Id;

    public List<GoogleSpreadSheetConverter> Converter = new List<GoogleSpreadSheetConverter>();

    public void Initialize() {
        SelectedCoverter = -1;
    }

    #region drawer

    //======================================================================
    // drawer
    //======================================================================

    public void Draw() {
        var rect = EditorGUILayout.BeginVertical();
        {
            Header();
            DrawConverterList();
        }
        EditorGUILayout.EndVertical();

        EventProcess( Event.current, rect );
    }

    [NonSerialized]
    public bool IsContextClick;
    [NonSerialized]
    public int SelectedCoverter = -1;
    private Vector2 converterListView;
    private void DrawConverterList() {
        if( ConverterUtility.ConverterTypeMenu == null || ConverterUtility.ConverterTypeMenu.Count == 0 ) {
            EditorGUILayout.HelpBox( "You have to create your own converter class.", MessageType.Error );
            return;
        }

        if( Converter == null || Converter.Count == 0 ) {
            EditorGUILayout.HelpBox( "no converers.", MessageType.Warning );
            return;
        }

        GUILayout.Space( 5f );

        converterListView = EditorGUILayout.BeginScrollView( converterListView );
        {
            for( int i = 0; i < Converter.Count; i++ ) {
                GUI.backgroundColor = i == SelectedCoverter ? Color.yellow : Color.white;
                if( GUILayout.Button( Converter[ i ].ConverterName, GUIHelper.Styles.ConverterList, GUILayout.Height( 30 ) ) ) {
                    UndoHelper.SpreadSheetUndo( this, UndoHelper.UNDO_SS_CHANGE );
                    SelectedCoverter = i;
                    EditorGUIUtility.keyboardControl = 0;
                }
            }
        }
        EditorGUILayout.EndScrollView();
        GUI.backgroundColor = Color.white;
    }

    private SpreadSheetConvertToolConverterPopupWindow popupWin;
    private void Header() {

        EditorGUILayout.BeginHorizontal();
        {
            Undo.IncrementCurrentGroup();
            UndoHelper.SpreadSheetUndo( this, UndoHelper.UNDO_SS_EDIT );
            GUILayout.Label( "Name", GUILayout.Width( 100 ) );
            Name = EditorGUILayout.TextField( Name );
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            Undo.IncrementCurrentGroup();
            UndoHelper.SpreadSheetUndo( this, UndoHelper.UNDO_SS_EDIT );
            GUILayout.Label( "SpreadSheet Id", GUILayout.Width( 100 ) );
            Id = EditorGUILayout.TextField( Id );
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space( 5 );

        EditorGUILayout.BeginHorizontal( GUIHelper.Styles.Header );
        {
            GUILayout.Label( "Converters" );
            GUILayout.FlexibleSpace();
            if( GUILayout.Button( "", GUIHelper.Styles.PlusButton ) ) {
                popupWin = new SpreadSheetConvertToolConverterPopupWindow();
                popupWin.Initialize( this );
                PopupWindow.Show( Rect.zero, popupWin );
                EditorGUIUtility.keyboardControl = 0;
            }
        }
        EditorGUILayout.EndHorizontal();
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

    #endregion drawer

}