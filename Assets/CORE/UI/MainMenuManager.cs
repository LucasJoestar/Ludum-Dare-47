// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using TMPro; 

namespace LudumDare47
{
	public class MainMenuManager : MonoBehaviour
    {
		#region Fields / Properties
		private static Resolution[] resolutions ;
		private static readonly uint sound_volume_id = AkSoundEngine.GetIDFromString("sound_volume");
		[HorizontalLine(1, order = 0), Section("OPTIONS", order = 1)]
		[SerializeField] private TextMeshProUGUI resolutionDisplayer = null;
		[SerializeField] private UnityEngine.UI.Toggle fullScreenCheckMark = null; 
		private int currentResolutionIndex = 0; 
		#endregion

		#region Methods
		public void StartGame() => GameManager.Instance.LoadNextLevel();

		public void QuitGame() => Application.Quit(); 

		public void SetSoundValue(float _value) => AkSoundEngine.SetRTPCValue(sound_volume_id, _value);

		public void SelectNextRes()
		{
			currentResolutionIndex++;
			if (currentResolutionIndex == resolutions.Length) currentResolutionIndex = 0; 
			if (resolutionDisplayer) resolutionDisplayer.text = $"{resolutions[currentResolutionIndex].width} x {resolutions[currentResolutionIndex].height}";
			Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].width, Screen.fullScreen);
		}

		public void SelectPreviousRes()
		{
			currentResolutionIndex--;
			if (currentResolutionIndex < 0 ) currentResolutionIndex = resolutions.Length-1;
			if (resolutionDisplayer) resolutionDisplayer.text = $"{resolutions[currentResolutionIndex].width} x {resolutions[currentResolutionIndex].height}";
			Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].width, Screen.fullScreen); 
		}

		public void SetFullScreen(bool _isFullScreen) => Screen.fullScreen = _isFullScreen;

		// ------------------------- // 
		private void Start()
		{
			resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				if(resolutions[i].height == Screen.currentResolution.height && resolutions[i].width == Screen.currentResolution.width)
				{
					currentResolutionIndex = i;
					if (resolutionDisplayer) resolutionDisplayer.text = $"{resolutions[i].width} x {resolutions[i].height}"; 
				}
			}
			if (fullScreenCheckMark)
				fullScreenCheckMark.isOn = Screen.fullScreen;
		}
		#endregion
	}
}
