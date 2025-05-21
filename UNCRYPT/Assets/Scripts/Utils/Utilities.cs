using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class Utilities
    {
        public static class JsonHelper
        {
            public static T[] FromJson<T>(string json)
            {
                string newJson = "{ \"array\": " + json + "}";
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
                return wrapper.array;
            }

            [System.Serializable]
            private class Wrapper<T>
            {
                public T[] array;
            }

        
        }
    
        public static int GetRandomDir()
        {
            return Random.Range(0, 100) < 50 ? 1 : -1;
        }

        public static int ComputeFNV1aHash(this string str)
        {
            uint hash = 2166136261;
            foreach (char c in str)
            {
                hash = (hash ^ c) * 16777619;
            }
            return unchecked((int)hash);
        }

        public static void DrawCylinder(Vector3 pos, float radius, float height, Color color = default)
        {
            #if UNITY_EDITOR
            Handles.color = color;
        
            Handles.DrawWireDisc(pos, Vector3.up, radius);
            Handles.DrawWireDisc(pos + Vector3.up * height, Vector3.up, radius);
        
            DrawUpLine(pos + Vector3.right * radius, height);
            DrawUpLine(pos + Vector3.right * -1 * radius, height);
            DrawUpLine(pos + Vector3.forward * radius, height);
            DrawUpLine(pos + Vector3.forward * -1 * radius, height);
            #endif
        }
    
        private static void DrawUpLine(Vector3 pos, float height)
        {
            #if UNITY_EDITOR
            Handles.DrawLine(pos, pos + Vector3.up * height);
            #endif
        }

        public static bool RadialCheck(Vector3 from, Vector3 to, float dist)
        {
            float distSqr = dist * dist; // Avoid using Mathf.Sqrt for performance
            return (to - from).sqrMagnitude <= distSqr;
        }


        public static float ExpLerp(float start, float end, float speed)
        {
            return Mathf.Lerp(start, end, 1 - Mathf.Exp(-speed));
        }
    
        public static IEnumerator InvokeLater(Action action)
        {
            yield return new WaitForEndOfFrame();
            action.Invoke();
        }

        public static IEnumerator WaitAndExecute(Action action, Func<bool> condition)
        {
            yield return new WaitUntil(condition);
            action.Invoke();
        }
        
        public static void SetAlphaToZero(SpriteRenderer spriteRenderer)
        {
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
        }

    }
}