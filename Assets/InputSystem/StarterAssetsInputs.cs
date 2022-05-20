using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		//[Header("Character Input Values")]
		public Vector2 Move { get; private set; }
		public Vector2 Look { get; private set; }
		public bool Jump { get; private set; }
		public bool HoldingJump { get; private set; }
		public bool Sprint { get; private set; }
		public bool Aim { get; private set; }

		// EVENTS
		public UnityEvent<InputAction.CallbackContext> FireEvent;
		public UnityEvent<InputAction.CallbackContext> PauseEvent;

		//[Header("Movement Settings")]
		public bool AnalogMovement { get; private set; }

#if !UNITY_IOS || !UNITY_ANDROID
		//[Header("Mouse Cursor Settings")]
		public bool CursorLocked { get; private set; } = true;
		public bool CursorInputForLook { get; private set; } = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnMove(InputAction.CallbackContext context)
		{
			MoveInput(context.ReadValue<Vector2>());
		}

		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnLook(InputAction.CallbackContext context)
		{
			if(CursorInputForLook)
			{
				LookInput(context.ReadValue<Vector2>());
			}
		}

		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnJump(InputAction.CallbackContext context)
		{
            JumpInput(context.action.triggered);

			// Christian: best way I could figure out how to handle holding of buttons since the input system doesn't work
            if (Jump && context.action.phase == InputActionPhase.Started)
				HoldingJump = true;
			else if (Jump && context.action.phase == InputActionPhase.Canceled)
				HoldingJump = false;
		}

		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnSprint(InputAction.CallbackContext context)
		{
			// TODO: Figure out why the sprint is pressed on the slightest touch on left gamepad stick
			//Debug.Log("Sprint was pressed");
			// hold
			//SprintInput(context.action.ReadValue<float>() == 1);

			// toggle
			if (context.action.ReadValue<float>() == 1)
				SprintInput(!Sprint);
		}

		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnAim(InputAction.CallbackContext context)
		{
			AimInput(context.action.IsPressed());
		}

		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnFire(InputAction.CallbackContext context)
		{
			FireEvent.Invoke(context);
		}

		/// <summary>
		/// Public to assign actions in inspector for PlayerInput
		/// </summary>
		public void OnPause(InputAction.CallbackContext context)
		{
			PauseEvent.Invoke(context);
		}
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			Move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			Look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			Jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			Sprint = newSprintState;
		}

		public void AimInput(bool newAimState)
		{
			Aim = newAimState;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(CursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}