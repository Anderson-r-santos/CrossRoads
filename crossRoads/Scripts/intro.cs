using Godot;
using System;

public class intro : Node
{
   private bool isCanJumpIntro = false;
    public override void _Ready()
    {
        canJumpIntro();
    }
    public override void _Input(InputEvent @event)
    {
        VideoPlayer videoPlayer = GetNode<VideoPlayer>("Control/VideoPlayer");
        if(Input.IsKeyPressed((int)KeyList.Enter) && isCanJumpIntro || !videoPlayer.IsPlaying())
        {
            changeToMenuScene();
        }
    }
    private async void canJumpIntro()
    {
        await ToSignal(GetTree().CreateTimer(5f),"timeout");
        GetNode<Label>("Control/Label").Visible = true;
        isCanJumpIntro = true;
    }
    private void changeToMenuScene()
    {
        GetTree().ChangeScene("res://Scenes/mainMenu.tscn");
    }
}
