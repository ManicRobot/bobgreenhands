using System.IO;
using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Utils;
using BobGreenhands.Persistence;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;


namespace BobGreenhands.Scenes
{
    public class NewSavegame : MovingBackgroundScene, IInputProcessor
    {

        private string _realFilename = "";

        // timer for when the player has entered an invalid filename
        private float _waitTime;

        private Nez.UI.TextField _textField;

        private TextButton _createButton;
        private TextButton _createAndPlayButton;
        private TextButton _backButton;

        // thanks you NTFS, very cool
        public static readonly char[] IllegalChars = {'/', '<', '>', ':', '"', '\\', '|', '?', '*'};

        // again, thanks Windows
        public static readonly string[] IllegalNames = {"CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"};

        public NewSavegame()
        {
            Game.SubscribeToInputHandler(this);

            Table table = UICanvas.Stage.AddElement(new Table());
            table.SetFillParent(true);
            table.Pad(25);
            table.Add(new Label(Language.Translate("newSavegame"), Game.NormalSkin).SetWrap(true).SetFontScale(6f)).SetFillX().SetExpandX().SetSpaceBottom(25f);
            table.Row();
            table.Add(new Label(Language.Translate("giveName"), Game.NormalSkin).SetWrap(true).SetFontScale(4f)).SetFillX().Space(25f);
            table.Row();
            table.Add(new Label(Language.Translate("location", Game.GameFolder.SavegamesFolder), Game.NormalSkin).SetWrap(true).SetAlignment(Align.TopLeft).SetFontScale(4f)).SetFillX().Top().Space(25f);
            table.Row();

            _textField = new Nez.UI.TextField("", Game.NormalSkin);
            _textField.OnTextChanged += TextField_onTextChanged;
            _textField.SetOnlyFontChars(true);
            _textField.SetMaxLength(128);
            table.Add(_textField).SetFillX().Expand().Top().Space(25f);
            table.Row();

            Table buttonTable = table.Add(new Table()).SetSpaceBottom(25f).SetExpandX().Bottom().GetElement<Table>();

            _createButton = new TextButton(Language.Translate("create"), Game.NormalSkin);
            _createButton.GetLabel().SetFontScale(3f);
            buttonTable.Add(_createButton).SetExpandX().Bottom().Right().Space(10f);
            _createButton.OnClicked += CreateButton_onClicked;

            _createAndPlayButton = new TextButton(Language.Translate("createAndPlay"), Game.NormalSkin);
            _createAndPlayButton.GetLabel().SetFontScale(3f);
            buttonTable.Add(_createAndPlayButton).Bottom().Right().Space(10f);
            _createAndPlayButton.OnClicked += CreateAndPlayButton_onClicked;

            _backButton = new TextButton(Language.Translate("back"), Game.NormalSkin);
            _backButton.GetLabel().SetFontScale(3f);
            buttonTable.Add(_backButton).Bottom().Right().Space(10f);
            _backButton.OnClicked += BackButton_onClicked;
        }

        private void TextField_onTextChanged(Nez.UI.TextField obj, string input)
        {
            _realFilename = input + ".bgs";
            obj.SetText(Game.NormalSkin.Filter(input));
        }

        private Savegame? CreateSavegame()
        {
            // check if name is empty
            if(_realFilename == "")
            {
                SetErrorMessage(Language.Translate("pleaseEnterAName"));
                return null;
            }
            // check if name has illegal characters
            foreach (char c in IllegalChars)
            {
                if(_realFilename.Contains(c))
                {
                    SetErrorMessage(Language.Translate("illegalChars"));
                    return null;
                }
            }
            // check if name can cause older versions of Windows to bluescreen (lol)
            foreach (string s in IllegalNames)
            {
                if (_realFilename.Replace(".bgs", "").ToUpper().Equals(s))
                {
                    SetErrorMessage(Language.Translate("illegalChars"));
                    return null;
                }
            }
            // check if already exists
            foreach (Savegame s in Game.GameFolder.Savegames)
            {
                if (s.Path == Path.Combine(Game.GameFolder.SavegamesFolder, _realFilename))
                {
                    SetErrorMessage(Language.Translate("alreadyExists"));
                    return null;
                }
            }
            Savegame savegame = Savegame.CreateSavegame(_realFilename);
            return savegame;
        }

        private void SetErrorMessage(string msg)
        {
            _waitTime = 3;
            _realFilename = "";
            _textField.SetText(msg);
            _textField.SetDisabled(true);
            _createButton.SetDisabled(true);
            _createAndPlayButton.SetDisabled(true);
        }

        private void CreateButton_onClicked(Button obj)
        {
            obj.Toggle();
            if(CreateSavegame() != null) {
                Game.GameFolder.RefreshSavegameList();
                Game.UnsubscribeFromInputHandler(this);
                Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
            }
        }

        private void CreateAndPlayButton_onClicked(Button obj)
        {
            obj.Toggle();
            var result = CreateSavegame();
            if(result != null) {
                Game.GameFolder.RefreshSavegameList();
                Game.UnsubscribeFromInputHandler(this);
                Core.StartSceneTransition(new WindTransition(() => new PlayScene(result)));
            }
        }

        private void BackButton_onClicked(Button obj)
        {
            obj.Toggle();
            Game.UnsubscribeFromInputHandler(this);
            Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
        }

        public override void Update()
        {
            base.Update();

            if(_waitTime > 0)
                _waitTime -= Time.DeltaTime;
            if(_waitTime < 0)
            {
                _waitTime = 0;
                _realFilename = "";
                _textField.SetText("");
                _textField.SetDisabled(false);
                _createButton.SetDisabled(false);
                _createAndPlayButton.SetDisabled(false);
            }
        }

        public void FirstExtendedMousePressed()
        {
        }

        public void FirstExtendedMouseReleased()
        {
        }

        public void KeyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                Game.UnsubscribeFromInputHandler(this);
                Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
            }
        }

        public void KeyReleased(Keys key)
        {
        }

        public void LeftMousePressed()
        {
        }

        public void LeftMouseReleased()
        {
        }

        public void MiddleMousePressed()
        {
        }

        public void MiddleMouseReleased()
        {
        }

        public void MouseMoved(Point delta)
        {
        }

        public void MouseScrolled(int delta)
        {
        }

        public void RightMousePressed()
        {
        }

        public void RightMouseReleased()
        {
        }

        public void ScaledMouseMoved(Vector2 delta)
        {
        }

        public void SecondExtendedMousePressed()
        {
        }

        public void SecondExtendedMouseReleased()
        {
        }
    }
}