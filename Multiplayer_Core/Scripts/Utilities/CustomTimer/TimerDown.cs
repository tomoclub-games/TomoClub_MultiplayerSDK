
namespace TomoClub.Util
{
	public class TimerDown : Timer
	{

		public TimerDown(int timerEndTime) : base(timerEndTime)
		{
			//Default Constructor
		}

		public TimerDown()
		{

		}

		protected override void AssignTimerStartValue()
		{
			currentTime = timerEndTime;
		}

		protected override void UpdateTimerValue()
		{
			currentTime -= 1;
		}

		protected override bool CheckForCompletion()
		{
			if (currentTime <= 0)
				return true;
			else
				return false;

		}
	} 
}
