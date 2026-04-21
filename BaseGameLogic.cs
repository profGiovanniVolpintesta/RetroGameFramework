using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;

namespace RetroGameFramework
{

    /*
    
    TODO: Documentazione
    TODO: elapsedTime, deltaTime (in BaseGameLogic)
    TODO: random generator (in GameUtils)
    TODO: print di una immagine (matrice) 
    TODO: testi
    TODO: aminations (serie di matrici, lancio animazione, loop)

    TODO: GameUtils (Vector2, Rect)
    TODO: Refractoring delle collisioni, implementazione BoxCollision e CircleCollision
    TODO: testi animati?
    TODO: promitive? Per esempio per i tasti del menu?
    TODO: fasi del gioco: menu, gioco, menu pausa, endgame
     
     */
    internal class BaseGameLogic
    {
        private static GameConfig _gameConfig;
        public static GameConfig GameConfig { get { return _gameConfig; } }

        private List<Keys> _pressedKeys;
        private List<Keys> _releasedKeys;
        private List<Keys> _prevFramePressedKeys;
        bool enabledInput = false;

        private bool _paused = false;
        public bool IsPaused() { return _paused; }
        public void SetPaused (bool paused) { _paused = paused; }

        private int _frameCount = 0;
        // Returns the total frames count from the start of the application. 
        public int FrameCount { get { return _frameCount; } }

        private int _frameCountRunning = 0;
        // Returns the frames count from the start of the application, excluding paused frames. 
        public int FrameCountRunning { get { return _frameCountRunning; } }
        // Returns the paused frames count from the start of the application. 
        public int FrameCountPaused { get { return _frameCount - _frameCountRunning; } }

        private float _elapsedTime = 0;
        // Returns the total elapsed time, in seconds, from the start of the application. 
        public float ElapsedTime { get { return _elapsedTime; } }

        private float _elapsedTimeRunning = 0;
        // Returns the total elapsed time, in seconds, from the start of the application, excluding when the paused time intervals. 
        public float ElapsedTimeRunning { get { return _elapsedTimeRunning; } }
        // Returns the elapsed paused time, in seconds, from the start of the application. 
        public float ElapsedTimePaused { get { return _elapsedTime - _elapsedTimeRunning; } }


        // CREATE GAME MATRIX
        static void Main(string[] args)
        {
            GameConfig GameConfig = new GameConfig();
            BaseGameLogic GameLogic = new GameLogic(GameConfig);
            GameLogic.InitGameConfig(GameConfig);
            int[,] PixelsMatrix = new int[GameConfig.PixelsMatrixWidth, GameConfig.PixelsMatrixHeight];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form gameForm = new GameForm(GameConfig, PixelsMatrix);

            bool continueGame = true;

            DateTime startGameTime = DateTime.UtcNow;

            TimeSpan timeAtPreviousFrame;
            TimeSpan timeAtThisFrame = TimeSpan.Zero;
            GameLogic._frameCount = 0;
            GameLogic._frameCountRunning = 0;
            GameLogic._elapsedTime = 0;
            GameLogic._elapsedTimeRunning = 0;

            GameLogic.StartGame(gameForm, PixelsMatrix);
            GameLogic.LoopGame(PixelsMatrix, 0);
            gameForm.Invalidate();
            gameForm.Update();

            System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000 / GameConfig.FrameRate;
            gameTimer.Tick += (s, e) =>
            {
                if (continueGame)
                {
                    timeAtPreviousFrame = timeAtThisFrame;
                    timeAtThisFrame = DateTime.UtcNow - startGameTime;
                    float deltaTime = (float)(timeAtThisFrame - timeAtPreviousFrame).TotalSeconds;
                    GameLogic._frameCount++;
                    GameLogic._elapsedTime += deltaTime;
                    if (!GameLogic.IsPaused())
                    {
                        GameLogic._frameCountRunning++;
                        GameLogic._elapsedTimeRunning += deltaTime;
                    }

                    gameForm.Invoke(new Action(() =>
                    {
                        // This is done in the gameForm owner thread (the main thread)
                        // through a delegated call. Multithread logic cannot be called
                        // https://visualstudiomagazine.com/articles/2010/11/18/multithreading-in-winforms.aspx
                        gameForm.Invalidate();
                        gameForm.Update();
                    }));

                    GameLogic.CheckKeyboardEvents();

                    if (!GameLogic.IsPaused())
                    {
                        GameLogic.LoopGame(PixelsMatrix, deltaTime);
                    }
                }
                else
                {
                    GameLogic.EndGame(gameForm);
                    gameTimer.Stop();
                }
            };
            gameTimer.Start();

            Application.Run(gameForm); // This runs the form with the main thread as owner
        }

        public BaseGameLogic(GameConfig GameConfig)
        {
            _gameConfig = GameConfig;
            _pressedKeys = new List<Keys>();
            _releasedKeys = new List<Keys>();
            _prevFramePressedKeys = new List<Keys>();
        }

        private void InitGameConfig(GameConfig GameConfig)
        {
            OnInitGameConfig(GameConfig);
            _gameConfig = GameConfig;
        }
        protected virtual void OnInitGameConfig(GameConfig GameConfig) { }

        private void StartGame(Form gameForm, int[,] pixels)
        {
            EnableInput(gameForm);
            OnStartGame(pixels);
        }
        protected virtual void OnStartGame(int[,] pixels) { }

        private void LoopGame(int[,] pixels, float deltaTime)
        {
            OnClear(pixels);
            OnLoopGame(deltaTime);
            OnDraw(pixels);
        }

        protected virtual void OnClear(int[,] pixels) { }
        protected virtual void OnLoopGame(float deltaTime) { }
        protected virtual void OnDraw(int[,] pixels) { }

        private void EndGame(Form gameForm)
        {
            OnEndGame();
            DisableInput(gameForm);
        }
        protected virtual void OnEndGame() { }



        private void EnableInput(Form gameForm)
        {
            enabledInput = true;
            gameForm.KeyDown += OnKeyDownCallback;
            gameForm.KeyUp += OnKeyUpCallback;
        }

        private void DisableInput(Form gameForm)
        {
            enabledInput = false;
            gameForm.KeyDown -= OnKeyDownCallback;
            gameForm.KeyUp -= OnKeyUpCallback;
        }

        private void OnKeyDownCallback(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.KeyCode))
            {
                _pressedKeys.Add(e.KeyCode);
            }
        }

        private void OnKeyUpCallback(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!_releasedKeys.Contains(e.KeyCode))
            {
                _releasedKeys.Add(e.KeyCode);
            }
        }
        private void CheckKeyboardEvents()
        {
            if (enabledInput)
            {
                // Create an array keysEvents of events, sorted firsst by priority, than by key.
                Keys[] keysEvents;
                List<Keys> keysToServe = new List<Keys>();

                foreach (Keys key in _pressedKeys) { if (!keysToServe.Contains(key)) keysToServe.Add(key); }
                foreach (Keys key in _releasedKeys) { if (!keysToServe.Contains(key)) keysToServe.Add(key); }
                foreach (Keys key in _prevFramePressedKeys) { if (!keysToServe.Contains(key)) keysToServe.Add(key); }

                keysEvents = keysToServe.ToArray();
                Keys[] priorities = GameConfig.KeysPriority != null ? GameConfig.KeysPriority : new Keys[0];
                Array.Sort(keysEvents, (Keys A, Keys B) =>
                {
                    int Apriority = Array.IndexOf(priorities, A);
                    int Bpriority = Array.IndexOf(priorities, B);
                    if (Apriority >= 0 && Bpriority >= 0) return Apriority - Bpriority;
                    else if (Apriority >= 0 && Bpriority < 0) return -1000;
                    else if (Bpriority >= 0 && Apriority < 0) return 1000;
                    else return (int)A - (int)B;
                });

                // For each key, raise the proper events.
                foreach (Keys key in keysEvents)
                {
                    bool raiseKeyDown = false, raiseKeyUp = false, raiseKeyPress = false;

                    if (_prevFramePressedKeys.Contains(key))
                    {
                        raiseKeyPress = true;
                    }

                    if (!raiseKeyPress && _pressedKeys.Contains(key))
                    {
                        raiseKeyDown = true;
                    }

                    if (_releasedKeys.Contains(key))
                    {
                        raiseKeyUp = true;

                        // Always invalidate pressed keys
                        _pressedKeys.Remove(key);
                        _prevFramePressedKeys.Remove(key);
                        raiseKeyPress = false;
                    }

                    if (raiseKeyDown && raiseKeyUp) // check the events raise condition and decide what to do
                    {
                        switch (GameConfig.PressReleaseRaceConditionPolicy)
                        {
                            case GameConfig.PressReleaseRaceConditionRule.CallNone:
                                raiseKeyDown = false;
                                raiseKeyUp = false;
                                break;
                            case GameConfig.PressReleaseRaceConditionRule.PressWins:
                                raiseKeyUp = false;
                                break;
                            case GameConfig.PressReleaseRaceConditionRule.ReleaseWins:
                                raiseKeyDown = false;
                                break;
                        }
                    }

                    if (raiseKeyDown)
                    {
                        OnKeyDown(key);
                    }

                    if (raiseKeyUp)
                    {
                        OnKeyUp(key);
                    }

                    if (raiseKeyPress)
                    {
                        OnKeyPress(key);
                    }
                }
            }

            _prevFramePressedKeys.Clear();
            _prevFramePressedKeys.AddRange(_pressedKeys);
            _pressedKeys.Clear();
            _releasedKeys.Clear();
        }

        // Called the first frame a key is pressed, and not called anymore unless the key is released
        protected virtual void OnKeyDown(Keys KeyCode) { }

        // Called if a key has been released (even in the same frame it has been released)
        protected virtual void OnKeyUp(Keys KeyCode) { }

        // Called during the frame a key is pressed and in all the following frames until it's released (excluding the frame it's released)
        protected virtual void OnKeyPress(Keys KeyCode) { }
    }
}
