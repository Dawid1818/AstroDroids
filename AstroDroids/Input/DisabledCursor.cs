using Gum.Wireframe;

namespace AstroDroids.Input
{
    public class DisabledCursor : ICursor
    {
        public Cursors? CustomCursor { get; set; }
        public InputDevice LastInputDevice => InputDevice.Mouse;

        public int X => -1000;
        public int Y => -1000;

        public double LastPrimaryPushTime => -1000;
        public double LastPrimaryClickTime => -1000;

        public int XChange => 0;
        public int YChange => 0;

        public int ScrollWheelChange => 0;
        public float ZVelocity => 0;

        public bool PrimaryPush => false;
        public bool PrimaryDown => false;
        public bool PrimaryClick => false;
        public bool PrimaryClickNoSlide => false;
        public bool PrimaryDoubleClick => false;
        public bool PrimaryDoublePush => false;

        public bool SecondaryPush => false;
        public bool SecondaryDown => false;
        public bool SecondaryClick => false;
        public bool SecondaryDoubleClick => false;

        public bool MiddlePush => false;
        public bool MiddleDown => false;
        public bool MiddleClick => false;
        public bool MiddleDoubleClick => false;

        public InteractiveGue WindowPushed { get; set; }
        public InteractiveGue VisualRightPushed { get; set; }
        public InteractiveGue WindowOver { get; set; }
        public InteractiveGue VisualOver { get; set; }

        public void Activity(double currentGameTimeTotalSeconds) { }

        public float XRespectingGumZoomAndBounds() => -1000;

        public float YRespectingGumZoomAndBounds() => -1000;
    }
}
