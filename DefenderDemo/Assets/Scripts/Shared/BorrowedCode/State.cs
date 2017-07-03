using UnityEngine;

namespace PGeneric.FSM
{
    /// <summary>
    /// Interface for singleton states
    /// </summary>
    public interface State<T>
	{
        void Enter(T entity);
        void Execute(T entity);
        void Exit(T entity);
        void FixedExecute(T entity);

        void OnClicked(T entity);

        void Message(T entity, string message);
	}
}
