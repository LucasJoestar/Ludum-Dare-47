// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	[CreateAssetMenu(fileName="Finite State Machine", menuName="AI/Finite State Machine", order=50)]
	public class FiniteStateMachine : ScriptableObject
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("FiniteStateMachine", order = 1)]
        [SerializeField] private BaseState[] behaviourStates = new BaseState[] { };
        public BaseState[] BehaviourStates => behaviourStates;

        [SerializeField] private int startStateIndex = 0;

        private bool isActive = true;
        public bool IsActive => isActive; 
        private bool hasToReset = false;

        public EnemyController Controller { get; private set; }
        #endregion

        #region Methods
        public void GoToState(BaseState _previousSate, StateType _nextType)
        {
            _previousSate.OnExitState();
            if (!isActive) return;
            if(hasToReset)
            {
                hasToReset = false;
                behaviourStates[startStateIndex].OnEnterState(this);
                return; 
            }
            for (int i = 0; i < behaviourStates.Length; i++)
            {
                if (behaviourStates[i].StateType == _nextType)
                {
                    behaviourStates[i].OnEnterState(this);
                    return;
                }
            }
        }

        // ----------------- //

        public void StartFSM(EnemyController _controller)
        {
            Controller = _controller;
            behaviourStates[startStateIndex].OnEnterState(this);
        }

        public void StopFSM() => isActive = false;

        public void ResetFSM()
        {
            if(!isActive)
            {
                isActive = true;
                behaviourStates[startStateIndex].OnEnterState(this);
                return; 
            }
            hasToReset = true; 
        }

        // ----------------- //

        public FiniteStateMachine Copy()
        {
            FiniteStateMachine _copiedMachine = Instantiate(this);
            _copiedMachine.isActive = true; 
            for (int i = 0; i < behaviourStates.Length; i++)
            {
                _copiedMachine.BehaviourStates[i] = Instantiate(behaviourStates[i]);
            }
            return _copiedMachine;
        }
        #endregion
    }
}
