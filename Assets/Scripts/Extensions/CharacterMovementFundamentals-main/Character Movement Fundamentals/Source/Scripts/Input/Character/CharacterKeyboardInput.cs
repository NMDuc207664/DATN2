using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.Logics.Controllers;
using DATN2.Assets.Scripts.Modals.Enum;
using UnityEngine;

namespace CMF
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
	public class CharacterKeyboardInput : CharacterInput
	{
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";
		public KeyCode jumpKey = KeyCode.Space;

		//If this is enabled, Unity's internal input smoothing is bypassed;
		public bool useRawInput = true;

		public override float GetHorizontalMovementInput()
		{
			if (KeyGameStateManager.Instance.IsInState(InGameActionType.None))
			{
				if (useRawInput)
					return Input.GetAxisRaw(horizontalInputAxis);
				else
					return Input.GetAxis(horizontalInputAxis);
			}
			return 0f;
		}

		public override float GetVerticalMovementInput()
		{
			if (KeyGameStateManager.Instance.IsInState(InGameActionType.None))
			{
				if (useRawInput)
					return Input.GetAxisRaw(verticalInputAxis);
				else
					return Input.GetAxis(verticalInputAxis);
			}
			return 0f;
		}

		public override bool IsJumpKeyPressed()
		{
			if (KeyGameStateManager.Instance.IsInState(InGameActionType.None))
			{
				return Input.GetKey(jumpKey);
			}
			return false;
		}
	}
}
