using Godot;
using System;

public class Enemy : Spatial
{
    private Player scPlayer;
    private KinematicBody targetPlayer = null;
    private const float moveSpeed = 10f;
    public bool playerInsideArea = false;
    private PathFollow pathPatrol;
    private Path path;
    

    private Vector3 closePoint;

    public enum EnemyState{
        FOLLOWPLAYER,ATTACK,PATROL
    }
    private EnemyState currentState;

    public EnemyState EnemyState_G_S
    {
        get{return currentState;}
        set{
            currentState = value;
        }
    }
   
    public override void _Ready()
    {
        scPlayer = GetTree().Root.GetNode<Player>("rootTree/Player");
        pathPatrol = GetNode<PathFollow>("Path/PathFollow");
        path = GetNode<Path>("Path");
        currentState = EnemyState.PATROL;
    }


    private void enemyStateHasChanged(EnemyState newState)
    {

    }
    public override void _Process(float delta)
    {
        switch(currentState)
        {
            case EnemyState.FOLLOWPLAYER:
                followPlayer(delta);
            break;

            case EnemyState.ATTACK:
            
            break;

            case EnemyState.PATROL:
                patrol(delta);
            break;

        }
    }

    public void enemyInsideBodyPlayer()
    {

    }
    public void takeDamage()
    {

    }
    private void playerEnterInArea(Node player)
    {
        GD.Print("body enter area " + player.Name);
        if(player.Name == "Player"){
            targetPlayer = (KinematicBody) player;
            currentState = EnemyState.FOLLOWPLAYER;
        }
    }

    private void playerExitedArea(Node player)
    {
        targetPlayer = null;
        currentState = EnemyState.PATROL;
        MeshInstance meshEnemy = GetNode<MeshInstance>("Path/PathFollow/mesh");
        meshEnemy.Translate(Vector3.Zero);
    }

    private void followPlayer(float delta)
    {
        MeshInstance meshEnemy = GetNode<MeshInstance>("Path/PathFollow/mesh");
        if(targetPlayer != null)
        {
            meshEnemy.LookAt(targetPlayer.GlobalTransform.origin,Vector3.Up);
            meshEnemy.Translate(Vector3.Forward * delta * moveSpeed);
  
        }
    }

    private void patrol(float delta)
    {

        if(pathPatrol.UnitOffset < 1){
            pathPatrol.Offset +=  moveSpeed * delta;
        }else{
            pathPatrol.UnitOffset = 0;
            pathPatrol.Offset = 0;
            pathPatrol.HOffset = 0;
            pathPatrol.VOffset = 0;
        }
 


    }

        //quando o player entra no campo de visao do inimigo
    public void playerEnteredArea(Node body)
    {
        if(body.Name == "Player")
        {
            playerInsideArea = true;
            scPlayer.enemyClose();
        }

    }
}
