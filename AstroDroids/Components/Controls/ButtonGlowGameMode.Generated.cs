//Code for Controls/ButtonGlowGameMode (Container)
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
namespace AstroDroids.Components.Controls;
partial class ButtonGlowGameMode : global::Gum.Forms.Controls.Button
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Controls/ButtonGlowGameMode") ?? throw new System.InvalidOperationException("Could not find an element named Controls/ButtonGlowGameMode - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ButtonGlowGameMode(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ButtonGlowGameMode)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Controls/ButtonGlowGameMode", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum ButtonCategory
    {
        Enabled,
        Disabled,
        Highlighted,
        Pushed,
        HighlightedFocused,
        Focused,
        DisabledFocused,
        FocusedGlow,
        FocusedGlownt,
    }
    public enum SlideCategory
    {
        Idle,
        Left,
        Right,
        Up,
    }

    ButtonCategory? _buttonCategoryState;
    public ButtonCategory? ButtonCategoryState
    {
        get => _buttonCategoryState;
        set
        {
            _buttonCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("ButtonCategory"))
                {
                    var category = Visual.Categories["ButtonCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "ButtonCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }

    SlideCategory? _slideCategoryState;
    public SlideCategory? SlideCategoryState
    {
        get => _slideCategoryState;
        set
        {
            _slideCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("SlideCategory"))
                {
                    var category = Visual.Categories["SlideCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "SlideCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public NineSliceRuntime Background { get; protected set; }
    public NineSliceRuntime FocusedIndicator { get; protected set; }
    public SpriteRuntime SpriteInstance { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }


    #region Animation Fields
    public AnimationRuntime GlowFocused {get; protected set;}
    public AnimationRuntime SlideInLeft {get; protected set;}
    public AnimationRuntime SlideInRight {get; protected set;}
    public AnimationRuntime SlideInUp {get; protected set;}
    public AnimationRuntime SlideOutLeft {get; protected set;}
    public AnimationRuntime SlideOutRight {get; protected set;}
    public AnimationRuntime SlideOutUp {get; protected set;}
    #endregion
    public string BannerImage
    {
        set => SpriteInstance.SourceFileName = value;
    }


    public ButtonGlowGameMode(InteractiveGue visual) : base(visual)
    {
    }
    public ButtonGlowGameMode()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.NineSliceRuntime;
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.NineSliceRuntime;
        SpriteInstance = this.Visual?.GetGraphicalUiElementByName("SpriteInstance") as global::Gum.GueDeriving.SpriteRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        GlowFocused = this.Visual.GetAnimation("GlowFocused");
        SlideInLeft = this.Visual.GetAnimation("SlideInLeft");
        SlideInRight = this.Visual.GetAnimation("SlideInRight");
        SlideInUp = this.Visual.GetAnimation("SlideInUp");
        SlideOutLeft = this.Visual.GetAnimation("SlideOutLeft");
        SlideOutRight = this.Visual.GetAnimation("SlideOutRight");
        SlideOutUp = this.Visual.GetAnimation("SlideOutUp");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
