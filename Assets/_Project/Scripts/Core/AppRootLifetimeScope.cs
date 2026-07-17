using VContainer;
using VContainer.Unity;

namespace GameplaySystemsAndTools.Core
{
    /// <summary>
    /// App-wide composition root. Register services here only when they must survive
    /// scene loads (save system, settings, analytics). Scene-local gameplay services
    /// belong in GameplayLifetimeScope instead.
    /// </summary>
    public class AppRootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
