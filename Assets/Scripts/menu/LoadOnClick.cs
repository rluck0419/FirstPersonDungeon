using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

	public void LoadScene(int level) {
		SceneManager.LoadSceneAsync (level);
	}
}
