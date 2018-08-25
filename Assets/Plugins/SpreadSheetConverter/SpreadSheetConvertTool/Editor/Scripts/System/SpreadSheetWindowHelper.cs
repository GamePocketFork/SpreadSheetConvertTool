using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SplitterState = charcolle.SpreadSheetConverter.SplitterState;

namespace charcolle.SpreadSheetConverter {

    internal static class SpreadSheetWindowHelper {

        public readonly static string WINDOW_TITLE = "SheetConverter";
        public readonly static Vector2 WINDOW_SIZE = new Vector2( 350, 200 );

        public readonly static string[] MENU_WINDOW = new string[] { "SpreadSheets", "Config" };

        public static List<GoogleSpreadSheetConfig> SpreadSheets = new List<GoogleSpreadSheetConfig>();

        public static SplitterState HorizontalState;

        //======================================================================
        // Initialize
        //======================================================================
        public static void Initialize( Rect window ) {
            HorizontalState = new SplitterState( new float[] { window.width * 0.25f, window.width * 0.75f },
                                                              new int[] { 250, 250 }, new int[] { 500, 1000 } );
            SpreadSheets = FileUtility.LoadGoogleSpreadSheetConfigData();
        }

        public static void ConstructData() {
            SpreadSheets = FileUtility.LoadGoogleSpreadSheetConfigData();
        }

        public static void OnGUIStart() {
            GUI.skin.label.richText = true;
        }

        public static void OnGUIEnd() {
            GUI.skin.label.richText = false;
        }

        //======================================================================
        // public
        //======================================================================

        public static bool CheckSpreadSheetIdx( int idx ) {
            return SpreadSheets != null && SpreadSheets.Count > idx;
        }

        public static void CreateNewSpreadSheetConfig( string configName ) {
            FileUtility.CreateSpreadSheetConfig( configName );
        }

        //======================================================================
        // property
        //======================================================================

        public static string[] SpreadSheetList {
            get {
                if( IsSpreadSheetDataExists )
                    return SpreadSheets.Select( s => s.Name ).ToArray();
                return null;
            }
        }

        public static bool IsSpreadSheetDataExists {
            get {
                return SpreadSheets != null && SpreadSheets.Count > 0;
            }
        }
    }
}