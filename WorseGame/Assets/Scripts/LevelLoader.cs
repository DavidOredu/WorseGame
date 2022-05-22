﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : SingletonDontDestroy<LevelLoader>
{
    private int currentScene;
    public TextMeshProUGUI progressText;
    public Slider slider;
    public GameObject loadingScreen;
    private void Start()
    {

    }
    private void Update()
    {
        var GetActiveScene = SceneManager.GetActiveScene().buildIndex;
        currentScene = GetActiveScene;
      
    }
    public void LoadLevel(int sceneIndex)
    {
        EndScene();
       LoadAsynchronously(sceneIndex);
    }
    public void LoadLevel(string sceneName)
    {
        EndScene();
        LoadAsynchronously(sceneName);
    }


    public void Reload()
    {
        LoadAsynchronously(currentScene);
       
    }

    public void LoadAsynchronously(int sceneIndex)
    {
       // AsyncOperation operation = 
            SceneManager.LoadSceneAsync(sceneIndex);
        
        //loadingScreen.SetActive(true);
       

        //while (!operation.isDone)
        //{
        //    float progress = Mathf.Clamp01(operation.progress / .9f);

        //    slider.value = progress;
        //    progressText.text = (progress * 100f).ToString("0").Normalize() + "%";

        //    yield return null;
            
        //}
        //if (operation.isDone)
        //{
        //    loadingScreen.SetActive(false);
        //}
    }
    public void LoadAsynchronously(string sceneName)
    {
        // AsyncOperation operation = 
        SceneManager.LoadSceneAsync(sceneName);
        
        //loadingScreen.SetActive(true);
       

        //while (!operation.isDone)
        //{
        //    float progress = Mathf.Clamp01(operation.progress / .9f);

        //    slider.value = progress;
        //    progressText.text = (progress * 100f).ToString("0").Normalize() + "%";

        //    yield return null;
            
        //}
        //if (operation.isDone)
        //{
        //    loadingScreen.SetActive(false);
        //}
    }
    public IEnumerator LoadAsynchronouslyNoChange(AsyncOperation operation)
    {

        loadingScreen.SetActive(true);


        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;
            progressText.text = (progress * 100f).ToString("0").Normalize() + "%";

            yield return null;

        }
        if (operation.isDone)
        {
            loadingScreen.SetActive(false);
        }
    }
    void EndScene()
    {
        SceneManager.UnloadSceneAsync(currentScene);
    }
}