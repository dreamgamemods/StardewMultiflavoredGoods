using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiFlavor
{
    public class RecipeData
    {
        public GenericConditionalData RecipeIngredients = new GenericConditionalData();
        public GenericConditionalData CheckIfPermitted = new GenericConditionalData();
        public bool? Visible = true;
        public Dictionary<string, ImageDisplayData> DisplayRules = new Dictionary<string, ImageDisplayData>();
    }

    public class OutputObjectData
    {
        public GenericConditionalData PermitNextInput = new GenericConditionalData();
        public Dictionary<string, RecipeData> Layers = new Dictionary<string, RecipeData>();
    }

    public class DisplayNameData
    {
        public string? DisplayNameFormat = "";
        public GenericConditionalData Conditions = new GenericConditionalData();
        public Dictionary<string, Dictionary<string, string>> ReplacementRules = new Dictionary<string, Dictionary<string, string>>();
    }

    public class ImageDisplayData
    {
        public bool? MachineSpecificRule = false;
        public string? Spritesheet = "";
        public int? SpriteIndex = -1;
        public int? RectangleSize= 16;
        public float? ScaleInMachine = 1f;
        public float? ScaleInMenu = 1f;
        public float? ScaleWhenHeld = 1f;
        public float? PaddingX = 8.0f;
        public float? PaddingY = 40.0f;
        public float? ColorOverlayIntensity = 1f;
        public ImageDisplayConditions DisplayConditions = new ImageDisplayConditions();

    }

    public class ImageDisplayConditions : GenericConditionalData
    {
        public string? OnlyWhenOutputColor = "";
        public bool? DisplayInsideMachine = false;
    }

    public class ConditionalTagData
    {
        public bool? VisibleInDescription = false;
        public bool? VisibleInDisplayName = false;
        public bool? CopyToOutputObject = false;
        public VisibilityData? VisibilityOptions = new VisibilityData();
        public GenericConditionalData? Conditions = new GenericConditionalData();
    }
    public class VisibilityData
    {
        public string? Description = "";
        public string? DescriptionPlacement = ""; //START, END, REPLACE
        public string? DisplayName = "";
        public string? DisplayNamePlacement = ""; //START, END, REPLACE
    }
    public class GenericConditionalData
    {
        public string? ItemId = "";
        public string? AnyTagsPresent = "";
        public string? AllTagsPresent = "";
        public string? NoTagsPresent = "";
        public Dictionary<string, int> MultipleObjectsUnderOutputMTag = new Dictionary<string, int>();
        public Dictionary<string, string> ItemIdPresentUnderOutputMTag = new Dictionary<string, string>();
    }

}
