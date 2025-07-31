namespace Tests.Editor.Binding
{
    public abstract class BaseBindingTest : BaseTest
    {
        public override void Setup()
        {
            CreateHierarchy();
        }

        protected abstract void CreateHierarchy();
    }
}