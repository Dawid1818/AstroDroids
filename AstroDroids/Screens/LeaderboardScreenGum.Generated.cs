//Code for LeaderboardScreenGum
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
partial class LeaderboardScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("LeaderboardScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named LeaderboardScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new LeaderboardScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(LeaderboardScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("LeaderboardScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum HeaderCategory
    {
        Out,
        In,
    }

    HeaderCategory? _headerCategoryState;
    public HeaderCategory? HeaderCategoryState
    {
        get => _headerCategoryState;
        set
        {
            _headerCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("HeaderCategory"))
                {
                    var category = Visual.Categories["HeaderCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "HeaderCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public ButtonGlow ReturnBtn { get; protected set; }
    public ContainerRuntime LeaderboardItems { get; protected set; }
    public TextRuntime Header { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    #endregion
    public LeaderboardScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public LeaderboardScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        LeaderboardItems = this.Visual?.GetGraphicalUiElementByName("LeaderboardItems") as global::Gum.GueDeriving.ContainerRuntime;
        Header = this.Visual?.GetGraphicalUiElementByName("Header") as global::Gum.GueDeriving.TextRuntime;
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.Header.Text = GumService.Default.LocalizationService.Translate("T_Leaderboard");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
    }
    partial void CustomInitialize();
}
