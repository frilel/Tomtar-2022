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
		public bool Sprint { get; private set; }
		public bool Aim { get; private set; }
		public bool Fire { get; private set; }
		public UnityEvent<InputAction.CallbackContext> FireEvent;

		//[Header("Movement Settings")]
		public bool AnalogMovement { get; private set; }

#if !UNITY_IOS || !UNITY_ANDROID
		//[Header("Mouse Cursor Settings")]
		public bool CursorLocked { get; private set; } = true;
		public bool CursorInputForLook { get; private set; } = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputAction.CallbackContext value)
		{
			MoveInput(value.ReadValue<Vector2>());
		}

		public void OnLook(InputAction.CallbackContext value)
		{
			if(CursorInputForLook)
			{
				LookInput(value.ReadValue<Vector2>());
			}
		}

		public void OnJump(InputAction.CallbackContext value)
		{
			JumpInput(value.action.triggered);
		}

		public void OnSprint(InputAction.CallbackContext value)
		{
			SprintInput(value.action.ReadValue<float>() == 1);
		}

		public void OnAim(InputAction.CallbackContext value)
		{
			AimInput(value.action.IsPressed());
		}

		public void OnFire(InputAction.CallbackContext value)
		{
			FireEvent.Invoke(value);
			FireInput(value.action.triggered);
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

		public void FireInput(bool newFireState)
		{
			Fire = newFireState;
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