using Godot;
using System;

public class mainMenu : Control
{
    private VideoPlayer video;
    private PackedScene mainScene = GD.Load<PackedScene>("res://Scenes/mainScene.scn");
    private PackedScene settingsScene = GD.Load<PackedScene>("res://Scenes/settingsScene.tscn");
    private Control settingsMenu;
    private Viewport menuScene;
    private Button returnToMenuButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        video = GetNode<VideoPlayer>("VideoPlayer");
        menuScene = GetTree().Root;

    }

    private void loopVideo()
    {
        
        if(!video.IsPlaying())
            video.Play();
    }

    private void changeToNewGameScene()
    {
        GetTree().ChangeSceneTo(mainScene);
    }
    private void changeToSettingsScene()
    {
        Control rootSceneMenu = (Control)GetTree().Root.GetChild(0);
        rootSceneMenu.Visible = false;
        settingsMenu = (Control)settingsScene.Instance();
        GetTree().Root.AddChild (settingsMenu);
        settingsMenu.GetNode<Button>("Button").Connect("pressed",this,"returnToMenu");

        
    }
    private void returnToMenu()
    {
        Control rootSceneMenu = (Control)GetTree().Root.GetChild(0);
        rootSceneMenu.Visible = true;
        settingsMenu.Visible = false;
    }
    private void exitGame()
    {
        GetTree().Quit();
    }
        
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     loopVideo();
 }
}
