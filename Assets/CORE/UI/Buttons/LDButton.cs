// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

namespace LudumDare47
{
	public class LDButton : Button
    {
		#region Fields / Properties
		//[HorizontalLine(1, order = 0), Section("LDButton", order = 1)]
		private static readonly uint play_menu_hover_ID = AkSoundEngine.GetIDFromString("Play_menu_hover");
		private static readonly uint play_menu_click_ID = AkSoundEngine.GetIDFromString("Play_menu_click");

		#endregion

		#region Methods
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			AkSoundEngine.PostEvent(play_menu_hover_ID, UIManager.Instance.gameObject); 
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			AkSoundEngine.PostEvent(play_menu_click_ID, UIManager.Instance.gameObject);
		}
		#endregion
	}
}
