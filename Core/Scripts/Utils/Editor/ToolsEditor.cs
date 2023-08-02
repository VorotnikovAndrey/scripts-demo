using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlayVibe
{
    public static class ToolsEditor
    {
        private static string UserModelPath => Path.Combine(Application.persistentDataPath, _userModelFileName);
        private const string _userModelFileName = "userData.data";

        [MenuItem("Tools/User/Delete User")]
        public static void DeleteUser()
        {
            File.Delete(UserModelPath);
            Debug.Log($"User deleted {UserModelPath.AddColorTag(Color.yellow)}".AddColorTag(Color.green));
        }
    }
}