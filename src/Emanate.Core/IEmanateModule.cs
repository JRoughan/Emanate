using Microsoft.Extensions.DependencyInjection;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void LoadServiceComponents(IServiceCollection services);
    }
}
