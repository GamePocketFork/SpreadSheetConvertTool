using UnityEngine;
using UnityEditor;

namespace charcolle.Utility.SpreadSheetConvertTool {

    public class GUIHelper {
        //======================================================================
        // GUIStyle
        //======================================================================

        public static class Styles {

            static Styles() {
                NoSpaceBox = new GUIStyle( GUI.skin.box );
                NoSpaceBox.margin = new RectOffset( 0, 0, 0, 0 );
                NoSpaceBox.padding = new RectOffset( 1, 1, 1, 1 );

                LabelWordWrap              = new GUIStyle( GUI.skin.label );
                LabelWordWrap.wordWrap     = true;
                LabelWordWrap.richText     = true;

                TextFieldWordWrap          = new GUIStyle( GUI.skin.textField );
                TextFieldWordWrap.wordWrap = true;

                LeftArea                   = new GUIStyle( "WindowBackground" );
                LeftArea.margin            = new RectOffset( 0, 0, 0, 0 );
                LeftArea.padding           = new RectOffset( 0, 0, 0, 0 );

                Header                     = new GUIStyle( "ProjectBrowserHeaderBgMiddle" );
                Header.margin              = new RectOffset( 0, 0, 0, 0 );
                Header.padding             = new RectOffset( 1, 1, 1, 1 );

                Footer                     = new GUIStyle( "Tooltip" );
                Footer.margin              = new RectOffset( 0, 0, 0, 0 );
                Footer.padding             = new RectOffset( 1, 1, 1, 1 );

                ConverterList              = new GUIStyle( "ShurikenModuleTitle" );
                ConverterList.fixedHeight  = 0f;

                PlusButton                 = new GUIStyle( "OL Plus" );

                ToolBar                    = new GUIStyle( EditorStyles.toolbarButton );
                ToolBar.margin             = new RectOffset( 0, 0, 0, 0 );
                ToolBar.padding            = new RectOffset( 0, 0, 0, 0 );
            }

            public static GUIStyle NoSpaceBox {
                get;
                private set;
            }

            public static GUIStyle LabelWordWrap {
                get;
                private set;
            }

            public static GUIStyle TextFieldWordWrap {
                get;
                private set;
            }

            public static GUIStyle NoOverflow {
                get;
                private set;
            }

            public static GUIStyle LeftArea {
                get;
                private set;
            }

            public static GUIStyle Header {
                get;
                private set;
            }

            public static GUIStyle Footer {
                get;
                private set;
            }

            public static GUIStyle ConverterList {
                get;
                private set;
            }

            public static GUIStyle PlusButton {
                get;
                private set;
            }

            public static GUIStyle ToolBar {
                get;
                private set;
            }

        }
    }
}