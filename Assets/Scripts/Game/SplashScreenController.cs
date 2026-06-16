using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    public float tiempoDeEspera = 3f;

    private void Start()
    {
        StartCoroutine(ContadorSplash());
    }

    private IEnumerator ContadorSplash()
    {
        yield return new WaitForSeconds(tiempoDeEspera);

        SceneManager.LoadScene(2);
    }
}