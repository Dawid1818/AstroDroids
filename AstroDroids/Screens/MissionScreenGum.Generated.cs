//Code for MissionScreenGum
using AstroDroids.Components.Controls;
using AstroDroids.Components.Custom;
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
partial class MissionScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("MissionScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named MissionScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new MissionScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(MissionScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("MissionScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum LevelContainerState
    {
        Idle,
        Right,
    }

    LevelContainerState? _levelContainerStateState;
    public LevelContainerState? LevelContainerStateState
    {
        get => _levelContainerStateState;
        set
        {
            _levelContainerStateState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("LevelContainerState"))
                {
                    var category = Visual.Categories["LevelContainerState"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "LevelContainerState");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public ContainerRuntime LevelContainer { get; protected set; }
    public LevelCard Level5Card { get; protected set; }
    public LevelCard Level4Card { get; protected set; }
    public LevelCard Level3Card { get; protected set; }
    public LevelCard Level2Card { get; protected set; }
    public LevelCard Level1Card { get; protected set; }
    public ContainerRuntime ContainerInstance { get; protected set; }
    public ContainerRuntime ContainerInstance1 { get; protected set; }
    public ButtonIconGlow PrevLevelBtn { get; protected set; }
    public ContainerRuntime ContainerInstance2 { get; protected set; }
    public ButtonIconGlow NextLevelBtn { get; protected set; }
    public ContainerRuntime ContainerInstance3 { get; protected set; }
    public ContainerRuntime ContainerInstance4 { get; protected set; }
    public ButtonGlow PlayBtn { get; protected set; }
    public ButtonGlow ReturnBtn { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    #endregion
    public MissionScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public MissionScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        LevelContainer = this.Visual?.GetGraphicalUiElementByName("LevelContainer") as global::Gum.GueDeriving.ContainerRuntime;
        Level5Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level5Card");
        Level4Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level4Card");
        Level3Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level3Card");
        Level2Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level2Card");
        Level1Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level1Card");
        ContainerInstance = this.Visual?.GetGraphicalUiElementByName("ContainerInstance") as global::Gum.GueDeriving.ContainerRuntime;
        ContainerInstance1 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance1") as global::Gum.GueDeriving.ContainerRuntime;
        PrevLevelBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonIconGlow>(this.Visual,"PrevLevelBtn");
        ContainerInstance2 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance2") as global::Gum.GueDeriving.ContainerRuntime;
        NextLevelBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonIconGlow>(this.Visual,"NextLevelBtn");
        ContainerInstance3 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance3") as global::Gum.GueDeriving.ContainerRuntime;
        ContainerInstance4 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance4") as global::Gum.GueDeriving.ContainerRuntime;
        PlayBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"PlayBtn");
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.PlayBtn.Text = GumService.Default.LocalizationService.Translate("T_Play");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
    }
    partial void CustomInitialize();
}
