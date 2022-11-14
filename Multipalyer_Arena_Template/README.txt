These are the arena manager and arean templetes that you'll during development of your game.

Steps

1. Drag Arena Setup prefab into your game scene, make sure your game scene name is "Game"
2. Create an assembly definition reference in your Project's Scripts folder and choose the "TomoClub.Arenas" Assembly as the reference (this will allow your project scripts to refer to this assembly)
3. Route all functionality through arenas and arena manager, that means arena manager acts as the single game manager for the game as it has access to arenas
4. Check out the sample game to see how this have been implemented
5. If your game extends the core functionality of base arenas then use the override functions given to extend them.

NOTE: DO NOT CHANGE THE BASE ARENA MANAGER AND BASE ARENA SCRIPTS