//Code for Elements/Icon2 (Container)
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.StateAnimation.Runtime;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary.Graphics;
using System.Linq;
namespace AstroDroids.Components.Elements;
partial class Icon2 : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Elements/Icon2") ?? throw new System.InvalidOperationException("Could not find an element named Elements/Icon2 - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new Icon2(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(Icon2)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Elements/Icon2", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum IconCategory
    {
        None,
        ArrowUpDown,
        Arrow1,
        Arrow2,
        Arrow3,
        Basket,
        Battery,
        Check,
        CheckeredFlag,
        Circle1,
        Circle2,
        Close,
        Crosshairs,
        Currency,
        Cursor,
        CursorText,
        Dash,
        Delete,
        Enter,
        Expand,
        Gamepad,
        GamepadNES,
        GamepadSNES,
        GamepadNintendo64,
        GamepadGamecube,
        GamepadSwitchPro,
        GamepadXbox,
        GamepadPlaystationDualShock,
        GamepadSegaGenesis,
        Gear,
        FastForward,
        FastForwardBar,
        FitToScreen,
        Flame1,
        Flame2,
        Heart,
        Info,
        Keyboard,
        Leaf,
        Lightning,
        Minimize,
        Monitor,
        Mouse,
        Music,
        Pause,
        Pencil,
        Play,
        PlayBar,
        Power,
        Radiation,
        Reduce,
        Shield,
        Shot,
        Skull,
        Sliders,
        SoundMaximum,
        SoundMinimum,
        Speech,
        Star,
        Stop,
        Temperature,
        Touch,
        Trash,
        Trophy,
        User,
        UserAdd,
        UserDelete,
        UserGear,
        UserMulti,
        UserRemove,
        Warning,
        Wrench,
        ControllerA,
        ControllerB,
        ControllerX,
        ControllerY,
        ControllerLeftJoystickPress,
        ControllerRightJoystickPress,
        ControllerRightJoystick,
        ControllerLeftJoystick,
        ZKey,
        XKey,
        ArrowKeys,
        MouseLMB,
        MouseRMB,
        MouseNMB,
    }

    IconCategory? _iconCategoryState;
    public IconCategory? IconCategoryState
    {
        get => _iconCategoryState;
        set
        {
            _iconCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("IconCategory"))
                {
                    var category = Visual.Categories["IconCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "IconCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public SpriteRuntime IconSprite { get; protected set; }


    public float IconHeight
    {
        get => IconSprite.Height;
        set => IconSprite.Height = value;
    }

    public float IconWidth
    {
        get => IconSprite.Width;
        set => IconSprite.Width = value;
    }

    public Icon2(InteractiveGue visual) : base(visual)
    {
    }
    public Icon2()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        IconSprite = this.Visual?.GetGraphicalUiElementByName("IconSprite") as global::Gum.GueDeriving.SpriteRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
