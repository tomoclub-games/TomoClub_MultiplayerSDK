using TomoClub.Arenas;

public class ArenaManager : BaseArenaManager<Arena> 
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

	public override void SpectateGame()
	{
		//Change of camera while spectating game
	}

	protected override void UpdateArenaPauseStateOnNetwork(int arenaNo, ArenaState updatedState)
	{
		RemoteProcedureCalls.Instance.UpdateArenaPauseStateOnNetwork(arenaNo, (int)updatedState);
	}

}
