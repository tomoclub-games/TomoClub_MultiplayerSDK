using TomoClub.Arenas;
using UnityEngine;

public class Arena : BaseArena
{
	protected override void Awake()
	{
		base.Awake();

	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void OnTimerCompleted()
	{
		RemoteProcedureCalls.Instance.UpdateArenaTimerCompletedOnNetwork(arenaNo);
	}

	protected override void OnTimerUpdate(int currentTime)
	{
		RemoteProcedureCalls.Instance.UpdateArenaTimerOnNetwork(arenaNo, currentTime);
	}

}
