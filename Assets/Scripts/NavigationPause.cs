using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationPause : MonoBehaviour
{
    public static bool GameIsPaused;
    public GameObject SettingsMenuUI;
    public GameObject MenuUI;
    public bool ING;
    public GameObject Transition;
    public Transform Center;
    public AudioSource ButtonClick;
    private bool SFXmuted;
    public Sprite SFXon;
    public Sprite SFXoff;
    public GameObject MuterButton;
    public GameObject SFXParent;
    // Start is called before the first frame update
    void Start()
    {
        if (!ING)
        {
            SettingsMenuUI.SetActive(false);
            MenuUI.SetActive(true);
        }

    }

    public void ReturnToMenu()
    {
        ButtonClick.Play();
        Instantiate(Transition, Center.position, Quaternion.identity);
        StartCoroutine(LoadLevelAfterDelay(0.7f, "Menu"));
    }

    public void Resume()
    {
        ButtonClick.Play();
        SettingsMenuUI.SetActive(false);
        MenuUI.SetActive(true);
        GameIsPaused =false;
    }
    public void Settings()
    {
        ButtonClick.Play();
        SettingsMenuUI.SetActive(true);
        MenuUI.SetActive(false);
        GameIsPaused=true;
    }
    public void Play()
    {
        ButtonClick.Play();
        Instantiate(Transition, Center.position, Quaternion.identity);
        StartCoroutine(LoadLevelAfterDelay(0.7f, "Lobby"));
    }
    public void QuitGame()
    {
        ButtonClick.Play();
        Application.Quit();
    }
    IEnumerator LoadLevelAfterDelay(float delay, string scene)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }

    public void SFXMuter()
    {
        ButtonClick.Play();
        if (SFXmuted == false)
        {
            SFXmuted = true;
            MuterButton.GetComponent<UnityEngine.UI.Image>().sprite = SFXoff;
            SFXParent.SetActive(false);
        }
        else
        {
            SFXmuted = false;
            MuterButton.GetComponent<UnityEngine.UI.Image>().sprite = SFXon;
            SFXParent.SetActive(true);
        }
    }
}