using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace YiNing.Fsm
{
	public class FiniteStateMachine<TState, TComand> where TState : IComparable
	{
		/// <summary>
		/// Gets the previousState
		/// </summary>
		public TState PreviousState { get; private set; }
		/// <summary>
		/// Gets the current state
		/// </summary>
		public TState CurrentState { get; private set; }

		/// <summary>
		/// Gets a boolean value indicating if the state machine is currently transitioning.
		/// </summary>
		public bool IsTransitioning { get { return currentTransition != null; } }

		public string Name { get; set; }

		private Transition<TState> currentTransition;
		private readonly Dictionary<TState, StateController> states;
		
		private readonly Dictionary<TState, Dictionary<TComand, Transition<TState>>> transitions;
		private bool isInitialisingState;
		private string stateMachineName;

		private event Action<TState> OnStateEnter;
		private event Action<TState> OnStateExit;
		private event Action<TState, TState> OnStateChange;

		/// <summary>
		/// Instantiates a new FiniteStateMachine using the enum values in an enum defined by the type parameter.
		/// The type parameter must be an enum type
		/// </summary>
		/// <returns></returns>
		public static FiniteStateMachine<TState, TComand> FromEnum()
		{
			if (!typeof(Enum).IsAssignableFrom(typeof(TState)))
			{
				throw new Exception("Cannot create finite");
			}

			var states = new List<TState>();
			foreach (TState value in Enum.GetValues(typeof(TState)))
			{
				states.Add(value);
			}

			return new FiniteStateMachine<TState, TComand>(states.ToArray());
		}

		/// <summary>
		/// Instantiates a new state machine using the provied states.
		/// </summary>
		/// <param name="states">The states that are used for the state machine</param>
		public FiniteStateMachine(params TState[] states)
		{
			if (states.Length < 1) { throw new ArgumentException("A FiniteStateMachine needs at least 1 state", "states"); }

			transitions = new Dictionary<TState, Dictionary<TComand, Transition<TState>>>();
			this.states = new Dictionary<TState, StateController>();
			foreach (var value in states)
			{
				this.states.Add(value, new StateController());
				transitions.Add(value, new Dictionary<TComand, Transition<TState>>());
			}
		}

		/// <summary>
		/// Adds a new transition between the 2 given states when the given command is issued.
		/// </summary>
		/// <param name="from">The from state of the transititon</param>
		/// <param name="to">The to state of the transition</param>
		/// <param name="command">The command that will trigger this transition</param>
		/// <param name="transition">[Optional] transition, if not provided, a DefaultTransition is used, that completes instantly</param>
		/// <returns>The instance of FiniteStatemachine to comply with fluent interface pattern</returns>
		public FiniteStateMachine<TState, TComand> AddTransition(TState from, TState to, TComand command, Transition<TState> transition = null)
		{
			if (!states.ContainsKey(from)) { throw new ArgumentException("unknown state", "from"); }
			if (!states.ContainsKey(to)) { throw new ArgumentException("unknown state", "to"); }

			// add the transition to the db (new it if it does not exist)
			transitions[from][command] = transition ?? new DefaultStateTransition<TState>(from, to);

			return this;
		}

		/// <summary>
		/// 在发出给定命令时，且仅在满足条件时，才在两个给定状态之间添加新的转换
		/// </summary>
		/// <param name="from">from 状态</param>
		/// <param name="to">to 状态</param>
		/// <param name="command">将触发转换的命令</param>
		/// <param name="condition">转换条件方法，只有在触发了之后执行，返回true的时候才会执行状态切换</param>
		/// <returns>this</returns>
		public FiniteStateMachine<TState, TComand> AddTransition(TState from, TState to, TComand command, Func<bool> condition)
		{
			if (from == null) { throw new ArgumentNullException("state"); }
			if (to == null) { throw new ArgumentNullException("to"); }
			if (!states.ContainsKey(from)) { throw new ArgumentException("unknown state", "from"); }
			if (!states.ContainsKey(to)) { throw new ArgumentException("unknown state", "to"); }
            if (command == null) { throw new ArgumentException("command cannot be null or empty", "command"); }

            // add a default transition to the db
            transitions[from][command] = new DefaultStateTransition<TState>(from, to, condition);

			return this;
		}

		/// <summary>
		/// Adds a handler for entry into the given state
		/// </summary>
		/// <param name="state">The state to handle entry for</param>
		/// <param name="handler">The handler</param>
		/// <returns>The instance of FiniteStatemachine to comply with fluent interface pattern</returns>
		public FiniteStateMachine<TState, TComand> OnEnter(TState state, Action handler)
		{
			if (state == null) { throw new ArgumentNullException("state"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }
			if (!states.ContainsKey(state)) { throw new ArgumentException("unknown state", "state"); }

			OnStateEnter += enteredState =>
			{
				if (enteredState.Equals(state))
				{
					handler();
				}
			};

			return this;
		}

		/// <summary>
		/// Adds a handler for exiting from the given state
		/// </summary>
		/// <param name="state">The state to handle exit from</param>
		/// <param name="handler">The handler</param>
		/// <returns>The instance of FiniteStatemachine to comply with fluent interface pattern</returns>
		public FiniteStateMachine<TState, TComand> OnExit(TState state, Action handler)
		{
			if (state == null) { throw new ArgumentNullException("state"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }
			if (!states.ContainsKey(state)) { throw new ArgumentException("unknown state", "state"); }

			OnStateExit += exitedState =>
			{
				if (exitedState.Equals(state))
				{
					handler();
				}
			};
			return this;
		}

		/// <summary>
		/// Adds a handler for any state change. The handler is called after transition, before OnEnter to the new state
		/// </summary>
		/// <param name="handler">A handler that provides the previous and new state</param>
		/// <returns>The instance of FiniteStatemachine to comply with fluent interface pattern</returns>
		public FiniteStateMachine<TState, TComand> OnChange(Action<TState, TState> handler)
		{
			if (handler == null) { throw new ArgumentNullException("handler"); }

			OnStateChange += handler;

			return this;
		}

		/// <summary>
		/// Adds a handler for when the given from state changes to the given to state. The handler is called after transition, before OnEnter to the new state
		/// </summary>
		/// <param name="from">The from state</param>
		/// <param name="to">The to state</param>
		/// <param name="handler">The handler</param>
		/// <returns>The instance of FiniteStatemachine to comply with fluent interface pattern</returns>
		public FiniteStateMachine<TState, TComand> OnChange(TState from, TState to, Action handler)
		{
			if (from == null) { throw new ArgumentNullException("from"); }
			if (to == null) { throw new ArgumentNullException("to"); }
			if (!states.ContainsKey(from)) { throw new ArgumentException("unknown state", "from"); }
			if (!states.ContainsKey(to)) { throw new ArgumentException("unknown state", "to"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }

			OnStateChange += (fromState, toState) =>
			{
				if (fromState.Equals(from) &&
					toState.Equals(to))
				{
					handler();
				}
			};

			return this;
		}

		/// <summary>
		/// Begins the finite state machine setting the initial state. This will not invoke any events
		/// </summary>
		/// <param name="firstState"></param>
		public void Begin(TState firstState)
		{
			if (firstState == null) { throw new ArgumentNullException("firstState"); }
			if (!states.ContainsKey(firstState)) { throw new ArgumentException("unknown state", "firstState"); }

			CurrentState = firstState;
            PreviousState = firstState;
        }

		/// <summary>
		/// 发出命令
		/// </summary>
		/// <param name="command"></param>
		public void IssueCommand(TComand command)
		{
			//Commands set during transitioning will be ignored
			if (IsTransitioning)
				return;

			var transitionsForCurrentState = transitions[CurrentState];
			if (transitionsForCurrentState.ContainsKey(command))
			{
				//Commands should not be issued from code called by
				//OnStateChange and OnStateEnter and will be ignored
				if (isInitialisingState)
				{
					Debug.WriteLine("Do not call IssueCommand from OnStateChange and OnStateEnter handlers");
					return;
				}

				var transition = transitionsForCurrentState[command];

				if (transition.TestCondition())
				{
					transition.OnComplete += HandleTransitionComplete;
					currentTransition = transition;
					if (OnStateExit != null)
					{
						OnStateExit(CurrentState);
					}
					transition.Begin();
				}
			}
		}

		private void HandleTransitionComplete()
		{
			currentTransition.OnComplete -= HandleTransitionComplete;

			var previousState = CurrentState;
			PreviousState = previousState;
			CurrentState = currentTransition.ToState;

			currentTransition = null;

			isInitialisingState = true;

			if (OnStateChange != null)
			{
				OnStateChange(previousState, CurrentState);
			}

			states[CurrentState].Enter();
			if (OnStateEnter != null)
			{
				OnStateEnter(CurrentState);
			}

			isInitialisingState = false;
		}
	}
}