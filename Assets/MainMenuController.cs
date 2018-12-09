using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

	[SerializeField] private CanvasGroup mainMenuCanvas;
	[SerializeField] private CanvasGroup creditsCanvas;
	
	[SerializeField] private Button startButton;
	[SerializeField] private Button demoButton;
	[SerializeField] private Button creditsButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private Button backButtons;
	[SerializeField] private Outline titleTextOutline;
	[SerializeField] private List<Color> titleColors;
	[SerializeField] private float titleTextAnimationDuration;

	private int titleColorIndex;
	
	private void Awake()
	{
		SetActiveMainMenuCanvas(true);
		
		startButton.onClick.AddListener(OnStartClicked);
		demoButton.onClick.AddListener(OnDemoClicked);
		creditsButton.onClick.AddListener(OnCreditsClicked);
		exitButton.onClick.AddListener(OnExitClicked);
		backButtons.onClick.AddListener(() => SetActiveMainMenuCanvas(true));
	}

	private void Start()
	{
		StartCoroutine(DoTitleAnimation());
	}

	private void OnStartClicked()
	{
		GameSettings.I.DemoMode = false;
		GameSettings.I.MaxLevel = 7;
		SceneManager.LoadScene("GameScene");
	}

	private void OnDemoClicked()
	{
		GameSettings.I.DemoMode = true;
		GameSettings.I.MaxLevel = 12;
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
		mainMenuCanvas.alpha = active ? 1f : 0f;
		creditsCanvas.alpha = active ? 0f : 1f;
		backButtons.gameObject.SetActive(!active);
	}

	private IEnumerator DoTitleAnimation()
	{
		while (true)
		{
			var startColor = titleColors[titleColorIndex];
			titleColorIndex = (titleColorIndex + 1) % titleColors.Count;
			var endColor = titleColors[titleColorIndex];
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / titleTextAnimationDuration)
			{
				titleTextOutline.effectColor = Color.Lerp(startColor, endColor, t);
				yield return null;
			}
		}
	}
}
