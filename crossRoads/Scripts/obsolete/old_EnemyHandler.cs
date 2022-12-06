using Godot;
using System.Collections.Generic;


public class old_EnemyHandler : Spatial
{
    private Player scPlayer;

    public bool playerInsideArea = false;
 
    private List<Vector3> spawnPosition = new List<Vector3>();   //guarda uma lista de spawn(somente em cima da pista)
    public List<Slot> slotList = new List<Slot>();        //lista contendo os slot e se estao vazio ou nao
    private List <boundsRoadStruct> roadInsideRangeArea = new List<boundsRoadStruct>();
    private List<old_Enemy> listEnemyCreated = new List<old_Enemy>();
   
    private PackedScene spawnParticles = (PackedScene) GD.Load("res://Prefabs/spawnParticles.tscn");
    private PackedScene enemy = (PackedScene) GD.Load("res://Prefabs/Enemy.tscn");

    private Timer timerToSpawnEnemy;    //tempo que o inimigo vai surgir apos o spawn ser criado
    private Timer timertoInstanceNewSpawn; //tempo de distancia em que os spawns vao surgir
    private Timer timerToEnemyPursuit;

    public Spatial floatCircle;        //objeto que vai ficar girando e os inimigo vao seguir

    public struct Slot{
       public  Spatial spatialPosition;
       public bool isEmpty;
    }


    private Area rangeArea;
    
    private Vector3[] arrayRandomPosition;

    private int amountEnemyToSpawn;
    private int spawnLeftInstance;

    private bool canInstanceSpawn = false; //se passou o tempo para poder instanciar outro spawn

    private bool hasBoundsRoad = false;
    private bool spawnCreated = false;

    private struct boundsRoadStruct
    {
       public Vector3 minX;
       public Vector3 maxX;

       public Vector3 minZ;
       public Vector3 maxZ;
    }



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
        scPlayer = GetTree().Root.GetNode<Player>("rootTree/Player");
        timerToSpawnEnemy = GetNode<Timer>("TimerToSpawnEnemy");
        timertoInstanceNewSpawn = GetNode<Timer>("TimertoInstanceNewSpawn");
        timerToEnemyPursuit = GetNode<Timer>("TimerToEnemyPursuit");
        floatCircle = GetNode<Spatial>("EnemyFloatCircle");
        rangeArea = GetNode<Area>("rangeArea");


        setupSlot();

    }

    //adc a estrada que esta dentro da area do spawn
    private void addRoadToList(StaticBody road)
    {
        GD.Print("adc a estrada com o nome de " + road.Name);
        boundsRoadStruct boundCurrentRoad;

        int childCountRoad  = road.GetChildCount();
        childCountRoad -=1;
        Spatial child1,child2,child3,child4;

        if(road.GetChild(childCountRoad).Name == "LightPoste")
        {
            childCountRoad --;
        }
        child1 = (Spatial)road.GetChild(childCountRoad);
        child2 = (Spatial)road.GetChild(childCountRoad -1);
        child3 = (Spatial)road.GetChild(childCountRoad -2);
        child4 = (Spatial)road.GetChild(childCountRoad -3);

        if(road.RotationDegrees.y == 0)
        {
            boundCurrentRoad.maxZ = child2.Transform.origin;
            boundCurrentRoad.minZ = child1.Transform.origin;
            boundCurrentRoad.maxX = child4.Transform.origin;
            boundCurrentRoad.minX = child3.Transform.origin;

            roadInsideRangeArea.Add(boundCurrentRoad);
            GD.Print("e ela tem uma rotacao de  0 graus");

        }else if (road.RotationDegrees.y == 90)
        {
            boundCurrentRoad.maxZ = child4.Transform.origin;
            boundCurrentRoad.minZ = child3.Transform.origin;
            boundCurrentRoad.maxX = child2.Transform.origin;
            boundCurrentRoad.minX = child1.Transform.origin;

            roadInsideRangeArea.Add(boundCurrentRoad);
            GD.Print("e ela tem uma rotacao de  90 graus");


        }
        

    }
    //cria posições aleatorias aonde os spaws serao instanciados
    private void createRandomPositionInRoad()
    {
        arrayRandomPosition = new Vector3[amountEnemyToSpawn];
    


        int amountEnemyByRoad = amountEnemyToSpawn / roadInsideRangeArea.Count;
        int initJ = 0;
        GD.Print("qnt de inimigo por pista e " + amountEnemyByRoad + " e qnt de pista total " + roadInsideRangeArea.Count);
        for(int i = 0; i < roadInsideRangeArea.Count; i++)
        {
            float width1,height1;
            float posX;
            float posZ;
            height1 = roadInsideRangeArea[i].maxX.x -  roadInsideRangeArea[i].minX.x;
            width1 = roadInsideRangeArea[i].maxZ.z -  roadInsideRangeArea[i].minZ.z;
            for(int j = initJ ; j <  amountEnemyByRoad; j++)
            {
                if(height1 < width1)
                {
                    posX = (float)GD.RandRange(roadInsideRangeArea[i].minX.x,roadInsideRangeArea[i].maxX.x);
                    posZ = (float)GD.RandRange(roadInsideRangeArea[i].minZ.z,roadInsideRangeArea[i].maxZ.z);
                    GD.Print("a pista ta com rotaçao 0");
                 
                }else{
                    posX = (float)GD.RandRange(roadInsideRangeArea[i].minX.z,roadInsideRangeArea[i].maxX.z);
                    posZ = (float)GD.RandRange(roadInsideRangeArea[i].minZ.x,roadInsideRangeArea[i].maxZ.x);
                    GD.Print("a pista ta com rotaçao 90");
                }
                // GD.Print("posicao Gerada X " + posX);
                // GD.Print("posicao Gerada Z " + posZ);

                arrayRandomPosition[j].x = posX;
                arrayRandomPosition[j].z = posZ; 
                arrayRandomPosition[j].y = 0.6f;
            }
            initJ = amountEnemyByRoad;
            amountEnemyByRoad *=2;
        }
        GD.Print("qnt de posicao criada "+ arrayRandomPosition.Length);

    }

    //define todos os slots que os inimigos pode ocupar
    private void setupSlot()
    {
        int amountSlotEnemy = floatCircle.GetChildCount();
        amountEnemyToSpawn = amountSlotEnemy;
        spawnLeftInstance = amountEnemyToSpawn;
         for(int i = 0 ; i < amountSlotEnemy ; i++)
        {
            Slot slot;
            slot.spatialPosition = (Spatial)floatCircle.GetChild(i);
            slot.isEmpty = true;
            slotList.Add(slot);
        }
    }
    //cria varios spawn em locais aleatorios que sejam em cima da pista
    private void instanceSpawn()
    {


        Node instanceSpawn = spawnParticles.Instance();
    
        ((Particles)instanceSpawn).Translation = arrayRandomPosition[spawnLeftInstance - 1];
        AddChild(instanceSpawn);   


        spawnEnemy(((Spatial)instanceSpawn).Transform.origin);
        // GD.Print("instanceSpawn position" +( (Spatial)instanceSpawn).Transform.origin.x);
        GD.Print("instanceSpawn GLOBAL position" +( (Spatial)instanceSpawn).GlobalTransform.origin);
        

       timerToSpawnEnemy.Start();
       timertoInstanceNewSpawn.Start();
       canInstanceSpawn = false;
   
    }

    //instancia inimigos na posição do Spawn
    private void spawnEnemy(Vector3 spawnPos)
    {

            spawnLeftInstance --;

            Node instanceEnemy = enemy.Instance();
            ((Spatial)instanceEnemy).Translation = spawnPos;
            AddChild(instanceEnemy);
            old_Enemy scEnemy = instanceEnemy.GetNode<old_Enemy>(".");
            listEnemyCreated.Add(scEnemy);
            scEnemy.EnemyHandlerPropertie = this;

            Spatial enemySlot = scEnemy.verifySlot();
           
            if(enemySlot != null)
            {
                scEnemy.currentStateEnemy = old_Enemy.STATE_ENEMY.SPAWN;
                scEnemy.currentSlotThisEnemy = enemySlot;


            }else{
                instanceEnemy.QueueFree();
            }


    }

    private void endTimerInstanceNewSpawn()
    {
        canInstanceSpawn = true;
        if(spawnCreated == false && spawnLeftInstance > 0)
        {
            createRandomPositionInRoad();
            spawnCreated =true;
        }
        if(spawnLeftInstance > 0)
        {
            if(canInstanceSpawn && playerInsideArea)
            {
                instanceSpawn();
            }
        }else{
            timertoInstanceNewSpawn.QueueFree();
        }
    }

    private void authorizeEnemyPursuit()
    {
        if(listEnemyCreated.Count > 0)
        {
            int lastPosList = listEnemyCreated.Count - 1 ;
            listEnemyCreated[lastPosList].currentStateEnemy = old_Enemy.STATE_ENEMY.MOVE;
            listEnemyCreated.RemoveAt(lastPosList);
        }
    }

    //quando o player entra no campo de visao do inimigo
    public void playerEnteredArea(Node body)
    {
        if(body.Name == "Player")
        {
            playerInsideArea = true;
            // GD.Print("enemy detectou alguem com o nome de " + body.Name);
            scPlayer.enemyClose();
            canInstanceSpawn = true;
        }else if(body.Name == "Spatial")
        {

        }
        else
        {
            if(body != null)
            {
                addRoadToList((StaticBody) body);
                timertoInstanceNewSpawn.Start();
            }
        }

    }

    private void playerExitedArea(Node body)
    {
        playerInsideArea = false;

    }

    public override void _Process(float delta)
    {
        floatCircle.RotateObjectLocal(Vector3.Up, 2 * delta);
        if(spawnLeftInstance <= 0 && timerToEnemyPursuit.IsStopped())
        {
            timerToEnemyPursuit.Start();
        }

    }
}
