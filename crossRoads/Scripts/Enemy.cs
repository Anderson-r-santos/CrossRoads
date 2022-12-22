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
    private RayCast ray;
    private Area enemyArea;
    private MeshInstance meshEnemy;
    

    private Vector3 closePoint;

    public enum EnemyState{
        FOLLOWPLAYER,PATROL
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
        pathPatrol = GetParent().GetNode<PathFollow>(".");
        path = GetParent().GetParent().GetNode<Path>(".");
        currentState = EnemyState.PATROL;
        ray = GetNode<RayCast>("mesh/RayCast");
        enemyArea = GetNode<Area>("../../../Area");
        enemyArea.Connect("body_entered",this,"playerEnteredArea");
        enemyArea.Connect("body_exited",this,"playerExitedArea");
        meshEnemy = GetNode<MeshInstance>("mesh");
    }


    private void enemyStateHasChanged(EnemyState newState)
    {

    }


    public void enemyInsideBodyPlayer()
    {

    }
    public void takeDamage()
    {

    }
  

    private void playerExitedArea(Node player)
    {
        targetPlayer = null;
        currentState = EnemyState.PATROL;
        MeshInstance meshEnemy = GetNode<MeshInstance>("mesh");
        meshEnemy.Translate(Vector3.Zero);
        playerInsideArea = false;
    }

    private void followPlayer(float delta)
    {
      
        if(targetPlayer != null && playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.UP_UMBRELLA)
        {
            Vector3 playerCenter = targetPlayer.GlobalTransform.origin;
            playerCenter.y +=4;
            meshEnemy.LookAt(playerCenter,Vector3.Up);
            meshEnemy.Translate(Vector3.Forward * delta * moveSpeed);

            if(ray.IsColliding())
            {
               KinematicBody player = (KinematicBody) ray.GetCollider();
               Player scPlayer =  player.GetNode<Player>(".");
              // scPlayer.damageReceived("tentacleInsideDamage");
               playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RECEIVE_DAMAGE_BASIC_ENEMY;
               scPlayer.ariseTentaclesInsideBody();
               QueueFree();
               

            }
  
        }else{
             currentState = EnemyState.PATROL;
        }
    }

    private void patrol(float delta)
    {

        if(playerInsideArea){
            currentState = EnemyState.FOLLOWPLAYER;
            
        }
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
    public void playerEnteredArea(Node player)
    {
        if(player.Name == "Player")
        {
            playerInsideArea = true;
            scPlayer.enemyClose();
            GD.Print("body enter area " + player.Name);
            targetPlayer = (KinematicBody) player;
            currentState = EnemyState.FOLLOWPLAYER;
        
        }

    }
    public override void _Process(float delta)
    {

        switch(currentState)
        {
            case EnemyState.FOLLOWPLAYER:
                followPlayer(delta);
            break;

             case EnemyState.PATROL:
                patrol(delta);
            break;

        }
    }
}
