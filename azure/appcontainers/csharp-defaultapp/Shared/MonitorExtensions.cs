using System;
using System.Collections.Generic;
using System.Linq;
using defaultapp.Factories;

namespace defaultapp.Shared;

public static class MonitorExtensions
{

    // 'Percentage CPU' for Virtual Machine Scale Sets and 'CpuPercentage' for App Service Plan.
    private static readonly List<string> SupportedMetrics =
        ["Percentage CPU", "CpuPercentage", "MemoryPercentage"];

    // microsoft.compute/virtualmachinescalesets, Microsoft.Web/serverfarms
    private static readonly List<string> SupportedMetricsNamespace =
        ["microsoft.compute/virtualmachinescalesets", "Microsoft.Web/serverfarms"];
    
    // Equals, NotEquals, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
    private static readonly List<string> Operators =
        ["Equals", "NotEquals", "GreaterThan", "GreaterThanOrEqual", "LessThan", "LessThanOrEqual"];
    
    // Average, Min, Max, Sum, Count
    private static readonly List<string> Statistic =
        ["Average", "Min", "Max", "Sum", "Count"];
    
    // ChangeCount, PercentChangeCount, ExactCount, ServiceAllowedNextValue
    private static readonly List<string> ScaleActionType =
        ["ChangeCount", "PercentChangeCount", "ExactCount", "ServiceAllowedNextValue"];
    
    // None, Increase, Decrease
    private static readonly List<string> ScaleDirection =
        ["None", "Increase", "Decrease"];
    
    public static void ValidateAutoscaleSetting(this CreateMonitorAutoscaleSettingArgs args)
    {
        if (args.AutoscaleSettingProfiles.SelectMany(p => p.Rules).Any(rule => !IsValidMetric(rule.Trigger.MetricName)))
        {
            throw new ArgumentNullException($"invalid metric names");
        }
        
        if (args.AutoscaleSettingProfiles.SelectMany(p => p.Rules).Any(rule => !IsValidMetricNamespace(rule.Trigger.MetricNamespace)))
        {
            throw new ArgumentNullException($"invalid metric namespace");
        }
        
        if (args.AutoscaleSettingProfiles.SelectMany(p => p.Rules).Any(rule => !IsValidOperator(rule.Trigger.Operator)))
        {
            throw new ArgumentNullException($"invalid rule operator");
        }
        
        if (args.AutoscaleSettingProfiles.SelectMany(p => p.Rules).Any(rule => !IsValidStatistic(rule.Trigger.Statistic)))
        {
            throw new ArgumentNullException($"invalid rule statistic");
        }
        
        if (args.AutoscaleSettingProfiles.SelectMany(p => p.Rules).Any(rule => !IsValidScaleDirection(rule.Action.Direction)))
        {
            throw new ArgumentNullException($"invalid scale action direction");
        }
        
        if (args.AutoscaleSettingProfiles.SelectMany(p => p.Rules).Any(rule => !IsValidScaleActionType(rule.Action.Type)))
        {
            throw new ArgumentNullException($"invalid scale action type");
        }
    }

    private static bool IsValidMetric(string arg) => SupportedMetrics.Contains(arg);
    private static bool IsValidMetricNamespace(string arg) => SupportedMetricsNamespace.Contains(arg);
    private static bool IsValidOperator(string arg) => Operators.Contains(arg);
    private static bool IsValidStatistic(string arg) => Statistic.Contains(arg);
    private static bool IsValidScaleActionType(string arg) => ScaleActionType.Contains(arg);
    private static bool IsValidScaleDirection(string arg) => ScaleDirection.Contains(arg);
}