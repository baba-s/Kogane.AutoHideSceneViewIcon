using System;
using System.Reflection;
using UnityEditor;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class AutoHideSceneViewIcon
    {
        private static readonly EditorFirstBootChecker CHECKER = new( nameof( AutoHideSceneViewIcon ) );

        static AutoHideSceneViewIcon()
        {
            if ( !CHECKER.IsFirstBoot() ) return;

            EditorApplication.delayCall += () =>
            {
                var annotation  = Type.GetType( "UnityEditor.Annotation, UnityEditor" );
                var classId     = annotation.GetField( "classID" );
                var scriptClass = annotation.GetField( "scriptClass" );

                var annotationUtility = Type.GetType( "UnityEditor.AnnotationUtility, UnityEditor" );
                var getAnnotations    = annotationUtility.GetMethod( "GetAnnotations", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );
                var setIconEnabled    = annotationUtility.GetMethod( "SetIconEnabled", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );

                var annotations = getAnnotations.Invoke( null, null ) as Array;

                foreach ( var n in annotations )
                {
                    var parameters = new object[]
                    {
                        ( int )classId.GetValue( n ),
                        ( string )scriptClass.GetValue( n ),
                        0,
                    };

                    setIconEnabled.Invoke( null, parameters );
                }
            };
        }
    }
}