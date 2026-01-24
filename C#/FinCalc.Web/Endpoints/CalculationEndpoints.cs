using FinCalc.Models;
using FinCalc.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FinCalc.Endpoints;

public static class CalculationEndpoints
{
    public static IEndpointRouteBuilder MapCalculationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/calculations");
        group.MapPost("/credit", static (CreditCalculationRequest request, ICalculationService service) =>
        {
            ICalculationResult result = service.Calculate(request);
            return Results.Ok((CreditCalculationResult)result);
        });

        group.MapPost("/refinancing", static (RefinancingCalculationRequest request, ICalculationService service) =>
        {
            ICalculationResult result = service.Calculate(request);
            return Results.Ok((RefinancingCalculationResult)result);
        });

        group.MapPost("/leasing", static (LeasingCalculationRequest request, ICalculationService service) =>
        {
            ICalculationResult result = service.Calculate(request);
            return Results.Ok((LeasingCalculationResult)result);
        });

        return endpoints;
    }
}
