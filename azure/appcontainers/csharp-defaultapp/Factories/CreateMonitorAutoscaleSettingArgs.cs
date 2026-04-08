using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public class CreateMonitorAutoscaleSettingArgs: CreateResourceArgs
{
    public string Alias { get; set; }
    public bool Enabled { get; set; } = true;
    public required Input<string> TargetResourceId { get; set; }
    public List<AutoscaleSettingProfile> AutoscaleSettingProfiles { get; set; }

    public static CreateMonitorAutoscaleSettingArgs DefaultAppServiceAutoscaleSetting(
        string alias, string name, string resourceGroup, Input<string> appServiceId, string location, Dictionary<string, string> tags, ComponentResource? componentResource)
    {
        return new CreateMonitorAutoscaleSettingArgs
        {
            Alias = alias,
            Name = name,
            ResourceGroupName =  resourceGroup,
            Location =  location,
            TargetResourceId = appServiceId,
            AutoscaleSettingProfiles = [
                AutoscaleSettingProfile.DefaultAppServiceAutoscaleProfile(appServiceId)
            ],
            Tags = tags,
            Parent = componentResource
        };
    }
}

public class AutoscaleSettingProfile
{
    public required string Name { get; set; }
    public required ScaleCapacity Capacity { get; set; }
    public required List<ScaleRule> Rules { get; set; }

    public static AutoscaleSettingProfile DefaultAppServiceAutoscaleProfile(Input<string> targetResourceId)
    {
        return new AutoscaleSettingProfile
        {
            Name = "DefaultProfile_AppService",
            Capacity = new ScaleCapacity
            {
                Default = "1",
                Minimum = "1",
                Maximum = "3",
            },
            Rules = [
                ScaleRule.ScaleOutBasedOnCpuUsage(targetResourceId),
                ScaleRule.ScaleInBasedOnCpuUsage(targetResourceId),
            ]
        };
    }
    
    public static AutoscaleSettingProfile BasicAppServiceAutoscaleProfile(Input<string> targetResourceId)
    {
        return new AutoscaleSettingProfile
        {
            Name = "BasicProfile_AppService",
            Capacity = new ScaleCapacity
            {
                Default = "1",
                Minimum = "1",
                Maximum = "3",
            },
            Rules = [
                ScaleRule.ScaleOutBasedOnCpuUsage(targetResourceId),
                ScaleRule.ScaleInBasedOnCpuUsage(targetResourceId),
                ScaleRule.ScaleOutBasedOnMemoryUsage(targetResourceId),
                ScaleRule.ScaleInBasedOnMemoryUsage(targetResourceId),
            ]
        };
    }
    
    public static AutoscaleSettingProfile DefaultVirtualMachineSetsAutoscaleProfile(Input<string> targetResourceId)
    {
        return new AutoscaleSettingProfile
        {
            Name = "DefaultProfile_VMSets",
            Capacity = new ScaleCapacity
            {
                Default = "1",
                Minimum = "1",
                Maximum = "3",
            },
            Rules = [
                ScaleRule.ScaleOutVMSetsBasedOnCpuUsage(targetResourceId),
                ScaleRule.ScaleInVMSetsBasedOnCpuUsage(targetResourceId),
            ]
        };
    }
}

public class ScaleCapacity
{
    public required string Default { get; set; }
    public required string Maximum { get; set; }
    public required string Minimum { get; set; }
}

public class ScaleRule
{
    public required MetricTrigger Trigger { get; set; }
    public required ScaleAction  Action { get; set; }

    public static ScaleRule ScaleOutBasedOnCpuUsage(Input<string> targetResourceId)
    {
        return new ScaleRule
        {
            Trigger = new MetricTrigger
            {
                MetricName = "CpuPercentage",
                MetricResourceUri = targetResourceId,
                MetricNamespace = "Microsoft.Web/serverfarms",
                Operator = "GreaterThan",
                Threshold = 75,
                Statistic = "Average",
                TimeAggregation = "Average",
                TimeGrain = "PT1M",
                TimeWindow = "PT5M",
            },
            Action = new ScaleAction
            {
                Cooldown = "PT1M",
                Direction = "Increase",
                Type = "ChangeCount",
                Value = "1",
            }
        };
    }
    
    public static ScaleRule ScaleInBasedOnCpuUsage(Input<string> targetResourceId)
    {
        return new ScaleRule
        {
            Trigger = new MetricTrigger
            {
                MetricName = "CpuPercentage",
                MetricResourceUri = targetResourceId,
                MetricNamespace = "Microsoft.Web/serverfarms",
                Operator = "LessThan",
                Threshold = 25,
                Statistic = "Average",
                TimeAggregation = "Average",
                TimeGrain = "PT1M",
                TimeWindow = "PT5M",
            },
            Action = new ScaleAction
            {
                Cooldown = "PT1M",
                Direction = "Decrease",
                Type = "ChangeCount",
                Value = "1",
            }
        };
    }
    
    public static ScaleRule ScaleOutBasedOnMemoryUsage(Input<string> targetResourceId)
    {
        return new ScaleRule
        {
            Trigger = new MetricTrigger
            {
                MetricName = "MemoryPercentage",
                MetricResourceUri = targetResourceId,
                MetricNamespace = "Microsoft.Web/serverfarms",
                Operator = "GreaterThan",
                Threshold = 75,
                Statistic = "Average",
                TimeAggregation = "Average",
                TimeGrain = "PT1M",
                TimeWindow = "PT5M",
            },
            Action = new ScaleAction
            {
                Cooldown = "PT1M",
                Direction = "Increase",
                Type = "ChangeCount",
                Value = "1",
            }
        };
    }
    
    public static ScaleRule ScaleInBasedOnMemoryUsage(Input<string> targetResourceId)
    {
        return new ScaleRule
        {
            Trigger = new MetricTrigger
            {
                MetricName = "MemoryPercentage",
                MetricResourceUri = targetResourceId,
                MetricNamespace = "Microsoft.Web/serverfarms",
                Operator = "LessThan",
                Threshold = 25,
                Statistic = "Average",
                TimeAggregation = "Average",
                TimeGrain = "PT1M",
                TimeWindow = "PT5M",
            },
            Action = new ScaleAction
            {
                Cooldown = "PT1M",
                Direction = "Decrease",
                Type = "ChangeCount",
                Value = "1",
            }
        };
    }
    
    public static ScaleRule ScaleOutVMSetsBasedOnCpuUsage(Input<string> targetResourceId)
    {
        return new ScaleRule
        {
            Trigger = new MetricTrigger
            {
                MetricName = "Percentage CPU",
                MetricResourceUri = targetResourceId,
                MetricNamespace = "Microsoft.Compute/virtualmachineScaleSets",
                Operator = "GreaterThan",
                Threshold = 75,
                Statistic = "Average",
                TimeAggregation = "Average",
                TimeGrain = "PT1M",
                TimeWindow = "PT5M",
            },
            Action = new ScaleAction
            {
                Cooldown = "PT1M",
                Direction = "Increase",
                Type = "ChangeCount",
                Value = "1",
            }
        };
    }
    
    public static ScaleRule ScaleInVMSetsBasedOnCpuUsage(Input<string> targetResourceId)
    {
        return new ScaleRule
        {
            Trigger = new MetricTrigger
            {
                MetricName = "Percentage CPU",
                MetricResourceUri = targetResourceId,
                MetricNamespace = "Microsoft.Compute/virtualmachineScaleSets",
                Operator = "LessThan",
                Threshold = 25,
                Statistic = "Average",
                TimeAggregation = "Average",
                TimeGrain = "PT1M",
                TimeWindow = "PT5M",
            },
            Action = new ScaleAction
            {
                Cooldown = "PT1M",
                Direction = "Decrease",
                Type = "ChangeCount",
                Value = "1",
            }
        };
    }
}

public class MetricTrigger
{
    public required string MetricName { get; set; }
    public required Input<string> MetricResourceUri { get; set; }
    public required string MetricNamespace { get; set; }
    public required string Operator { get; set; } // Equals, NotEquals, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
    public required string Statistic { get; set; } // Average, Min, Max, Sum, Count
    public required double Threshold { get; set; }
    public required string TimeAggregation { get; set; } // Average, Minimum, Maximum, Total, Count, Last
    public required string TimeGrain { get; set; }
    public required string TimeWindow { get; set; }
};

public class ScaleAction
{
    public required string Cooldown { get; set; }
    public required string Direction { get; set; } // None, Increase, Decrease
    public required string Type { get; set; } // ChangeCount, PercentChangeCount, ExactCount, ServiceAllowedNextValue
    public required string Value { get; set; }
}