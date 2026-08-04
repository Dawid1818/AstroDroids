//Code for Controls/ButtonGlow (Container)
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
partial class ButtonGlow : global::Gum.Forms.Controls.Button
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Controls/ButtonGlow") ?? throw new System.InvalidOperationException("Could not find an element named Controls/ButtonGlow - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ButtonGlow(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ButtonGlow)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Controls/ButtonGlow", () => 
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
        FocusedActive,
    }
    public enum SlideCategory
    {
        Idle,
        Right,
        Left,
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
    public TextRuntime TextInstance { get; protected set; }


    #region Animation Fields
    public AnimationRuntime GlowFocused {get; protected set;}
    public AnimationRuntime GlowActive {get; protected set;}
    public AnimationRuntime SlideIn {get; protected set;}
    public AnimationRuntime SlideOut {get; protected set;}
    #endregion

    public ButtonGlow(InteractiveGue visual) : base(visual)
    {
    }
    public ButtonGlow()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.NineSliceRuntime;
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.NineSliceRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        GlowFocused = this.Visual.GetAnimation("GlowFocused");
        GlowActive = this.Visual.GetAnimation("GlowActive");
        SlideIn = this.Visual.GetAnimation("SlideIn");
        SlideOut = this.Visual.GetAnimation("SlideOut");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
