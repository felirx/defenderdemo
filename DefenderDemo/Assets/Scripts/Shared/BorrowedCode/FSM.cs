using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PGeneric.FSM
{
    /// <summary>
    /// Fixed State Machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSM<T>
    {
        public T Owner;
        public State<T> CurrentState, PreviousState;
        public State<T> CurrentGlobalState, PreviusGlobalState;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Staring state</param>
        /// <param name="GlobalState">Optional Global state </param>
        public FSM(T owner, State<T> state, State<T> GlobalState = null)
        {
            if (owner == null)
                Debug.LogError("owner is null");

            if (state == null)
                Debug.LogError("Starting or Global state cant be null");

            Owner = owner;
            CurrentState = state;
            CurrentState.Enter(owner);

            if (GlobalState != null)
            {
                CurrentGlobalState = GlobalState;
                CurrentGlobalState.Enter(owner);
            }
        }
        /// <summary>
        /// Calls exit on current state, changes the state and calls
        /// enter on the newly selected state.
        /// </summary>
        /// <param name="nextState"></param>
        public void ChangeState(State<T> nextState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit(Owner);
                PreviousState = CurrentState;
            }

            Debug.Log("New State: " + nextState.ToString());
            CurrentState = nextState;
            CurrentState.Enter(Owner);
        }
        /// <summary>
        /// Calls exit on current state, changes the state and calls
        /// enter on the newly selected state.
        /// </summary>
        /// <param name="nextState"></param>
        public void GlobalChangeState(State<T> nextState)
        {
            if (CurrentGlobalState != null)
            {
                CurrentGlobalState.Exit(Owner);
                PreviusGlobalState = CurrentState;
            }
            CurrentGlobalState = nextState;
            CurrentGlobalState.Enter(Owner);
        }
        /// <summary>
        /// calls Exit on current state and changes the state back
        /// to previously selected state and calls it's Enter method.
        /// </summary>
        public void BackToPreviousState()
        {
            if (PreviousState == null)
                return;

            CurrentState.Exit(Owner);
            CurrentState = PreviousState;
            CurrentState.Enter(Owner);
        }
        public void GlobalBackToPreviousState()
        {
            if (PreviusGlobalState == null)
                return;

            CurrentGlobalState.Exit(Owner);
            CurrentGlobalState = PreviusGlobalState;
            CurrentGlobalState.Enter(Owner);
        }
        public void Update()
        {
            if (CurrentGlobalState != null)
                CurrentGlobalState.Execute(Owner);

            //Debug.Log(CurrentState);
            CurrentState.Execute(Owner);
        }
        public void FixedUpdate()
        {
            if (CurrentGlobalState != null)
                CurrentGlobalState.FixedExecute(Owner);

            CurrentState.FixedExecute(Owner);
        }
    }
}
