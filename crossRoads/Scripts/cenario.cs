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
      Spatial node = (Spatial)hand.Instance();
      GetTree().Root.AddChild(node);
      AnimationPlayer animPlayer = node.GetNode<AnimationPlayer>("AnimationPlayer");
      node.Translate(initPos);
      node.RotateY(rotation);
      rotation+=10;
      ray = node.GetNode<RayCast>("Armature/Skeleton/BoneAttachment/KinematicBody/RayCast");
      animPlayer.Play("agarrar");
      
    }
    public void deleteHand(Node hand)
    {
        hand.QueueFree();
        ray = null;
    }

    private async void attackPlayer(Player scPlayer)
    {
        await ToSignal(GetTree().CreateTimer(2f),"timeout");
        
        
    }
 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {

    if(ray != null)
    {
        if(ray.IsColliding())
        
        {
            KinematicBody player = (KinematicBody)ray.GetCollider();
            if(player.Name == "Player"){
                ray = null;
                playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RECEIVE_DAMAGE_HAND_GROUND;
            }
        }
    }
 }
}
