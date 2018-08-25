using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace charcolle.SpreadSheetConverter {

    public static class FileUtility {

        private readonly static string SEARCH_WORD              = "SpreadSheetConvertToolWindow";
        private readonly static string SEARCH_OAUTHCONFIG       = "GoogleAPIOAuthEditorConfig";
        private readonly static string SEARCH_SPREADSHEETCONFIG = "GoogleSpreadSheetConfig";

        private readonly static string RELATIVEPATH_SAVEDATA    = "Data/";
        private readonly static string RELATIVEPATH_TEMPLATE    = "SpreadSheetConvertTool/Editor/Template/";
        private readonly static string RELATIVEPATH_CONFIGDATA  = "SpreadSheetConvertTool/Editor/Data/";

        private readonly static string NAME_OAUTHCONFIG         = "GoogleAPIConfig.asset";
        private readonly static string NAME_TEMPLATE            = "ConverterScriptTemplate.template";

        //=======================================================
        // path
        //=======================================================

        public static string SaveDataDirectoryPath {
            get {
                return pathSlashFix( Path.Combine( SpreadSheetConvertToolRootPath, RELATIVEPATH_SAVEDATA ) );
            }
        }

        public static string ConverterTemplatePath {
            get {
                return pathSlashFix( Path.Combine( SpreadSheetConvertToolRootPath, RELATIVEPATH_TEMPLATE ) );
            }
        }

        public static string EditorConfigSavePath {
            get {
                return pathSlashFix( Path.Combine( SpreadSheetConvertToolRootPath, RELATIVEPATH_CONFIGDATA ) );
            }
        }

        public static string SpreadSheetConvertToolRootPath {
            get {
                var guid = getAssetGUID( SEARCH_WORD );

                if( string.IsNullOrEmpty( guid ) ) {
                    Debug.LogError( "fatal error." );
                    return null;
                }
                var filePath        = Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guid ) );
                var scriptPath      = Path.GetDirectoryName( filePath );
                var editorPath      = Path.GetDirectoryName( scriptPath );
                var spreadSheetPath = Path.GetDirectoryName( editorPath );
                var rootPath        = Path.GetDirectoryName( spreadSheetPath );

                return pathSlashFix( rootPath );
            }
        }

        //=======================================================
        // public
        //=======================================================

        public static GoogleAPIOAuthEditorConfig LoadGoogleAPIOAuthConfigData() {
            var saveData = FindAssetByType<GoogleAPIOAuthEditorConfig>( SEARCH_OAUTHCONFIG );
            if( saveData == null ) {
                if( !Directory.Exists( EditorConfigSavePath ) )
                    Directory.CreateDirectory( EditorConfigSavePath );

                var savePath = Path.Combine( EditorConfigSavePath, NAME_OAUTHCONFIG );
                saveData = ScriptableObject.CreateInstance<GoogleAPIOAuthEditorConfig>();
                AssetDatabase.CreateAsset( saveData, savePath );
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            return saveData;
        }

        public static List<GoogleSpreadSheetConfig> LoadGoogleSpreadSheetConfigData() {
            var data = FindAssetsByType<GoogleSpreadSheetConfig>( SEARCH_SPREADSHEETCONFIG );
            return data;
        }

        public static void CreateSpreadSheetConfig( string configName ) {
            if( !Directory.Exists( SaveDataDirectoryPath ) )
                Directory.CreateDirectory( SaveDataDirectoryPath );
            var savePath = Path.Combine( SaveDataDirectoryPath, configName + ".asset" );
            if( File.Exists( savePath ) ) {
                Debug.LogWarning( "GoogleSpreadSheetConvertTool: file already exists. : " + configName );
                return;
            }

            var saveData               = ScriptableObject.CreateInstance<GoogleSpreadSheetConfig>();
            saveData.Name              = configName;
            saveData.OAuthClientId     = GoogleAPIOAuthEditorConfig.Instance.OAuthClientId;
            saveData.OAuthClientSecret = GoogleAPIOAuthEditorConfig.Instance.OAuthClientSecret;
            saveData.RefreshToken      = GoogleAPIOAuthEditorConfig.Instance.RefreshToken;

            AssetDatabase.CreateAsset( saveData, savePath );
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void CreateSpreadSheetConverter( Type converterType, string converterName, GoogleSpreadSheetConfig parentConfig ){
            if( !Directory.Exists( SaveDataDirectoryPath ) )
                Directory.CreateDirectory( SaveDataDirectoryPath );

            var saveData           = ScriptableObject.CreateInstance( converterType ) as GoogleSpreadSheetConverter;
            saveData.ConverterName = converterName;
            saveData.name          = converterName;
            saveData.ScriptName    = converterType.Name;
            parentConfig.Converter.Add( saveData );

            AssetDatabase.AddObjectToAsset( saveData, AssetDatabase.GetAssetPath( parentConfig ) );
            AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( parentConfig ) );
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void OpenInEditor( string scriptName, int scriptLine = 0 ) {
            var searchFilter = "t:Script " + scriptName;
            var guids = AssetDatabase.FindAssets( searchFilter );

            for( int i = 0; i < guids.Length; i++ ) {
                var path = AssetDatabase.GUIDToAssetPath( guids[i] );
                string fileName = Path.GetFileNameWithoutExtension( path );
                if( fileName.Equals( scriptName ) ) {
                    MonoScript script = AssetDatabase.LoadAssetAtPath( path, typeof( MonoScript ) ) as MonoScript;
                    if( script != null ) {
                        if( !AssetDatabase.OpenAsset( script, scriptLine ) ) {
                            Debug.LogWarning( "Couldn't open script : " + scriptName );
                        }
                        break;
                    } else {
                        Debug.LogError( script );
                    }
                    break;
                }
            }
        }

        private readonly static string REPLACE_SCRIPTNAME = "#SCRIPTNAME#";
        public static void CreateConverterScript( string ScriptName ) {
            if( string.IsNullOrEmpty( ScriptName ) ) {
                Debug.LogWarning( "SpreadSheetConvertTool: Input script name." );
                return;
            }

            var templatePath = Path.Combine( ConverterTemplatePath, NAME_TEMPLATE );
            var script = "";
            try {
                StreamReader sr = new StreamReader( templatePath, Encoding.UTF8 );
                script = sr.ReadToEnd();
                sr.Close();
            } catch( Exception ex ) {
                Debug.LogError( "GoogleSpreadSheetConvertTool: Cannot read the script template. : " + ex );
                return;
            }
            var savePath = EditorUtility.SaveFilePanel( "Create Custom Converter", Application.dataPath, ScriptName, "cs" );
            if( !string.IsNullOrEmpty( savePath ) ) {
                if( File.Exists( savePath ) ) {
                    Debug.LogWarning( "SpreadSheetConvertTool: File already exists. :" + savePath );
                    return;
                }

                if( savePath.Contains( "/Editor/" ) ) {
                    Debug.LogWarning( "SpreadSheetConvertTool: You must not create a converter script at Editor Folder." );
                    return;
                }

                var scriptName = Path.GetFileNameWithoutExtension( savePath );
                script = script.Replace( REPLACE_SCRIPTNAME, scriptName );

                try {
                    StreamWriter sw = new StreamWriter( savePath, false, Encoding.UTF8 );
                    sw.Write( script );
                    sw.Close();
                    AssetDatabase.Refresh();
                } catch( Exception ex ) {
                    Debug.LogError( "SpreadSheetConvertTool: Cannot save the script. : " + ex );
                }
            }
        }

        //=======================================================
        // utility
        //=======================================================

        /// <summary>
        /// get path of dropped file
        /// </summary>
        public static string GetDraggedObjectPath( Event curEvent, Rect dropArea ) {
            int ctrlID = GUIUtility.GetControlID( FocusType.Passive );
            switch( curEvent.type ) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if( !dropArea.Contains( curEvent.mousePosition ) )
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.activeControlID = ctrlID;

                    if( curEvent.type == EventType.DragPerform ) {
                        DragAndDrop.AcceptDrag();
                        foreach( var draggedObj in DragAndDrop.objectReferences ) {
                            return AssetDatabase.GetAssetPath( draggedObj );
                        }
                    }
                    curEvent.Use();
                    break;
            }
            return null;
        }

        //=======================================================
        // process
        //=======================================================

        private static T FindAssetByType<T>( string type ) where T : Object {
            var searchFilter = "t:" + type;
            var guid = getAssetGUID( searchFilter );
            if( string.IsNullOrEmpty( guid ) )
                return null;
            var assetPath = AssetDatabase.GUIDToAssetPath( guid );
            return AssetDatabase.LoadAssetAtPath<T>( assetPath );
        }

        private static List<T> FindAssetsByType<T>( string type ) where T : Object {
            var searchFilter = "t:" + type;
            var guids = AssetDatabase.FindAssets( searchFilter );
            if( guids == null || guids.Length == 0 ) {
                return null;
            }
            var list = new List<T>();
            for( int i = 0; i < guids.Length; i++ ) {
                var assetPath = AssetDatabase.GUIDToAssetPath( guids[ i ] );
                list.Add( AssetDatabase.LoadAssetAtPath<T>( assetPath ) );
            }
            return list;
        }

        private static string getAssetGUID( string searchFilter ) {
            var guids = AssetDatabase.FindAssets( searchFilter );
            if( guids == null || guids.Length == 0 ) {
                return null;
            }

            if( guids.Length > 1 ) {
                Debug.LogWarning( "more than one file was found." );
            }
            return guids[ 0 ];
        }

        private const string forwardSlash = "/";
        private const string backSlash = "\\";
        private static string pathSlashFix( string path ) {
            return path.Replace( backSlash, forwardSlash );
        }

    }
}