using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using FileUtility = charcolle.Utility.SpreadSheetConvertTool.FileUtility;

namespace charcolle.Utility.SpreadSheetConvertTool {

    internal static class ConverterUtility {

        public static List<string> ConverterTypeMenu = new List<string>();
        public static List<Type> ConverterType       = new List<Type>();

        public static void GetConverterSubClass() {
            var types = Assembly
                        .GetAssembly( typeof( GoogleSpreadSheetConverter ) )
                        .GetTypes()
                        .Where( t => {
                            return t.IsSubclassOf( typeof( GoogleSpreadSheetConverter ) ) && !t.IsAbstract;
                        } );

            ConverterTypeMenu = new List<string>();
            ConverterType     = new List<Type>();

            ConverterTypeMenu = types.Select( t => t.Name ).ToList();
            ConverterType     = types.ToList();
        }

        public static void CreateConverter( int selectedType, string converterName, GoogleSpreadSheetConfig parentConfig ) {
            if( selectedType < 0 || selectedType >= ConverterType.Count )
                return;

            var type = ConverterType[ selectedType ];
            FileUtility.CreateSpreadSheetConverter( type, converterName, parentConfig );
        }

        public static string[] ConverterMenu {
            get {
                return ConverterTypeMenu.OrderBy( s => s ).ToArray();
            }
        }

        public static string ToBold( this string str ) {
            return string.Format( "<b>{0}</b>", str );
        }

        public static string ToMiddleBold( this string str ) {
            return string.Format( "<size=12><b>{0}</b></size>", str );
        }

    }

}