using UnityEngine.SceneManagement;

namespace CoreSystems
{
    public static class SceneRouter
    {
        public const string Loading = "LoadingScene";
        public const string Login = "LoginScene";
        public const string Main = "MainScene";
        public const string Town = "TownScene";
        
        public static void Go(string sceneName) => SceneManager.LoadScene(sceneName);
    }
}