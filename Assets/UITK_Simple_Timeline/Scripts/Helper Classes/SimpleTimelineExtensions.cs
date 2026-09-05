using UnityEngine;
using System.Collections.Generic;
namespace UITK_SimpleTimeline
{
    public static class SimpleTimelineExtensions
    {
        /// <summary>
        /// Copies some sort of class by making an instance of a string using the JsonUtility.ToJson, and then returning
        /// that copy string as the original type using .FromJson. Useful for copying [SerializeReference] values 
        /// without having multiple components/objects referring back to the same reference.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="original">The original object.</param>
        /// <returns>A duplicate of the original object.</returns>
        public static T DeepCopyFromJSON<T>(T original) where T : class
        {
            if (original == null) return null;

            string jason = JsonUtility.ToJson(original);

            return JsonUtility.FromJson(jason, original.GetType()) as T;
        }
        /// <summary>
        /// Copies a list of some sort of class by making an instance of a string using the JsonUtility.ToJson, 
        /// and then returning that copy string as the original type using .FromJson. Useful for copying [SerializeReference] 
        /// values without having multiple components/objects referring back to the same reference.
        /// </summary>
        /// <typeparam name="T">The type of objects being copied.</typeparam>
        /// <param name="originalList">The original list of objects.</param>
        /// <returns>A duplicate of the original list.</returns>
        public static List<T> DeepCopyListFromJSON<T>(List<T> originalList) where T : class
        {
            List<T> toReturn = new();

            foreach(T obj in originalList)
            {
                toReturn.Add(DeepCopyFromJSON(obj));
            }

            return toReturn;
        }
        /// <summary>
        /// Copies an array of some sort of class by making an instance of a string using the JsonUtility.ToJson, 
        /// and then returning that copy string as the original type using .FromJson. Useful for copying [SerializeReference] 
        /// values without having multiple components/objects referring back to the same reference.
        /// </summary>
        /// <typeparam name="T">The type of objects being copied.</typeparam>
        /// <param name="originalArray">The original array of objects.</param>
        /// <returns>A duplicate of the original array.</returns>
        public static T[] DeepCopyArrayFromJSON<T>(T[] originalArray) where T : class
        {
            T[] toReturn = new T[originalArray.Length];
            for (int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = DeepCopyFromJSON(originalArray[i]);
            }
            return toReturn;
        }

        public static async Awaitable<T> DeepCopyFromJSONAsync<T>(T og) where T : class
        {
            await Awaitable.BackgroundThreadAsync();
            return DeepCopyFromJSON(og);
        }

        public static async Awaitable<List<T>> DeepCopyListFromJSONAsync<T>(List<T> originalList) where T : class
        {
            List<T> toReturn = new();

            foreach(T obj in originalList)
            {
                toReturn.Add(await DeepCopyFromJSONAsync(obj));
            }
            await Awaitable.MainThreadAsync();
            return toReturn;
        }

        public static async Awaitable<T[]> DeepCopyArrayFromJSONAsync<T>(T[] originalArray) where T : class
        {
            T[] toReturn = new T[originalArray.Length];
            for(int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = await DeepCopyFromJSONAsync(originalArray[i]);
            }
            await Awaitable.MainThreadAsync();
            return toReturn;
        }

        public static float[] ToArray(this Vector3 v3)
        {
            return new float[] { v3.x, v3.y, v3.z };
        }

        public static Vector3 ToVector3(this float[] floatArray)
        {
            return floatArray.Length switch
            {
                0 => Vector3.zero,
                1 => new Vector3(floatArray[0], 0),
                2 => new Vector3(floatArray[0], floatArray[1]),
                _ => new Vector3(floatArray[0], floatArray[1], floatArray[2])
            };
        }

        public static bool HasNaN(this Vector3 v3)
        {
            return float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z);
        }
        public static bool HasNaN(this Quaternion q)
        {
            return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
        }
    }
}
