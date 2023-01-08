using Godot;
using System;

public class TipsVideo : Control
{

    private Control uiTipsVideo;
    private VideoPlayer videoPlayer;
    private Label messageLabel;
    private mainScene scMainScene;

    public override void _Ready()
    {
        uiTipsVideo = GetNode<Control>("tipVideo");
        videoPlayer = GetNode<VideoPlayer>("tipVideo/VideoPlayer");
        messageLabel = GetNode<Label>("tipMessage/CenterContainer/HBoxContainer/TextTips");
        scMainScene = GetTree().Root.GetNode<mainScene>("rootTree");
    }

    public void showTipsVideo(VideoStreamWebm video)
    {
        videoPlayer.Stream = video;
        scMainScene.pauseGame();
        uiTipsVideo.Visible = true;
        videoPlayer.Play();

    }
    public void showTipsText(string message)
    {
        
        messageLabel.Text = message;
        messageLabel.VisibleCharacters = 0;
        uiTipsVideo.Visible = false;
        animateMessage(message.Length);

    }

    private async void animateMessage(int lengthCharacter)
    {
        //GetNode<AnimationPlayer>("AnimationPlayer").Play("openMessageLabel");
        Panel backGroundMsg = GetNode<Panel>("tipMessage/CenterContainer/HBoxContainer/Label/background");
        for(int i=0;i < lengthCharacter; i++)
        {
            await ToSignal(GetTree().CreateTimer(0.1f),"timeout");
            messageLabel.VisibleCharacters +=1;
           backGroundMsg.MarginRight += 600/lengthCharacter;
            
        }
        hideMessage();
    }

    private async void hideMessage()
    {
      await ToSignal(GetTree().CreateTimer(4f),"timeout");
      GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("openMessageLabel");
      int lengthCharacter = messageLabel.Text.Length;
        for(int i=lengthCharacter;i > 0; i--)
        {
            await ToSignal(GetTree().CreateTimer(0.02f),"timeout");
            messageLabel.VisibleCharacters -=1;
        }
    }
    private void deleteTipsVideo()
    {
        scMainScene.resumeGame();
        uiTipsVideo.Visible = false;
        videoPlayer.Stop();

    }
    public override void _Process(float delta)
    {
        if(Visible && !videoPlayer.IsPlaying())
        {
             videoPlayer.Play();
        }
    }
}
