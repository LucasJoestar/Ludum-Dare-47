// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace LudumDare47
{
    [Serializable]
    public class Dialog
    {
        public int ID = 1059;
        public int NextID = 0;
        public float Duration = 0;

        public string Sentence = string.Empty;
        public Sprite Sprite = null;
    }

    [CreateAssetMenu(fileName = "DAT_DialogDatabase", menuName = "Datas/Dialog Database", order = 50)]
    public class DialogDatabase : ScriptableObject
    {
        #region Dialogs
        [HorizontalLine(1, order = 0), Section("DIALOG DATABASE", order = 1)]

        [SerializeField] private Dialog[] dialogs = new Dialog[] { };

        // -----------------------

        public Dialog GetDialog(int _id)
        {
            for (int _i = 0; _i < dialogs.Length; _i++)
            {
                Dialog _dialog = dialogs[_i];
                if (_dialog.ID == _id)
                    return _dialog;
            }
            return null;
        }
        #endregion
    }
}
