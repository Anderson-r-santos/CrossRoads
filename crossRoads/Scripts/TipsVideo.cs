using Godot;
using System;

public class TipsVideo : Area
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private Control uiTipsVideo;
    private VideoPlayer videoTips;
    private mainScene scMainScene;

    public override void _Ready()
    {
        uiTipsVideo = GetNode<Control>("Control");
        videoTips = GetNode<VideoPlayer>("Control/VideoPlayer");
        scMainScene = GetTree().Root.GetNode<mainScene>("rootTree");
    }

    private void showTipsVideo(Node body)
    {
        scMainScene.pauseGame();
        uiTipsVideo.Visible = true;
        videoTips.Play();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    private void deleteTipsVideo()
    {
        scMainScene.pauseGameEnd();
        QueueFree();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
