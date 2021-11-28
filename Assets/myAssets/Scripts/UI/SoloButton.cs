using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoloButton : MonoBehaviour
{
    public void ButtonClick()
    {
        StartCoroutine(LoadSoloMode());
    }

    private IEnumerator LoadSoloMode()
    {
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene("SoloMode");
        yield break;
    }
}
