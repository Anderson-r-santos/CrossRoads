using Godot;
using System;

public class cenario : Spatial
{

    private PackedScene hand = (PackedScene)GD.Load("res://Prefabs/mao.tscn");
    private Timer timerInstance;

    private Vector3 initPos;
    bool startedTimer = false;
    private float rotation;
    private RayCast ray;
    private KinematicBody player;

    public override void _Ready()
    {
        timerInstance = GetNode<Timer>("TimerToInstanceHand");
        player = GetTree().Root.GetNode<KinematicBody>("rootTree/Player");
        
    }
    public void startTimer(Vector3 initPos)
    {
        if(!startedTimer)
        {
            timerInstance.Start();
        }
        //initPos.y +=10;
        this.initPos = initPos;
        startedTimer = true;
    }
    public void playerIsNotInGround()
    {
        timerInstance.Stop();
        startedTimer = false;
        
    }
    public void instanceHand()
    {
      GD.Print("INSTANCIANDO UMA MAO");
      RigidBody node = (RigidBody)hand.Instance();
      GetTree().Root.AddChild(node);
      AnimationPlayer animPlayer = node.GetNode<AnimationPlayer>("AnimationPlayer");
      node.Translate(initPos);
      node.RotateY(rotation);
      rotation+=10;
      ray = node.GetNode<RayCast>("RayCast");
      animPlayer.Play("agarrar");
      
    }
    public void deleteHand(Node hand)
    {
        hand.QueueFree();
        ray = null;
    }
 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {

    if(ray != null)
    {
 
        
        //player = ray.IsColliding() ? (KinematicBody) : null;
        
        if(ray.IsColliding() && ray.GetCollider()!= null)
        
        {
            KinematicBody player = (KinematicBody)ray.GetCollider();
            if(player.Name == "Player"){
                Player scPlayer = GetTree().Root.GetNode<Player>("rootTree/Player");
                playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RECEIVE_DAMAGE_GROUND_ENEMY;
            }
        }
    }
 }
}
