using UnityEngine;
using UnityEditor;

namespace charcolle.SpreadSheetConverter {

    internal class GUIHelper {

        //======================================================================
        // GUIStyle
        //======================================================================

        internal static class Styles {

            static Styles() {
                NoSpaceBox = new GUIStyle( GUI.skin.box ) {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 1, 1 )
                };

                LabelWordWrap = new GUIStyle( GUI.skin.label ) {
                    wordWrap = true,
                    richText = true
                };

                TextFieldWordWrap = new GUIStyle( GUI.skin.textField ) {
                    wordWrap = true
                };

                LeftArea = new GUIStyle( GUI.skin.box ) {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 0, 0 )
                };

                RightArea = new GUIStyle( EditorStyles.textArea ) {
                    margin = new RectOffset( 1, 0, 0, 0 ),
                    padding = new RectOffset( 0, 0, 0, 0 )
                };

                Header = new GUIStyle( EditorStyles.helpBox );

                SheetConverterLabel = new GUIStyle( "ProjectBrowserHeaderBgMiddle" ) {
                    padding = new RectOffset( 0, 0, 2, 2 )
                };

                Footer = new GUIStyle( GUI.skin.box ) {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 1, 1 )
                };

                ConverterList = new GUIStyle( "ShurikenModuleTitle" ) {
                    fixedHeight = 0f,
                    margin = new RectOffset( 1, 1, 0, 0 )
                };

                PlusButton                  = new GUIStyle( "OL Plus" );

                ToolBar = new GUIStyle( EditorStyles.toolbarButton );

                SheetName = new GUIStyle( GUI.skin.box ) {
                    margin = new RectOffset( 0, 0, 0, 0 )
                };
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

            public static GUIStyle RightArea {
                get;
                private set;
            }

            public static GUIStyle Header {
                get;
                private set;
            }

            public static GUIStyle SheetConverterLabel {
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

            public static GUIStyle SheetName {
                get;
                private set;
            }

        }

        internal static class Textures {

            static Textures() {
                ScriptableObjectIcon = EditorGUIUtility.Load( "ScriptableObject Icon" ) as Texture2D;
                ScriptIcon           = EditorGUIUtility.Load( "cs Script Icon" ) as Texture2D;
            }

            public static Texture2D ScriptableObjectIcon {
                get;
                private set;
            }

            public static Texture2D ScriptIcon {
                get;
                private set;
            }

        }

        internal static class Colors {

            static Colors() {
                DefaultFontColor = GUI.contentColor;
            }

            public static Color DefaultFontColor {
                get;
                private set;
            }

        }

    }
}