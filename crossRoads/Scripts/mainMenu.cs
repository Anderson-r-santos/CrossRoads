using Godot;
using System;


/// <summary>
/// configura o menu principal
/// </summary>
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
    /// <summary>
    /// coloca em loop o video do menu
    /// </summary>
    private void loopVideo()
    {
        
        if(!video.IsPlaying())
            video.Play();
    }
    /// <summary>
    /// muda para a cena inicial do jogo ao clicar em "new Game"
    /// </summary>
    private void changeToNewGameScene()
    {
        GetTree().ChangeSceneTo(mainScene);
    }

    /// <summary>
    /// muda para a seção de configurações do menu principal
    /// </summary>
    private void changeToSettingsScene()
    {
        Control rootSceneMenu = (Control)GetTree().Root.GetChild(0);
        rootSceneMenu.Visible = false;
        settingsMenu = (Control)settingsScene.Instance();
        GetTree().Root.AddChild (settingsMenu);
        settingsMenu.GetNode<Button>("Button").Connect("pressed",this,"returnToMenu");

        
    }

    /// <summary>
    /// sai da seção de configurações e volta para o menu principal
    /// </summary>
    private void returnToMenu()
    {
        Control rootSceneMenu = (Control)GetTree().Root.GetChild(0);
        rootSceneMenu.Visible = true;
        settingsMenu.Visible = false;
    }

    /// <summary>
    /// sai do jogo ao clickar na opção escolhida no menu
    /// </summary>
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
