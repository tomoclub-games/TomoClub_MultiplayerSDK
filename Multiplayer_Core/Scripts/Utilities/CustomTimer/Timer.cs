using UnityEngine;
using System;

namespace TomoClub.Util
{
	public abstract class Timer
	{
		protected int currentTime; //in seconds
		protected int timerEndTime; // in seconds

		protected float perFrameTime;//timer per frame (for frame rate independence)

		private bool hasStarted = false;// to check for timer start
		private bool isPaused = false;// to check for timer pause

		public Action<int> TimerUpdatePerSecond; //to update per second activity for that timer
		public Action TimerCompleted; //to update when timer is completed

		/// <summary>
		/// Sets the timer with value of timerEndTime
		/// </summary>
		public Timer(int timerEndTime)
		{
			this.timerEndTime = timerEndTime;
			AssignTimerStartValue();
		}

		/// <summary>
		/// Sets the timer with default value of 5 seconds
		/// </summary>
		public Timer()
		{
			timerEndTime = 5;
			AssignTimerStartValue();
		}


		//Update Timer should run per frame
		public void UpdateTimer()
		{
			if (!hasStarted || isPaused) return;

			TimerCount();

			if (CheckForCompletion())
			{
				hasStarted = false;
				AssignTimerStartValue();
				TimerCompleted?.Invoke();
			}

		}

		private void TimerCount()
		{
			perFrameTime += Time.deltaTime;

			if (perFrameTime >= 1f)
			{
				perFrameTime = 0f;
				UpdateTimerValue();
				TimerUpdatePerSecond?.Invoke(currentTime);
			}
		}

		protected abstract void UpdateTimerValue();

		protected abstract bool CheckForCompletion();


		public void PauseTimer()
		{
			if (hasStarted) isPaused = true;
		}

		public void PlayTimer()
		{
			if (hasStarted) isPaused = false;
		}


		public void ResetTimer()
		{
			hasStarted = false;
			AssignTimerStartValue();
		}

		/// <summary>
		/// Resets from a different point and start the timer
		/// </summary>
		public void SetAndStartTimer(int timerResetFrom)
		{
			//Stop Timer
			hasStarted = false;
			SetTimer(timerResetFrom);
			StartTimer();
		}

		public void SetTimer(int timerResetFrom)
		{
			timerEndTime = timerResetFrom;
			AssignTimerStartValue();
		}

		public void RestartTimer()
		{
			AssignTimerStartValue();
			StartTimer();
		}

		public void StartTimer() => hasStarted = true;


		protected abstract void AssignTimerStartValue();

		public bool IsRunning()
		{
			return hasStarted && !isPaused;
		}


	} 
}
