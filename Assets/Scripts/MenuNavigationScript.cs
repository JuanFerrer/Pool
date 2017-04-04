using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigationScript : MonoBehaviour {

    // Use this for initialization
    public void LoadByIndex(int sceneIndex)
    {
<<<<<<< HEAD
        Time.timeScale = 1;
=======
>>>>>>> refs/remotes/origin/master
        SceneManager.LoadScene(sceneIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
