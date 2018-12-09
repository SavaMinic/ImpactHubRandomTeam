using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

	[SerializeField] private GameObject mainMenuCanvas;
	[SerializeField] private GameObject creditsCanvas;
	
	[SerializeField] private Button startButton;
	[SerializeField] private Button creditsButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private Button backButtons;
	
	private void Awake()
	{
		SetActiveMainMenuCanvas(true);
		
		startButton.onClick.AddListener(() => OnStartClicked());
		creditsButton.onClick.AddListener(() => OnCreditsClicked());
		exitButton.onClick.AddListener(() => OnExitClicked());
		backButtons.onClick.AddListener(() => SetActiveMainMenuCanvas(true));
	}

	private void OnStartClicked()
	{
		SceneManager.LoadScene("GameScene");
	}

	private void OnCreditsClicked()
	{	
		SetActiveMainMenuCanvas(false);
	}
	
	private void OnExitClicked()
	{
		Application.Quit();
	}

	private void SetActiveMainMenuCanvas(bool active)
	{
		mainMenuCanvas.SetActive(active);
		creditsCanvas.SetActive(!active);
	}
}
