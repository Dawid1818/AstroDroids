//Code for Custom/KeyboardGlow (Container)
using AstroDroids.Components.Controls;
using AstroDroids.Components.Elements;
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
namespace AstroDroids.Components.Custom;
partial class KeyboardGlow : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/KeyboardGlow") ?? throw new System.InvalidOperationException("Could not find an element named Custom/KeyboardGlow - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new KeyboardGlow(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(KeyboardGlow)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/KeyboardGlow", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum CursorMoveCategory
    {
        LeftRightMoveSupported,
        NoMovement,
    }

    CursorMoveCategory? _cursorMoveCategoryState;
    public CursorMoveCategory? CursorMoveCategoryState
    {
        get => _cursorMoveCategoryState;
        set
        {
            _cursorMoveCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("CursorMoveCategory"))
                {
                    var category = Visual.Categories["CursorMoveCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "CursorMoveCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public ContainerRuntime Row1Keys { get; protected set; }
    public ContainerRuntime AllRows { get; protected set; }
    public KeyboardKeyGlow Key1 { get; protected set; }
    public KeyboardKeyGlow KeyQ { get; protected set; }
    public KeyboardKeyGlow KeyA { get; protected set; }
    public KeyboardKeyGlow KeyZ { get; protected set; }
    public KeyboardKeyGlow KeyParenLeft { get; protected set; }
    public KeyboardKeyGlow KeyW { get; protected set; }
    public KeyboardKeyGlow KeyS { get; protected set; }
    public KeyboardKeyGlow KeyX { get; protected set; }
    public KeyboardKeyGlow KeyParenRight { get; protected set; }
    public KeyboardKeyGlow KeyE { get; protected set; }
    public KeyboardKeyGlow KeyD { get; protected set; }
    public KeyboardKeyGlow KeyC { get; protected set; }
    public KeyboardKeyGlow KeySpace { get; protected set; }
    public KeyboardKeyGlow KeyR { get; protected set; }
    public KeyboardKeyGlow KeyF { get; protected set; }
    public KeyboardKeyGlow KeyV { get; protected set; }
    public KeyboardKeyGlow KeyT { get; protected set; }
    public KeyboardKeyGlow KeyG { get; protected set; }
    public KeyboardKeyGlow KeyB { get; protected set; }
    public KeyboardKeyGlow KeyY { get; protected set; }
    public KeyboardKeyGlow KeyH { get; protected set; }
    public KeyboardKeyGlow KeyN { get; protected set; }
    public KeyboardKeyGlow KeyU { get; protected set; }
    public KeyboardKeyGlow KeyJ { get; protected set; }
    public KeyboardKeyGlow KeyM { get; protected set; }
    public KeyboardKeyGlow KeyI { get; protected set; }
    public KeyboardKeyGlow KeyK { get; protected set; }
    public KeyboardKeyGlow KeyComma { get; protected set; }
    public KeyboardKeyGlow KeyQuestion { get; protected set; }
    public KeyboardKeyGlow KeyO { get; protected set; }
    public KeyboardKeyGlow KeyL { get; protected set; }
    public KeyboardKeyGlow KeyPeriod { get; protected set; }
    public KeyboardKeyGlow KeyBang { get; protected set; }
    public KeyboardKeyGlow KeyP { get; protected set; }
    public KeyboardKeyGlow KeyUnderscore { get; protected set; }
    public KeyboardKeyGlow KeyHyphen { get; protected set; }
    public KeyboardKeyGlow KeyAmpersand { get; protected set; }
    public KeyboardKeyGlow Key2 { get; protected set; }
    public KeyboardKeyGlow Key3 { get; protected set; }
    public KeyboardKeyGlow Key4 { get; protected set; }
    public KeyboardKeyGlow Key5 { get; protected set; }
    public KeyboardKeyGlow Key6 { get; protected set; }
    public KeyboardKeyGlow Key7 { get; protected set; }
    public KeyboardKeyGlow Key8 { get; protected set; }
    public KeyboardKeyGlow Key9 { get; protected set; }
    public KeyboardKeyGlow Key0 { get; protected set; }
    public ContainerRuntime Row2Keys { get; protected set; }
    public ContainerRuntime Row3Keys { get; protected set; }
    public ContainerRuntime Row4Keys { get; protected set; }
    public ContainerRuntime Row5Keys { get; protected set; }
    public KeyboardKeyGlow KeyBackspace { get; protected set; }
    public KeyboardKeyGlow KeyReturn { get; protected set; }
    public KeyboardKeyGlow KeyLeft { get; protected set; }
    public KeyboardKeyGlow KeyRight { get; protected set; }
    public RectangleRuntime HighlightRectangle { get; protected set; }
    public Icon IconInstance { get; protected set; }
    public Icon IconInstance1 { get; protected set; }
    public Icon IconInstance2 { get; protected set; }
    public Icon IconInstance3 { get; protected set; }

    public KeyboardGlow(InteractiveGue visual) : base(visual)
    {
    }
    public KeyboardGlow()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Row1Keys = this.Visual?.GetGraphicalUiElementByName("Row1Keys") as global::Gum.GueDeriving.ContainerRuntime;
        AllRows = this.Visual?.GetGraphicalUiElementByName("AllRows") as global::Gum.GueDeriving.ContainerRuntime;
        Key1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key1");
        KeyQ = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyQ");
        KeyA = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyA");
        KeyZ = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyZ");
        KeyParenLeft = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyParenLeft");
        KeyW = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyW");
        KeyS = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyS");
        KeyX = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyX");
        KeyParenRight = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyParenRight");
        KeyE = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyE");
        KeyD = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyD");
        KeyC = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyC");
        KeySpace = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeySpace");
        KeyR = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyR");
        KeyF = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyF");
        KeyV = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyV");
        KeyT = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyT");
        KeyG = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyG");
        KeyB = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyB");
        KeyY = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyY");
        KeyH = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyH");
        KeyN = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyN");
        KeyU = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyU");
        KeyJ = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyJ");
        KeyM = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyM");
        KeyI = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyI");
        KeyK = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyK");
        KeyComma = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyComma");
        KeyQuestion = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyQuestion");
        KeyO = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyO");
        KeyL = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyL");
        KeyPeriod = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyPeriod");
        KeyBang = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyBang");
        KeyP = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyP");
        KeyUnderscore = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyUnderscore");
        KeyHyphen = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyHyphen");
        KeyAmpersand = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyAmpersand");
        Key2 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key2");
        Key3 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key3");
        Key4 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key4");
        Key5 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key5");
        Key6 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key6");
        Key7 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key7");
        Key8 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key8");
        Key9 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key9");
        Key0 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"Key0");
        Row2Keys = this.Visual?.GetGraphicalUiElementByName("Row2Keys") as global::Gum.GueDeriving.ContainerRuntime;
        Row3Keys = this.Visual?.GetGraphicalUiElementByName("Row3Keys") as global::Gum.GueDeriving.ContainerRuntime;
        Row4Keys = this.Visual?.GetGraphicalUiElementByName("Row4Keys") as global::Gum.GueDeriving.ContainerRuntime;
        Row5Keys = this.Visual?.GetGraphicalUiElementByName("Row5Keys") as global::Gum.GueDeriving.ContainerRuntime;
        KeyBackspace = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyBackspace");
        KeyReturn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyReturn");
        KeyLeft = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyLeft");
        KeyRight = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardKeyGlow>(this.Visual,"KeyRight");
        HighlightRectangle = this.Visual?.GetGraphicalUiElementByName("HighlightRectangle") as global::Gum.GueDeriving.RectangleRuntime;
        IconInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon>(this.Visual,"IconInstance");
        IconInstance1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon>(this.Visual,"IconInstance1");
        IconInstance2 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon>(this.Visual,"IconInstance2");
        IconInstance3 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon>(this.Visual,"IconInstance3");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
