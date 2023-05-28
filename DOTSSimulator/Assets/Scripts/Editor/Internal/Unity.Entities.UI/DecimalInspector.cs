using JetBrains.Annotations;
using UnityEngine.UIElements;
using Unity.Entities.UI;
using Unity.Properties;

namespace Editor.Internal.Unity.Entities.UI
{
    [UsedImplicitly]
    internal class DecimalInspector : PropertyInspector<decimal>
    {
        private TextField field;
        
        static DecimalInspector()
        {
            TypeConversion.Register((ref decimal v) => v.ToString());
            TypeConversion.Register((ref string v) => decimal.Parse(v));
        }
        
        
        public override VisualElement Build()
        {
            field = new TextField(DisplayName) { value = Target.ToString() };
            return field;
        }
        
        public override void Update()
        {
            field.SetValueWithoutNotify(Target.ToString());
        }
    }
}