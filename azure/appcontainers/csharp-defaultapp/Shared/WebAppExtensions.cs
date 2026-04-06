using System;
using System.Collections.Generic;
using System.Linq;
using defaultapp.Factories;

namespace defaultapp.Shared;

public static class WebAppExtensions
{
    
    private static readonly List<string> AllKinds =
        ["app", "linux", "container", "hyperV", "windows", "kubernetes", "functionapp"];
    
    /**
     * Basic, Standard, PremiumV2, PremiumV3, PremiumV4, IsolatedV2, Functions Premium (sometimes called the Elastic Premium plan).
     */
    private static readonly List<string> AllPlans =
    [
        "B1", "B2", "B3", "D1", "F1", "I1", "I2", "I3", "I1v2", "I1mv2", "I2v2", "I2mv2", "I3v2", "I3mv2", "I4v2",
        "I4mv2", "I5v2", "I5mv2", "I6v2", "P1v2", "P2v2", "P3v2", "P0v3", "P1v3", "P2v3", "P3v3", "P1mv3", "P2mv3",
        "P3mv3", "P4mv3", "P5mv3", "P0v4", "P1v4", "P2v4", "P3v4", "P1mv4", "P2mv4", "P3mv4", "P4mv4", "P5mv4", "S1",
        "S2", "S3", "SHARED", "EP1", "EP2", "EP3", "FC1", "WS1", "WS2", "WS3", "Y1"
    ];
    private static readonly List<string> IsolatedPlans =
        ["I1v2", "I1mv2", "I2v2", "I2mv2", "I3v2", "I3mv2", "I4v2", "I4mv2", "I5v2", "I5mv2", "I6v2"];
    
    private static readonly List<string> FunctionOnlyPlans =
        ["Y1", "FC1", "EP1", "EP2", "EP3"];
    
    public static void ValidateServicePlan(this CreateAppServicePlanArgs args)
    {
        if (!IsValidPlan(args.SkuName))
        {
            throw new ArgumentNullException($"invalid sku name '{args.SkuName}'");
        }
        
        if (args.Isolated)
        {
            if (!IsValidIsolatedPlan(args.SkuName))
            {
                throw new ArgumentNullException($"invalid sku name for isolated plan '{args.SkuName}'");
            }

            if (args.EnvironmentId is null)
            {
                throw new ArgumentNullException($"invalid environment - cannot be null");
            }

            args.EnvironmentId?.Ensure(env => !string.IsNullOrEmpty(env),
                $"invalid environmentId - cannot be empty");
        }

        if (!IsValidKind(args.Kind))
        {
            throw new ArgumentNullException($"invalid kind '{args.Kind}'");
        }
    }
    
    public static void ValidateWebApp(this CreateWebAppArgs args)
    {
        if (args.Containerized)
        {
            if (args.ImageName is null)
            {
                throw new ArgumentNullException($"invalid parameter '{nameof(args.ImageName)}'");
            }
        }
        else
        {
            if (args.Runtime is null)
            {
                throw new ArgumentNullException($"invalid parameter '{nameof(args.Runtime)}'");
            }
            if (args.StartupCommandLine is null)
            {
                throw new ArgumentNullException($"invalid parameter '{nameof(args.StartupCommandLine)}'");
            }
        }

        if (!IsValidKind(args.Kind))
        {
            throw new ArgumentNullException($"invalid kind '{args.Kind}'");
        }

        if (args.AppInsightsEnabled)
        {
            if (args.Stack is null)
            {
                throw new ArgumentNullException($"invalid parameter '{nameof(args.Stack)}'");
            }
            
            args.AppInsightsConnectionString?.Ensure(env => !string.IsNullOrEmpty(env),
                $"invalid connectionString - cannot be empty");
            args.AppInsightsInstrumentationKey?.Ensure(env => !string.IsNullOrEmpty(env),
                $"invalid connectionString - cannot be empty");
        }
    }

    private static bool IsValidPlan(string plan) => AllPlans.Contains(plan);
    private static bool IsValidIsolatedPlan(string plan) => IsolatedPlans.Contains(plan);
    private static bool IsValidFunctionPlan(string plan) => FunctionOnlyPlans.Contains(plan);

    private static bool IsValidKind(string plan)
    {
        return plan.Split(",").All(kind => AllKinds.Contains(kind));
    }
}