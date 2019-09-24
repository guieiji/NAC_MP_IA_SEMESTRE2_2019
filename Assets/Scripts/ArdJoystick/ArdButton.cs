namespace ArdJoystick
{
	public enum ArdKeyCode
	{
		BUTTON_A,
		BUTTON_B,
		BUTTON_X,
		BUTTON_Y,
        BUTTON_SELECT,
        BUTTON_START,
        BUTTON_UP,
        BUTTON_LEFT,
        BUTTON_RIGHT,
        BUTTON_DOWN,
        Luz

    }

	public class ArdButton
	{
		private ArdKeyCode keyCode;

		public bool keyDown = false;
		public bool keyPress = false;
		public bool keyUp = false;

		private int oldData = 0;

		public ArdButton(ArdKeyCode keyCode)
		{
			this.keyCode = keyCode;
		}

		public void ProcessData(int data)
		{
			if (data == 1 && oldData == 0)
			{
				keyDown = true;
			}
			else if (data == 0 && oldData == 1)
			{
				keyUp = true;
			}

			if (data == 1)
			{
				keyPress = true;
			}
			else
			{
				keyPress = false;
			}

			oldData = data;
		}
	}
}
