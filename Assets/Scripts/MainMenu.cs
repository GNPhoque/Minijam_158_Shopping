using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static void StartGameScene()
	{
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
}
