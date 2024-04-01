using Microsoft.Extensions.DependencyInjection;

namespace Green.CT.Asyncify.Net.Extensions;

public static class ControllerTypeExtensions
{
    public static object BuildConstructor(this Type asyncController, IServiceProvider applicationServices)
    {
        var constructorInfos = asyncController
            .GetConstructors()
            .FirstOrDefault();

        if (constructorInfos == null)
            throw new NullReferenceException($"{asyncController.Name} doesn't have any constructor.");

        var constructorParameters = constructorInfos
            .GetParameters();

        var services = constructorParameters
            .Select(parameter => parameter.ParameterType)
            .Select(applicationServices.GetRequiredService)
            .ToArray();

        return constructorInfos.Invoke(services);
    }
}