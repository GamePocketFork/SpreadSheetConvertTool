using UnityEditor;
using UnityEngine;

namespace charcolle.SpreadSheetConverter {

    internal abstract class EditorWindowItem<T> {

        protected T data;

        public EditorWindowItem( T data ) {
            this.data = data;
        }

        public T Data {
            get {
                return data;
            }
        }

        public void OnGUI() {
            if( data == null ) {
                DrawIfDataIsNull();
                return;
            }
            EditorGUI.BeginChangeCheck();
            Draw();
            if( EditorGUI.EndChangeCheck() )
                GUI.changed = true;
        }

        protected abstract void Draw();
        protected virtual void DrawIfDataIsNull() { }

    }

}