// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using System.Collections.Generic;
using UnityEngine;

namespace LudumDare47
{
    #region Update Interfaces
    // -------------------------------------------
    // Update Interfaces
    // -------------------------------------------

    public interface IUpdate                { void Update(); }
    public interface IInputUpdate           { void Update(); }
    public interface IMovableUpdate         { void Update(); }
    public interface ILateUpdate            { void Update(); }
    #endregion

    public class UpdateManager : MonoBehaviour
    {
        #region Fields / Properties
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static UpdateManager Instance => GameManager.Instance.UpdateManager;

        // -----------------------

        private List<IUpdate> updates = new List<IUpdate>();
        private List<IInputUpdate> inputUpdates = new List<IInputUpdate>();
        private List<IMovableUpdate> movableUpdates = new List<IMovableUpdate>();
        private List<ILateUpdate> lateUpdates = new List<ILateUpdate>();
        #endregion

        #region Methods

        #region Registrations
        /// <summary>
        /// Registers an object on global update.
        /// </summary>
        public void Register(IUpdate _update) => updates.Add(_update);

        /// <summary>
        /// Unregisters an object from global update.
        /// </summary>
        public void Unregister(IUpdate _update) => updates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Registers an object on Input update.
        /// </summary>
        public void Register(IInputUpdate _update) => inputUpdates.Add(_update);

        /// <summary>
        /// Unregisters an object from Input update.
        /// </summary>
        public void Unregister(IInputUpdate _update) => inputUpdates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Register an object on Movable update.
        /// </summary>
        public void Register(IMovableUpdate _update) => movableUpdates.Add(_update);

        /// <summary>
        /// Unregister an object from Movable update.
        /// </summary>
        public void Unregister(IMovableUpdate _update) => movableUpdates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Register an object on Late update.
        /// </summary>
        public void Register(ILateUpdate _update) => lateUpdates.Add(_update);

        /// <summary>
        /// Unregister an object from Late update.
        /// </summary>
        public void Unregister(ILateUpdate _update) => lateUpdates.Remove(_update);
        #endregion

        #region Management
        public void ResetUpdates()
        {
            updates.Clear();
            inputUpdates.Clear();
            movableUpdates.Clear();
            lateUpdates.Clear();
        }
        #endregion

        #region Monobehaviour
        private void Update()
        {
            // Call all registered interfaces update.
            int _i;
            for (_i = 0; _i < inputUpdates.Count; _i++)
                inputUpdates[_i].Update();

            for (_i = 0; _i < updates.Count; _i++)
                updates[_i].Update();

            for (_i = 0; _i < movableUpdates.Count; _i++)
                movableUpdates[_i].Update();

            for (_i = 0; _i < lateUpdates.Count; _i++)
                lateUpdates[_i].Update();
        }
        #endregion

        #endregion
    }
}
