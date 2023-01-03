using Godot;
using System;

public class mainMenu : Control
{
    private VideoPlayer video;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        video = GetNode<VideoPlayer>("VideoPlayer");
    }

    private void loopVideo()
    {
        
        if(!video.IsPlaying())
            video.Play();
    }
        
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     loopVideo();
 }
}
