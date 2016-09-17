using GTA;
using GTA.Math;
using GTA.Native;
using Wildfire.Events;

namespace Wildfire.Utility
{
    public class FreeviewCamera
    {
        private float _currentLerpTime;
        private Scaleform _instructionalButtons;
        private Vector3 _previousPos;
        private Camera _mainCamera;

        private readonly float LerpTime = 0.8f;
        private readonly float RotationSpeed = 1.54f;

        public readonly GamepadHandler GamepadHandler;

        public Camera MainCamera { get { return _mainCamera; } }

        public FreeviewCamera(Vector3 position, Vector3 rotation)
        {
            this.GamepadHandler = new GamepadHandler();
            this.GamepadHandler.LeftStickChanged += LeftStickChanged;
            this.GamepadHandler.RightStickChanged += RightStickChanged;
            this.GamepadHandler.LeftStickPressed += LeftStickPressed;
            this._instructionalButtons = new Scaleform(Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "instructional_buttons"));
            this._mainCamera = World.CreateCamera(position, rotation, 50f);
            this._mainCamera.IsActive = false;
        }

        public FreeviewCamera() : this(Vector3.Zero, Vector3.Zero)
        { }

        private void LeftStickChanged(object sender, AnalogStickChangedEventArgs e)
        {
            if (_mainCamera.IsInterpolating) return;

            if (e.X > sbyte.MaxValue)
                _previousPos -= Helpers.RotationToDirection(_mainCamera.Rotation).RightVector(new Vector3(0, 0, 1f)) *
                    (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 218) * -3f);
            if (e.X < sbyte.MaxValue)
                _previousPos += -Helpers.RotationToDirection(_mainCamera.Rotation).RightVector(new Vector3(0, 0, 1f)) *
                    (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 218) * -3f);
            if (e.Y != sbyte.MaxValue)
                _previousPos += Helpers.RotationToDirection(_mainCamera.Rotation) *
                    (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, 8) * -5f);

            _currentLerpTime += 0.02f;

            if (_currentLerpTime > LerpTime)
                _currentLerpTime = LerpTime;

            float amount = _currentLerpTime / LerpTime;

            _mainCamera.Position = Vector3.Lerp(_mainCamera.Position, _previousPos, amount);
        }

        private void RightStickChanged(object sender, AnalogStickChangedEventArgs e)
        {
            if (_mainCamera.IsInterpolating) return;
            _mainCamera.Rotation += new Vector3(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 221) * -10f, 0,
            Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 220) * -11f) * RotationSpeed;
        }

        private void LeftStickPressed(object sender, ButtonPressedEventArgs e)
        {
            if (_mainCamera.IsInterpolating) return;
            _previousPos += Helpers.RotationToDirection(_mainCamera.Rotation) *
                (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 230) * -5f);
        }

        public void EnterCameraView(Vector3 position)
        {
            _mainCamera.Position = position;
            _mainCamera.IsActive = true;
            World.RenderingCamera = _mainCamera;
        }

        public void ExitCameraView()
        {
            _mainCamera.IsActive = false;

            Function.Call(Hash.CLEAR_FOCUS);

            World.RenderingCamera = null;
        }

        public void Update()
        {
            if (_mainCamera.IsActive)
            {
                /* if (Function.Call(Hash.collisio)
                 {
                     Function.Call(Hash._0x0923DBF87DFF735E, _mainCamera.Position.X, _mainCamera.Position.Y, _mainCamera.Position.Z);
                     _renderSceneTimer.Reset();
                 }*/
                UI.ShowSubtitle(_mainCamera.Position.ToString());

                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.VehicleCinCam, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MultiplayerInfo, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MeleeAttackLight, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MeleeAttackAlternate, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MeleeAttack2, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.Phone, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.VehicleLookBehind, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendRs, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendLs, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendX, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayShowhotkey, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayTools, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ScriptPadDown, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendDown, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.PhoneDown, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.HUDSpecial, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.SniperZoomOutSecondary, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.CharacterWheel, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayNewmarker, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayStartStopRecording, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayStartStopRecordingSecondary, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayPause, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveUpDown, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveLeftRight, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveLeftOnly, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveRightOnly, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveUpOnly, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveDownOnly, true);

                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

                Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 18);

                //render local scene

                var lastFocus = MemoryAccess.GetLastFocusArea();

                if (_mainCamera.Position.DistanceTo(new Vector3(lastFocus[0], lastFocus[1], lastFocus[2])) > 60.0f)
                {
                    Function.Call(Hash._SET_FOCUS_AREA, _mainCamera.Position.X, _mainCamera.Position.Y, _mainCamera.Position.Z, lastFocus[0], lastFocus[1], lastFocus[2]);

                }

                _previousPos = _mainCamera.Position;

                GamepadHandler.Update();
            //    RenderIntructionalButtons();

                if (_currentLerpTime > 0) _currentLerpTime -= 0.01f;
            }
        }

     /*   private void DrawMarker(int type, Vector3 position, Vector3 direction, Vector3 rotation, Vector3 scale3D, Color color, bool animate = false, bool faceCam = false, bool rotate = false)
        {
            Function.Call(Hash.DRAW_MARKER, type, position.X, position.Y, position.Z, direction.X, direction.Y, direction.Z, rotation.X, rotation.Y, rotation.Z, scale3D.X, scale3D.Y, scale3D.Z, color.R, color.G, color.B, color.A, animate, faceCam, 2, rotate, 0, 0, 0);
        }*/

        public void InterpolateToPosition()
        {
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 0, _mainCamera.Handle, 1, 0, 1);
        }

        private void RenderIntructionalButtons()
        {
            _instructionalButtons.CallFunction("CLEAR_ALL");
            _instructionalButtons.CallFunction("TOGGLE_MOUSE_BUTTONS", false);

            string str = Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 24, 0);

            _instructionalButtons.CallFunction("SET_DATA_SLOT", 4, str, "Create Perimeter");

            str = Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 245, 0);

            _instructionalButtons.CallFunction("SET_DATA_SLOT", 3, str, "Create Node");

            string[] args = new string[] {
                    Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 326, 0),
                    Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 48, 0) };

            _instructionalButtons.CallFunction("SET_DATA_SLOT", 2, args[1], args[0], "Remove Perimeter");

            args = new string[] {
                    Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 326, 0),
                    Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 254, 0),
                    Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 48, 0) };

            _instructionalButtons.CallFunction("SET_DATA_SLOT", 1, args[2], args[1], args[0] , "Remove Node");        

            str = Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, 24, 0);
            _instructionalButtons.CallFunction("SET_DATA_SLOT", 0, str, "Exit");

            _instructionalButtons.CallFunction("SET_BACKGROUND_COLOUR", 0, 0, 0, 80);
            _instructionalButtons.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", 0);
            _instructionalButtons.Render2D();
        }
    }

}
