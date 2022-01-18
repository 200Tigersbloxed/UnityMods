using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomLabratNPC
{
    public static class LabratTools
    {
        public static bool HasComponent<T>(GameObject obj) => obj.GetComponent<T>() != null;

        public static GameObject LocalPlayer =>
            SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(x => x.name == "PlayerV2");

        public static bool IsGameScene(Scene scene = default)
        {
            if (scene == default)
                scene = SceneManager.GetActiveScene();
            return scene.name == "Game";
        }
        
        public static PlayerStats _playerStats;
        public static bool isLocalPlayerDead
        {
            get
            {
                if (_playerStats != null)
                    return _playerStats.dead;
                _playerStats = LocalPlayer.GetComponent<PlayerStats>();
                return SceneManager.GetActiveScene().name == "Game";
            }
        }

        public static bool IsSCPMoving([NotNull] EnemyController ec)
        {
            Type ecType = ec.GetType();
            FieldInfo isMoving_Field =
                ecType.GetField("isMoving", BindingFlags.NonPublic | BindingFlags.Instance);
            if (isMoving_Field != null)
            {
                return (bool) isMoving_Field.GetValue(ec);
            }
            LogHelper.Warn($"No Field isMoving found for {ec.gameObject.name}");

            return false;
        }
    }
}