using DumpOrSlumpGame.Components.InteractableItems;
using DumpOrSlumpGame.Components.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameEngine.Components;
using GameEngine.Core;
using Microsoft.Xna.Framework.Audio;

namespace DumpOrSlumpGame.Components
{
    /// <summary>
    /// Main playable character component. Handles input‑driven movement, equipment toggling, animation state machine, and SFX
    /// </summary>
    internal class Player : Component
    {
        // animation frame arrays
        Rectangle[] animation_idle;
        Rectangle[] animation_walk;
        Rectangle[] animation_vacuuming;
        Rectangle[] animation_cleaning;
        Rectangle[] animation_sorting;
        Rectangle[] animation_dancing;
        Rectangle[] animation_crying;
        Rectangle[] animation_vacuuming_idle;
        Rectangle[] animation_cleaning_idle;
        Rectangle[] animation_sorting_idle;
        
        double timer;
        double frame_time;
        byte saved_animation;
        Rectangle[] animation_type;
        bool walking_right = false;
        bool isVacuuming = false;
        bool isCleaning = false;
        bool isSorting = false;
        Vacuum vacuum;
        ClothesBasket clothsBasket;
        GarbageBag garbageBag;
        bool[] last = [false, false, false];

        Texture2D animation;
        
        SpriteRenderer spriteRenderer;

        public SoundEffectInstance _soundEffect;

        public Player(Node parent) : base(parent) { }

        // init animations, assets, references & audio
        public override void Start(IScene scene)
        {
            animation = Globals.content.Load<Texture2D>("SpriteSheets/ChildPlayerSpriteSheet");
            timer = 0;
            frame_time = 0.2;

            spriteRenderer = Parent.GetComponent<SpriteRenderer>();

            animation_idle = new Rectangle[8];
            animation_idle[0] = new Rectangle(0, 0, 256, 256);
            animation_idle[1] = new Rectangle(256, 0, 256, 256);
            animation_idle[2] = new Rectangle(256 * 2, 0, 256, 256);
            animation_idle[3] = new Rectangle(256, 0, 256, 256);
            animation_idle[4] = new Rectangle(0, 0, 256, 256);
            animation_idle[5] = new Rectangle(256 * 3, 0, 256, 256);
            animation_idle[6] = new Rectangle(256 * 4, 0, 256, 256);
            animation_idle[7] = new Rectangle(256 * 3, 0, 256, 256);

            animation_walk = new Rectangle[5];
            animation_walk[0] = new Rectangle(0, 256, 256, 256);
            animation_walk[1] = new Rectangle(256, 256, 256, 256);
            animation_walk[2] = new Rectangle(256 * 2, 256, 256, 256);
            animation_walk[3] = new Rectangle(256 * 3, 256, 256, 256);
            animation_walk[4] = new Rectangle(256 * 4, 256, 256, 256);

            animation_vacuuming = new Rectangle[5];
            animation_vacuuming[0] = new Rectangle(0, 256 * 2, 256 * 2, 256);
            animation_vacuuming[1] = new Rectangle(256 * 2, 256 * 2, 256 * 2, 256);
            animation_vacuuming[2] = new Rectangle(256 * 4, 256 * 2, 256 * 2, 256);
            animation_vacuuming[3] = new Rectangle(256 * 6, 256 * 2, 256 * 2, 256);
            animation_vacuuming[4] = new Rectangle(256 * 8, 256 * 2, 256 * 2, 256);

            animation_sorting = new Rectangle[5];
            animation_sorting[0] = new Rectangle(0, 256 * 3, 256 * 2, 256);
            animation_sorting[1] = new Rectangle(256 * 2, 256 * 3, 256 * 2, 256);
            animation_sorting[2] = new Rectangle(256 * 4, 256 * 3, 256 * 2, 256);
            animation_sorting[3] = new Rectangle(256 * 6, 256 * 3, 256 * 2, 256);
            animation_sorting[4] = new Rectangle(256 * 8, 256 * 3, 256 * 2, 256);

            animation_cleaning = new Rectangle[5];
            animation_cleaning[0] = new Rectangle(0, 256 * 4, 256 * 2, 256);
            animation_cleaning[1] = new Rectangle(256 * 2, 256 * 4, 256 * 2, 256);
            animation_cleaning[2] = new Rectangle(256 * 4, 256 * 4, 256 * 2, 256);
            animation_cleaning[3] = new Rectangle(256 * 6, 256 * 4, 256 * 2, 256);
            animation_cleaning[4] = new Rectangle(256 * 8, 256 * 4, 256 * 2, 256);
            
            animation_vacuuming_idle = new Rectangle[5];
            animation_vacuuming_idle[0] = new Rectangle(0, 256 * 5, 256 * 2, 255);
            animation_vacuuming_idle[1] = new Rectangle(256 * 2, 256 * 5, 256 * 2, 255);
            animation_vacuuming_idle[2] = new Rectangle(256 * 4, 256 * 5, 256 * 2, 255);
            animation_vacuuming_idle[3] = new Rectangle(256 * 6, 256 * 5, 256 * 2, 255);
            animation_vacuuming_idle[4] = new Rectangle(256 * 8, 256 * 5, 256 * 2, 255);

            animation_sorting_idle = new Rectangle[5];
            animation_sorting_idle[0] = new Rectangle(0, 256 * 6, 256 * 2, 256);
            animation_sorting_idle[1] = new Rectangle(256 * 2, 256 * 6, 256 * 2, 256);
            animation_sorting_idle[2] = new Rectangle(256 * 4, 256 * 6, 256 * 2, 256);
            animation_sorting_idle[3] = new Rectangle(256 * 6, 256 * 6, 256 * 2, 256);
            animation_sorting_idle[4] = new Rectangle(256 * 8, 256 * 6, 256 * 2, 256);

            animation_cleaning_idle = new Rectangle[5];
            animation_cleaning_idle[0] = new Rectangle(0, 256 * 7, 256 * 2, 256);
            animation_cleaning_idle[1] = new Rectangle(256 * 2, 256 * 7, 256 * 2, 256);
            animation_cleaning_idle[2] = new Rectangle(256 * 4, 256 * 7, 256 * 2, 256);
            animation_cleaning_idle[3] = new Rectangle(256 * 6, 256 * 7, 256 * 2, 256);
            animation_cleaning_idle[4] = new Rectangle(256 * 8, 256 * 7, 256 * 2, 256);

            animation_dancing = new Rectangle[3];
            animation_dancing[0] = new Rectangle(0, 256 * 9, 256, 256);
            animation_dancing[1] = new Rectangle(256, 256 * 9, 256, 256);
            animation_dancing[2] = new Rectangle(256 * 2, 256 * 9, 256, 256);

            animation_crying = new Rectangle[3];
            animation_crying[0] = new Rectangle(0, 256 * 8, 256, 256);
            animation_crying[1] = new Rectangle(256, 256 * 8, 256, 256);
            animation_crying[2] = new Rectangle(256 * 2, 256 * 8, 256, 256);

            var animationDataIdle = new AnimationData(animation, animation_idle, frame_time, true, true, false);
            var animationDataWalk = new AnimationData(animation, animation_walk, frame_time, true, true, false);
            var animationDataVacuuming = new AnimationData(animation, animation_vacuuming, frame_time, true, true, false);
            var animationDataSorting = new AnimationData(animation, animation_sorting, frame_time, true, true, false);
            var animationDataCleaning = new AnimationData(animation, animation_cleaning, frame_time, true, true, false);
            var animationDataVacuumingIdle = new AnimationData(animation, animation_vacuuming_idle, frame_time, true, true, false);
            var animationDataSortingIdle = new AnimationData(animation, animation_sorting_idle, frame_time, true, true, false);
            var animationDataCleaningIdle = new AnimationData(animation, animation_cleaning_idle, frame_time, true, true, false);
            var animationDataDancing = new AnimationData(animation, animation_dancing, frame_time, true, true, false);
            var animationDataCrying = new AnimationData(animation, animation_crying, frame_time, true, true, false);
            
            spriteRenderer.AddAnimation("idle", animationDataIdle);
            spriteRenderer.AddAnimation("walk", animationDataWalk);
            spriteRenderer.AddAnimation("vacuuming", animationDataVacuuming);
            spriteRenderer.AddAnimation("sorting", animationDataSorting);
            spriteRenderer.AddAnimation("cleaning", animationDataCleaning);
            spriteRenderer.AddAnimation("vacuumingIdle", animationDataVacuumingIdle);
            spriteRenderer.AddAnimation("sortingIdle", animationDataSortingIdle);
            spriteRenderer.AddAnimation("cleaningIdle", animationDataCleaningIdle);
            spriteRenderer.AddAnimation("dancing", animationDataDancing);
            spriteRenderer.AddAnimation("crying", animationDataCrying);

            saved_animation = 2;

            vacuum = scene.FindNodeByName("Vacuum").GetComponent<Vacuum>();
            clothsBasket = scene.FindNodeByName("ClothesBasket").GetComponent<ClothesBasket>();
            garbageBag = scene.FindNodeByName("GarbageBag").GetComponent<GarbageBag>();
            
            // load vacuum SFX
            _soundEffect = Globals.content.Load<SoundEffect>("SoundEffect/vacuum").CreateInstance();
            _soundEffect.Volume = 1.0f;
            _soundEffect.IsLooped = true;
        }

        public enum player_states
        {
            idle,
            walk,
            vacuuming,
            cleaning,
            sorting,
            vacuuming_idle,
            cleaning_idle,
            sorting_idle,
            dance,
            cry,
        }

        public player_states player_state = player_states.idle;
        player_states previous_state = player_states.idle;

        // per‑frame: handle input, state machine, animations, movement
        public override void Update(GameTime gameTime, TouchCollection touchCollection)
        {
            if (Game1.isGameWon)
            {
                garbageBag.isCleaning = false;
                vacuum.isVacuuming = false;
                clothsBasket.isSorting = false;
                animation_type = animation_dancing;
                player_state = player_states.dance;
                if (spriteRenderer.GetCurrentKey() != "dancing")
                {
                    spriteRenderer.SetAnimation("dancing");
                }
            }

            if (Game1.isGameOver)
            {
                garbageBag.isCleaning = false;
                vacuum.isVacuuming = false;
                clothsBasket.isSorting = false;
                animation_type = animation_crying;
                player_state = player_states.cry;
                
                if (spriteRenderer.GetCurrentKey() != "crying")
                {
                    spriteRenderer.SetAnimation("crying");
                }
            }

            if (Controller.drag_direction != new Vector2(0, 0) || vacuum.isVacuuming || clothsBasket.isSorting || garbageBag.isCleaning)
            {
                if (vacuum.isVacuuming)
                {
                    player_state = player_states.vacuuming;
                }
                else if (garbageBag.isCleaning)
                {
                    player_state = player_states.cleaning;
                }
                else if (clothsBasket.isSorting)
                {
                    player_state = player_states.sorting;
                }
                else
                {
                    player_state = player_states.walk;
                }
            }
            else
            {
                if (Game1.isGameWon)
                {
                    player_state = player_states.dance;
                }else if (Game1.isGameOver)
                {
                    player_state = player_states.cry;
                }
                else { 
                    player_state = player_states.idle; 
                }

            }

            if (player_state != previous_state)
            {
                saved_animation = 0;
                previous_state = player_state;
            }
            Parent.Transform.Position += new Vector3(Controller.drag_direction.X, 0, Controller.drag_direction.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds * 5;
            
            if (Controller.drag_direction != new Vector2(0, 0)) { 
                if (Controller.drag_direction.X < 0)
                {
                    spriteRenderer.Flipped = true;
                }
                else
                {
                    spriteRenderer.Flipped = false;
                }
            }
            
            switch (player_state)
            {
                case player_states.idle:
                    spriteRenderer.SetAnimation("idle");
                    break;
                case player_states.walk:
                    spriteRenderer.SetAnimation("walk");
                    animation_type = animation_walk;
                    break;
                case player_states.vacuuming:
                    if (Controller.drag_direction.Length() > 0.01f)
                    {
                        spriteRenderer.SetAnimation("vacuuming");
                        animation_type = animation_vacuuming;
                    }
                    else
                    {
                        spriteRenderer.SetAnimation("vacuumingIdle");
                        animation_type = animation_vacuuming_idle;
                    }
                    break;
                case player_states.cleaning:
                    if (Controller.drag_direction.Length() > 0.01f)
                    {
                        spriteRenderer.SetAnimation("cleaning");
                        animation_type = animation_cleaning;
                    }
                    else
                    {
                        spriteRenderer.SetAnimation("cleaningIdle");
                        animation_type = animation_cleaning_idle;
                    }
                    break;
                case player_states.sorting:
                    if (Controller.drag_direction.Length() > 0.01f)
                    {
                        spriteRenderer.SetAnimation("sorting");
                        animation_type = animation_sorting;
                    }
                    else
                    {
                        spriteRenderer.SetAnimation("sortingIdle");
                        animation_type = animation_sorting_idle;
                    }
                    break;
                case player_states.sorting_idle:
                    spriteRenderer.SetAnimation("sortingIdle");
                    animation_type = animation_sorting_idle;
                    break;
                case player_states.cleaning_idle:
                    spriteRenderer.SetAnimation("cleaningIdle");
                    animation_type = animation_cleaning_idle;
                    break;
                case player_states.dance:
                    spriteRenderer.SetAnimation("dance");
                    animation_type = animation_dancing;
                    break;
                case player_states.cry:
                    spriteRenderer.SetAnimation("crying");
                    animation_type = animation_crying;
                    break;
            }
        }

        // equip vacuum if no other equipment
        public void toggleVacuum()
        {
            if (!isCleaning && !isSorting && !isVacuuming)
            {
                isVacuuming = true;
                vacuum.isVacuuming = true;
                _soundEffect.Play();
            }
        }

        // equip garbage bag if free
        public void toggleBin()
        {
            if (!isCleaning && !isSorting && !isVacuuming)
            {
                isCleaning = true;
                garbageBag.isCleaning = true;
                _soundEffect.Stop();
            }
        }
        
        // equip clothes basket if free
        public void toggleClothesBasket()
        {
            if (!isCleaning && !isSorting && !isVacuuming)
            {
                isSorting = true;
                clothsBasket.isSorting = true;
                _soundEffect.Stop();
            }
        }

        // remove any equipment & reset state
        public void detachEquipment()
        {
            isCleaning = false;
            isSorting = false;
            isVacuuming = false;
            garbageBag.isCleaning = false;
            vacuum.isVacuuming = false;
            clothsBasket.isSorting = false;
            player_state = player_states.idle;
            _soundEffect.Stop();
        }
    }
}
