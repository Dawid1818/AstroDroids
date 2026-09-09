//Code for ContinueScreenGum
using AstroDroids.Components.Controls;
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
namespace AstroDroids.Screens;
partial class ContinueScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("ContinueScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named ContinueScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ContinueScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ContinueScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("ContinueScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum Animations
    {
        In,
        Out,
        OutRight,
    }

    Animations? _animationsState;
    public Animations? AnimationsState
    {
        get => _animationsState;
        set
        {
            _animationsState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("Animations"))
                {
                    var category = Visual.Categories["Animations"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "Animations");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public TextRuntime TextInstance { get; protected set; }
    public ContainerRuntime ButtonContainer { get; protected set; }
    public ButtonGlow ContinueBtn { get; protected set; }
    public ButtonGlow NewGameBtn { get; protected set; }
    public ButtonGlow BackBtn { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    #endregion
    public ContinueScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public ContinueScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        ButtonContainer = this.Visual?.GetGraphicalUiElementByName("ButtonContainer") as global::Gum.GueDeriving.ContainerRuntime;
        ContinueBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ContinueBtn");
        NewGameBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"NewGameBtn");
        BackBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"BackBtn");
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.BackBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
        this.ContinueBtn.Text = GumService.Default.LocalizationService.Translate("T_ContinueGame");
        this.NewGameBtn.Text = GumService.Default.LocalizationService.Translate("T_NewGame");
        this.TextInstance.Text = GumService.Default.LocalizationService.Translate("T_ContinueGameQuestion");
    }
    partial void CustomInitialize();
}
