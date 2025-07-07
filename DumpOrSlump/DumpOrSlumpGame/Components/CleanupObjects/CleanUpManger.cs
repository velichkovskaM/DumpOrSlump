using System.Collections.Generic;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using AudioEmitter = GameEngine.Components.AudioEmitter;
using BoundingBox = GameEngine.Core.BoundingBox;

namespace DumpOrSlumpGame.Components.CleanupObjects;

/// <summary>
/// Spawns an initial batch of interactive cleanup objects (clothes, clutter, dust) at random positions inside a list
/// of designer‑defined spawn areas
/// </summary>
public class CleanUpManger : Component
{
    int spawnClothes = 0;
    int spawnClutter = 0;
    int spawnDust = 0;
    private List<BoundingBox> spawnArea;

    private Node clothesHolder;
    private Node clutterHolder;
    private Node dustHolder;
    
    private bool isCircleActivated = false;
    
    // Records counts, spawn areas, and holder nodes
    public CleanUpManger(
        Node parent,
        int _spawnClothes,
        int _spawnClutter,
        int _spawnDust,
        List<BoundingBox> _spawnArea,
        Node _clothesHolder,
        Node _clutterHolder,
        Node _dustHolder,
        bool isCircleInteraction,
        bool active = true) : base(parent, active)
    {
        spawnClothes = _spawnClothes;
        spawnClutter = _spawnClutter;
        spawnDust = _spawnDust;
        spawnArea = _spawnArea;
        clothesHolder = _clothesHolder;
        clutterHolder = _clutterHolder;
        dustHolder = _dustHolder;
        isCircleActivated = isCircleInteraction;
    }

    // Instantiates each requested object type using getRandomPosition, adds sprite and behavior components, and parents
    // them under the corresponding holder node
    public override void Start(IScene scene)
    {
        for (int i = 0; i < spawnClothes; i++)
        {
            var node = new Node("Clothes", getRandomPosition(-0.1f), scale: new Vector3(0.5f, 0.5f, 0.5f));
            var spriteRenderer = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
            node.AddComponent(spriteRenderer);
            
            var soundEmitter = new AudioEmitter(node);
            soundEmitter.SetSoundEffect(Globals.content.Load<SoundEffect>("SoundEffect/pickup_clothes"));
            node.AddComponent(soundEmitter);
            
            var clothesScript = new Clothes(node, isCircleActivated);
            node.AddComponent(clothesScript);
            
            scene.SafeInsert(node);
            clothesHolder.Children.Add(node);
        }
        
        for (int i = 0; i < spawnClutter; i++)
        {
            var node = new Node("Clutter", getRandomPosition(0.2f), scale: new Vector3(0.15f, 0.15f, 0.15f));
            var spriteRenderer = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
            node.AddComponent(spriteRenderer);
            var clutterScript = new Clutter(node, isCircleActivated);
            node.AddComponent(clutterScript);
            
            scene.SafeInsert(node);
            clutterHolder.Children.Add(node);
        }
        
        for (int i = 0; i < spawnDust; i++)
        {
            var node = new Node("Dust",
                position: getRandomPosition(0.3f),
                scale: new Vector3(1f, 1f, 1f),
                rotation: new Vector3(90, 0, 0)
                );
            var spriteRenderer = new SpriteRenderer(node, Globals._Graphics.GraphicsDevice);
            node.AddComponent(spriteRenderer);
            var dustScript = new Dust(node, isCircleActivated);
            node.AddComponent(dustScript);
            
            scene.SafeInsert(node);
            dustHolder.Children.Add(node);
        }
    }

    // Chooses a uniformly random position within one of the available spawn areas, weighted by each bounding box’s
    // surface area (larger areas generate more points)
    public Vector3 getRandomPosition(float y)
    {
        // Compute total area for weighting
        var totalArea = 0f;
        foreach (var boundingBox in spawnArea)
        {
            totalArea += boundingBox.Area();
        }
        
        // Select bounding box based on proportional area
        float selectedRandomNumber = (float)Globals.rand.NextDouble() * totalArea;
        float currentAreaFromBoxes = 0;
        int selectedIndex = -1;

        for(int i = 0; i < spawnArea.Count; i++)
        {
            var boundingBox = spawnArea[i];
            currentAreaFromBoxes += boundingBox.Area();
            if (currentAreaFromBoxes >= selectedRandomNumber)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == -1)
        {
            Logger.Error($"Could not find random position for spawn area {selectedRandomNumber}");
        }
        
        // Pick a random (x,z) inside the chosen box
        var selectedBox = spawnArea[selectedIndex]; 
        var x = (float)(Globals.rand.NextDouble() * (selectedBox.maximum.X - selectedBox.minimum.X) + selectedBox.minimum.X);
        var z = (float)(Globals.rand.NextDouble() * (selectedBox.maximum.Y - selectedBox.minimum.Y) + selectedBox.minimum.Y);
        return new Vector3(x, y, z);
    }
}