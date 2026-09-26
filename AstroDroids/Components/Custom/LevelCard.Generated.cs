//Code for Custom/LevelCard (Container)
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary.Graphics;
using System.Linq;
namespace AstroDroids.Components.Custom;
partial class LevelCard : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/LevelCard") ?? throw new System.InvalidOperationException("Could not find an element named Custom/LevelCard - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new LevelCard(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(LevelCard)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/LevelCard", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public NineSliceRuntime PanelBG { get; protected set; }
    public SpriteRuntime Preview { get; protected set; }
    public TextRuntime NameLabel { get; protected set; }
    public ContainerRuntime LockLayer { get; protected set; }
    public ColoredRectangleRuntime LockedPreview { get; protected set; }
    public SpriteRuntime LockIcon { get; protected set; }

    public string PreviewSourceFile
    {
        set => Preview.SourceFileName = value;
    }

    public LevelCard(InteractiveGue visual) : base(visual)
    {
    }
    public LevelCard()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        PanelBG = this.Visual?.GetGraphicalUiElementByName("PanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        Preview = this.Visual?.GetGraphicalUiElementByName("Preview") as global::Gum.GueDeriving.SpriteRuntime;
        NameLabel = this.Visual?.GetGraphicalUiElementByName("NameLabel") as global::Gum.GueDeriving.TextRuntime;
        LockLayer = this.Visual?.GetGraphicalUiElementByName("LockLayer") as global::Gum.GueDeriving.ContainerRuntime;
        LockedPreview = this.Visual?.GetGraphicalUiElementByName("LockedPreview") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        LockIcon = this.Visual?.GetGraphicalUiElementByName("LockIcon") as global::Gum.GueDeriving.SpriteRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
