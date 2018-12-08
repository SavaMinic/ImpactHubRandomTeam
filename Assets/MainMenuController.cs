using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

	[SerializeField] private Button startButton;
	[SerializeField] private Button creditsButton;
	[SerializeField] private Button exitButton;

	private void Awake()
	{
		startButton.onClick.AddListener(() => OnStartClicked());
		creditsButton.onClick.AddListener(() => OnCreditsClicked());
		exitButton.onClick.AddListener(() => OnExitClicked());
	}

	private void OnStartClicked()
	{
		SceneManager.LoadScene("GameScene");
	}

	private void OnCreditsClicked()
	{	
	}
	
	private void OnExitClicked()
	{
		Application.Quit();
	}

}
