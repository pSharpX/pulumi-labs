using System;
using System.Linq;
using defaultapp.Shared;
using Pulumi;
using Pulumi.AzureNative.Monitor;
using Pulumi.AzureNative.Monitor.Inputs;

namespace defaultapp.Factories;

public static class MonitorAutoscaleSettingFactory
{
    public static AutoscaleSetting Create(CreateMonitorAutoscaleSettingArgs args)
    {
        args.ValidateAutoscaleSetting();
        
        var autoscaleSettingArgs = new AutoscaleSettingArgs
        {
            Name = args.Name,
            AutoscaleSettingName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Enabled = args.Enabled,
            TargetResourceUri = args.TargetResourceId,
            Profiles = args.AutoscaleSettingProfiles.Select(profile => new AutoscaleProfileArgs
            {
                Name = profile.Name,
                Capacity = new ScaleCapacityArgs
                {
                    Default = profile.Capacity.Default,
                    Maximum = profile.Capacity.Maximum,
                    Minimum = profile.Capacity.Minimum,
                },
                Rules = profile.Rules.Select(rule => new ScaleRuleArgs
                {
                    MetricTrigger = new MetricTriggerArgs
                    {
                        MetricName = rule.Trigger.MetricName,
                        MetricResourceUri = rule.Trigger.MetricResourceUri,
                        MetricNamespace =  rule.Trigger.MetricNamespace,
                        Operator = Enum.Parse<ComparisonOperationType>(rule.Trigger.Operator),
                        Statistic = Enum.Parse<MetricStatisticType>(rule.Trigger.Statistic),
                        Threshold = rule.Trigger.Threshold,
                        TimeAggregation = Enum.Parse<TimeAggregationType>(rule.Trigger.TimeAggregation),
                        TimeGrain = rule.Trigger.TimeGrain,
                        TimeWindow = rule.Trigger.TimeWindow
                    },
                    ScaleAction = new ScaleActionArgs
                    {
                        Cooldown = rule.Action.Cooldown,
                        Direction = Enum.Parse<ScaleDirection>(rule.Action.Direction),
                        Type = Enum.Parse<ScaleType>(rule.Action.Type),
                        Value = rule.Action.Value
                    }
                }).ToList()
            }).ToList(),
            TargetResourceLocation = args.Location,
            Tags = args.Tags!,
        };
        return new AutoscaleSetting($"OneBank_MonitorAutoscaleSetting_{args.Alias}", autoscaleSettingArgs, new CustomResourceOptions { Parent = args.Parent });
    }
}