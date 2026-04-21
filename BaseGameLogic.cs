using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace RetroGameFramework
{
    internal class BaseGameLogic
    {
        private GameConfig _gameConfig;
        public GameConfig GameConfig { get { return _gameConfig; } }

        private List<Keys> _pressedKeys;
        private List<Keys> _releasedKeys;
        private List<Keys> _prevFramePressedKeys;
        bool enabledInput = false;

        private bool _paused = false;
        public bool IsPaused() { return _paused; }
        public void SetPaused (bool paused) { _paused = paused; }

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

            GameLogic.StartGame(gameForm, PixelsMatrix);
            GameLogic.LoopGame(PixelsMatrix);
            gameForm.Invalidate();
            gameForm.Update();

            System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000 / GameConfig.FrameRate;
            gameTimer.Tick += (s, e) =>
            {
                if (continueGame)
                {
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
                        GameLogic.LoopGame(PixelsMatrix);
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

        private void LoopGame(int[,] pixels)
        {
            OnClear(pixels);
            OnLoopGame();
            OnDraw(pixels);
        }

        protected virtual void OnClear(int[,] pixels) { }
        protected virtual void OnLoopGame() { }
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
