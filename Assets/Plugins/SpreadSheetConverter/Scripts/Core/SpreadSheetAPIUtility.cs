using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace charcolle.SpreadSheetConverter {

    public static class SpreadSheetAPIUtility {

        public static string GetRange( int row, int col ) {
            return row.ToAlphabet() + col.ToString();
        }

        /// <summary>
        /// http://zecl.hatenablog.com/entry/20080813/p1
        /// </summary>
        private static string ToAlphabet( this int self ) {
            if( self <= 0 )
                return "";
            int n = self % 26;
            n = ( n == 0 ) ? 26 : n;
            string s = ( ( char )( n + 64 ) ).ToString();
            if( self == n )
                return s;
            return ( ( self - n ) / 26 ).ToAlphabet() + s;
        }

#if UNITY_EDITOR
        // disgusting :(
        public static void SaveData( string data, string path, string fileName, bool checkIfAlreadyExist = true ) {
            if( !Directory.Exists( path ) )
                throw new DirectoryNotFoundException();
            if( string.IsNullOrEmpty( path ) )
                throw new ArgumentException( "SavePath is null." );
            if( string.IsNullOrEmpty( fileName ) )
                throw new ArgumentException( "FileName is null." );

            var savePath = pathSlashFix( Path.Combine( path, fileName ) );
            if( File.Exists( savePath ) ) {
                if( checkIfAlreadyExist ) {
                    if( !EditorUtility.DisplayDialog( "The file already exists.", "Are you sure you want to overwrite? : " + savePath, "ok", "cancel" ) ) {
                        Debug.LogWarning( "SpreadSheetConvertTool: Save process was canceled." );
                        return;
                    }
                }
            }

            StreamWriter sw = new StreamWriter( savePath, false, Encoding.UTF8 );
            sw.Write( data );
            sw.Close();
            AssetDatabase.Refresh();
        }

        public static void SaveData( ScriptableObject data, string path, string fileName, bool checkIfAlreadyExist = true ) {
            if( !Directory.Exists( path ) )
                throw new DirectoryNotFoundException();
            if( string.IsNullOrEmpty( path ) )
                throw new ArgumentException( "SavePath is null." );
            if( string.IsNullOrEmpty( fileName ) )
                throw new ArgumentException( "FileName is null." );

            var savePath = pathSlashFix( Path.Combine( path, fileName ) );
            if( File.Exists( savePath ) ) {
                if( checkIfAlreadyExist ) {
                    if( !EditorUtility.DisplayDialog( "The file already exists.", "Are you sure you want to overwrite? : " + savePath, "ok", "cancel" ) ) {
                        Debug.LogWarning( "SpreadSheetConvertTool: Save process was canceled." );
                        return;
                    }
                }
            }

            AssetDatabase.CreateAsset( data, savePath );
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private const string forwardSlash = "/";
        private const string backSlash = "\\";
        private static string pathSlashFix( string path ) {
            return path.Replace( backSlash, forwardSlash );
        }

#endif

    }

}