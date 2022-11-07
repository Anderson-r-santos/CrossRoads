using Godot;
using System;

public class lighBlink : Node
{
    AnimationPlayer lightAnimationPlayer;
    public override void _Ready()
    {
        lightAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        lightAnimationPlayer.PlaybackSpeed = 1.2f;
        lightAnimationPlayer.Play("badContactLight");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
