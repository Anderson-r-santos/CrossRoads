using Godot;
using System;

public class road_tentacles : Spatial
{
    private AnimationPlayer animTenTacles;
    private AnimationNodeStateMachinePlayback stateMachine;
    private AnimationTree animTree;
    public override void _Ready()
    {
        animTenTacles = GetNode<AnimationPlayer>("AnimationPlayer");
        // animTree = GetNode<AnimationTree>("AnimationTree");
        // //animTenTacles = GetNode<AnimationPlayer>("AnimationPlayer");
        // stateMachine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
        // stateMachine.Start("left_to_right");
        animTenTacles.Play("swing001");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
    //animTree.
  // animTenTacles.Play();
  if(!animTenTacles.IsPlaying()){
    animTenTacles.Play("swing001");
  }
 }
}
