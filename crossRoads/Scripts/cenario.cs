using Godot;
using System;


/// <summary>
/// responsável pela instanciação das mãos que surgem no terreno quando o jogador sai da pista
/// </summary>
public class cenario : Spatial
{

    private PackedScene hand = (PackedScene)GD.Load("res://Prefabs/mao.tscn");
    private Timer timerInstance;

    private Vector3 initPos;
    bool startedTimer = false;
    private float rotation;

    private KinematicBody player;

    public override void _Ready()
    {
        timerInstance = GetNode<Timer>("TimerToInstanceHand");
        player = GetTree().Root.GetNode<KinematicBody>("rootTree/Player");
        
    }

    /// <summary>
    /// inicia um contador de tempo,que a mão foi instanciada no terreno
    /// </summary>
    /// <param name="initPos"></param>
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

    /// <summary>
    /// chamado após o jogador retornar para a pista
    /// </summary>
    public void playerIsNotInGround()
    {
        timerInstance.Stop();
        startedTimer = false;
        
    }

    /// <summary>
    /// instancia as mãos no terreno,nao posição que o jogador esta
    /// </summary>
    public void instanceHand()
    {
      GD.Print("INSTANCIANDO UMA MAO");
      Spatial node = (Spatial)hand.Instance();
      GetTree().Root.AddChild(node);
      AnimationPlayer animPlayer = node.GetNode<AnimationPlayer>("AnimationPlayer");
      node.Translate(initPos);
      node.RotateY(rotation);
      rotation+=10;

      animPlayer.Play("agarrar");
      
    }

    /// <summary>
    /// após um determinado tempo que as mãos foram instanciada,ela sao deletas sequêncialmente
    /// </summary>
    /// <param name="hand"></param>
    public void deleteHand(Node hand)
    {
        hand.QueueFree();
    }


    /// <summary>
    /// cria um temporizador entre os ataque do jogador
    /// </summary>
    /// <param name="scPlayer"></param>
    /// <returns></returns>
    private async void attackPlayer(Player scPlayer)
    {
        await ToSignal(GetTree().CreateTimer(2f),"timeout");
        
        
    }
    }
 // Called every frame. 'delta' is the elapsed time since the previous frame.

