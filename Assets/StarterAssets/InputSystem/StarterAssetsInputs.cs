using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool dash;
		public bool jump;
		public bool sprint;
		public bool attack1;
		public bool attack2;
        public float axis;
        public bool swap;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnDash(InputValue value)
		{
			DashInput(value.isPressed);
		}

		public void OnAttack1(InputValue value)
		{
			Attack1Input(value.isPressed);
		}

		public void OnAttack2(InputValue value)
		{
			Attack2Input(value.isPressed);
		}

        public void OnSwitch(InputValue value)
        {
            SwitchInput(value.Get<float>());
        }

        public void OnSwitch1(InputValue value)
        {
            SwitchInput1(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void DashInput(bool newDashState)
		{
			dash = newDashState;
		}
		public void Attack1Input(bool newAttack1State)
		{
			attack1 = newAttack1State;
		}

		public void Attack2Input(bool newAttack2State)
		{
			attack2 = newAttack2State;
		}

        public void SwitchInput1(bool newSwitch2State)
        {
            swap = newSwitch2State;
        }

        public void SwitchInput(float newSwitchState)
        {
            axis = newSwitchState;
        }

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

}