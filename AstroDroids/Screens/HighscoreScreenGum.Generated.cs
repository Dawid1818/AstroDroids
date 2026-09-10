//Code for HighscoreScreenGum
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
partial class HighscoreScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("HighscoreScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named HighscoreScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new HighscoreScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(HighscoreScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("HighscoreScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum KeyboardCategory
    {
        Out,
        In,
        OutRight,
    }
    public enum AnimCategory1
    {
        Out,
        In,
    }
    public enum AnimCategory2
    {
        Out,
        In,
    }

    KeyboardCategory? _keyboardCategoryState;
    public KeyboardCategory? KeyboardCategoryState
    {
        get => _keyboardCategoryState;
        set
        {
            _keyboardCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("KeyboardCategory"))
                {
                    var category = Visual.Categories["KeyboardCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "KeyboardCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }

    AnimCategory1? _animCategory1State;
    public AnimCategory1? AnimCategory1State
    {
        get => _animCategory1State;
        set
        {
            _animCategory1State = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("AnimCategory1"))
                {
                    var category = Visual.Categories["AnimCategory1"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "AnimCategory1");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }

    AnimCategory2? _animCategory2State;
    public AnimCategory2? AnimCategory2State
    {
        get => _animCategory2State;
        set
        {
            _animCategory2State = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("AnimCategory2"))
                {
                    var category = Visual.Categories["AnimCategory2"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "AnimCategory2");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public ButtonGlow ReturnBtn { get; protected set; }
    public KeyboardGlow GlowKeyboard { get; protected set; }
    public TextRuntime NameLabel { get; protected set; }
    public TextRuntime ResultLabel { get; protected set; }
    public TextRuntime AchievedHighscoreLabel { get; protected set; }
    public TextRuntime EnterYourNameLabel { get; protected set; }
    public TextRuntime ScoreLabel { get; protected set; }
    public TextRuntime ScoreDisplay { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    #endregion
    public HighscoreScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public HighscoreScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        GlowKeyboard = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<KeyboardGlow>(this.Visual,"GlowKeyboard");
        NameLabel = this.Visual?.GetGraphicalUiElementByName("NameLabel") as global::Gum.GueDeriving.TextRuntime;
        ResultLabel = this.Visual?.GetGraphicalUiElementByName("ResultLabel") as global::Gum.GueDeriving.TextRuntime;
        AchievedHighscoreLabel = this.Visual?.GetGraphicalUiElementByName("AchievedHighscoreLabel") as global::Gum.GueDeriving.TextRuntime;
        EnterYourNameLabel = this.Visual?.GetGraphicalUiElementByName("EnterYourNameLabel") as global::Gum.GueDeriving.TextRuntime;
        ScoreLabel = this.Visual?.GetGraphicalUiElementByName("ScoreLabel") as global::Gum.GueDeriving.TextRuntime;
        ScoreDisplay = this.Visual?.GetGraphicalUiElementByName("ScoreDisplay") as global::Gum.GueDeriving.TextRuntime;
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.AchievedHighscoreLabel.Text = GumService.Default.LocalizationService.Translate("T_AchievedHighscore");
        this.EnterYourNameLabel.Text = GumService.Default.LocalizationService.Translate("T_EnterName");
        this.ResultLabel.Text = GumService.Default.LocalizationService.Translate("T_Victory");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
        this.ScoreDisplay.Text = GumService.Default.LocalizationService.Translate("T_Score");
        this.ScoreLabel.Text = GumService.Default.LocalizationService.Translate("T_Score");
    }
    partial void CustomInitialize();
}
